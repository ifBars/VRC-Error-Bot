using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
