using Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace ICode.API.Extension
{
    public class HangFireDb
    {
        private readonly IConfiguration _configuration;
        public HangFireDb(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetHangfireConnectionString()
        {
            string dbName = "HangFire";
            string connectionString = _configuration.GetConnectionString("Master");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(string.Format(
                    @"IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{0}') 
                                    create database [{0}];
                      ", dbName), connection))
                {
                    command.ExecuteNonQuery();
                }
            }

            return _configuration.GetConnectionString("HangFire");
        }


    }
}
