using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using System.Threading.Tasks;

namespace ContactsApp
{
    public class DataHelper
    {
        //Connection to the SQLite DB
        private readonly SQLiteAsyncConnection _db;

        public DataHelper(string dbPath)
        {
            _db = new SQLiteAsyncConnection(dbPath);
            _db.CreateTableAsync<Contact>().Wait();
        }

        public Task<List<Contact>> GetAllContactsAsync(string filterText)
        {
            if (String.IsNullOrEmpty(filterText))
                // return all items
                return _db.Table<Contact>().ToListAsync();

            // return filtered items
            return (_db.Table<Contact>().Where(c => c.FirstName.Contains(filterText))).ToListAsync();
        }

        public Task<int> SaveContactAsync(Contact contact)
        {
            //ID = 0 , default value >> new diet
            if (contact.ID == 0)
                return _db.InsertAsync(contact);

            //Update diet
            return _db.UpdateAsync(contact);
        }

        public Task<int> DeleteContactAsync(Contact contact)
        {
            return _db.DeleteAsync(contact);
        }

    }
}
