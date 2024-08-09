namespace LinkClicker_API.Models.Admin
{
    public class CreateLinkRequestModel
    {
        public string Url { get; set; }

        public string Username { get; set; }

        public int NumberOfLinks { get; set; }

        public int ClicksPerLink { get; set; }

        public int? ExpiryInMinutes { get; set; }
    }
}

