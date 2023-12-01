using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using _490Bot.Handlers.ProfileHandler;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace _490Bot {
    public class Database {
        private readonly MySqlConnection _connection = new("server=127.0.0.1;uid=root;pwd=root;database=CSCI-490");

        public async Task OpenConnection() {
            try {
                await _connection.OpenAsync();
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task CloseConnection() {
            try {
                await _connection.CloseAsync();
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task InsertProfile(Profile profile) {
            try {
                await OpenConnection();
                MySqlCommand query = new()
                {
                    CommandText = $"INSERT INTO profile VALUES({profile.UserID}, NULL, NULL, 1, 0, 100)",
                    Connection = _connection
                };
                await query.ExecuteNonQueryAsync();
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }

            await CloseConnection();
        }

        public async Task InsertPermissions(ulong userID) {
            try {
                await OpenConnection();
                MySqlCommand query = new() {
                    CommandText = $"INSERT INTO Permissions VALUES({userID}, 1);",
                    Connection = _connection
                };
                await query.ExecuteNonQueryAsync();
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }
            
            await CloseConnection();
        }

        public async Task<Profile> GetProfile(ulong userID) {
            Profile profile = new() {
                UserID = userID
            };
            try {
                await OpenConnection();
                MySqlCommand query = new() {
                    CommandText = $"SELECT * FROM Profile WHERE MemberID={userID};",
                    Connection = _connection
                };

                MySqlDataReader reader = (MySqlDataReader)await query.ExecuteReaderAsync();
                while (await reader.ReadAsync()) {
                    profile.StatusField = (string)reader["Status"];
                    profile.AboutField = (string)reader["About"];
                    profile.Level = (int)reader["Level"];
                    profile.ExperienceCurrent = (int)reader["CurrentExp"];
                    profile.ExperienceNeeded = (int)reader["NeededExp"];
                }
                await reader.CloseAsync();
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
            }

            await CloseConnection();
            return profile;
        }

        public async Task<int> GetPermissionLevel(ulong userID) {
            int level = 0;
            try {
                await OpenConnection();
                MySqlCommand query = new() {
                    CommandText = $"SELECT PermLevel FROM Permissions WHERE MemberID={userID};",
                    Connection = _connection
                };

                MySqlDataReader reader = (MySqlDataReader)await query.ExecuteReaderAsync(); {
                    level = (int)reader["PermLevel"];
                }
                await reader.CloseAsync();
            } catch (Exception ex) {
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
                    CommandText = $"UPDATE Profile SET Status={profile.StatusField}, About={profile.AboutField}, Level={profile.Level}, CurrentExp={profile.ExperienceCurrent}, NeededExp={profile.ExperienceNeeded} WHERE MemberID={profile.UserID};",
                    Connection = _connection
                };
                await query.ExecuteNonQueryAsync();
            } catch (Exception ex)
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
                    CommandText = $"UPDATE Permissions SET PermLevel={newPerm} WHERE MemberID={userID}",
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
    }
}
