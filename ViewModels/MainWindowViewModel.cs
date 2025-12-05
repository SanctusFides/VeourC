using Caliburn.Micro;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Veour.Exceptions;
using Veour.Models;
using Veour.Services;

namespace Veour.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        // When window loads, populate Cities list from the utility class and initialize Forecast collection
        public MainWindowViewModel()
        {
            Forecast = [];
            try
            {
                Cities = UtilityDriver.LoadCityList();
            }
            catch (LocationFileNotFoundException)
            {
                throw new ElevatedException();
            }
        }


        // API driver is instanced here and is used to look up and set the lat and long from user input
        readonly ApiDriver _apiDriver = new();
        private string? Latitude { get; set; }
        private string? Longitude { get; set; }

        // Cities here holds the list for the autocomplete box that the user looks up their City and State in
        public ObservableCollection<string> Cities { get; set; } = [];

        // Forecast Collection is what will hold the forecast and be bound to the UI to display the weather data'
        public BindableCollection<Forecast> Forecast { get; set; }

        // if an error is thrown, this variable holds the message that is presented to the user
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

        // used for binding the error message to the view
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                Forecast[] forecasts = ApiDriver.FetchWeather(Latitude, Longitude);
                foreach (Forecast forecast in forecasts)
                {
                    Forecast.Add(forecast);
                }
            }
        }

        private void SetCoordinates(string cityState)
        {
            // Split the user input into parts for formatting
            var parts = cityState.ToLower().Split(',');
            // If there aren't 2 parts then it mean their formatting for City,State wasn't correct and display error to user
            if (parts.Length != 2)
            {
                ErrorMessage = "Location must be 'City, State' format, please check spelling and select your location from the list";
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
                ErrorMessage = "Unable to locate City and State, please check spelling and select your location from the list";
                throw new CoordsNotFoundException();
            }
        }
    }
}
