using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using SimpleCrypto;

namespace Server
{
    public static class LoginEncryption
    {
        static LoginEncryptionDataContext db;
        static ICryptoService cryptoService;
        static LoginEncryption()
        {
            db = new LoginEncryptionDataContext();
            cryptoService = new PBKDF2();
        }

        public static void CreateUser(string username)
        {
            Console.WriteLine("New user creation.");
            bool validNewUser;
            string password = "";
            do
            {
                try
                {
                    Console.WriteLine("Password: ");
                    password = Console.ReadLine();
                    Console.WriteLine("Confirm password: ");
                    if (password == Console.ReadLine())
                    {
                        validNewUser = true;
                    }
                    else
                    {
                        validNewUser = false;
                        Console.WriteLine("Please confirm your passwords match.");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    validNewUser = false;
                }
            }
            while (validNewUser == false);

            User user = new User
            {
                UserName = username,
                Salt = cryptoService.GenerateSalt(),
                Password = cryptoService.Compute(password)
            };
            db.Users.InsertOnSubmit(user);
            db.SubmitChanges();
        }
        public static User ExistingUserLogIn(string username)
        {
            Console.WriteLine("Existing user login.");
            bool validLogin;
            string password = "";
            User user = new User();
            do
            {
                try
                {
                    Console.WriteLine("Password: ");
                    password = Console.ReadLine();

                    user = db.Users.Where(u => u.UserName == username).Single();

                    string hashedPassword = cryptoService.Compute(password, user.Salt);
                    validLogin = cryptoService.Compare(hashedPassword, user.Password);

                    if (validLogin == false)
                    {
                        Console.WriteLine("Invalid password. Please try again.");
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    validLogin = false;
                }
            }
            while (validLogin == false);
            return user;
        }

        public static bool CheckForUser(string username)
        {
            var users = db.Users.Where(u => u.UserName == username);

            if (users.Count() == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
