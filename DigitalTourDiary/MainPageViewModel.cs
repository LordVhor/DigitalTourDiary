using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DigitalTourDiary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalTourDiary
{
    public partial class MainPageViewModel : ObservableObject
    {
        private ITourDatabase database;
        public ObservableCollection<Tour> Tours { get; set; }

        [ObservableProperty]
        private Tour selectedTour;

        [ObservableProperty]
        private Tour editedTour;

        
        async partial void OnEditedTourChanged(Tour value)
        {
            if (value != null)
            {
                if (SelectedTour != null)
                {
                    Tours.Remove(SelectedTour);
                    SelectedTour = null;
                    await database.UpdateTourAsync(value);
                }
                else
                {
                    await database.CreateTourAsync(value);
                }
                Tours.Add(value);
            }
        }

        
        public bool IsFullViewEnabled
        {
            get
            {
                return Preferences.Default.Get("fullview", true);
            }
            set
            {
                Preferences.Default.Set("fullview", value);
                OnPropertyChanged();
            }

        }

        public MainPageViewModel(ITourDatabase database)
        {
            this.database = database;
            Tours = new ObservableCollection<Tour>();
            
        }

        public async Task InitializeAsync()
        {
            var petList = await database.GetToursAsync();
            Tours.Clear();
            petList.ForEach(p => Tours.Add(p));
        }

        [RelayCommand]
        public async Task ShowTourDetailsAsync()
        {
            if (SelectedTour != null)
            {
                var param = new ShellNavigationQueryParameters
            {
                { "Tour", SelectedTour }
            };
                await Shell.Current.GoToAsync("Tourdetails", param);
            }
        }

        [RelayCommand]
        public async Task NewTourAsync()
        {
            selectedTour = null;
            var param = new ShellNavigationQueryParameters
            {
                { "Tour", new Tour(){Date = DateTime.Today} }
            };
            await Shell.Current.GoToAsync("edittour", param);
        }

        [RelayCommand]
        public async Task EditTourAsync()
        {
            if (selectedTour != null)
            {
                var param = new ShellNavigationQueryParameters
            {
                { "Tour", selectedTour }
            };
                await Shell.Current.GoToAsync("edittour", param);
            }
            else
            {
                WeakReferenceMessenger.Default.Send("Select a tour to edit.");
            }
        }

        [RelayCommand]
        public void DeleteTour()
        {
            if (selectedTour != null)
            {
                database.DeleteTourAsync(selectedTour);
                Tours.Remove(selectedTour);
                SelectedTour = null;
            }
            else
            {
                WeakReferenceMessenger.Default.Send("Select a Tour to delete.");
            }

        }


    }
}
