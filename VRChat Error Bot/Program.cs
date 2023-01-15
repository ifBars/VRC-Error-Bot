using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VRChat_Error_Bot
{
    class Program
    {

        static Version v = new Version();

        static async Task Main(string[] args)
        {

            v.vType = "Stable";
            v.v = "v1.0.0";

            string db = "database.json";

            string json = File.ReadAllText(db);
            List<ErrorCode> errorCodeList = JsonConvert.DeserializeObject<List<ErrorCode>>(json);

            Console.WriteLine("Type ? for help.");

            bool isCommand;

            while (true)
            {
                isCommand = false;
                string userInput = Console.ReadLine();
                if (userInput.ToLower() == "exit")
                    break;

                if (userInput == "clear")
                {
                    isCommand = true;
                    Console.Clear();
                }

                if (userInput.ToLower().Contains("version"))
                {

                    bool hasInput = true;

                    Console.Clear();

                    if (userInput == "version")
                    {
                        Console.WriteLine("Invalid command. Type 'version check' to check current version.");
                        Console.WriteLine("Type 'version stable' to convert to the stable version.");
                        Console.WriteLine("Type 'version experimental' to convert to the stable version.");
                        hasInput = false;
                    }

                    if (hasInput == true)
                    {
                        

                        isCommand = true;
                        
                        string versionInput = userInput.ToLower().Substring("version ".Length);
                        // Do something with versionInput
                        if (versionInput == "check")
                        {
                            Console.WriteLine("Current version is: " + v.v + " " + v.vType);

                            if (await v.CheckUpdateAsync() == false)
                            {
                                Console.WriteLine("You are on the latest version!");
                            } else
                            {
                                Console.WriteLine("The latest version is " + v.newV);
                                Console.WriteLine("Please update at " + v.url);
                            }

                        }
                        else if (versionInput == "stable")
                        {
                            Console.WriteLine("Converting to stable version...");
                            v.vType = "Stable";
                            db = "database.json";
                            
                            Console.WriteLine("Current version is: " + v.v + " " + v.vType);
                        }
                        else if (versionInput == "experimental")
                        {
                            Console.WriteLine("Converting to experimental version...");
                            v.vType = "Experimental";
                            db = "databaseex.json";
                            
                            Console.WriteLine("Current version is: " + v.v + " " + v.vType);
                        }
                        else
                        {
                            Console.WriteLine("Invalid command. Type 'version check' to check current version.");
                            Console.WriteLine("Type 'version stable' to convert to the stable version.");
                            Console.WriteLine("Type 'version experimental' to convert to the stable version.");
                        }
                    }

                }

                if (userInput == "?")
                {
                    isCommand = true;
                    Console.Clear();
                    Console.WriteLine("");
                    Console.WriteLine("Type in an error code or message to find a response.");
                    Console.WriteLine("");
                    Console.WriteLine("? - Displays this help menu");
                    Console.WriteLine("clear - Clears the program");
                    Console.WriteLine("version - version check, version stable, version experimental");
                    Console.WriteLine("exit - Exits the program");
                }

                if (isCommand == false)
                {

                    if (userInput.Length < 9)
                        userInput = "NONE";

                        string errorCode = Regex.Match(userInput, @"\b(error|code)?\s?(\d{3})\b").Groups[2].Value;
                        var error = errorCodeList.Find(e => e.Code == errorCode || e.Message.IndexOf(userInput, StringComparison.OrdinalIgnoreCase) >= 0);
                        if (error != null)
                            Console.WriteLine(error.Response);
                        else
                            Console.WriteLine("No matching error code or message was found.");

                }

            }
        }
    }
}
