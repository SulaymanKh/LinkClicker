using LinkClicker_API.Models.Admin;
using System.Collections.Concurrent;
using LinkClicker_API.Interfaces;

namespace LinkClicker_API.Services
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly ConcurrentQueue<CreateLinkRequestModel> _queue = new ConcurrentQueue<CreateLinkRequestModel>();
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);
        private readonly ILogger<BackgroundTaskQueue> _logger;

        public BackgroundTaskQueue(ILogger<BackgroundTaskQueue> logger)
        {
            _logger = logger;
        }

        public void QueueBackgroundWorkItem(CreateLinkRequestModel request)
        {
            try
            {
                _queue.Enqueue(request);
                _signal.Release();
                _logger.LogInformation("Enqueued a background work item.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while enqueuing a background work item.");
            }
        }

        public async Task<CreateLinkRequestModel> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _queue.TryDequeue(out var workItem);
            return workItem;
        }
    }
}
