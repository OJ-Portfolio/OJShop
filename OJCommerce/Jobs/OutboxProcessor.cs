using Microsoft.EntityFrameworkCore;
using OJCommerce.Data;
using OJCommerce.Domain.Events;
using System.Text.Json;

namespace OJCommerce.Jobs
{
    public class OutboxProcessor : BackgroundService
    {
        private readonly AppDbContext _context;
        private readonly IServiceProvider _provider;

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                var messages = await _context.OutboxMessages
                    .Where(x => !x.Processed)
                    .Take(10)
                    .ToListAsync(ct);

                foreach (var msg in messages)
                {
                    if (msg.Type == nameof(PaymentCompletedEvent))
                    {
                        var evt = JsonSerializer.Deserialize<PaymentCompletedEvent>(msg.Payload);
                        using var scope = _provider.CreateScope();
                        var handler = scope.ServiceProvider
                            .GetRequiredService<PaymentCompletedEventHandler>();

                        await handler.Handle(evt);
                    }

                    msg.Processed = true;
                }

                await _context.SaveChangesAsync(ct);
                await Task.Delay(1000, ct);
            }
        }
    }

}
