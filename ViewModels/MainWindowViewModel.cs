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

        private String[] LatitudeLongitude { get; set; }

        public MainWindowViewModel() 
        {
            Cities = LoadCityList();
            Debug.WriteLine($"Loaded {Cities.Count} cities.");
        }

        public ObservableCollection<string> LoadCityList()
        {
            try
            {
                string CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string File = Path.Combine(CurrentDirectory, @"Assets\Files\locations-ranked.txt");
                string FilePath = Path.GetFullPath(File);
                using StreamReader sr = new StreamReader(FilePath);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    Cities.Add(line);
                }
            } catch (Exception e) {
                    Debug.WriteLine("Could not read the file");
                    Debug.WriteLine(e.Message);
            }
            return Cities;
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

        private string CreateTestCoordinates()
        {
            //return coordinates for Houston,TX
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            SqlDataAccess dataAccess = new SqlDataAccess(configuration);
            Task<String[]> res = dataAccess.GetLatAndLong("houston", "texas");
            return string.Join(",", res.Result);
        }
    }
}
