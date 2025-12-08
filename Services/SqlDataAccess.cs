using Microsoft.Data.Sqlite;
using System.IO;

namespace Veour.Services {
    public class SqlDataAccess {

    // Using SQLite which does not support Stored Procedures to make my query call with, so it's not as locked down against SQL injections,
    // but user does not have control over what gets searched because the Combo Box is onl valid if the input matches a values in the bound autocomplete list.
    // Summary: User is not allowed to search for just any value they enter, they must enter a value deemed correct.

        private readonly string _connectionString = "";
        private const string Query = "SELECT lat, long FROM locations WHERE city = @city AND state = @state";

        public SqlDataAccess()
        {
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string filepath = Path.Combine(currentDirectory, @"Assets\Files\localdb.db");
            _connectionString = "Data Source="+ filepath;
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
                arr[0] = reader.GetString(0);
                arr[1] = reader.GetString(1);
            }
            
            return arr;
        }
    }
}
