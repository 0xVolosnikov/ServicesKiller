using Onstogram.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Onstogram.DataLayer.SQL
{
    public class DataLayer : IDataLayer
    {
        private readonly string _connectionString;

        public DataLayer(string connectionString)
        {
            if (connectionString == null)
                throw new ArgumentNullException(nameof(connectionString));

            _connectionString = connectionString;
        }

        public Comment AddCommentToImage(Comment comment, Guid imageId)
        {
            throw new NotImplementedException();
        }

        public Image AddImage(Image image)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    image.Id = Guid.NewGuid();
                    command.CommandText = "insert into images (id, [user id], picture, time) values (@id, @uid, @picture, @time)";
                    command.Parameters.AddWithValue("@id", image.Id);
                    command.Parameters.AddWithValue("@uid", image.UserId);
                    command.Parameters.AddWithValue("@picture", image.Picture);
                    command.Parameters.AddWithValue("@time", image.Time);
                    command.ExecuteNonQuery();
                    return image;
                }
            }
        }

        public User AddUser(User user)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    user.Id = Guid.NewGuid();
                    command.CommandText = "insert into users (id, nickname) values (@id, @nickname)";
                    command.Parameters.AddWithValue("@id", user.Id);
                    command.Parameters.AddWithValue("@nickname", user.NickName);
                    command.ExecuteNonQuery();
                    return user;
                }
            }
        }

        public Comment DeleteComment(Guid commentId)
        {
            throw new NotImplementedException();
        }

        public Image DeleteImage(Guid imageId)
        {
            throw new NotImplementedException();
        }

        public Image GetImage(Guid imageId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, picture, time from images where id = @id";
                    command.Parameters.AddWithValue("@id", imageId);
                    using (var reader = command.ExecuteReader())
                    {
                        reader.Read();
                        var img = new Image();
                        img.Id = reader.GetGuid(0);
                        img.Picture = new byte[((byte[])reader["picture"]).Length];
                        img.Picture = (byte[])reader["picture"];
                        img.Time = reader.GetDateTime(2);
                        return img;
                    }

                }
            }
        }

        public Comment[] GetImageComments(Guid imageId)
        {
            throw new NotImplementedException();
        }

        public Image[] GetLastImages()
        {
            throw new NotImplementedException();
        }

        public User GetUser(Guid id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "select id, nickname from users where id = @id";
                    command.Parameters.AddWithValue("@id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        reader.Read();
                        return new User
                        {
                            Id = reader.GetGuid(0),
                            NickName = reader.GetString(1)
                        };
                    }
                    
                }
            }
        }


    }
}
