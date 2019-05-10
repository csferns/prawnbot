namespace Prawnbot.Core.Framework
{
    public class Response<T> : ResponseBase
    {
        public T Entity { get; set; }
    }
}
