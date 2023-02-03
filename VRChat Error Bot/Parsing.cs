using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace VRChat_Error_Bot
{
    class Parsing
    {

        public CommonWords cw;
        public List<ErrorCode> ecl;

        public string cdbp = "";
        public bool cdb;

        public void changePath(bool customDB, string dbPath)
        {
            cdb = customDB;
            cdbp = dbPath;
        }

        public void loadData()
        {

            var pathToDefaultDB = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "/database.json";
            var pathToCommonWords = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "/common.json";
            var pathToCustomDB = cdbp;

            //Console.WriteLine("Debug: Initializing DB");

            if (cdb)
            {
                var json = File.ReadAllText(pathToCustomDB);
                //Console.WriteLine("Debug: Loading custom DB");
                List<ErrorCode> errorCodeList = JsonConvert.DeserializeObject<List<ErrorCode>>(json);
                ecl = errorCodeList;
            }
            else
            {
                var json = File.ReadAllText(pathToDefaultDB);
                //Console.WriteLine("Debug: Loading default DB");
                List<ErrorCode> errorCodeList = JsonConvert.DeserializeObject<List<ErrorCode>>(json);
                ecl = errorCodeList;
            }

            var json2 = File.ReadAllText(pathToCommonWords);
            CommonWords commonWords = JsonConvert.DeserializeObject<CommonWords>(json2);
            cw = commonWords;

            //Console.WriteLine("Debug: Using custom DB - " + cdb.ToString());
            //Console.WriteLine("Debug: custom DB path - " + pathToCustomDB.ToString());

        }
    }

}
