using _490Bot.Utilities;

namespace _490Bot.Handlers {
    public class PassivePermissionsHandler {
        private readonly Database _database = new();

        public async Task<int> GetPerms(ulong userID) {
            return await _database.GetPermissionLevel(userID);
        }

        public async Task UpdatePerms(ulong userID, int value) {
            await _database.UpdatePermissionLevel(userID, value);
        }
    }
}
