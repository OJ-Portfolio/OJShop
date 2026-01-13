namespace OJCommerce.Services.QuartzServices
{
    public interface IJobScheduler
    {
        Task EnqueueWebhookProcessingAsync(long webhookEventId);
    }
}
