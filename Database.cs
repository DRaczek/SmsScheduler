using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmsScheduler
{
    public class Database
    {
        private static Database _instance;

        private readonly SQLiteAsyncConnection _database;

        private Database(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Contact>().Wait();
            _database.CreateTableAsync<Group>().Wait();
            _database.CreateTableAsync<GroupContact>().Wait();
            _database.CreateTableAsync<DelayedSms>().Wait();
        }

        public static Database GetInstance(string dbPath)
        {
            if (_instance == null)
            {
                _instance = new Database(dbPath);
            }
            return _instance;
        }

        public Task<List<Contact>> GetContactsAsync() =>
            _database.Table<Contact>().ToListAsync();

        public Task<int> SaveContactAsync(Contact contact) =>
            _database.InsertAsync(contact);

        public Task<int> DeleteContactAsync(Contact contact) =>
            _database.DeleteAsync(contact);

        public Task<List<Group>> GetGroupsAsync() =>
            _database.Table<Group>().ToListAsync();

        public Task<Group> GetGroupAsync(int groupId) =>
            _database.GetAsync<Group>(groupId);

        public Task<int> SaveGroupAsync(Group group) =>
            _database.InsertAsync(group);
        public Task<int> UpdateGroupAsync(Group group) =>
            _database.UpdateAsync(group);

        public Task<int> SaveGroupContactAsync(GroupContact gc) =>
            _database.InsertAsync(gc);

        public Task<List<Contact>> GetContactsForGroupAsync(int groupId) =>
            _database.QueryAsync<Contact>(
                "SELECT c.* FROM Contact c " +
                "INNER JOIN GroupContact gc ON c.Id = gc.ContactId " +
                "WHERE gc.GroupId = ?", groupId);

        public Task<int> DeleteGroupAsync(Group group) =>
            _database.DeleteAsync(group);
           
        public Task<int> DeleteGroupContactAsync(GroupContact groupContact) =>
    _database.DeleteAsync(groupContact);

        public Task<GroupContact?> GetGroupContactAsync(int groupId, int contactId) =>
    _database.Table<GroupContact>()
             .Where(gc => gc.GroupId == groupId && gc.ContactId == contactId)
             .FirstOrDefaultAsync();

        public Task<int> SaveDalayedSms(DelayedSms sms) =>
            _database.InsertAsync(sms);

        public Task<List<DelayedSms>> GetDalayedSmses() =>
    _database.Table<DelayedSms>().ToListAsync();

        public Task<int> DeleteDelayedSms(DelayedSms sms) =>
    _database.DeleteAsync(sms);
    }
}
