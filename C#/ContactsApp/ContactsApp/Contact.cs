using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace ContactsApp
{
    public class Contact
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public bool IsBlocked { get; set; }
    }
}
