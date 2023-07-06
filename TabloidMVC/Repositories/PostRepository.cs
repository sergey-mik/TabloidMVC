using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection.PortableExecutable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TabloidMVC.Models;
using TabloidMVC.Utils;

namespace TabloidMVC.Repositories
{
    public class PostRepository : BaseRepository, IPostRepository
    {
        public PostRepository(IConfiguration config) : base(config) { }
        public List<Post> GetAllPublishedPosts()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                       SELECT p.Id, p.Title, p.Content, 
                              p.ImageLocation AS HeaderImage,
                              p.CreateDateTime, p.PublishDateTime, p.IsApproved,
                              p.CategoryId, p.UserProfileId,
                              c.[Name] AS CategoryName,
                              u.FirstName, u.LastName, u.DisplayName, 
                              u.Email, u.CreateDateTime, u.ImageLocation AS AvatarImage,
                              u.UserTypeId, 
                              ut.[Name] AS UserTypeName
                         FROM Post p
                              LEFT JOIN Category c ON p.CategoryId = c.id
                              LEFT JOIN UserProfile u ON p.UserProfileId = u.id
                              LEFT JOIN UserType ut ON u.UserTypeId = ut.id
                        WHERE PublishDateTime < SYSDATETIME()
                        ORDER BY p.PublishDateTime DESC";
                    var reader = cmd.ExecuteReader();

                    var posts = new List<Post>();

                    while (reader.Read())
                    {
                        posts.Add(NewPostFromReader(reader));
                    }

                    reader.Close();

