using System.Windows.Media.Imaging;
using Veour.Services;

namespace Veour.Models;

public class Forecast {

    public double Temp {get; set;}
    public string TempString 
    {
        get { return Temp + "°"; }
    }

    public double High {get; set;}
    public string HighString { 
        get { return High + "°"; }
    }

    public double Low {get; set;}
    public string LowString { 
        get { return Low + "°"; }
    }

    public double FeelsLikeTemp {get; set;}
    public string FeelsLikeTempString { 
        get { return FeelsLikeTemp + "°"; }
    }

    public double Humidity {get; set;}
    public string HumidityString { 
        get { return Humidity + "%"; }
    }

    public double Precipitation {get; set;}
    public string PrecipitationString { 
        get { return Precipitation + "%"; }
    }

    // Immediately convert date to day name when setting
    private DateTime _date;
    public DateTime Date 
    {
        get { 
            return _date; 
        }
        set { 
            _date = value;
            DayName = GetWeatherDay();
        }
    }
    public string DayName {get; set;} = string.Empty;


    // immediately map the weather code to both the description and the image when setting
    private string _weatherCode = string.Empty;
    public string WeatherCode 
    {
        get { 
            return _weatherCode;
        }
        set { 
            _weatherCode = value;
            WeatherDescription = UtilityDriver.MapWeatherCodeToDescription(WeatherCode);
            WeatherImage = UtilityDriver.GetWeatherImage(WeatherCode);
        }
    }

    public BitmapImage? WeatherImage { get; set; }

    public string WeatherDescription {get; set;} = string.Empty;
    
    public double WindSpeed { get; set; }
    public string WindSpeedString 
    {
        get { return WindSpeed + " mp/h"; }
    }

    private int _windDirection;
    public int WindDirection { 
        get { 
            return _windDirection; 
        }
        set { 
            _windDirection = value;
            WindDirectionImage = UtilityDriver.GetWindDirectionArrowImage(WindDirection);
        }
    }
    public BitmapImage? WindDirectionImage { get; set; }


    public Forecast() {}


    public string GetWeatherDay()
    {
        return Date.DayOfWeek.ToString().ToUpper();
    }
    
    public override string ToString()
    {
        return "Forecast{" +
                              "temp=" + Temp +
                              ", high=" + High +
                              ", low=" + Low +
                              ", feelsLikeTemp=" + FeelsLikeTemp +
                              ", humidity=" + Humidity +
                              ", precipitation=" + Precipitation +
                              ", date=" + Date +
                              ", weatherCode='" + WeatherCode + '\'' +
                              ", weatherDescription='" + WeatherDescription + '\'' +
                              ", windSpeed=" + WindSpeed +
                              ", windDirection=" + WindDirection +
                              '}';
    }
}