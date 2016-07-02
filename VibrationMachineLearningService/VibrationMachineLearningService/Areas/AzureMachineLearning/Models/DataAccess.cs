using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http.Headers;
using System.Web.Http.Description;

namespace VibrationMachineLearningService.Areas.AzureMachineLearning.Models
{
    /// <summary>
    /// The model that represents an API displayed on the help page.
    /// </summary>

        public class DataAccess
        {
            private string connectionString;

            public DataAccess(string connString)
            {
                connectionString = connString;
            }

            public void InsertData(double velocity, DateTime date)
            {
                using (var connection = new SqlConnection(connectionString))
                {
                    var cmd = new SqlCommand();

                    SqlParameter velocityParam = new SqlParameter("Velocity", SqlDbType.Decimal);
                    velocityParam.Precision = 18;
                    velocityParam.Scale = 8;
                    velocityParam.Value = velocity;

                    cmd.CommandText = "InsertVibrationData";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add(velocityParam);
                    cmd.Parameters.Add("Date", SqlDbType.DateTime2).Value = date;


                    cmd.Connection = connection;

                    connection.Open();
                    //Console.WriteLine("Connected successfully.");

                    cmd.ExecuteNonQuery();
                   // Console.WriteLine("Insert successfull");
                }
            }
        }
}