using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
            v.v = "v1.3.1";

            string db = "/database.json";

            Console.WriteLine("Type ? for help.");

            bool isCommand;

            while (true)
            {

                var assembly = Assembly.GetExecutingAssembly();
                //Getting names of all embedded resources
                var pathToFile = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + db;
                var pathToFile2 = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "/common.json";

                var json = File.ReadAllText(pathToFile);
                var json2 = File.ReadAllText(pathToFile2);

                List<ErrorCode> errorCodeList = JsonConvert.DeserializeObject<List<ErrorCode>>(json);

                CommonWords commonWords = JsonConvert.DeserializeObject<CommonWords>(json2);

                // Create a list to store the keywords
                List<string> keywords = new List<string>();

                // Iterate through error codes
                foreach (ErrorCode errorCode in errorCodeList)
                {
                    // Split the code into words
                    string[] words = errorCode.Code.Split(' ');
                    // Add each word to the keywords list
                    keywords.AddRange(words.Where(w => w.Length >= 4));
                }

                // Remove duplicates
                keywords = keywords.Distinct().ToList();

                // Remove common words
                keywords = keywords.Except(commonWords.common_words).ToList();

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
                                if (v.limited == true)
                                {
                                    Console.WriteLine("You are currently limited on the GitHub API, please try again in 1+ hour(s).");
                                }
                                else 
                                { 
                                    Console.WriteLine("You are on the latest version!");
                                }
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
                            db = "/database.json";
                            pathToFile = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + db;
                            json = File.ReadAllText(pathToFile);
                            errorCodeList = JsonConvert.DeserializeObject<List<ErrorCode>>(json);

                            Console.WriteLine("Current version is: " + v.v + " " + v.vType);
                        }
                        else if (versionInput == "experimental")
                        {
                            Console.WriteLine("Converting to experimental version...");
                            v.vType = "Experimental";
                            db = "/databaseex.json";
                            pathToFile = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + db;
                            json = File.ReadAllText(pathToFile);
                            errorCodeList = JsonConvert.DeserializeObject<List<ErrorCode>>(json);

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

                    string modifiedInput = Regex.Replace(userInput, @"'(.*?)'", "''");
                    string errorCode = Regex.Match(modifiedInput, @"\b(error|code)?\s?(\d{3})\b").Groups[2].Value;
                    var error = errorCodeList.Find(e => e.Code.IndexOf(modifiedInput, StringComparison.OrdinalIgnoreCase) >= 0 || e.Message.IndexOf(modifiedInput, StringComparison.OrdinalIgnoreCase) >= 0);

                    foreach (string keyword in keywords)
                    {
                        if (error != null)
                        {
                            Console.Clear();
                            Console.WriteLine(error.Response);
                            break;
                        }
                        else if (modifiedInput.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            error = errorCodeList.Find(e => e.Code.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 || e.Message.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
                            Console.Clear();
                            Console.WriteLine(error.Response);
                            break;
                        }
                    }

                    if (error == null)
                    {
                        Console.Clear();
                        Console.WriteLine("No matching error code or message was found.");
                    }

                }

            }
        }
    }
}
