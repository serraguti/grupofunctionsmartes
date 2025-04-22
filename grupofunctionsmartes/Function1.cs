using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using System.Reflection.Metadata;

namespace grupofunctionsmartes
{
    public class Function1
    {
        private readonly ILogger<Function1> _logger;

        public Function1(ILogger<Function1> logger)
        {
            _logger = logger;
        }

        [Function("Function1")]
        public IActionResult Run
            ([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]
        HttpRequest req)
        {
            string idempleado = req.Query["idempleado"];
            if (idempleado == null)
            {
                return new BadRequestObjectResult
                    ("Debe incluir un Id de empleado");
            }
            else
            {
                string connectionString = @"Data Source=sqltajamarpgs.database.windows.net;Initial Catalog=AZURETAJAMAR;Persist Security Info=True;User ID=adminsql;Password=Admin123;Encrypt=True;Trust Server Certificate=True";
                SqlConnection cn = new SqlConnection(connectionString);
                SqlCommand com = new SqlCommand();
                com.Connection = cn;
                com.CommandType = System.Data.CommandType.Text;
                com.CommandText = "update EMP set SALARIO=SALARIO+1 "
                    + " where EMP_NO=" + idempleado;
                cn.Open();
                com.ExecuteNonQuery();
                com.CommandText = "select * from EMP where EMP_NO="
                    + idempleado;
                SqlDataReader reader = com.ExecuteReader();
                reader.Read();
                string mensaje = "El empleado "
                    + reader["APELLIDO"] + " ha incrementado su salario "
                    + " a " + reader["SALARIO"];
                cn.Close();
                _logger.LogInformation("C# HTTP trigger function processed a request.");
                return new OkObjectResult(mensaje);
            }
        }
    }
}
