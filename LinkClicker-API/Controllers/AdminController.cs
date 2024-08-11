using Microsoft.AspNetCore.Mvc;
using LinkClicker_API.Models.Admin;
using LinkClicker_API.Models.Generic;
using System.Collections.Generic;
using System;
using LinkClicker_API.Database;
using LinkClicker_API.Models.LinkClickerDatabase;
using Microsoft.EntityFrameworkCore;
using LinkClicker_API.Interfaces;

namespace LinkClicker_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly LinkDbContext _context;
        private readonly ILogger<AdminController> _logger;
        private readonly IBackgroundTaskQueue _taskQueue;

        public AdminController(
            LinkDbContext context, 
            ILogger<AdminController> logger,
            IBackgroundTaskQueue linkCreationQueueService)
        {
            _context = context;
            _logger = logger;
            _taskQueue = linkCreationQueueService;
        }

        [HttpPost("create-link")]
        public async Task<IActionResult> CreateLink(CreateLinkRequestModel request)
        {
            var response = new ResponseWrapper<List<LinkInfoModel>>();

            try
            {
                var rowCount = await _context.Links.CountAsync();

                if (rowCount >= 200000)
                {
                    response.IsError = true;
                    response.Information = "The system has reached the maximum number of links (200,000). No more requests can be accepted.";
                    return Ok(response); 
                }

                _taskQueue.QueueBackgroundWorkItem(request);

                response.Information = "Success";
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating links.");
                response.IsError = true;
                response.Information = "An unexpected error occurred.";
                return StatusCode(500, response);
            }
        }

        [HttpGet("link-details/{id}")]
        public async Task<IActionResult> GetLinkDetails(Guid id)
        {
            var response = new ResponseWrapper<GetLinkDetailsModel>();

            try
            {
                var linkData = await _context.Links.FindAsync(id);

                if (linkData != null)
                {
                    CheckAndValidateStatus(linkData);

                    if (linkData.Status == LinkStatus.Active)
                    {
                        linkData.ClickCount++;

                        _context.Links.Update(linkData);
                        await _context.SaveChangesAsync();

                        response.Information = $"You have found the secret, {linkData.Username}!.";
                        response.Data = new GetLinkDetailsModel
                        {
                            IsExpired = false,
                            Username = linkData.Username,
                            Url = linkData.Url,
                            ExpiryTime = linkData.ExpiryTime,
                            MaxClicks = linkData.MaxClicks,
                            ClickCount = linkData.ClickCount,
                            Status = linkData.Status
                        };
                    }
                    else
                    {
                        response.IsError = true;
                        response.Information = "There are no secrets here.";
                    }
                }
                else
                {
                    response.IsError = true;
                    response.Information = "Link not found.";
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting link information.");
                response.IsError = true;
                response.Information = "An unexpected error occurred.";
                return StatusCode(500, response);
            }
        }

        [HttpPost("all-links")]
        public async Task<IActionResult> GetAllLinks([FromBody] PaginatedRequest paginatedRequest)
        {
            var response = new PaginatedResponseWrapper<LinkInfoModel>();

            try
            {
                var query = _context.Links.AsQueryable();

                var totalRecords = await query.CountAsync();
                var links = await query
                    .Skip((paginatedRequest.PageNumber - 1) * paginatedRequest.PageSize)
                    .Take(paginatedRequest.PageSize)
                    .ToListAsync();

                foreach (var linkData in links)
                {
                    CheckAndValidateStatus(linkData);
                }

                var statusLinks = links.Where(link => link.Status != LinkStatus.Active).ToList();
                if (statusLinks.Any())
                {
                    _context.Links.UpdateRange(statusLinks);
                    await _context.SaveChangesAsync();
                }

                var linkInfos = links.Select(linkData => new LinkInfoModel
                {
                    Username = linkData.Username,
                    Link = $"{linkData.Url}/{linkData.Id}",
                    ExpiryTime = linkData.ExpiryTime,
                    MaxClicks = linkData.MaxClicks,
                    Status = linkData.Status
                }).ToList();

                response.Data = linkInfos;
                response.TotalRecords = totalRecords;
                response.PageNumber = paginatedRequest.PageNumber;
                response.PageSize = paginatedRequest.PageSize;

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving all links.");
                response.IsError = true;
                response.Information = "An unexpected error occurred.";
                return StatusCode(500, response);
            }
        }

        [HttpDelete("delete-links")]
        public async Task<IActionResult> DeleteLinks(DeleteLinksRequestModel request)
        {
            var response = new ResponseWrapper<string>();

            try
            {
                var query = _context.Links.AsQueryable();

                if (!request.DeleteAll)
                {
                    query = query.Where(link => request.Statuses.Contains(link.Status));
                }

                var linksToDelete = await query.ToListAsync();

                _context.Links.RemoveRange(linksToDelete);
                await _context.SaveChangesAsync();

                response.Data = "Links have been deleted based on the provided criteria.";
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting links.");
                response.IsError = true;
                response.Information = "An unexpected error occurred.";
                return StatusCode(500, response);
            }
        }

        private void CheckAndValidateStatus(Link linkData)
        {
            if (linkData.Status == LinkStatus.Active)
            {
                if (linkData.ExpiryTime.HasValue && linkData.ExpiryTime <= DateTime.UtcNow)
                {
                    linkData.Status = LinkStatus.ExpiredByTime;
                }
                else if (linkData.ClickCount >= linkData.MaxClicks)
                {
                    linkData.Status = LinkStatus.ExpiredByClicks;
                }
            }
        }
    }
}
