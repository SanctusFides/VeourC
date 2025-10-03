using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Extensions.Configuration;
using Veour.Models;
using Veour.Services;

namespace Veour
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            SqlDataAccess dataAccess = new SqlDataAccess(configuration);
            Task<String[]> res = dataAccess.GetLatAndLong("houston","texas");
            
            var forecast = new Forecast();
            forecast.Temp = 100;
            forecast.High = 200;
            forecast.Low = 001;
            forecast.FeelsLikeTemp = 500;
            forecast.Date = DateTime.Now;
            forecast.Humidity = 60;
            forecast.Precipitation = 10;
            forecast.WeatherCode = "168";
            forecast.WeatherDescription = "snowy";
            forecast.WindSpeed = 20;
            forecast.WindDirection = "NW";
            
            Console.WriteLine(forecast);
        }
    }
}