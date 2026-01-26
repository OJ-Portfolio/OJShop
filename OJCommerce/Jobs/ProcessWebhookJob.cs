using Microsoft.EntityFrameworkCore;
using OJCommerce.Exceptions;
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

        public ProcessWebhookJob(IServiceProvider serviceProvider, ILogger<ProcessWebhookJob> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var webhookEventId = context.JobDetail.JobDataMap.GetLong("WebhookEventId");

            _logger.LogInformation(" Starting webhook processing for event {WebhookEventId}", webhookEventId);

            using var scope = _serviceProvider.CreateScope();
            var paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>(); // ← Fixed!

            // Create retry policy inside Execute to avoid shared state issues
            var retryPolicy = Policy
                .Handle<Exception>(ex => IsTransientError(ex))
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                    onRetry: (exception, timeSpan, retryCount, ctx) =>
                    {
                        _logger.LogWarning(exception,
                            " Retry {RetryCount}/3 for webhook {WebhookEventId} after {Delay}s - Error: {Message}",
                            retryCount, webhookEventId, timeSpan.TotalSeconds, exception.Message);
                    });

            try
            {
                await retryPolicy.ExecuteAsync(async () =>
                {
                    await paymentService.ProcessPendingWebhookAsync(webhookEventId);
                });

                _logger.LogInformation(" Successfully processed webhook event {WebhookEventId}", webhookEventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    " Failed to process webhook event {WebhookEventId} after all retries. Error: {Message}",
                    webhookEventId, ex.Message);

                // Don't rethrow - let the job complete gracefully
                // The webhook event remains in DB with Processed=false for manual investigation
            }
        }

        /// <summary>
        /// Determines if an exception is transient (temporary) and worth retrying
        /// </summary>
        private static bool IsTransientError(Exception ex)
        {
            return ex switch
            {
                // Database errors that might be temporary
                DbUpdateException dbEx when IsDeadlock(dbEx) => true,
                TimeoutException => true,

                // Network errors
                HttpRequestException => true,
                TaskCanceledException => true,

                // Don't retry business logic errors
                BusinessRuleViolationException => false,
                NotFoundException => false,

                // Retry other errors
                _ => true
            };
        }

        private static bool IsDeadlock(DbUpdateException ex)
        {
            // MySQL deadlock error code: 1213
            return ex.InnerException?.Message?.Contains("1213") ?? false;
        }
    }


}
