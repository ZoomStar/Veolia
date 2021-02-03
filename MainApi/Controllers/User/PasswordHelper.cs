using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MainApi.Controllers
{
    public static class PasswordHelper
    {
        public static bool CheckPassword(string PassWord, string DatabaseHashPassword)
        {
            string[] CrackedPassWord = DatabaseHashPassword.Split(':');
            var ResultedPassWord = ComputePassWordHash(PassWord, CrackedPassWord[1], int.Parse(CrackedPassWord[0]));
            if (ResultedPassWord == CrackedPassWord[2])
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static string CreatePassword(string Password)
        {
            Guid Salt = Guid.NewGuid();
            Random Pepper = new Random();
            int Interations = Pepper.Next(500, 5000);
            string HashedPassWord = ComputePassWordHash(Password, Salt.ToString(), Interations);
            return string.Format("{0}:{1}:{2}", Interations, Salt.ToString(), HashedPassWord);
        }
        public static string ComputePassWordHash(string Password, string salt, int Interations)
        {
            string TempString = salt + Password;
            for (int y = 0; y < Interations; y++)
            {

                using (SHA256 sha256Hash = SHA256.Create())
                {
                    // ComputeHash - returns byte array  
                    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(TempString));

                    // Convert byte array to a string   
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        builder.Append(bytes[i].ToString("x2"));
                    }
                    TempString = builder.ToString();
                }
            }
            return TempString;
            // Create a SHA256   

        }

    }
}
