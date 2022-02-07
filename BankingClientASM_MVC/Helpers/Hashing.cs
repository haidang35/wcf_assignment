﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BankingClientASM_MVC.Helpers
{
    public class Hashing
    {
        public static string Hash(string value)
        {
            return Convert.ToBase64String(
                System.Security.Cryptography.SHA256.Create().
                ComputeHash(System.Text.Encoding.UTF8.GetBytes(value))
                );
        }

        public static bool CheckHash(string value, string valueHashing)
        {
            return string.Compare(Hash(value), valueHashing) == 0;
        }
    }
}