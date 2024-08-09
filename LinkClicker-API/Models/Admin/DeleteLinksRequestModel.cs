namespace LinkClicker_API.Models.Admin
{
    public class DeleteLinksRequestModel
    {
        public bool DeleteAll { get; set; }
        public List<LinkStatus> Statuses { get; set; }
    }
}
