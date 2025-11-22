using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

namespace DigitalTourDiary.Models
{
    public partial class Tour : ObservableObject
    {
        [ObservableProperty]
        [property: PrimaryKey]
        [property: AutoIncrement]
        int id;

        [ObservableProperty]
        string name;

        [ObservableProperty]
        double distance;

        [ObservableProperty]
        DateTime date;

        [ObservableProperty]
        TimeSpan duration;

        public Tour GetCopy()
        {
            return (Tour)this.MemberwiseClone();
        }
        
    }
}
