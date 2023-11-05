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

namespace _490Bot {
    public class Database {
        private static String connectionString = "server=127.0.0.1;uid=root;pwd=root;database=CSCI-490";
        private MySqlConnection _connection = new MySqlConnection(connectionString);

        public async void OpenConnection() {
            try {
                await _connection.OpenAsync();
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }

        public async void CloseConnection() {
            try {
                await _connection.CloseAsync();
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }

        public int Insert(Badge badge) {
            int result = 0;
            try {
                OpenConnection();
                MySqlCommand query = new MySqlCommand();
                String queryText = $"INSERT INTO badge VALUES(@BadgeName, @BadgeDesc, @BadgeIcon, 0)";
                query.CommandText = queryText;
                query.Connection = _connection;
                query.Parameters.AddWithValue("@BadgeName", badge.BadgeName);
                query.Parameters.AddWithValue("@BadgeDesc", badge.BadgeDesc);
                query.Parameters.AddWithValue("@BadgeIcon", badge.BadgeIcon);
                result = query.ExecuteNonQuery();
            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }

            CloseConnection();
            return result;
        }

        public int Insert(Profile profile) {
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
    }
}
