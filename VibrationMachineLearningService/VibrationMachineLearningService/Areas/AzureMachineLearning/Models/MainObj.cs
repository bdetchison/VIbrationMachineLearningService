using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VibrationMachineLearningService.Areas.AzureMachineLearning.Models
{
    public class MainObj
    {
       public Results Results { get; set; }
    }

    public class Results
    {
        public output1 output1 { get; set; }
    }

    public class output1
    {
        public string type { get; set; }
        public value value { get; set; }
    }

    public class value
    {
        public List<string> ColumnNames { get; set; }
        public List<string> ColumnTypes { get; set; }
        public List<List<double>> Values { get; set; }
    }
}