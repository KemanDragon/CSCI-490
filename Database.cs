using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;

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
    }
}
