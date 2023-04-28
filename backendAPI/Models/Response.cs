namespace backendAPI.Models
{
    public class Response
    {
        public enum RequestStatus { Success = 0, Error = 1 }

        public RequestStatus Status { get; set; } = RequestStatus.Success;

        public Dictionary<string, object> Payload { get; set; }

        public string Message { get; set; }

        public object Errors { get; set; }
    }
}
