using System.Diagnostics.Eventing.Reader;

namespace LinkClicker_API.Models.Admin
{
    public class GetLinkDetailsModel
    {
        public string Username { get; set; }

        public string Link { get; set; }

        public DateTime ExpiryTime { get; set; }

        public int MaxClicks { get; set; }

        public bool IsExpired { get; set; }

        public string Url { get; set; }

        public int ClickCount {  get; set; }
    }
}
