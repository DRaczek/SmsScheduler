using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsScheduler
{
    public class Contact
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(200), NotNull]
        public string Name { get; set; }

        [MaxLength(20), NotNull]
        public string PhoneNumber { get; set; }
    }

    public class Group
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(100), NotNull]
        public string Name { get; set; }
    }

    public class GroupContact
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int GroupId { get; set; }

        [Indexed]
        public int ContactId { get; set; }
    }

    public class DelayedSms
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed]
        public int GroupId { get; set; }

        public string Message { get; set; }

        public DateTime DateTime { get; set; }

        private List<Contact> contacts;

        [Ignore]
        public List<Contact> Contacts
        {
            get { return contacts; }
            set { 
                contacts = value;
                if(contacts != null) ContactsCount = contacts.Count;
            }
        }

        [Ignore]
        public int ContactsCount { get; set; }

    }



}
