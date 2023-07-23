using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using StringMatchingTools;

namespace VRChat_Error_Bot
{
    class BarsComparison
    {

        public double Weight(string inp, ErrorCode error)
        {

            double similarityScoreMessage = SMT.Check(inp.ToLower(), error.Message);
            double similarityScoreCode = SMT.Check(inp.ToLower(), error.Code);

            return (similarityScoreMessage + similarityScoreCode);

        }

        public ErrorCode massCheck(string inp)
        {

            string db = "/database.json";

            ErrorCode e = new ErrorCode("code", "message", "response");

            //Getting names of all embedded resources
            var pathToFile = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + db;

            //Getting ErrorCodes
            var json = File.ReadAllText(pathToFile);
            List<ErrorCode> errorCodeList = JsonConvert.DeserializeObject<List<ErrorCode>>(json);

            using (StreamWriter writer = new StreamWriter("output.txt"))
            {
                foreach (ErrorCode errorCode in errorCodeList)
                {
                    double codeMatch = SMT.Check(inp, errorCode.Code);
                    double messageMatch = SMT.Check(inp, errorCode.Message);
                    writer.WriteLine("Code: " + errorCode.Code + " " + codeMatch + " & " + messageMatch);
                }
            }

            return e;
        }

    }
}
