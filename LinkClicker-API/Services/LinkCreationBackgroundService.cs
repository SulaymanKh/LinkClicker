using LinkClicker_API.Database;
using LinkClicker_API.Models.LinkClickerDatabase;
using LinkClicker_API.Models.Admin;
using LinkClicker_API.Interfaces;

namespace LinkClicker_API.Services
{
    public class LinkCreationBackgroundService : BackgroundService
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<LinkCreationBackgroundService> _logger;

        public LinkCreationBackgroundService(IBackgroundTaskQueue taskQueue, IServiceScopeFactory serviceScopeFactory, ILogger<LinkCreationBackgroundService> logger)
        {
            _taskQueue = taskQueue;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var request = await _taskQueue.DequeueAsync(stoppingToken);

                if (request != null)
                {
                    await ProcessLinkCreationRequest(request, stoppingToken);
                }
            }
        }

        private async Task ProcessLinkCreationRequest(CreateLinkRequestModel request, CancellationToken stoppingToken)
        {
            try
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<LinkDbContext>();

                    var links = new List<LinkInfoModel>();
                    for (int i = 0; i < request.NumberOfLinks; i++)
                    {
                        var linkId = Guid.NewGuid();

                        var linkData = new Link
                        {
                            Id = linkId,
                            Url = request.Url,
                            Username = request.Username,
                            ExpiryTime = request.ExpiryInMinutes.HasValue
                                 ? DateTime.UtcNow.AddMinutes(request.ExpiryInMinutes.Value) : (DateTime?)null,
                            MaxClicks = request.ClicksPerLink,
                            ClickCount = 0,
                            Status = LinkStatus.Active
                        };

                        context.Links.Add(linkData);

                        var linkInfo = new LinkInfoModel
                        {
                            Username = linkData.Username,
                            Link = $"{request.Url}/{linkId}",
                            ExpiryTime = linkData.ExpiryTime,
                            MaxClicks = linkData.MaxClicks,
                            Status = linkData.Status
                        };

                        links.Add(linkInfo);
                    }

                    await context.SaveChangesAsync(stoppingToken);
                }

                _logger.LogInformation("Links created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating links.");
            }
        }
    }
}
