using LinkClicker_API.Models.Admin;
using System.Collections.Concurrent;
using LinkClicker_API.Interfaces;

namespace LinkClicker_API.Services
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly ConcurrentQueue<CreateLinkRequestModel> _queue = new ConcurrentQueue<CreateLinkRequestModel>();
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void QueueBackgroundWorkItem(CreateLinkRequestModel request)
        {
            _queue.Enqueue(request);
            _signal.Release();
        }

        public async Task<CreateLinkRequestModel> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _queue.TryDequeue(out var workItem);
            return workItem;
        }
    }
}
