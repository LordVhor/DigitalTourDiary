using DigitalTourDiary.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTourDiary
{
    internal class SQLightTourDatabase : ITourDatabase
    {
        SQLite.SQLiteOpenFlags Flags =
        SQLite.SQLiteOpenFlags.ReadWrite |  
        SQLite.SQLiteOpenFlags.Create;      

        
        string databasePath =
            Path.Combine(FileSystem.Current.AppDataDirectory, "tours.db");
        SQLiteAsyncConnection database;

        public SQLightTourDatabase()
        {

            //if (File.Exists(databasePath))
            //    File.Delete(databasePath);
            database = new SQLiteAsyncConnection(databasePath, Flags); 


            

            database.CreateTableAsync<Tour>().Wait();
        }

        public async Task CreateTourAsync(Tour tour)
        {
            await database.InsertAsync(tour);
        }

        public async Task DeleteTourAsync(Tour tour)
        {
            await database.DeleteAsync(tour);
        }

        public async Task<Tour> GetTourAsync(int id)
        {
            return await database.Table<Tour>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<Tour>> GetToursAsync()
        {
            return await database.Table<Tour>().ToListAsync();
        }

        public async Task UpdateTourAsync(Tour tour)
        {
            await database.UpdateAsync(tour);
        }
    }
}
