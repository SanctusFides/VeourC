using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;

namespace Veour.Services
{
    public static class UtilityDriver
    {
        // Loads the city list from a text file and returns it as an ObservableCollection to be used for autocomplete
        public static ObservableCollection<string> LoadCityList()
        {
            ObservableCollection<string> cities = [];
            try
            {
                string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string file = Path.Combine(currentDirectory, @"Assets\Files\locations-ranked.txt");
                string filePath = Path.GetFullPath(file);
                using StreamReader sr = new StreamReader(filePath);
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    cities.Add(line);
                }
            }
            catch (Exception e)
            {
                // TODO log error
                Debug.WriteLine("Could not read the file");
                Debug.WriteLine(e.Message);
            }
            return cities;
        }

        // Values coorespond to directional degrees, I've set thresholds that make sense for each arrow
        public static BitmapImage GetWindDirectionArrowImage(int code)
        {
            Uri imagePath;
            if (code < 10)
            {
                imagePath = new Uri("pack://application:,,,/Assets/Images/Arrows/south.png");
            } else if (code < 80)
            {
                imagePath = new Uri("pack://application:,,,/Assets/Images/Arrows/southwest.png");
            } else if (code < 100)
            {
                imagePath = new Uri("pack://application:,,,/Assets/Images/Arrows/west.png");
            } else if (code < 170)
            {
                imagePath = new Uri("pack://application:,,,/Assets/Images/Arrows/northwest.png");
            } else if (code < 190)
            {
                imagePath = new Uri("pack://application:,,,/Assets/Images/Arrows/north.png");
            } else if (code < 260)
            {
                imagePath = new Uri("pack://application:,,,/Assets/Images/Arrows/northeast.png");
            } else if (code < 280)
            {
                imagePath = new Uri("pack://application:,,,/Assets/Images/Arrows/east.png");
            } else if (code < 350)
            {
                imagePath = new Uri("pack://application:,,,/Assets/Images/Arrows/southeast.png");
            } else if (code <= 360)
            {
                imagePath = new Uri("pack://application:,,,/Assets/Images/Arrows/south.png");
            } else
            {
                imagePath = new Uri("pack://application:,,,/Assets/Images/error.png");
            }
            return new BitmapImage(imagePath);
        }


        // These code values are mapped to the WMO Weather interpretation codes which can be looked up online
        public static BitmapImage GetWeatherImage(string code)
        {
            Uri imagePath;
            if (code.Equals("0"))
            {
                imagePath = new Uri("pack://application:,,,/Assets/Images/Weather/sun.png");
            }
            else if (code.Equals("1") || code.Equals("2")) {
                imagePath = new Uri("pack://application:,,,/Assets/Images/Weather/partly-cloudy.png");
            }
            else if (code.Equals("3"))
            {
                imagePath = new Uri("pack://application:,,,/Assets/Images/Weather/cloudy.png");
            }
            else if (code.Equals("45") || code.Equals("48"))
            {
                imagePath = new Uri("pack://application:,,,/Assets/Images/Weather/fog.png");
            }
            else if (code.Equals("51") || code.Equals("53") || code.Equals("55") || code.Equals("56") || code.Equals("57"))
            {
                imagePath = new Uri("pack://application:,,,/Assets/Images/Weather/drizzle.png");
            }
            else if (code.Equals("61") || code.Equals("63") || code.Equals("65") || code.Equals("66") || code.Equals("67"))
            {
                imagePath = new Uri("pack://application:,,,/Assets/Images/Weather/rain.png");
            }
            else if (code.Equals("71") || code.Equals("73") || code.Equals("75") || code.Equals("77"))
            {
                imagePath = new Uri("pack://application:,,,/Assets/Images/Weather/snow.png");
            }
            else if (code.Equals("80") || code.Equals("81") || code.Equals("82"))
            {
                imagePath = new Uri("pack://application:,,,/Assets/Images/Weather/rain.png");
            }
            else if (code.Equals("85") || code.Equals("86"))
            {
                imagePath = new Uri("pack://application:,,,/Assets/Images/Weather/snow.png");
            }
            else if (code.Equals("95") || code.Equals("96") || code.Equals("99"))
            {
                imagePath = new Uri("pack://application:,,,/Assets/Images/Weather/storm.png");
            }
            else
            {
                imagePath = new Uri("pack://application:,,,/Assets/Images/error.png");
            }
            return new BitmapImage(imagePath);
        }

        // These code values are mapped to the WMO Weather interpretation codes which can be looked up online
        public static string MapWeatherCodeToDescription(string code)
        {
            return code switch
            {
                "0" => "Clear Sky",
                "1" => "Mostly Clear",
                "2" => "Partly cloudy",
                "3" => "Overcast",
                "45" or "48" => "Fog",
                "51" or "53" or "55" => "Drizzle",
                "56" or "57" => "Freezing Drizzle",
                "61" => "Slight Rain",
                "63" => "Rain",
                "65" => "Heavy Rain",
                "66" or "67" => "Freezing Rain",
                "71" => "Slight Snow",
                "73" => "Snow",
                "75" => "Heavy Snow",
                "77" => "Snow Grains",
                "80" => "Slight Showers",
                "81" => "Showers",
                "82" => "Heavy Shower",
                "85" or "86" => "Snow Showers",
                "95" => "Thunderstorm",
                "96" or "99" => "Hail Thunderstorms",
                _ => "error",
            };
        }

    }
}
