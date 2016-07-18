using Microsoft.Azure.NotificationHubs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;
using VibrationMachineLearningService.Areas.AzureMachineLearning.Models;
using System.Threading.Tasks;
using System.Web;

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


        private async void SendPushNotification(string message)
        {
            Microsoft.Azure.NotificationHubs.NotificationOutcome outcome = null;
            HttpStatusCode ret = HttpStatusCode.InternalServerError;

            // Windows 8.1 / Windows Phone 8.1
            var toast = @"<toast><visual><binding template=""ToastText01""><text id=""1"">" +
                        message + "</text></binding></visual></toast>";

            
            outcome = await Notifications.Instance.Hub.SendWindowsNativeNotificationAsync(toast);

            if (outcome != null)
            {
                if (!((outcome.State == Microsoft.Azure.NotificationHubs.NotificationOutcomeState.Abandoned) ||
                    (outcome.State == Microsoft.Azure.NotificationHubs.NotificationOutcomeState.Unknown)))
                {
                    ret = HttpStatusCode.OK;
                }
            }
        }

        public class Notifications
        {
            public static Notifications Instance = new Notifications();

            public NotificationHubClient Hub { get; set; }

            private Notifications()
            {
                Hub = NotificationHubClient.CreateClientFromConnectionString("Endpoint=sb://vibrationprediction.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=QTDCRfLKVk1yYCY+ifJA7dbzq0Cj41p+7C7pMtO6UqM=",
                                                                             "VibrationPredictionFailure");
            }
        }

        private void ProcessMachineLearningResult(string jsonObj, double lastDataReading)
        {
            if (jsonObj.Length > 0)
            {
                JavaScriptSerializer json_serializer = new JavaScriptSerializer();

                var responseObject = json_serializer.Deserialize<MainObj>(jsonObj);
                var predictedValue = responseObject.Results.output1.value.Values.First()[1];

                dataAccess.InsertMachineLearningData(predictedValue, lastDataReading);
                

                if ( predictedValue> lastDataReading + .01 
                    || predictedValue < lastDataReading -.01
                    || predictedValue == 0)//0 is listed if there is an outlier
                {
                    //failure coming
                    SendPushNotification(string.Format("Failure Predicted:  Current actual reading:  {0}",lastDataReading));
                }
            }
        }
    }
}
