using System;
using System.Collections.Generic;

namespace Teku
{
    public enum UserRole
    {
        Guest,
        Student,
        Teacher,
        Admin
    }

    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
        public string Group { get; set; } // For students
    }

    public static class AuthService
    {
        private static List<User> _users = new List<User>
        {
            new User { Username = "teacher1", Password = "pass", Role = UserRole.Teacher },
            new User { Username = "student1", Password = "pass", Role = UserRole.Student, Group = "Group A" }
        };

        public static User CurrentUser { get; private set; } = new User { Role = UserRole.Guest };

        public static bool Login(string username, string password)
        {
            var user = _users.Find(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                CurrentUser = user;
                return true;
            }
            return false;
        }

        public static void Logout()
        {
            CurrentUser = new User { Role = UserRole.Guest };
        }

        public static bool Register(string username, string password, UserRole role, string group = null)
        {
            if (_users.Exists(u => u.Username == username))
                return false;

            _users.Add(new User { Username = username, Password = password, Role = role, Group = group });
            return true;
        }
    }
}