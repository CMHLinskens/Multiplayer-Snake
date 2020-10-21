using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Server
{
    internal static class FileReadWriter
    {
        /*
         * Saves all modified and new accounts to the saved-accounts.json file.
         */
        public static void SaveAllAccounts(List<Account> accounts)
        {
            using (var file = File.CreateText(@"..\data\saved-accounts.json"))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, accounts);
                file.Close();
            }
        }

        /*
         * Retrieves all saved accounts in the saved-accounts.json file.
         */
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
                        accounts = serializer.Deserialize<List<Account>>(jsonTextReader) ?? new List<Account>();
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
