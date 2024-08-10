using Microsoft.AspNetCore.Mvc;
using LinkClicker_API.Models.Admin;
using LinkClicker_API.Models.Generic;
using System.Collections.Generic;
using System;
using LinkClicker_API.Database;
using LinkClicker_API.Models.LinkClickerDatabase;
using Microsoft.EntityFrameworkCore;

namespace LinkClicker_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly LinkDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(LinkDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("create-link")]
        public async Task<IActionResult> CreateLink(CreateLinkRequestModel request)
        {
            var response = new ResponseWrapper<List<LinkInfoModel>>();

            try
            {
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
                             ? DateTime.UtcNow.AddMinutes(request.ExpiryInMinutes.Value): (DateTime?)null,
                        MaxClicks = request.ClicksPerLink,
                        ClickCount = 0,
                        Status = LinkStatus.Active
                    };

                    _context.Links.Add(linkData);

                    var linkInfo = new LinkInfoModel
                    {
                        Username = linkData.Username,
                        Link = $"{request.Url}/{linkId}",
                        ExpiryTime = linkData.ExpiryTime.HasValue ? linkData.ExpiryTime.Value : (DateTime?)null,
                        MaxClicks = request.ClicksPerLink,
                        Status = linkData.Status
                    };

                    links.Add(linkInfo);
                }

                await _context.SaveChangesAsync();
                response.Data = links;
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
                response.Data = null;
                return StatusCode(500, response);
            }
        }

        [HttpGet("all-links")]
        public async Task<IActionResult> GetAllLinks()
        {
            var response = new ResponseWrapper<List<LinkInfoModel>>();

            try
            {
                var links = await _context.Links.ToListAsync();

                foreach (var linkData in links)
                {
                    CheckAndValidateStatus(linkData);

                    if (linkData.Status != LinkStatus.Active)
                    {
                        _context.Links.Update(linkData);
                    }
                }

                await _context.SaveChangesAsync();

                var linkInfos = links.Select(linkData => new LinkInfoModel
                {
                    Username = linkData.Username,
                    Link = $"{linkData.Url}/{linkData.Id}",
                    ExpiryTime = linkData.ExpiryTime,
                    MaxClicks = linkData.MaxClicks,
                    Status = linkData.Status
                }).ToList();

                response.Data = linkInfos;
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
