namespace VRChat_Error_Bot
{
    class ErrorCode
    {

        public string Code { get; set; }
        public string Message { get; set; }
        public string Response { get; set; }

        public ErrorCode(string code, string message, string response)
        {
            Code = code;
            Message = message;
            Response = response;
        }

    }
}
