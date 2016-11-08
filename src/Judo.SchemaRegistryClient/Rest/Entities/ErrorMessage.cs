namespace Judo.SchemaRegistryClient.Rest.Entities
{

    public class ErrorMessage
    {   
        public ErrorMessage(){}

        public ErrorMessage(int errorCode, string message)
        {
            ErrorCode = errorCode;
            Message = message;
        }
        public int ErrorCode {get;set;}
        public string Message {get;set;
        }
    }
}
