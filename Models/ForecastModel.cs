namespace Veour.Models;

public class Forecast {

    public double Temp {get; set;}
    public double High {get; set;}
    public double Low {get; set;}
    public double FeelsLikeTemp {get; set;}
    public double Humidity {get; set;}
    public double Precipitation {get; set;}
    public DateTime Date {get; set;}
    
    public string WeatherCode {get; set;} = string.Empty;
    public string WeatherDescription {get; set;} = string.Empty;
    
    public double WindSpeed {get; set;}
    public int WindDirection {get; set;}
    
    public Forecast() {}

    public string GetWeatherDay()
    {
        return Date.DayOfWeek.ToString();
    }
    
    public string GetTempString() {
        return Temp +"°";
    }
    public string GetHighTempString() {
        return High +"°";
    }
    public string GetLowTempString() {
        return Low +"°";
    }
    public string GetFeelsLikeTempString() {
        return FeelsLikeTemp +"°";
    }
    public string GetHumidityString() {
        return Humidity +"%";
    }
    public string GetPrecipitationString() {
        return Precipitation +"%";
    }
    public string GetWindSpeedString() {
        return WindSpeed +" mp/h";
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