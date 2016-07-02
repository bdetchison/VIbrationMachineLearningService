using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using VibrationMachineLearningService.Areas.AzureMachineLearning.Models;

namespace VibrationMachineLearningService.Areas.AzureMachineLearning.Controllers
{
    public class ProcessVIbrationDataController : ApiController
    {
        private DataAccess dataAccess;
        private string ConnectionString;

        public ProcessVIbrationDataController()
        {
            dataAccess = new DataAccess(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        }

    //    // GET: api/ProcessVIbrationData
    //    public IEnumerable<string> Get()
    //    {
    //        return new string[] { "value1", "value2" };
    //    }

    //    // GET: api/ProcessVIbrationData/5
    //    public string Get(int id)
    //    {
    //        return "value";
    //    }

        // POST: api/ProcessVIbrationData
        public void Post(double velocity, DateTime  date)
        {
            dataAccess.InsertData(velocity, date);

            //get all data

            List<double> velocityReadings = new List<double>();

            MachineLearningRequest.InvokeRequestResponseService(ProcessMachineLearningResult, velocityReadings);
        }

        private void ProcessMachineLearningResult(string jsonObj, double lastDataReading)
        {
            if (jsonObj.Length>0 && lastDataReading != null)
            {
                JavaScriptSerializer json_serializer = new JavaScriptSerializer();

                var responseObject = json_serializer.Deserialize<MainObj>(jsonObj);

                if (responseObject.Results.output1.value.Values.First()[1] > lastDataReading + .1)
                {
                    //failure coming
                }
            }
        }

        //// PUT: api/ProcessVIbrationData/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE: api/ProcessVIbrationData/5
        //public void Delete(int id)
        //{
        //}
    }

    struct MainObj
    {
        public Results Results { get; set; }
    }

    struct Results
    {
        public output1 output1 { get; set; }
    }


    struct output1
    {
        public string type { get; set; }
        public value value { get; set; }
    }


    struct value
    {
        public List<string> ColumnNames { get; set; }
        public List<string> ColumnTypes { get; set; }
        public List<List<double>> Values { get; set; }
    }
}
