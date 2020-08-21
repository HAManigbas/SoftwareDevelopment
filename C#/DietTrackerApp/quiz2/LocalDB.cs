using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
using System.Threading.Tasks;

namespace quiz2
{
    public class LocalDB
    {
        //Connection to the SQLite DB
        private readonly SQLiteAsyncConnection _db;

        public LocalDB(string dbPath)
        {
            _db = new SQLiteAsyncConnection(dbPath);
            _db.CreateTableAsync<Diet>().Wait();
        }

        public Task<List<Diet>> GetAllDietsAsync(string filterText)
        {
            if(String.IsNullOrEmpty(filterText))
                // return all items
                return _db.Table<Diet>().ToListAsync();

            // return filtered items
            return (_db.Table<Diet>().Where(d => d.Category.Contains(filterText))).ToListAsync();
        }

        public Task<int> SaveDietAsync(Diet diet)
        {
            //ID = 0 , default value >> new diet
            if (diet.ID == 0)
                return _db.InsertAsync(diet);

            //Update diet
            return _db.UpdateAsync(diet);
        }

        public Task<int> DeleteDietAsync(Diet diet)
        {
            return _db.DeleteAsync(diet);
        }
    }
}
