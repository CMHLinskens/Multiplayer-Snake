﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Server
{
    internal static class FileReadWriter
    {
        public static void SaveAllAccounts(List<Account> accounts)
        {
            using (var file = File.CreateText(@"..\data\saved-accounts.json"))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, accounts);
                file.Close();
            }
        }

        public static List<Account> RetrieveAllAccounts()
        {
            if (File.Exists(@"..\data\saved-accounts.json"))
            {
                List<Account> accounts;
                using (var file = new StreamReader(@"..\data\saved-accounts.json"))
                {
                    using (var jsonTextReader = new JsonTextReader(file))
                    {
                        var serializer = new JsonSerializer();
                        accounts = serializer.Deserialize<List<Account>>(jsonTextReader);
                        jsonTextReader.Close();
                    }
                    file.Close();
                }
                return accounts;
            }
            Directory.CreateDirectory(@"..\data");
            File.CreateText(@"..\data\saved-accounts.json").Dispose();
            return new List<Account>();
        }
    }
}
