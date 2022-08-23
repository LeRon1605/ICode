using System;
using System.Collections.Generic;
using System.Text;

namespace CodeStudy.Models
{
    public class UserDTO
    {
        public string ID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
        public string Gender { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public override bool Equals(object obj)
        {
            UserDTO user = obj as UserDTO;
            if (user == null)
            {
                return false;
            }
            else
            {
                return user.ID == ID;
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ID, Username, Email, Avatar, Gender, CreatedAt, UpdatedAt);
        }
    }
}
