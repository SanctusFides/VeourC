using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Resources;
using Veour.Models;
using Veour.Services;

namespace Veour.ViewModel
{

    public class MainWindowViewModel
    {
        readonly ApiDriver _apiDriver = new();
        public ObservableCollection<string> Cities { get; set; } = [];
        public String Latitude { get; set; }
        public String Longitude { get; set; }

        public MainWindowViewModel() 
        {
            Cities = Utility.LoadCityList();
        }



        public void HandleSearch(string cityState)
        {
            try
            {
                SetCoordinates(cityState);
                _apiDriver.FetchWeather(Latitude, Longitude);
            }
            catch (Exception e)
            {
                // TODO handle pushing error to UI
                Debug.WriteLine($"Error retrieving coordinates for {cityState}: {e.Message}");
            }
        }

        private void SetCoordinates(string cityState)
        {
            // TODO validate input format
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
            SqlDataAccess dataAccess = new SqlDataAccess(configuration);
            Task<String[]> res = dataAccess.GetLatAndLong(city, state);
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

        public Forecast CreateTestForecast()
        {
            var forecast = new Forecast
            {
                Temp = 75,
                High = 80,
                Low = 65,
                FeelsLikeTemp = 77,
                Date = DateTime.Now,
                Humidity = 50,
                Precipitation = 20,
                WeatherCode = "100",
                WeatherDescription = "Sunny",
                WindSpeed = 10,
                WindDirection = "NE"
            };
            return forecast;
        }
    }
}