                    return posts;
                }
            }
        }

        public Post GetPublishedPostById(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                       SELECT p.Id, p.Title, p.Content, 
                              p.ImageLocation AS HeaderImage,
                              p.CreateDateTime, p.PublishDateTime, p.IsApproved,
                              p.CategoryId, p.UserProfileId,
                              c.[Name] AS CategoryName,
                              u.FirstName, u.LastName, u.DisplayName, 
                              u.Email, u.CreateDateTime, u.ImageLocation AS AvatarImage,
                              u.UserTypeId, 
                              ut.[Name] AS UserTypeName
                         FROM Post p
                              LEFT JOIN Category c ON p.CategoryId = c.id
                              LEFT JOIN UserProfile u ON p.UserProfileId = u.id
                              LEFT JOIN UserType ut ON u.UserTypeId = ut.id
                        WHERE IsApproved = 1 AND PublishDateTime < SYSDATETIME()
                        
                              AND p.id = @id";

                    cmd.Parameters.AddWithValue("@id", id);
                    var reader = cmd.ExecuteReader();

                    Post post = null;

                    if (reader.Read())
                    {
                        post = NewPostFromReader(reader);
                    }

                    reader.Close();

                    return post;
                }
            }
        }

        public List<Post> GetAllPostsByCurrentUser(int userProfileId)
        {
            using (SqlConnection conn  = Connection)
            { 
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                    SELECT p.id, p.Title, p.Content, p.ImageLocation as 'PostImageLoc', p.CreateDateTime as 'PostCreateTime', 
                                    p.PublishDateTime as 'PostPublishTime', p.IsApproved, p.CategoryId, p.UserProfileId,
                                    up.FirstName, up.LastName, up.DisplayName, up.Email, up.CreateDateTime as 'UserCreateTime', up.ImageLocation as 'UserImageLoc',
                                    up.UserTypeId, up.UserTypeId, c.Name, ut.Name as 'UserTypeName'
                                    FROM Post p
                                    JOIN UserProfile up ON p.UserProfileId = up.Id
                                    JOIN Category c ON p.CategoryId = c.Id
                                    JOIN UserType ut ON up.UserTypeId = ut.Id
                                    WHERE up.Id = @id
                                    ORDER BY up.CreateDateTime ASC";


                    cmd.Parameters.AddWithValue("@id", userProfileId);
                    
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Post> posts = new List<Post>();
                        while (reader.Read())
                        {
                            Post post = new Post()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Content = reader.GetString(reader.GetOrdinal("Content")),
                                ImageLocation = reader.GetString(reader.GetOrdinal("PostImageLoc")),
                                CreateDateTime = reader.GetDateTime(reader.GetOrdinal("PostCreateTime")),
                                PublishDateTime = reader.GetDateTime(reader.GetOrdinal("PostPublishTime")),
                                IsApproved = reader.GetBoolean(reader.GetOrdinal("IsApproved")),
                                CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                                UserProfileId = reader.GetInt32(reader.GetOrdinal("UserProfileId")),
                                Category = new Category()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                                    Name = reader.GetString(reader.GetOrdinal("Name"))
                                },
                                UserProfile = new UserProfile()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("UserProfileId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    DisplayName = reader.GetString(reader.GetOrdinal("DisplayName")),
                                    Email = reader.GetString(reader.GetOrdinal("Email")),
                                    CreateDateTime = reader.GetDateTime(reader.GetOrdinal("UserCreateTime")),
                                    UserTypeId = reader.GetInt32(reader.GetOrdinal("UserTypeId")),
                                    UserType = new UserType()
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("UserTypeId")),
                                        Name = reader.GetString(reader.GetOrdinal("UserTypeName"))
                                    }
                                }
                            };
                            if (!reader.IsDBNull(reader.GetOrdinal("UserImageLoc")))
                            {
                                post.UserProfile.ImageLocation = reader.GetString(reader.GetOrdinal("UserImageLoc"));
                            }

                            posts.Add(post);
                        }
                        return posts;
                    }
                }
            }
        }

        public Post GetUserPostById(int id, int userProfileId)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                       SELECT p.Id, p.Title, p.Content, 
                              p.ImageLocation AS HeaderImage,
                              p.CreateDateTime, p.PublishDateTime, p.IsApproved,
                              p.CategoryId, p.UserProfileId,
                              c.[Name] AS CategoryName,
                              u.FirstName, u.LastName, u.DisplayName, 
                              u.Email, u.CreateDateTime, u.ImageLocation AS AvatarImage,
                              u.UserTypeId, 
                              ut.[Name] AS UserTypeName
                         FROM Post p
                              LEFT JOIN Category c ON p.CategoryId = c.id
                              LEFT JOIN UserProfile u ON p.UserProfileId = u.id
                              LEFT JOIN UserType ut ON u.UserTypeId = ut.id
                        WHERE p.id = @id AND p.UserProfileId = @userProfileId";

                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@userProfileId", userProfileId);
                    var reader = cmd.ExecuteReader();

                    Post post = null;

                    if (reader.Read())
                    {
                        post = NewPostFromReader(reader);
                    }

                    reader.Close();

                    return post;
                }
            }
        }

        public void Add(Post post)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO Post (
                            Title, Content, ImageLocation, CreateDateTime, PublishDateTime,
                            IsApproved, CategoryId, UserProfileId )
                        OUTPUT INSERTED.ID
                        VALUES (
                            @Title, @Content, @ImageLocation, @CreateDateTime, @PublishDateTime,
                            @IsApproved, @CategoryId, @UserProfileId )";
                    cmd.Parameters.AddWithValue("@Title", post.Title);
                    cmd.Parameters.AddWithValue("@Content", post.Content);
                    cmd.Parameters.AddWithValue("@ImageLocation", DbUtils.ValueOrDBNull(post.ImageLocation));
                    cmd.Parameters.AddWithValue("@CreateDateTime", post.CreateDateTime);
                    cmd.Parameters.AddWithValue("@PublishDateTime", DbUtils.ValueOrDBNull(post.PublishDateTime));
                    cmd.Parameters.AddWithValue("@IsApproved", post.IsApproved);
                    cmd.Parameters.AddWithValue("@CategoryId", post.CategoryId);
                    cmd.Parameters.AddWithValue("@UserProfileId", post.UserProfileId);

                    post.Id = (int)cmd.ExecuteScalar();
                }
            }
        }
        public void DeletePost(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Post WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void EditPost(Post post)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "UPDATE Post Set Title = @title, Content = @content, CategoryId = @categoryId, ImageLocation = @image, PublishDateTime = @pubDate WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", post.Id);
                    cmd.Parameters.AddWithValue("@title", post.Title);
                    cmd.Parameters.AddWithValue("@content", post.Content);
                    cmd.Parameters.AddWithValue("@categoryId", post.CategoryId);
                    cmd.Parameters.AddWithValue("@image", post.ImageLocation);
                    cmd.Parameters.AddWithValue("@pubDate", post.PublishDateTime);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        private Post NewPostFromReader(SqlDataReader reader)
        {
            return new Post()
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Content = reader.GetString(reader.GetOrdinal("Content")),
                ImageLocation = DbUtils.GetNullableString(reader, "HeaderImage"),
                CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime")),
                PublishDateTime = DbUtils.GetNullableDateTime(reader, "PublishDateTime"),
                CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                IsApproved = reader.GetBoolean(reader.GetOrdinal("IsApproved")),
                Category = new Category()
                {
                    Id = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                    Name = reader.GetString(reader.GetOrdinal("CategoryName"))
                },
                UserProfileId = reader.GetInt32(reader.GetOrdinal("UserProfileId")),
                UserProfile = new UserProfile()
                {
                    Id = reader.GetInt32(reader.GetOrdinal("UserProfileId")),
                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                    DisplayName = reader.GetString(reader.GetOrdinal("DisplayName")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime")),
                    ImageLocation = DbUtils.GetNullableString(reader, "AvatarImage"),
                    UserTypeId = reader.GetInt32(reader.GetOrdinal("UserTypeId")),
                    UserType = new UserType()
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("UserTypeId")),
                        Name = reader.GetString(reader.GetOrdinal("UserTypeName"))
                    }
                }
            };
        }

        public int GetWordCount(Post post)
        {
            return post.Content.Split(new char[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        public int GetEstimatedReadingTime(Post post)
        {
            int wordCount = GetWordCount(post);
            double readingTimeInMinutes = (double)wordCount / 265;
            return (int)Math.Ceiling(readingTimeInMinutes);
        }

        public void ToggleApproval(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                UPDATE Post 
                SET IsApproved = CASE WHEN IsApproved = 1 THEN 0 ELSE 1 END 
                WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
