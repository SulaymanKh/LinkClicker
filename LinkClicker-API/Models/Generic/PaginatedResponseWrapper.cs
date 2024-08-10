namespace LinkClicker_API.Models.Generic
{
    public class PaginatedResponseWrapper<T>
    {
        public List<T> Data { get; set; } = new List<T>();
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
        public bool IsError { get; set; }
        public string Information { get; set; }
    }
}
