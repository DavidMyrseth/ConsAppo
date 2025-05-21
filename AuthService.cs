using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        public string Group { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }

        public override string ToString()
        {
            return $"{Username}|{Password}|{Role}|{Group}|{FullName}|{Email}";
        }

        public static User FromString(string line)
        {
            var parts = line.Split('|');
            return new User
            {
                Username = parts[0],
                Password = parts[1],
                Role = (UserRole)Enum.Parse(typeof(UserRole), parts[2]),
                Group = parts.Length > 3 ? parts[3] : null,
                FullName = parts.Length > 4 ? parts[4] : null,
                Email = parts.Length > 5 ? parts[5] : null
            };
        }
    }

    public static class AuthService
    {
        private static List<User> _users;
        private static readonly string UsersFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "users.txt");

        public static User CurrentUser { get; private set; } = new User { Role = UserRole.Guest };

        public static event Action UserChanged;
        public static event Action UserLoggedIn;
        public static event Action UserLoggedOut;

        static AuthService()
        {
            LoadUsers();
        }

        private static void LoadUsers()
        {
            _users = new List<User>();

            if (File.Exists(UsersFilePath))
            {
                try
                {
                    foreach (var line in File.ReadAllLines(UsersFilePath))
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            _users.Add(User.FromString(line));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading users: {ex.Message}");
                    CreateDefaultUsers();
                }
            }
            else
            {
                CreateDefaultUsers();
            }
        }

        private static void CreateDefaultUsers()
        {
            _users = new List<User>
            {
                new User
                {
                    Username = "teacher1",
                    Password = "pass",
                    Role = UserRole.Teacher,
                    FullName = "John Smith",
                    Email = "teacher1@school.edu"
                },
                new User
                {
                    Username = "student1",
                    Password = "pass",
                    Role = UserRole.Student,
                    Group = "Group A",
                    FullName = "Alice Johnson",
                    Email = "student1@school.edu"
                },
                new User
                {
                    Username = "admin1",
                    Password = "adminpass",
                    Role = UserRole.Admin,
                    FullName = "Admin Admin",
                    Email = "admin@school.edu"
                }
            };
            SaveUsers();
        }

        private static void SaveUsers()
        {
            try
            {
                File.WriteAllLines(UsersFilePath, _users.Select(u => u.ToString()));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving users: {ex.Message}");
            }
        }

        public static bool Login(string username, string password)
        {
            var user = _users.Find(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                CurrentUser = user;
                UserChanged?.Invoke();
                UserLoggedIn?.Invoke();
                return true;
            }
            return false;
        }

        public static void Logout()
        {
            CurrentUser = new User { Role = UserRole.Guest };
            UserChanged?.Invoke();
            UserLoggedOut?.Invoke();
        }

        public static bool Register(string username, string password, UserRole role, string group = null, string fullName = null, string email = null)
        {
            if (_users.Exists(u => u.Username == username))
                return false;

            var newUser = new User
            {
                Username = username,
                Password = password,
                Role = role,
                Group = group,
                FullName = fullName,
                Email = email
            };

            _users.Add(newUser);
            SaveUsers();
            return true;
        }

        public static bool UpdateUser(User updatedUser)
        {
            var existingUser = _users.Find(u => u.Username == updatedUser.Username);
            if (existingUser == null)
                return false;

            existingUser.Password = updatedUser.Password;
            existingUser.Group = updatedUser.Group;
            existingUser.FullName = updatedUser.FullName;
            existingUser.Email = updatedUser.Email;

            SaveUsers();

            if (CurrentUser.Username == updatedUser.Username)
            {
                CurrentUser = existingUser;
                UserChanged?.Invoke();
            }

            return true;
        }

        public static List<User> GetAllUsers()
        {
            return new List<User>(_users);
        }

        public static bool DeleteUser(string username)
        {
            var userToRemove = _users.Find(u => u.Username == username);
            if (userToRemove == null)
                return false;

            _users.Remove(userToRemove);
            SaveUsers();
            return true;
        }
    }
}