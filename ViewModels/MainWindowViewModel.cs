using Caliburn.Micro;
using Microsoft.Extensions.Configuration;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using Veour.Models;
using Veour.Services;

namespace Veour.ViewModels
{
    public class MainWindowViewModel 
    {
        // API driver is instanced here and is used to look up and set the lat and long from user input
        readonly ApiDriver _apiDriver = new();
        private string? Latitude { get; set; }
        private string? Longitude { get; set; }

        // Cities here holds the list for the autocomplete box that the user looks up their City and State in
        public ObservableCollection<string> Cities { get; set; } = [];

        // Forecast Collection is what will hold the forecast and be bound to the UI to display the weather data'
        public BindableCollection<Forecast> Forecast { get; set; }
        // When window loads, populate Cities list from the utility class and initialize Forecast collection
        public MainWindowViewModel()
        {
            Forecast = []; 
            Cities = Utility.LoadCityList();
        }


        public void HandleSearch(string cityState)
        {
            // TODO validate input format - lift the current formatting out of SetCoords and move it into a validator here
            try
            {
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

                    //    // WHY DOES QUEENS NY REPORT THAT THERE ARE NO COORDS? I VERIFY THEY EXIST IN THE DB
                    //    // 20	queens	new+york	NY	40.7498	-73.7976 so check the city+state input results and find out what's going 

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
            catch (Exception e)
            {
                // TODO handle pushing error to UI
                Debug.WriteLine($"Error retrieving coordinates for {cityState}: {e.Message}");
            }
        }

        private void SetCoordinates(string cityState)
        {
            Debug.WriteLine(cityState);

            var parts = cityState.ToLower().Split(',');
            if (parts.Length != 2)
            {
                throw new ArgumentException("Input must be in the format 'City,State'");
            }
            string city = parts[0].Trim();
            string state = parts[1].Trim();
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            // Retrieve latitude and longitude from SQL database using GetLatAndLong method which as a predefined query
            SqlDataAccess dataAccess = new SqlDataAccess(configuration);
            Task<string[]> res = dataAccess.GetLatAndLong(city, state);
            try
            {
                // Ensure valid coordinates are retrieved and the latitude starts with a negative value
                if (res.Result[0] != null || res.Result[1] != null && res.Result[1].StartsWith("-")) 
                {
                    Latitude = res.Result[0];
                    Longitude = res.Result[1];
                } else {                     
                    throw new Exception("Invalid coordinates retrieved from database."); 
                }
            }
            catch (Exception e)
            {
                // TODO handle pushing error to UI
                Debug.WriteLine($"Error retrieving coordinates for {cityState}: {e.Message}");
            }

        }
    }
}
