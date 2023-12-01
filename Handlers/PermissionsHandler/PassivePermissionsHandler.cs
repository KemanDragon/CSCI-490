using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Discord;

using MySql.Data.MySqlClient;

using _490Bot.Utilities;

namespace _490Bot.Handlers.PermissionsHandler {
    public class PassivePermissionsHandler {
        private readonly Database _database = new();

        public async void updatePerms(ulong userID, int value) {
            await _database.UpdatePermissionLevel(userID, value);
        }
    }
}
