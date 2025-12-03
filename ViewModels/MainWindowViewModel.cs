using Caliburn.Micro;
using Microsoft.Extensions.Configuration;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Veour.Exceptions;
using Veour.Models;
using Veour.Services;

namespace Veour.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        // API driver is instanced here and is used to look up and set the lat and long from user input
        readonly ApiDriver _apiDriver = new();
        private string? Latitude { get; set; }
        private string? Longitude { get; set; }

        // Cities here holds the list for the autocomplete box that the user looks up their City and State in
        public ObservableCollection<string> Cities { get; set; } = [];

        // Forecast Collection is what will hold the forecast and be bound to the UI to display the weather data'
        public BindableCollection<Forecast> Forecast { get; set; }


        public event PropertyChangedEventHandler? PropertyChanged;

        private string _errorMessage = "";
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value; 
                OnPropertyChanged(nameof(ErrorMessage));

            }
        }
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // When window loads, populate Cities list from the utility class and initialize Forecast collection
        public MainWindowViewModel()
        {
            Forecast = []; 
            Cities = UtilityDriver.LoadCityList();
        }

        

        public void HandleSearch(string cityState)
        {
            // TODO validate input format - lift the current formatting out of SetCoords and move it into a validator here

            // If there is a previous forecast, clear it to an empty array or else a previous will load on UI
            if (Forecast.Any())
            {
                Forecast = [];
            }
            // Set coordinates by looking up city and state in SQL database
            SetCoordinates(cityState);
            // If valid coordinates are retrieved, fetch weather data from API
            if (!string.IsNullOrEmpty(Latitude) && !string.IsNullOrEmpty(Longitude))
            {
                Forecast[] forecasts = _apiDriver.FetchWeather(Latitude, Longitude);
                foreach (Forecast forecast in forecasts)
                {
                    Forecast.Add(forecast);
                }
            }
            else
            {
                Debug.WriteLine($"Error: Latitude or Longitude is null for {cityState}.");
                // TODO handle pushing error to UI
            }
        }

        private void SetCoordinates(string cityState)
        {
            // split the user input into parts for formatting
            var parts = cityState.ToLower().Split(',');
            // if there aren't 2 parts then it mean their formatting for City,State wasn't correct and display error to user
            if (parts.Length != 2)
            {
                ErrorMessage = "Input must be in the format 'City,State'";
                throw new ArgumentException();
            }
            // Remove any trailing space and replace all " " with "+"
            string city = parts[0].Trim().Replace(" ","+");
            string state = parts[1].Trim().Replace(" ", "+");

            SqlDataAccess dataAccess = new SqlDataAccess();
            // Retrieve latitude and longitude from SQL database using GetLatAndLong method which as a predefined query
            Task<string[]> res = dataAccess.GetLatAndLong(city, state);

            // Ensure valid coordinates are retrieved and the latitude starts with a negative value
            if (res.Result[0] != null || res.Result[1] != null && res.Result[1].StartsWith("-")) 
            {
                Latitude = res.Result[0];
                Longitude = res.Result[1];
            } else {
                // TODO present user with error asking to check location again - need to think about this, maybe even have a contact ability
                ErrorMessage = "Location not found, only locations in the list are currently accepted";
                throw new CoordsNotFoundException();
            }
        }
    }
}
