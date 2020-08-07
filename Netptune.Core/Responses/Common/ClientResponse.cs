namespace Netptune.Core.Responses.Common
{
    public class ClientResponse
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; }

        public static ClientResponse Success(string message = null)
        {
            return new ClientResponse
            {
                IsSuccess = true,
                Message = message,
            };
        }

        public static ClientResponse Failed(string message = null)
        {
            return new ClientResponse
            {
                IsSuccess = false,
                Message = message,
            };
        }
    }

    public class ClientResponse<TPayload> : ClientResponse
    {
        public TPayload Payload { get; set; }

        public static new ClientResponse<TPayload> Success(string message = null)
        {
            return new ClientResponse<TPayload>
            {
                IsSuccess = true,
                Message = message,
            };
        }

        public static new ClientResponse<TPayload> Failed(string message = null)
        {
            return new ClientResponse<TPayload>
            {
                IsSuccess = false,
                Message = message,
            };
        }

        public static ClientResponse<TPayload> Success(TPayload payload, string message = null)
        {
            return new ClientResponse<TPayload>
            {
                IsSuccess = true,
                Message = message,
                Payload = payload,
            };
        }

        public static ClientResponse<TPayload> Failed(TPayload payload, string message = null)
        {
            return new ClientResponse<TPayload>
            {
                IsSuccess = false,
                Message = message,
                Payload = payload,
            };
        }
    }
}