using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class Account
    {
        public string ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int GamesWon { get; set; }
        public int GamesLost { get; set; }

        /*
         * Use this method for creating new accounts.
         */
        public Account(string iD, string username, string password)
        {
            ID = iD;
            Username = username;
            Password = password;
            GamesWon = 0;
            GamesLost = 0;
        }
    }
}
