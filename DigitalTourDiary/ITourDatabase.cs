using DigitalTourDiary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTourDiary
{
    public interface ITourDatabase 
    {

        Task<List<Tour>> GetToursAsync();
        Task<Tour> GetTourAsync(int id);
        Task CreateTourAsync(Tour tour);
        Task UpdateTourAsync(Tour tour);
        Task DeleteTourAsync(Tour tour);
    }
}
