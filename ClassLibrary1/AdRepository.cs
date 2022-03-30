using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class AdRepository
    {
        private readonly string _ConnString;
        public AdRepository(string conn)
        {
            _ConnString = conn;
        }
        public void AddPerson(string name, string email, string password)
        {
            using var connection = new SqlConnection(_ConnString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"insert into person 
                            values(@name, @email, @hash)";
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@hash", BCrypt.Net.BCrypt.HashPassword(password));

            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public User GetByEmail(string email)
        {
            using var connection = new SqlConnection(_ConnString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"select * from person p where p.email = @email";
            cmd.Parameters.AddWithValue("@email", email);
            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    Id = (int)reader["id"],
                    Name = (string)reader["name"],
                    Email = (string)reader["email"],
                    HashPassword = (string)reader["password"]
                };
            };
            return null;
        }
        public User LogIn(string email, string password)
        {
            User person = GetByEmail(email);
            if (person == null)
            {
                return null;
            }
            bool isValid = BCrypt.Net.BCrypt.Verify(password, person.HashPassword);
            return isValid ? person : null;
        }
        public void AddAd(string phone, string description, string email)
        {
            var user = GetByEmail(email);
            using var connection = new SqlConnection(_ConnString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"insert into ad 
                                values(@phone, @desc, @pId, GETDATE())";
            cmd.Parameters.AddWithValue("@phone", phone);
            cmd.Parameters.AddWithValue("@desc", description);
            cmd.Parameters.AddWithValue("@pId", user.Id);

            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public List<Ad> GetAds()
        {
            using var connection = new SqlConnection(_ConnString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"select * from ad a
                                join person p 
                                on a.personId = p.id";
            connection.Open();
            List<Ad> ads = new();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ads.Add(new Ad
                {
                    Id = (int)reader["Id"],
                    Phone = (string)reader["phone"],
                    Description = (string)reader["description"],
                    DateSubmitted = (DateTime)reader["datesubmitted"],
                    UserName = (string)reader["name"],
                    UserId = (int)reader["id"],
                    Email = (string)reader["Email"]
                });
            }
            return ads;
        }
        public void DeleteAd(int id)
        {
            using var connection = new SqlConnection(_ConnString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"delete from ad where id = @id";
            cmd.Parameters.AddWithValue("@id", id);

            connection.Open();
            cmd.ExecuteNonQuery();
        }
        public List<Ad> GetLogedInUsersAds(int id)
        {
            using var connection = new SqlConnection(_ConnString);
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"select * from ad a
                                join person p 
                                on a.personId = p.id
                                where p.id = @id";
            cmd.Parameters.AddWithValue("@id", id);
            connection.Open();
            List<Ad> ads = new();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ads.Add(new Ad
                {
                    Id = (int)reader["Id"],
                    Phone = (string)reader["phone"],
                    Description = (string)reader["description"],
                    DateSubmitted = (DateTime)reader["datesubmitted"],
                    UserName = (string)reader["name"],
                    UserId = (int)reader["id"],
                    Email = (string)reader["Email"]
                });
            }
            return ads;
        }
    }
}

