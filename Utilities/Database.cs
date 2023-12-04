using MySql.Data.MySqlClient;

using _490Bot.Handlers;
using Discord;

namespace _490Bot.Utilities
{
    public class Database
    {
        private readonly MySqlConnection _connection = new("server=127.0.0.1;uid=root;pwd=root;database=CSCI-490");

        public async Task OpenConnection()
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

        public async Task CloseConnection()
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

        public async Task InsertProfile(Profile profile)
        {
            try
            {
                await OpenConnection();
                MySqlCommand query = new()
                {
                    CommandText = $"INSERT INTO profile (MemberID, Username, DisplayName, Status, About, Level, CurrentExp, NeededExp, Color, DateJoined) VALUES(@UserID, @Username, @Name, ' ', ' ', 1, 0, 100, '000000', @DateJoined);",
                    Connection = _connection
                };
                query.Parameters.AddWithValue("@UserID", profile.UserID);
                query.Parameters.AddWithValue("@Username", profile.Username);
                query.Parameters.AddWithValue("@Name", profile.Name);
                query.Parameters.AddWithValue("@DateJoined", profile.DateJoined);
                await query.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            await CloseConnection();
        }

        public async Task InsertPermissions(ulong userID)
        {
            try
            {
                await OpenConnection();
                MySqlCommand query = new()
                {
                    CommandText = $"INSERT INTO Permissions VALUES({userID}, 1);",
                    Connection = _connection
                };
                await query.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            await CloseConnection();
        }

        public async Task<Profile> GetProfile(ulong userID)
        {
            Profile profile = new()
            {
                UserID = userID
            };
            try
            {
                await OpenConnection();
                MySqlCommand query = new()
                {
                    CommandText = $"SELECT * FROM Profile WHERE MemberID={userID};",
                    Connection = _connection
                };
                
                MySqlDataReader reader = (MySqlDataReader)await query.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    profile.Username = reader.GetString(1);
                    profile.Name = reader.GetString(2);
                    profile.StatusField = reader.GetString(3);
                    profile.AboutField = reader.GetString(4);
                    profile.Level = reader.GetInt32(5);
                    profile.ExperienceCurrent = reader.GetInt32(6);
                    profile.ExperienceNeeded = reader.GetInt32(7);
                    profile.Color = reader.GetString(8);
                }
                await reader.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            await CloseConnection();
            return profile;
        }

        public async Task<bool> CheckIfProfileExists(ulong userID)
        {
            await OpenConnection();
            MySqlCommand query = new()
            {
                CommandText = $"SELECT COUNT(*) FROM Profile WHERE MemberID = {userID};",
                Connection = _connection
            };

            var result = await query.ExecuteScalarAsync();

            if (result != null)
            {
                int userExists = Convert.ToInt32(result);
                if (userExists == 0)
                {
                    await CloseConnection();
                    return false;
                }
                else
                {
                    await CloseConnection();
                    return true;
                }
            }
            else
            {
                await CloseConnection();
                return true; // Just in case something weird happens, default to returning true to avoid duplicate entries
            }
        }

        public async Task<int> GetPermissionLevel(ulong userID)
        {
            int level = 0;
            try
            {
                await OpenConnection();
                MySqlCommand query = new()
                {
                    CommandText = $"SELECT PermLevel FROM Permissions WHERE MemberID=@UserID;",
                    Connection = _connection
                };

                query.Parameters.AddWithValue("@UserID", userID);
                MySqlDataReader reader = (MySqlDataReader)await query.ExecuteReaderAsync();
                while (await reader.ReadAsync()) {
                    level = (int)reader["PermLevel"];
                }
                await reader.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            await CloseConnection();
            return level;
        }

        public async Task UpdateProfile(Profile profile)
        {
            try
            {
                await OpenConnection();
                MySqlCommand query = new()
                {
                    CommandText = $"UPDATE Profile SET Status = {profile.StatusField}, Username = {profile.Username}, DisplayName = {profile.Name} About = {profile.AboutField}, Level = {profile.Level}, CurrentExp = {profile.ExperienceCurrent}, NeededExp = {profile.ExperienceNeeded}, Color = {profile.Color} WHERE MemberID = {profile.UserID};",
                    Connection = _connection
                };
                await query.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            await CloseConnection();
        }

        public async Task UpdatePermissionLevel(ulong userID, int newPerm)
        {
            try
            {
                await OpenConnection();
                MySqlCommand query = new()
                {
                    CommandText = $"UPDATE Permissions SET PermLevel = {newPerm} WHERE MemberID = {userID}",
                    Connection = _connection
                };
                await query.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            await CloseConnection();
        }

        public async Task Insert(Logs logs)
        {
            try
            {
                await OpenConnection();
                using MySqlCommand query = new MySqlCommand
                {
                    CommandText = "INSERT INTO logs (UserID, LogID, LogLevel, LogMessage, Reason, SomeOtherColumn) " +
                                  "VALUES (@UserID, @LogID, @LogLevel, @LogMessage, @Reason, 0)",
                    Connection = _connection
                };

                query.Parameters.AddWithValue("@UserID", logs.UserID);
                query.Parameters.AddWithValue("@LogID", logs.LogID);
                query.Parameters.AddWithValue("@LogLevel", logs.LogLevel);
                query.Parameters.AddWithValue("@LogMessage", logs.LogMessage);
                query.Parameters.AddWithValue("@Reason", logs.Reason);

                await query.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                await CloseConnection();
            }
        }
    }
}
