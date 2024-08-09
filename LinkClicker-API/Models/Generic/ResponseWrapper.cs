namespace LinkClicker_API.Models.Generic
{
    public class ResponseWrapper<T>
    {
        public T Data { get; set; }
        public bool IsError { get; set; }
        public string Information { get; set; }
    }
}
