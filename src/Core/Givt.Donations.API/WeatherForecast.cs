namespace Givt.API
{
    public class WeatherForecast
    {
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC * 5f / 9f);

        public string Summary { get; set; }
    }
}