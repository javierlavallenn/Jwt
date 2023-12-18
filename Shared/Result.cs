namespace Application.Shared
{
    public class Result<T> where T : class
    {
        public bool Failure { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
    }
}
