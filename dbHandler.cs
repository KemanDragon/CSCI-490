using MySql.Data.MySqlClient;
using System;

namespace dataBasee{
        public class Database
        {
            private readonly string _connectionString;

            public Database(string connectionString)
            {
                _connectionString = connectionString;
            }
            
            public bool InsertLog(string userID, string userName, string message, DateTime timeStamp)
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    try
                    {
                        connection.Open();
                        string sql = "INSERT INTO userLog (userID, userName, message, timeStamp) VALUES (@userID, @userName, @message, @timeStamp)";
                        MySqlCommand cmd = new MySqlCommand(sql, connection);
                        cmd.Parameters.AddWithValue("@userID", userID);
                        cmd.Parameters.AddWithValue("@userName", userName);
                        cmd.Parameters.AddWithValue("@message", message);
                        cmd.Parameters.AddWithValue("@timeStamp", timeStamp);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        Console.WriteLine($"{rowsAffected} row(s) inserted.");
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                        return false;
                    }
                } 
            }
            
            
    }
}
