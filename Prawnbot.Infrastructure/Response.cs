namespace Prawnbot.Infrastructure
{
    public class Response<T> : ResponseBase
    {
        public T Entity { get; set; }
        public bool HasData => Entity != null;
    }
}
