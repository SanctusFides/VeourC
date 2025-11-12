using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veour.Models;
using Veour.Services;

namespace Veour.ViewModels
{
    internal class ForecastViewModel
    {
        // API driver is instanced here and is used to look up and set the lat and long from user input
        readonly ApiDriver _apiDriver = new();

        // Forecast Collection is what will hold the forecast and be bound to the UI to display the weather data
        public BindableCollection<Forecast> Forecast { get; set; }


        public ForecastViewModel() 
        {
            Forecast = new BindableCollection<Forecast>();

            // These lines below are only for testing purposes, to be removed later
            Forecast[] weatherArray = _apiDriver.FetchWeather("29.79453", "-95.384476");
            foreach (var forecast in weatherArray)
            {
                Forecast.Add(forecast);
            }
        }
    }
}
