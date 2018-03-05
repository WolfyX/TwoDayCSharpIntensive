using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace TwoDaysCSharpIntensive
{
    public class WeatherUnit
    {
        private DateTime date;
        public DateTime Date => date;
        public string DateStr { set { DateTime.TryParse(value, out date); } }

        private Double pressure;
        public Double Pressure => pressure;
        public string PressureStr { set { Double.TryParse(value.Replace('.', ','), out pressure); } }

        private Double temperature;
        public Double Temperature => temperature;
        public string TemperatureStr { set { Double.TryParse(value.Replace('.', ','), out temperature); } }
    }

    public class WeatherFeature
    {
        private const string VORONEZH_WEATHER_SERVISE_URL = @"http://www.eurometeo.ru/russia/voronezhskaya-oblast/voronezh/export/xml/data/";
        private WebClient webClient;

        public WeatherFeature()
        {
            webClient = new WebClient();
            webClient.Encoding = Encoding.UTF8;
        }

        public string GetWeather(int listCount = 4)
        {
            string result = string.Empty;
            List<WeatherUnit> WeatherUnits = GetWeatherUnitList(listCount).ToList();
            foreach (WeatherUnit weatherUnit in WeatherUnits)
            {
                result += GetStringWeather(weatherUnit);
            }
            return result;
        }

        public IEnumerable<WeatherUnit> GetWeatherUnitList(int listCount = 4)
        {
            string resXml = webClient.DownloadString(VORONEZH_WEATHER_SERVISE_URL);

            List<XElement> xElementList = XDocument.Parse(resXml)
                .Descendants("weather")
                .Descendants("city")
                .Descendants("step")
                .ToList();

            int maxIndex = Math.Min(listCount, xElementList.Count);
            for (int i = 0; i < maxIndex; i++)
            {
                yield return new WeatherUnit()
                {
                    DateStr = GetValue(xElementList[i], "datetime"),
                    PressureStr = GetValue(xElementList[i], "pressure"),
                    TemperatureStr = GetValue(xElementList[i], "temperature")
                };

            }
            yield break;
        }

        private static string GetValue(XElement xElement, string elementCaption)
        {
            return xElement.Element(elementCaption).Value;
        }

        private static string GetStringWeather(WeatherUnit weatherUnit)
        {
            string dateString = weatherUnit.Date.ToString("d.M.yy H:mm");
            string pressureString = weatherUnit.Pressure.ToString("F0");
            string temperatureString = weatherUnit.Temperature.ToString("F1");

            return $"{dateString,9} {pressureString,3}torr {temperatureString,4}°C \n";
        }
    }
}
