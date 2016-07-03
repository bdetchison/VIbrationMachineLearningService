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

        public void InsertMachineLearningData(double predication, double actual)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand();

                SqlParameter predictionParam = new SqlParameter("Prediction", SqlDbType.Decimal);
                predictionParam.Precision = 18;
                predictionParam.Scale = 8;
                predictionParam.Value = predication;

                SqlParameter actualParam = new SqlParameter("Actual", SqlDbType.Decimal);
                actualParam.Precision = 18;
                actualParam.Scale = 8;
                actualParam.Value = actual;

                cmd.CommandText = "InsertMachineLearningData";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add(predictionParam);
                cmd.Parameters.Add(actualParam);


                cmd.Connection = connection;

                connection.Open();
                //Console.WriteLine("Connected successfully.");

                cmd.ExecuteNonQuery();
                // Console.WriteLine("Insert successfull");
            }
        }

        public List<double> GetAllData()
        {
            var vibrationData = new List<double>();

            using (var connection = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand();

                cmd.CommandText = "GetAllVibrationData";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;


                cmd.Connection = connection;

                connection.Open();

                using (SqlDataReader rdr = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (rdr.Read())
                    {
                        vibrationData.Add(Convert.ToDouble(rdr.GetDecimal(rdr.GetOrdinal("Velocity"))));
                    }
                    rdr.Close();
                }

            }

            return vibrationData;
        }
    }
}