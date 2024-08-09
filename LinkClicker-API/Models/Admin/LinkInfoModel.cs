namespace LinkClicker_API.Models.Admin
{
    public class LinkInfoModel
    {
        public string Username { get; set; }
        public string Link { get; set; }
        public DateTime ExpiryTime { get; set; }
        public int MaxClicks { get; set; }
    }
}
