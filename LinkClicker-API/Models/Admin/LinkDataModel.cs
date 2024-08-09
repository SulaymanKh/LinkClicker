namespace LinkClicker_API.Models.Admin
{
    public class LinkDataModel
    {
        public string Id { get; set; }

        public string Url { get; set; }

        public string Username { get; set; }

        public DateTime? ExpiryTime { get; set; }

        public int MaxClicks { get; set; }

        public int ClickCount { get; set; }

        public LinkStatus Status { get; set; }
    }

    public enum LinkStatus
    {
        Active,
        ExpiredByTime,
        ExpiredByClicks
    }
}
