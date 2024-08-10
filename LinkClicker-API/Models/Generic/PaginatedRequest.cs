namespace LinkClicker_API.Models.Generic
{
    public class PaginatedRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
