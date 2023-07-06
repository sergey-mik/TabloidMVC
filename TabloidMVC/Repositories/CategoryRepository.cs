using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using TabloidMVC.Models;
using Microsoft.Data.SqlClient;
using System.Xml;

namespace TabloidMVC.Repositories
{
    public class CategoryRepository : BaseRepository, ICategoryRepository
    {
        public CategoryRepository(IConfiguration config) : base(config) { }
        public List<Category> GetAll()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name FROM Category";
                    var reader = cmd.ExecuteReader();

                    var categories = new List<Category>();

                    while (reader.Read())
                    {
                        categories.Add(new Category()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                        });
                    }

                    reader.Close();

                    return categories;
                }
            }
        }
        public void CreateCategory(Category category)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Category (Name)
                                        OUTPUT INSERTED.Id
                                        VALUES (@name)";
                    cmd.Parameters.AddWithValue("@name", category.Name);
                    int id = (int)cmd.ExecuteScalar();
                    category.Id = id;                  
                }
            }
        }
        public void EditCategory(Category category)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Category
                                        SET [Name] = @name
                                        WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@name", category.Name);
                    cmd.Parameters.AddWithValue("@id", category.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public Category GetCategoryById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Name FROM Category
                                        WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Category category = new Category();
                        if (reader.Read())
                        {
                            category.Name = reader.GetString(reader.GetOrdinal("Name"));
                            category.Id = id;
                        };
                        return category;
                    }
                }
            }
        }
        public void DeleteCategory(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Category WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
