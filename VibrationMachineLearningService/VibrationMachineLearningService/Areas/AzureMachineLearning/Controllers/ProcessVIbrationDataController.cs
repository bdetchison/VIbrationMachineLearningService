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

        public void Post(double velocity, DateTime  date)
        {
            dataAccess.InsertData(velocity, date);

            //get all data

            List<double> velocityReadings = dataAccess.GetAllData();

            MachineLearningRequest.InvokeRequestResponseService(ProcessMachineLearningResult, velocityReadings);
        }

        private void ProcessMachineLearningResult(string jsonObj, double lastDataReading)
        {
            if (jsonObj.Length > 0)
            {
                JavaScriptSerializer json_serializer = new JavaScriptSerializer();

                var responseObject = json_serializer.Deserialize<MainObj>(jsonObj);
                var predictedValue = responseObject.Results.output1.value.Values.First()[1];

                dataAccess.InsertMachineLearningData(predictedValue, lastDataReading);

                if ( predictedValue> lastDataReading + .1 
                    || predictedValue == 0)//0 is listed if there is an outlier
                {
                    //TODO Add push notification
                    //failure coming
                }
            }
        }
    }
}
