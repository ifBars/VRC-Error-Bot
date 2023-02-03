using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VRChat_Error_Bot
{
    class Program
    {
        static Version v = new Version();

        static async Task Main(string[] args)
        {

            Parsing pars = new Parsing();

            pars.loadData();

            v.vType = "Stable";
            v.v = "v1.4.0";

            Console.WriteLine("Type ? for help.");

            bool isCommand;

            while (true)
            {

                // Create a list to store the keywords
                List<string> keywords = new List<string>();

                // Iterate through error codes
                foreach (ErrorCode errorCode in pars.ecl)
                {
                    // Split the code into words
                    string[] words = errorCode.Code.Split(' ');
                    // Add each word to the keywords list
                    keywords.AddRange(words.Where(w => w.Length >= 4));
                }

                // Remove duplicates
                keywords = keywords.Distinct().ToList();

                // Remove common words
                keywords = keywords.Except(pars.cw.common_words).ToList();

                isCommand = false;
                string userInput = Console.ReadLine();
                if (userInput.ToLower() == "exit")
                    break;

                if (userInput == "clear")
                {
                    isCommand = true;
                    Console.Clear();
                }

                if (userInput.ToLower().Contains("load"))
                {
                    isCommand = true;

                    bool hasInput = true;

                    Console.Clear();

                    if (userInput.ToLower() == "load")
                    {
                        Console.WriteLine("Invalid command. Type 'load 'filename' to load a custom databse");
                        Console.WriteLine("The database file must be .json and formatted correctly, reference the default database.");
                        hasInput = false;
                    }

                    if (hasInput == true)
                    {
                        string fileName = userInput.Replace("load ", "");

                        if (fileName.Contains(".json"))
                        {

                            v.vType = "CUSTOM DB: " + Path.GetFileName(fileName);
                            pars.changePath(true, fileName);
                            pars.loadData();
                            Console.WriteLine(Path.GetFileName(fileName) + " has been loaded.");
                            
                        }
                        else
                        {
                            Console.WriteLine("Please input a path to a json file like 'load C/%user%/desktop/mydb.json'");
                        }

                    }
                }

                if (userInput.ToLower().Contains("mass"))
                {

                    bool hasInput = true;

                    Console.Clear();

                    isCommand = true;

                    if (userInput == "mass")
                    {
                        Console.WriteLine("Debug: Please input a string to mass check.");
                        hasInput = false;
                    }

                    if (hasInput == true)
                    {
                        string massInput = userInput.ToLower().Substring("mass ".Length);
                        BarsComparison comp = new BarsComparison();
                        comp.massCheck(massInput);
                        Console.WriteLine("Debug: Mass check has been outputted.");
                    }

                }

                if (userInput.ToLower().Contains("version"))
                {

                    bool hasInput = true;

                    Console.Clear();

                    if (userInput.ToLower() == "version")
                    {

                        isCommand = true;

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
                            }
                            else
                            {
                                Console.WriteLine("The latest version is " + v.newV);
                                Console.WriteLine("Please update at " + v.url);
                            }

                        }
                        else if (versionInput == "stable")
                        {
                            Console.WriteLine("Converting to stable version...");
                            v.vType = "Stable";
                            pars.loadData();

                            Console.WriteLine("Current version is: " + v.v + " " + v.vType);
                        }
                        else if (versionInput == "experimental")
                        {
                            Console.WriteLine("Converting to experimental version...");
                            v.vType = "Experimental";
                            pars.changePath(false, Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory) + "/databaseex.json");
                            pars.loadData();

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

                if (userInput == "?" || userInput == "help")
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
                    var error = pars.ecl.Find(e => e.Code.IndexOf(modifiedInput, StringComparison.OrdinalIgnoreCase) >= 0 || e.Message.IndexOf(modifiedInput, StringComparison.OrdinalIgnoreCase) >= 0);

                    BarsComparison comp = new BarsComparison();            

                    foreach (string keyword in keywords)
                    {
                        if (error != null)
                        {
                            double weightedAverage = comp.Weight(modifiedInput, error);

                            Console.Clear();
                            Console.WriteLine(error.Response);
                            Console.WriteLine("Confidence rate: " + weightedAverage + "/1");
                            Console.WriteLine("");

                            break;
                        }
                        else if (modifiedInput.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            //Console.WriteLine("Debug: Matching using KEYWORDS");

                            error = pars.ecl.Find(e => e.Code.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0 || e.Message.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);

                            double weightedAverage = comp.Weight(keyword, error);

                            if (weightedAverage > 1)
                            {
                                weightedAverage = 1;
                            }

                            Console.Clear();
                            Console.WriteLine(error.Response);
                            Console.WriteLine("Confidence rate: " + weightedAverage + "/1");
                            Console.WriteLine("");

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
