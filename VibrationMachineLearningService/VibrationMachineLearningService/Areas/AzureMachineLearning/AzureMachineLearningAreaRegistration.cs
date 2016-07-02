using System.Web.Mvc;

namespace VibrationMachineLearningService.Areas.AzureMachineLearning
{
    public class AzureMachineLearningAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "AzureMachineLearning";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "AzureMachineLearning_default",
                "AzureMachineLearning/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}