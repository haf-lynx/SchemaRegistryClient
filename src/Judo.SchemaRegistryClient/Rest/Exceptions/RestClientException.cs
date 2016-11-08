
namespace Judo.SchemaRegistryClient.Rest.Exceptions
{
    using System;
    public class RestClientException : Exception
    {   
        public RestClientException(string message, int status, int errorCode) : base(message)
        {
            Status = status;
            ErrorCode = errorCode;
        }

        public int Status { get; private set; }
        public int ErrorCode { get; private set; }
    }
}