using MySql.Data.MySqlClient;
using System;

namespace dataBase{
        public class Database
        {
            private readonly string _connectionString;

            public Database(string connectionString)
            {
                _connectionString = connectionString;
            }
            
            public bool InsertLog(string userID, DateTime timeStamp)
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    try
                    {
                        connection.Open();
                        string sql = "INSERT INTO userLog (userID, timeStamp) VALUES (@_userID, @timeStamp)";
                        MySqlCommand cmd = new MySqlCommand(sql, connection);
                        cmd.Parameters.AddWithValue("@_userID", userID);
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