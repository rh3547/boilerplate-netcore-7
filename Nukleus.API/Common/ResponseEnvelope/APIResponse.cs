namespace Nukleus.API.Common.ResponseEnvelope
{
    // https://stackoverflow.com/questions/4424030/c-system-object-vs-generics
    internal class ResponseEnvelope<T>
    {
        public T? Payload { set; get; }
        public string TraceID { get; set; } = null!;
        public string? ErrorMessage { get; set; }
    }
}