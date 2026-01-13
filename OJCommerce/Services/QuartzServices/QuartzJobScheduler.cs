using OJCommerce.Jobs;
using Quartz;

namespace OJCommerce.Services.QuartzServices
{
    public class QuartzJobScheduler : IJobScheduler
    {
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly ILogger<QuartzJobScheduler> _logger;

        public QuartzJobScheduler(ISchedulerFactory schedulerFactory, ILogger<QuartzJobScheduler> logger)
        {
            _schedulerFactory = schedulerFactory;
            _logger = logger;
        }

        public async Task EnqueueWebhookProcessingAsync(long webhookEventId)
        {
            var scheduler = await _schedulerFactory.GetScheduler();

            // Create job with webhook event ID
            var job = JobBuilder.Create<ProcessWebhookJob>()
                .WithIdentity($"webhook-{webhookEventId}", "webhooks")
                .UsingJobData("WebhookEventId", webhookEventId)
                .Build();

            // Trigger immediately
            var trigger = TriggerBuilder.Create()
                .WithIdentity($"webhook-trigger-{webhookEventId}", "webhooks")
                .StartNow()
                .Build();

            await scheduler.ScheduleJob(job, trigger);

            _logger.LogInformation("Enqueued webhook processing job for event {WebhookEventId}", webhookEventId);
        }
    }
}
