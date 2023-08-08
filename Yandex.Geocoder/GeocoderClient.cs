using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using Yandex.Geocoder.Enums;
using Yandex.Geocoder.Models;

namespace Yandex.Geocoder
{
    public class GeocoderClient
    {
        public const string DefaultGeocoderBaseUrl = "https://geocode-maps.yandex.ru/1.x/";

        private static HttpClient DefaultHttpClient = new HttpClient();
        private readonly HttpClient httpClient;

        public GeocoderClient(string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));
            Key = key;
            httpClient = DefaultHttpClient;
        }

        public GeocoderClient(HttpClient httpClient, string key)
        {
            if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

            this.httpClient = httpClient;
            Key = key;
        }

        public string Key { get; }

        public Task<GeocoderResponseType> Geocode(UriGeocoderRequest uriGeocoderRequest)
        {
            var parameters = new NameValueCollection
            {
                { "uri", uriGeocoderRequest.Uri }
            };
            return ExecuteQuery(parameters, uriGeocoderRequest);
        }


        public Task<GeocoderResponseType> Geocode(GeocoderRequest geocoderRequest)
        {
            var parameters = new NameValueCollection
            {
                { "geocode", geocoderRequest.Request }
            };

            if (!geocoderRequest.BordersArea.IsEmpty() || !geocoderRequest.SearchArea.IsEmpty())
            {
                parameters.Add("rspn", Convert.ToInt32(geocoderRequest.IsRestrictArea).ToString());

                if (!geocoderRequest.BordersArea.IsEmpty())
                {
                    var lowerLeftCorner = new Coordinate(geocoderRequest.BordersArea.LowerLatitude, geocoderRequest.BordersArea.LowerLongitude);
                    var upperRightCorner = new Coordinate(geocoderRequest.BordersArea.UpperLatitude, geocoderRequest.BordersArea.UpperLongitude);

                    parameters.Add("bbox", $"{lowerLeftCorner}~{upperRightCorner}");
                }
                else
                {
                    var coordinate = new Coordinate(geocoderRequest.SearchArea.Latitude, geocoderRequest.SearchArea.Longitude);
                    var span = new Coordinate(geocoderRequest.SearchArea.LatitudeSpan, geocoderRequest.SearchArea.LongitudeSpan);

                    parameters.Add("ll", coordinate.ToString());
                    parameters.Add("spn", span.ToString());
                }
            }

            return ExecuteQuery(parameters, geocoderRequest);
        }

        public Task<GeocoderResponseType> ReverseGeocode(ReverseGeocoderRequest reverseGeocoderRequest)
        {
            var restRequest = new NameValueCollection();
            if (!reverseGeocoderRequest.Kind.Equals(AddressComponentKind.None))
            {
                restRequest.Add("kind", reverseGeocoderRequest.Kind.ToString().ToLower());
            }

            var coordinate = new Coordinate(reverseGeocoderRequest.Latitude, reverseGeocoderRequest.Longitude);
            restRequest.Add("geocode", coordinate.ToString());

            return ExecuteQuery(restRequest, reverseGeocoderRequest);
        }

        private static JsonSerializerSettings Settings = new JsonSerializerSettings
        {

        };

        protected async Task<GeocoderResponseType> ExecuteQuery(NameValueCollection parameters, BaseGeocoderRequest baseGeocoderRequest)
        {
            parameters.Add("format", "json");
            parameters.Add("lang", baseGeocoderRequest.Language.ToString());

            if (baseGeocoderRequest.Skip != 0)
                parameters.Add("skip ", baseGeocoderRequest.Skip.ToString());

            if (!string.IsNullOrEmpty(Key))
                parameters.Add("apikey", Key);
            parameters.Add("results", baseGeocoderRequest.MaxCount.ToString());
            var isFirst = true;
            var resultUrl = "https://geocode-maps.yandex.ru/1.x?";
            foreach (var key in parameters.AllKeys)
            {
                if (!isFirst)
                    resultUrl += "&";
                resultUrl += key + "=" + HttpUtility.UrlEncode(parameters[key]);
                isFirst = false;
            }
            var response = await httpClient.GetAsync(resultUrl);
            response.EnsureSuccessStatusCode();
            var str = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<GeocoderResponse>(str, Settings)?.Response;
        }
    }
}