using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DigitalTourDiary.Models;
using System.Collections.ObjectModel;

namespace DigitalTourDiary
{
    [QueryProperty(nameof(Photos), "Photos")]
    [QueryProperty(nameof(CurrentIndex), "CurrentIndex")]
    public partial class PhotoViewerPageViewModel : ObservableObject
    {
        private ObservableCollection<TourPhoto> photos;
        public ObservableCollection<TourPhoto> Photos
        {
            get => photos;
            set
            {
                if (SetProperty(ref photos, value))
                {
                    // Amikor Photos változik, frissítsd a CurrentPhoto-t
                    if (value != null && CurrentIndex >= 0 && CurrentIndex < value.Count)
                    {
                        CurrentPhoto = value[CurrentIndex];
                    }
                }
            }
        }

        [ObservableProperty]
        private int currentIndex;

        [ObservableProperty]
        private TourPhoto currentPhoto;

        partial void OnCurrentIndexChanged(int value)
        {
            if (Photos != null && value >= 0 && value < Photos.Count)
            {
                CurrentPhoto = Photos[value];
            }
        }

        public string PhotoCounter => Photos != null ? $"{CurrentIndex + 1}/{Photos.Count}" : "0/0";

        public bool HasPrevious => CurrentIndex > 0;
        public bool HasNext => Photos != null && CurrentIndex < Photos.Count - 1;

        [RelayCommand]
        public void PreviousPhoto()
        {
            if (HasPrevious)
            {
                CurrentIndex--;
                OnPropertyChanged(nameof(PhotoCounter));
                OnPropertyChanged(nameof(HasPrevious));
                OnPropertyChanged(nameof(HasNext));
            }
        }

        [RelayCommand]
        public void NextPhoto()
        {
            if (HasNext)
            {
                CurrentIndex++;
                OnPropertyChanged(nameof(PhotoCounter));
                OnPropertyChanged(nameof(HasPrevious));
                OnPropertyChanged(nameof(HasNext));
            }
        }

        [RelayCommand]
        public async Task Close()
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}