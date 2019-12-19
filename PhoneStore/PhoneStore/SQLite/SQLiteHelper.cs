using PhoneStore.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PhoneStore.SQLite
{
    public class SQLiteHelper
    {
        SQLiteAsyncConnection db;
        public SQLiteHelper(string dbPath)
        {
            db = new SQLiteAsyncConnection(dbPath);
            db.CreateTableAsync<CartModel>().Wait();
            db.CreateTableAsync<UserModel>().Wait();
        }

        #region Cart
        public Task<List<CartModel>> GetItemsAsync()
        {
            return db.Table<CartModel>().ToListAsync();
        }

        public Task<CartModel> GetItemAsync(string itemCode)
        {
            return db.Table<CartModel>().Where(i => i.Code == itemCode).FirstOrDefaultAsync();
        }

        public Task<int> SaveItemAsync(CartModel cart)
        {
            var exitsItem = Task.Run(async () => await GetItemAsync(cart.Code)).Result;
            if (exitsItem != null)
            {
                return db.UpdateAsync(cart);
            }
            else
            {
                return db.InsertAsync(cart);
            }
        }

        public Task<int> DeleteItemAsync(CartModel cart)
        {
            return db.DeleteAsync(cart);
        }
        #endregion

        #region User

        public Task<List<UserModel>> GetUsersAsync()
        {
            return db.Table<UserModel>().ToListAsync();
        }

        public Task<UserModel> GetUserAsync(string userEmail)
        {
            return db.Table<UserModel>().Where(i => i.Email == userEmail).FirstOrDefaultAsync();
        }

        public Task<int> SaveUserAsync(UserModel user)
        {
            var exitsUser = Task.Run(async () => await GetItemAsync(user.Email)).Result;
            if (exitsUser != null)
            {
                return db.UpdateAsync(user);
            }
            else
            {
                return db.InsertAsync(user);
            }
        }
        public Task<int> DeleteUserAsync(UserModel user)
        {
            return db.DeleteAsync(user);
        }
        #endregion
    }
}
