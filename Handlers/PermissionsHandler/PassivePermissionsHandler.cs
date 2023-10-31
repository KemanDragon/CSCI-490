using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using MySql.Data.MySqlClient;

namespace _490Bot.Handlers.PermissionsHandler {
    public sealed class PassivePermissionsHandler {
        IGuild server;
        MySqlConnection _connection;
        String connectionString = "server=127.0.0.1;uid=root;pwd=root;database=CSCI-490";

        async void updatePerms(int userID, int value) {
            _connection = new MySqlConnection(connectionString);
            await _connection.OpenAsync();
        }
    }
}
