using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using _490Bot.Handlers.ProfileHandler;
using MySql.Data;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using MySqlX.XDevAPI.Relational;
using static System.Net.Mime.MediaTypeNames;

namespace _490Bot
{
    public class Database
    {
        private static String connectionString = "server=127.0.0.1;uid=root;pwd=root;database=CSCI-490";
        private MySqlConnection _connection = new MySqlConnection(connectionString);
        private static String connectionString = "server=127.0.0.1;uid=root;pwd=W3$TEr45;database=LoggerClass";
        public async void OpenConnection()
        {
            try
            {
                await _connection.OpenAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async void CloseConnection()
        {
            try
            {
                await _connection.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public int Insert(Badge badge)
        {
            int result = 0;
            try
            {
                OpenConnection();
                MySqlCommand query = new MySqlCommand();
                String queryText = $"INSERT INTO badge VALUES(@BadgeName, @BadgeDesc, @BadgeIcon, 0)";
                query.CommandText = queryText;
                query.Connection = _connection;
                query.Parameters.AddWithValue("@BadgeName", badge.BadgeName);
                query.Parameters.AddWithValue("@BadgeDesc", badge.BadgeDesc);
                query.Parameters.AddWithValue("@BadgeIcon", badge.BadgeIcon);
                result = query.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            CloseConnection();
            return result;
        }

        public int Insert(Profile profile)
        {
            int result = 0;
            try
            {
                OpenConnection();
                MySqlCommand query = new MySqlCommand();
                String queryText = $"INSERT INTO badge VALUES(@BadgeName, @BadgeDesc, @BadgeIcon, 0)";
                query.CommandText = queryText;
                query.Connection = _connection;
                query.Parameters.AddWithValue("@BadgeName", profile.);
                query.Parameters.AddWithValue("@BadgeDesc", badge.BadgeDesc);
                query.Parameters.AddWithValue("@BadgeIcon", badge.BadgeIcon);
                result = query.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            CloseConnection();
            return result;
        }

        // Insert a log into the database
        public int Insert(Logs log)
        {
            int result = 0;
            try
            {
                OpenConnection();
                MySqlCommand query = new MySqlCommand();
                string queryText = "INSERT INTO logs (UserID, LogID, LogLevel, LogMessage, Reason) VALUES (@UserID, @LogID, @LogLevel, @LogMessage, @Reason)";
                query.CommandText = queryText;
                query.Connection = _connection;
                query.Parameters.AddWithValue("@UserID", log.UserID);
                query.Parameters.AddWithValue("@LogID", log.LogID);
                query.Parameters.AddWithValue("@LogLevel", log.LogLevel);
                query.Parameters.AddWithValue("@LogMessage", log.LogMessage);
                query.Parameters.AddWithValue("@Reason", log.Reason);
                result = query.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            CloseConnection();
            return result;
        }


        // Retrieve logs based on UserID
        .
        public List<Logs> RetrieveLogs()
        {
            List<Logs> logs = new List<Logs>();
            try
            {
                OpenConnection();
                MySqlCommand query = new MySqlCommand("SELECT * FROM logs", _connection);
                using (MySqlDataReader reader = query.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Logs log = new Logs(
                            (ulong)reader["UserID"],
                            (ulong)reader["LogID"],
                            reader["LogLevel"].ToString(),
                            reader["LogMessage"].ToString(),
                            reader["Reason"].ToString()
                        );
                        logs.Add(log);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            CloseConnection();
            return logs;
        }





        // Update a log based on LogID
        public int UpdateLog(Logs log)
        {
            int result = 0;
            try
            {
                OpenConnection();
                MySqlCommand query = new MySqlCommand();
                string queryText = "UPDATE logs SET LogLevel = @LogLevel, LogMessage = @LogMessage, Reason = @Reason WHERE LogID = @LogID";
                query.CommandText = queryText;
                query.Connection = _connection;
                query.Parameters.AddWithValue("@LogID", log.LogID);
                query.Parameters.AddWithValue("@LogLevel", log.LogLevel);
                query.Parameters.AddWithValue("@LogMessage", log.LogMessage);
                query.Parameters.AddWithValue("@Reason", log.Reason);
                result = query.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            CloseConnection();
            return result;
        }

        // Delete a log based on LogID
        public int DeleteLog(ulong logId)
        {
            int result = 0;
            try
            {
                OpenConnection();
                MySqlCommand query = new MySqlCommand();
                string queryText = "DELETE FROM logs WHERE LogID = @LogID";
                query.CommandText = queryText;
                query.Connection = _connection;
                query.Parameters.AddWithValue("@LogID", logId);
                result = query.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            CloseConnection();
            return result;
        }
    }
}