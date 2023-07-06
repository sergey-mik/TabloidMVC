using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using TabloidMVC.Models;
using TabloidMVC.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting;
using System;

namespace TabloidMVC.Repositories
{
    public class UserProfileRepository : BaseRepository, IUserProfileRepository
    {
        public UserProfileRepository(IConfiguration config) : base(config) { }

        public UserProfile GetByEmail(string email)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                       SELECT u.id, u.FirstName, u.LastName, u.DisplayName, u.Email,
                              u.CreateDateTime, u.ImageLocation, u.UserTypeId,
                              ut.[Name] AS UserTypeName
                         FROM UserProfile u
                              LEFT JOIN UserType ut ON u.UserTypeId = ut.id
                        WHERE email = @email";
                    cmd.Parameters.AddWithValue("@email", email);

                    UserProfile userProfile = null;
                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        userProfile = new UserProfile()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DisplayName = reader.GetString(reader.GetOrdinal("DisplayName")),
                            CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime")),
                            ImageLocation = DbUtils.GetNullableString(reader, "ImageLocation"),
                            UserTypeId = reader.GetInt32(reader.GetOrdinal("UserTypeId")),
                            UserType = new UserType()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("UserTypeId")),
                                Name = reader.GetString(reader.GetOrdinal("UserTypeName"))
                            },
                        };
                    }

                    reader.Close();

                    return userProfile;
                }
            }
        }

        public UserProfile GetByUserId(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                       SELECT u.id, u.FirstName, u.LastName, u.DisplayName, u.Email,
                              u.CreateDateTime, u.ImageLocation, u.UserTypeId,
                              ut.[Name] AS UserTypeName
                         FROM UserProfile u
                              LEFT JOIN UserType ut ON u.UserTypeId = ut.id
                        WHERE u.id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    UserProfile userProfile = null;
                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        userProfile = new UserProfile()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DisplayName = reader.GetString(reader.GetOrdinal("DisplayName")),
                            CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime")),
                            UserTypeId = reader.GetInt32(reader.GetOrdinal("UserTypeId")),
                            UserType = new UserType()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("UserTypeId")),
                                Name = reader.GetString(reader.GetOrdinal("UserTypeName"))
                            },
                        };

                        if (!reader.IsDBNull(reader.GetOrdinal("ImageLocation")))
                        {
                            userProfile.ImageLocation = reader.GetString(reader.GetOrdinal("ImageLocation"));
                        }
                    }

                    reader.Close();

                    return userProfile;
                }
            }
        }

        public List<UserProfile> GetUserProfiles()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT UserProfile.Id as 'UserId', FirstName, LastName, DisplayName, Email, CreateDateTime, ImageLocation, UserTypeId, UserType.Name as 'UserTypeName'
                     FROM UserProfile
                     JOIN UserType
                     on UserProfile.UserTypeId=UserType.Id
                     ORDER BY DisplayName ASC";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<UserProfile> userProfiles = new List<UserProfile>();

                        while (reader.Read())
                        {
                            UserProfile userProfile = new UserProfile()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("UserId")),
                                DisplayName = reader.GetString(reader.GetOrdinal("DisplayName")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime")),
                                UserTypeId = reader.GetInt32(reader.GetOrdinal("UserTypeId")),
                                UserType = new UserType
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("UserTypeId")),
                                    Name = reader.GetString(reader.GetOrdinal("UserTypeName")),
                                }
                            };
                            userProfiles.Add(userProfile);
                        }

                        return userProfiles;
                    }
                }

            }
        }

        public void DeactivateProfile(UserProfile profile)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (profile.UserTypeId == 1)
                    {
                        cmd.CommandText = @"UPDATE UserProfile
                                        SET UserTypeId = 3
                                        WHERE Id = @id";
                        cmd.Parameters.AddWithValue("@id", profile.Id);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        cmd.CommandText = @"UPDATE UserProfile
                                        SET UserTypeId = 4
                                        WHERE Id = @id";
                        cmd.Parameters.AddWithValue("@id", profile.Id);
                        cmd.ExecuteNonQuery();
                    }
                }

            }
        }

        public void ActivateProfile(UserProfile profile)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (profile.UserTypeId == 3)
                    {
                        cmd.CommandText = @"UPDATE UserProfile
                                        SET UserTypeId = 1
                                        WHERE Id = @id";
                        cmd.Parameters.AddWithValue("@id", profile.Id);
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        cmd.CommandText = @"UPDATE UserProfile
                                        SET UserTypeId = 2
                                        WHERE Id = @id";
                        cmd.Parameters.AddWithValue("@id", profile.Id);
                        cmd.ExecuteNonQuery();
                    }
                }

            }
        }

        public List<UserProfile> GetDeactivatedUserProfiles()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT UserProfile.Id as 'UserId', FirstName, LastName, DisplayName, Email, CreateDateTime, ImageLocation, UserTypeId, UserType.Name as 'UserTypeName'
                     FROM UserProfile
                     JOIN UserType
                     on UserProfile.UserTypeId=UserType.Id
                     WHERE UserTypeId = 3
                     OR UserTypeId = 4
                     ORDER BY DisplayName ASC";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<UserProfile> userProfiles = new List<UserProfile>();

                        while (reader.Read())
                        {
                            UserProfile userProfile = new UserProfile()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("UserId")),
                                DisplayName = reader.GetString(reader.GetOrdinal("DisplayName")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime")),
                                UserTypeId = reader.GetInt32(reader.GetOrdinal("UserTypeId")),
                                UserType = new UserType
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("UserTypeId")),
                                    Name = reader.GetString(reader.GetOrdinal("UserTypeName")),
                                }
                            };
                            userProfiles.Add(userProfile);
                        }

                        return userProfiles;
                    }
                }

            }
        }

        public void UpdateProfile(UserProfile profile)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                        UPDATE UserProfile
                                        SET
                                         DisplayName = @displayName,
                                         FirstName = @firstName,
                                         LastName = @lastName,
                                         Email = @email,
                                         ImageLocation = @imageLoc,
                                         UserTypeId = @userTypeId
                                        WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@displayName", profile.DisplayName);
                    cmd.Parameters.AddWithValue("@firstName", profile.FirstName);
                    cmd.Parameters.AddWithValue("@lastName", profile.LastName);
                    cmd.Parameters.AddWithValue("@email", profile.Email);
                    cmd.Parameters.AddWithValue("@userTypeId", profile.UserTypeId);
                    cmd.Parameters.AddWithValue("@id", profile.Id);

                    if (profile.ImageLocation == "" || profile.ImageLocation == null)
                    {
                        cmd.Parameters.AddWithValue("@imageLoc", DBNull.Value);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@imageLoc", profile.ImageLocation);
                    }


                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void AddUser(UserProfile userProfile)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                        INSERT INTO UserProfile (FirstName, LastName, DisplayName, Email, CreateDateTime, ImageLocation, UserTypeId)
                                        OUTPUT INSERTED.Id
                                        VALUES (@firstName, @lastName, @displayName, @email, @createDateTime, @imageLocation, @userTypeId)";
                    cmd.Parameters.AddWithValue("@firstName", userProfile.FirstName);
                    cmd.Parameters.AddWithValue("@lastName", userProfile.LastName);
                    cmd.Parameters.AddWithValue("@displayName", userProfile.DisplayName);
                    cmd.Parameters.AddWithValue("@email", userProfile.Email);
                    cmd.Parameters.AddWithValue("@createDateTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@imageLocation", DBNull.Value);
                    cmd.Parameters.AddWithValue("@userTypeId", 2);
                    int id = (int)cmd.ExecuteScalar();
                    userProfile.Id = id;
                }
            }
        }
    }
}
