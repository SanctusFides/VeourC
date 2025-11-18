using System.Diagnostics;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Veour.Models;

namespace Veour.Services;

public class ApiDriver {

    ///*
    /// Important notes about the flow of this project:
    /// There is a SQL database that stores city, state, latitude, and longitude information.
    /// There is a web API for Open Meteo that a network call is made to, using the lat and long retrieved from the database.
    /// So the Lat and Long are retrieved, then fed into the FetchWeatherTask function which makes the API call and returns an HTTP response message.
    /// The Message is then converted to JSON to make it easier to parse and extract the necessary weather data and nested groups.
    /// 
    /// VERY IMPORTANT TO NOTE - There is only 1 API call needed to get the full week's forecast. Each forecast property has all 7 days of data in arrays.
    /// For this, we make the 1 call, convert into JSON and then dive into each nested array to extract that days value. This is why we use 1 forecast mode to 
    /// return an entire array of forecast objects.
    /// */

    public Forecast[] FetchWeather(string lat, string lon)
    {
        Task<HttpResponseMessage> res = FetchWeatherTask(lat, lon);
        if ( !res.Result.StatusCode.Equals(System.Net.HttpStatusCode.BadRequest)) 
        { 
            JsonElement weatherJson = ConvertHttpResponseToJson(res.Result);
            Forecast[] weekForecast = BuildForecast(ConvertHttpResponseToJson(res.Result));
            // TODO Remove Debug line
            //Debug.WriteLine(string.Join(",", weekForecast.Select(f => f.ToString())));
            return weekForecast;
        } else
        {
            // TODO handle null response
            Debug.WriteLine("No response received from weather API.");
            return Array.Empty<Forecast>();
        }
    }


    private static async Task<HttpResponseMessage> FetchWeatherTask(string lat, string lon) {
        try
        {
            using HttpClient client = new();
            HttpResponseMessage response = await client.GetAsync(CreateWeatherApiUrl(lat, lon)).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            return response;
        } 
        catch (HttpRequestException e)
        {
            Debug.WriteLine("Request error: " + e.Message);
            HttpResponseMessage ErrorResponse = new()
            {
                StatusCode = System.Net.HttpStatusCode.BadRequest
            };
            return ErrorResponse;
        }
    }


    private static Forecast[] BuildForecast(JsonElement weather)
    {
        // Takes in a JsonElement of the full weekly Forecast full of arrays of each property. This parses the Json to extracts each property to pass into the
        // BuildWeekForecast function which will iterate through for each day's values
        JsonElement current = weather.GetProperty("current");
        JsonElement daily = weather.GetProperty("daily");

        JsonElement time = daily.GetProperty("time");
        JsonElement avgTemp = daily.GetProperty("temperature_2m_mean");
        JsonElement maxTemp = daily.GetProperty("temperature_2m_max");
        JsonElement minTemp = daily.GetProperty("temperature_2m_min");
        JsonElement feelsLikeTemp = daily.GetProperty("apparent_temperature_mean");
        JsonElement humidity = daily.GetProperty("relative_humidity_2m_mean");
        JsonElement precipitation = daily.GetProperty("precipitation_probability_mean");
        JsonElement weatherCode = daily.GetProperty("weather_code");
        JsonElement windDirection = daily.GetProperty("wind_direction_10m_dominant");
        JsonElement windSpeed = daily.GetProperty("wind_speed_10m_mean");
        
        return BuildWeekForecast(current, time, avgTemp, maxTemp, minTemp,
                                 feelsLikeTemp, humidity, precipitation, 
                                 weatherCode, windDirection, windSpeed);
    }

    private static Forecast[] BuildWeekForecast(JsonElement current, JsonElement time, JsonElement avgTemp, 
                                         JsonElement maxTemp, JsonElement minTemp, JsonElement feelsLikeTemp,
                                         JsonElement humidity,JsonElement precip, JsonElement weatherCode,
                                         JsonElement windDirection, JsonElement windSpeed)
    {
        // Loops through each day's values in the arrays and builds a Forecast object for each day, returning an array of 7 Forecast objects
        Forecast[] weekForecast = new Forecast[7];
        for (int i = 0; i < 7; i++)
        {
            Forecast dailyForecast = new Forecast();
            // The API puts current day's data in a separate object so we have the handle the first day differently while values after the Else aren't separated
            if (i == 0)
            {
                dailyForecast.Temp = current.GetProperty("temperature_2m").GetDouble();
                dailyForecast.FeelsLikeTemp = current.GetProperty("apparent_temperature").GetDouble();
                dailyForecast.Humidity = current.GetProperty("relative_humidity_2m").GetDouble();
                dailyForecast.Precipitation = current.GetProperty("precipitation").GetDouble();
                dailyForecast.WindDirection = current.GetProperty("wind_direction_10m").GetInt32();
                dailyForecast.WindSpeed = current.GetProperty("wind_speed_10m").GetDouble();
                dailyForecast.WeatherCode = current.GetProperty("weather_code").ToString();
            }
            else
            {
                dailyForecast.Temp = avgTemp[i].GetDouble();
                dailyForecast.FeelsLikeTemp = feelsLikeTemp[i].GetDouble();
                dailyForecast.Humidity = humidity[i].GetDouble();
                dailyForecast.Precipitation = precip[i].GetDouble();
                dailyForecast.WindDirection = windDirection[i].GetInt32();
                dailyForecast.WindSpeed = windSpeed[i].GetDouble();
                dailyForecast.WeatherCode = weatherCode[i].ToString();
            }

            string? dateString = time[i].GetString();
            dailyForecast.Date = dateString != null ? DateTime.Parse(dateString) : default;
            dailyForecast.High = maxTemp[i].GetDouble();
            dailyForecast.Low = minTemp[i].GetDouble();

            weekForecast[i] = dailyForecast;
        }
        return weekForecast;
    }


    public static string CreateWeatherApiUrl(string lat, string lon)
    {
        return "https://api.open-meteo.com/v1/forecast?latitude=" +
                lat + "&longitude=" + lon +
                "&daily=temperature_2m_max,temperature_2m_min,rain_sum,showers_sum,weather_code,temperature_2m_mean," +
                "precipitation_probability_mean,relative_humidity_2m_mean,apparent_temperature_mean," +
                "wind_direction_10m_dominant,wind_speed_10m_mean&current=temperature_2m,precipitation," +
                "relative_humidity_2m,apparent_temperature,weather_code,rain,showers,wind_speed_10m,wind_direction_10m" +
                "&timezone=America%2FChicago&wind_speed_unit=mph&temperature_unit=fahrenheit&precipitation_unit=inch";
    }

    private static JsonElement ConvertHttpResponseToJson(HttpResponseMessage response)
    {
        return JsonSerializer.Deserialize<JsonElement>(response.Content.ReadAsStringAsync().Result);
    }

}