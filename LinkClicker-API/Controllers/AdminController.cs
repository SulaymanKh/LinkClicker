using Microsoft.AspNetCore.Mvc;
using LinkClicker_API.Models.Admin;
using LinkClicker_API.Models.Generic;
using System.Collections.Generic;
using System;

namespace LinkClicker_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly ILogger<AdminController> _logger;
        private static readonly Dictionary<string, LinkDataModel> _links = new Dictionary<string, LinkDataModel>();

        public AdminController(ILogger<AdminController> logger)
        {
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
                    var linkId = Guid.NewGuid().ToString();
                    var linkData = new LinkDataModel
                    {
                        Id = linkId,
                        Url = request.Url,
                        Username = request.Username,
                        ExpiryTime = DateTime.UtcNow.AddMinutes(request.ExpiryInMinutes),
                        MaxClicks = request.ClicksPerLink,
                        ClickCount = 0
                    };

                    _links.Add(linkId, linkData);

                    var linkInfo = new LinkInfoModel
                    {
                        Username = linkData.Username,
                        Link = $"{request.Url}/{linkId}",
                        ExpiryTime = linkData.ExpiryTime,
                        MaxClicks = request.ClicksPerLink
                    };

                    links.Add(linkInfo);
                }

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
        public async Task<IActionResult> GetLinkDetails(string id)
        {
            var response = new ResponseWrapper<GetLinkDetailsModel>();

            try
            {
                if (_links.TryGetValue(id, out var linkData))
                {
                    linkData.ClickCount++;

                    if (linkData.ExpiryTime <= DateTime.UtcNow || linkData.ClickCount > linkData.MaxClicks)
                    {
                        response.IsError = true;
                        response.Information = "There are no secrets here.";
                        response.Data = null;
                    }
                    else
                    {
                        response.Information = $"You have found the secret, {linkData.Username}!.";
                        response.Data = new GetLinkDetailsModel()
                        {
                            IsExpired = false,
                            Username = linkData.Username,
                            Url = linkData.Url,
                            ExpiryTime = linkData.ExpiryTime,
                            MaxClicks = linkData.MaxClicks,
                            ClickCount = linkData.ClickCount
                        };
                    }
                }
                else
                {
                    response.IsError = true;
                    response.Information = "Link not found.";
                    response.Data = null;
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
    }
}
