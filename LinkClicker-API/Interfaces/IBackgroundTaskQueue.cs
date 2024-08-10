using LinkClicker_API.Models.Admin;

namespace LinkClicker_API.Interfaces
{
    public interface IBackgroundTaskQueue
    {
        public void QueueBackgroundWorkItem(CreateLinkRequestModel request);

        public Task<CreateLinkRequestModel> DequeueAsync(CancellationToken cancellationToken);
    }
}
