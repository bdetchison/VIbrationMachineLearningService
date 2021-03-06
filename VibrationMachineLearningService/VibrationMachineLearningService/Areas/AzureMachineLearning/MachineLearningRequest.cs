﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace VibrationMachineLearningService.Areas.AzureMachineLearning
{

    public class StringTable
    {
        public string[] ColumnNames { get; set; }
        //public string[,] Values { get; set; }
        public List<List<string>> Values { get; set; }
    }

    public static class MachineLearningRequest
    {
        public static async Task InvokeRequestResponseService(Action<string, double> responseAction, List<double> velocityReadings)
        {
            var allReadings = new List<List<string>>();

            var count = 0;
            foreach (double reading in velocityReadings)
            {
                var machineLearningReading = new List<string>();
                machineLearningReading.Add("2014-03-04T09:56:48"); //Keep this as a placeholder because when we change the ML model we will want the date.  but for simple forecasing its not needed.
                machineLearningReading.Add(count.ToString());
                machineLearningReading.Add(reading.ToString());

                allReadings.Add(machineLearningReading);
                count++;
            }

            using (var client = new HttpClient())
            {
                var scoreRequest = new
                {

                    Inputs = new Dictionary<string, StringTable>() {
                        {
                            "input1",
                            new StringTable()
                            {
                                ColumnNames = new string[] {"Date", "Increment", "Velocity"},

                                //Values = new string[,] {  { "2014-03-04T09:56:48", "0", ".034" }, { "2014-03-04T09:57:48", "1", ".035" }, { "2014-03-04T09:58:48", "2", ".15" }, }
                                Values = allReadings
                            }
                        },
                    },
                    GlobalParameters = new Dictionary<string, string>() {
                        { "Relational expression", string.Format("\\\"time_increment\" <= {0}", allReadings.Count-2) },
                    }
                };
                

                const string apiKey = "FxzjiO4NfAJeiR9LSQpzeOMvw9LwWU5wDfKvBuGLcHF2mrYWgumAgWC9CwLmrszNYBFUFnBrWzxEWE7ruth8Vw=="; // Replace this with the API key for the web service
                //const string apiKey = "cxt7co9L9hawG3H+1kjb+wHfSCUsCs2xjXUoGTRrRnPqIHPYg6ItAMiX/oFG8jQzc/RDv4ECUwMOr6fgy2B9aQ==";


                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                //reall one
                client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/ef962331cffb473a833ec8b955046b7c/services/6786357ae0594731a49e46b7601aa0e3/execute?api-version=2.0&details=true");
                //test one
                //client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/ef962331cffb473a833ec8b955046b7c/services/d86484a3418049e9907efae7f8a31c10/execute?api-version=2.0&details=true");



                // WARNING: The 'await' statement below can result in a deadlock if you are calling this code from the UI thread of an ASP.Net application.
                // One way to address this would be to call ConfigureAwait(false) so that the execution does not attempt to resume on the original context.
                // For instance, replace code such as:
                //      result = await DoSomeTask()
                // with the following:
                //      result = await DoSomeTask().ConfigureAwait(false)
                

                HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();

                    if (velocityReadings.Count > 0)
                    {
                        responseAction(result, velocityReadings.Last());
                        Console.WriteLine("Result: {0}", result);
                    }
                }
                else
                {
                    Console.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));

                    // Print the headers - they include the requert ID and the timestamp, which are useful for debugging the failure
                    Console.WriteLine(response.Headers.ToString());

                    string responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseContent);
                }
            }
        }

    }
}