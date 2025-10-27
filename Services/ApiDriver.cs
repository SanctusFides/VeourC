using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;

namespace Veour.Services;

public class ApiDriver {


    public void FetchWeather(string lat, string lon)
    {
        Task<HttpResponseMessage> res = FetchWeatherTask(lat, lon);
        if ( !res.Result.StatusCode.Equals(System.Net.HttpStatusCode.BadRequest)) 
        { 
            Debug.WriteLine(res.Result.GetType());
            Debug.WriteLine(ConvertHttpResponseToJson(res.Result).GetType());
        } else
        {
            // TODO handle null response
            Debug.WriteLine("No response received from weather API.");
        }
    }


    private async Task<HttpResponseMessage> FetchWeatherTask(string lat, string lon) {
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


    public string CreateWeatherApiUrl(string lat, string lon)
    {
        return "https://api.open-meteo.com/v1/forecast?latitude=" +
                lat + "&longitude=" + lon +
                "&daily=temperature_2m_max,temperature_2m_min,rain_sum,showers_sum,weather_code,temperature_2m_mean," +
                "precipitation_probability_mean,relative_humidity_2m_mean,apparent_temperature_mean," +
                "wind_direction_10m_dominant,wind_speed_10m_mean&current=temperature_2m,precipitation," +
                "relative_humidity_2m,apparent_temperature,weather_code,rain,showers,wind_speed_10m,wind_direction_10m" +
                "&timezone=America%2FChicago&wind_speed_unit=mph&temperature_unit=fahrenheit&precipitation_unit=inch";
    }

    private static Object ConvertHttpResponseToJson(HttpResponseMessage response)
    {
        return JsonSerializer.Deserialize<Object>(response.Content.ReadAsStringAsync().Result) ?? new Object();
    }

}