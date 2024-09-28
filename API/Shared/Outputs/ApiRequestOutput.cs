namespace UnityExtension.Api
{
    public abstract class ApiRequestOutput
    {
        public bool IsError { get; set; } = false;
        public HttpResponseCode HttpResponseCode { get; set; } = HttpResponseCode.Ok;
    }
}