using OJCommerce.Services.Payments;
using Polly;
using Polly.Retry;
using Quartz;

namespace OJCommerce.Jobs
{
    public class ProcessWebhookJob : IJob
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ProcessWebhookJob> _logger;
        private readonly AsyncRetryPolicy _asyncRetryPolicy;

        public ProcessWebhookJob(IServiceProvider serviceProvider, ILogger<ProcessWebhookJob> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;

            _asyncRetryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), // Exponential backoff
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning(exception,
                            "Retry {RetryCount} after {Delay}s due to error: {Message}",
                            retryCount, timeSpan.TotalSeconds, exception.Message);
                    });
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var webhookEventId = context.JobDetail.JobDataMap.GetLong("WebhookEventId");

            _logger.LogInformation("Processing webhook event {WebhookEventId}", webhookEventId);

            // Create a scope to get scoped services
            using var scope = _serviceProvider.CreateScope();
            var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();

            try
            {
                await _asyncRetryPolicy.ExecuteAsync(async () =>
                {
                    await paymentService.ProcessPendingWebhookAsync(webhookEventId);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing webhook event {WebhookEventId}", webhookEventId);
            }
        }
    }
}
