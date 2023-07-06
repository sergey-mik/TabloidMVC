using TabloidMVC.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace TabloidMVC.Repositories
{
    public class ReactionRepository : BaseRepository, IReactionRepository
    {
        public ReactionRepository(IConfiguration config) : base(config) { }
        public void CreateReaction(Reaction r)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                        INSERT INTO Reaction ([Name], ImageLocation)
                                        OUTPUT INSERTED.Id
                                        VALUES (@name, @imageLocation)";
                    cmd.Parameters.AddWithValue("@name", r.Name);
                    cmd.Parameters.AddWithValue("@imageLocation", r.ImageLocation);
                    int id = (int)cmd.ExecuteScalar();
                    r.Id = id;
                }
            }
        }
        public List<Reaction> GetAll()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                        SELECT [Name], ImageLocation FROM Reaction";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Reaction> reactions = new List<Reaction>();
                        while (reader.Read())
                        {
                            Reaction reaction = new Reaction()
                            {
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                ImageLocation = reader.GetString(reader.GetOrdinal("ImageLocation"))
                            };
                            reactions.Add(reaction);
                        }
                        return reactions;
                    }
                }
            }
        }
    }
}
