using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using TabloidMVC.Models;
using System.Linq;

namespace TabloidMVC.Repositories
{
    public class TagRepository : ITagRepository 
    {
        private string _connectionString;
        public TagRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        protected SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_connectionString);
            }
        }

        public List<Tag> GetAllTags()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Tag ORDER BY Name ASC";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Tag> tags = new List<Tag>();
                        while (reader.Read())
                        {
                            Tag tag = new Tag()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            };

                            tags.Add(tag);
                        }
                        return tags;
                    }
                }
            }
        }

        public Tag GetTagById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM Tag WHERE Tag.Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Tag tag = new Tag()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                            };

                            return tag;
                        }
                        else { return null; }
                    }
                }
            }
        }

        public void AddTag(Tag tag)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO Tag ([Name]) OUTPUT INSERTED.Id VALUES (@name)";
                    cmd.Parameters.AddWithValue("@name", tag.Name);

                    int newlyCreatedId = (int)cmd.ExecuteScalar();

                    tag.Id = newlyCreatedId;
                }
            }
        }

        public void UpdateTag(Tag tag)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE Tag SET [Name] = @name WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", tag.Id);
                    cmd.Parameters.AddWithValue("@name", tag.Name);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void DeleteTag(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Tag Where Id = @id";

                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
