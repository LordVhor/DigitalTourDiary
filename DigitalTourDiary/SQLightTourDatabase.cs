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



            database.CreateTableAsync<TourPhoto>().Wait();
        }

        public async Task CreateTourAsync(Tour tour)
        {
            await database.InsertAsync(tour);
            // FONTOS: Az insert után már van ID! De az objektumon még nem látszik!
            // Újra lekérdezzük az utoljára beszúrt tour-t
            var inserted = await database.Table<Tour>()
                .OrderByDescending(t => t.Id)
                .FirstOrDefaultAsync();

            if (inserted != null)
            {
                tour.Id = inserted.Id;  // Frissítsd az eredeti objektumon is!
            }
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








        

        // Fotók lekérése egy túrához
        public async Task<List<TourPhoto>> GetTourPhotosAsync(int tourId)
        {
            return await database.Table<TourPhoto>()
                .Where(p => p.TourId == tourId)
                .OrderBy(p => p.Timestamp)
                .ToListAsync();
        }
        public async Task<TourPhoto> GetPhotoAsync(int id)
        {
            return await database.Table<TourPhoto>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }
        public async Task CreatePhotoAsync(TourPhoto photo)
        {
            await database.InsertAsync(photo);
        }

        public async Task DeletePhotoAsync(TourPhoto photo)
        {
            await database.DeleteAsync(photo);
        }
    }
}
