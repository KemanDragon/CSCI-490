using _490Bot.Utilities;

namespace _490Bot.Handlers
{
    public class PassivePermissionsHandler
    {
        private static readonly Database _database = new();

        public static async Task<int> GetPerms(ulong userID)
        {
            return await _database.GetPermissionLevel(userID);
        }

        public static async Task UpdatePerms(ulong userID, int value)
        {
            await _database.UpdatePermissionLevel(userID, value);
        }
    }
}