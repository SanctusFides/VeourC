using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace Veour.Services {
    public class SqlDataAccess {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString = "";
        private const string Query = "SELECT lat, long FROM locations WHERE city = @city AND state = @state";


        public SqlDataAccess(IConfiguration configuration)
        {
            this._configuration = configuration;
            _connectionString = "Data Source="+_configuration["SqlString"];
        }

        public async Task<string[]> GetLatAndLong(string city, string state)
        {
            string[] arr = new string[2];
            await using var connection = new SqliteConnection(_connectionString);
            
            await connection.OpenAsync();
            await using var command = new SqliteCommand(Query, connection);
            command.Parameters.AddWithValue("@city", city);
            command.Parameters.AddWithValue("@state", state);
            
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                Console.WriteLine(reader.GetString(0));
                Console.WriteLine(reader.GetString(1));
                arr[0] = reader.GetString(0);
                arr[1] = reader.GetString(1);
            }
            
            return arr;
        }
    }
}
