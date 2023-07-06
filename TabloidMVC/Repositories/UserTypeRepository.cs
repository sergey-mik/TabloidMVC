using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using TabloidMVC.Models;
using Microsoft.Data.SqlClient;
using System.Xml;

namespace TabloidMVC.Repositories
{
	public class UserTypeRepository : BaseRepository, IUserTypeRepository
    {
		public UserTypeRepository(IConfiguration config) : base(config) { }

        public List<UserType> GetAllTypes()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, Name
                    FROM UserType";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<UserType> userTypes = new List<UserType>();

                        while (reader.Read())
                        {
                            UserType userType = new UserType()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                               
                            };
                            userTypes.Add(userType);
                        }

                        return userTypes;
                    }
                }

            }
        }
    }
}

