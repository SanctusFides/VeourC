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
        public ObservableCollection<string> Cities { get; set; } = new ObservableCollection<string>();
        public String[] LatitudeLongitude { get; set; } = Array.Empty<string>();


        public MainWindowViewModel() 
        {
            Cities = Utility.LoadCityList();
        }

        public void HandleSearch(string cityState)
        {
            try
            {
                SetCoordinates(cityState);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error retrieving coordinates for {cityState}: {e.Message}");
            }
        }

        private void SetCoordinates(string cityState)
        {
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
            if (res.Result[0] != null || res.Result[1] != null)
            {
                LatitudeLongitude = res.Result;
            }
            Debug.WriteLine($"Coordinates for {cityState}: {LatitudeLongitude[0]}, {LatitudeLongitude[1]}");
        }

        private Forecast CreateTestForecast()
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
