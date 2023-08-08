using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Yandex.Geocoder.Enums;
using Yandex.Geocoder.Extenstions;

namespace Yandex.Geocoder.Tests
{
    public class GeocoderTests
    {
        private string apiKey;

        public GeocoderTests()
        {
            var key = Environment.GetEnvironmentVariable("YANDEX.GEOCODER_API_KEY");
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("YANDEX.GEOCODER_API_KEY environment variable is not set");
            }

            apiKey = key;
        }

        [Fact]
        public async Task GeocodeCity()
        {
            var request = new GeocoderRequest { Request = "Ярославль серова 13" };
            var client = new GeocoderClient(apiKey);

            var response = await client.Geocode(request);

            var firstGeoObject = response.GeoObjectCollection.FeatureMember.FirstOrDefault();
            var coordinate = firstGeoObject.GeoObject.Point.Pos;
            var addressComponents = firstGeoObject.GeoObject.MetaDataProperty.GeocoderMetaData.Address.Components;
            var country = addressComponents.FirstOrDefault(c => c.Kind.Equals(AddressComponentKind.Country));
            var province = addressComponents.LastOrDefault(c => c.Kind.Equals(AddressComponentKind.Province));
            var area = addressComponents.FirstOrDefault(c => c.Kind.Equals(AddressComponentKind.Area));
            var city = addressComponents.FirstOrDefault(c => c.Kind.Equals(AddressComponentKind.Locality));
            var street = addressComponents.FirstOrDefault(c => c.Kind.Equals(AddressComponentKind.Street));
            var house = addressComponents.FirstOrDefault(c => c.Kind.Equals(AddressComponentKind.House));

            Assert.Equal("Россия", country.Name);
            Assert.Equal("Ярославская область", province.Name);
            Assert.Equal("городской округ Ярославль", area.Name);
            Assert.Equal("Ярославль", city.Name);
            Assert.Equal("улица Серова", street.Name);
            Assert.Equal("13", house.Name);
            Assert.Equal("39.80232 57.608724", coordinate);
        }

        [Fact]
        public async Task GeocodeSettlementWithoutStreet()
        {
            var request = new GeocoderRequest { Request = "Ярославская область Октябрьский 12" };
            var client = new GeocoderClient(apiKey);

            var response = await client.Geocode(request);

            var firstGeoObject = response.GeoObjectCollection.FeatureMember.FirstOrDefault();
            var coordinate = firstGeoObject.GeoObject.Point.Pos;
            var addressComponents = firstGeoObject.GeoObject.MetaDataProperty.GeocoderMetaData.Address.Components;
            var country = addressComponents.FirstOrDefault(c => c.Kind.Equals(AddressComponentKind.Country));
            var province = addressComponents.LastOrDefault(c => c.Kind.Equals(AddressComponentKind.Province));
            var area = addressComponents.FirstOrDefault(c => c.Kind.Equals(AddressComponentKind.Area));
            var city = addressComponents.FirstOrDefault(c => c.Kind.Equals(AddressComponentKind.Locality));
            var street = addressComponents.FirstOrDefault(c => c.Kind.Equals(AddressComponentKind.Street));
            var house = addressComponents.FirstOrDefault(c => c.Kind.Equals(AddressComponentKind.House));

            Assert.Equal("Россия", country.Name);
            Assert.Equal("Ярославская область", province.Name);
            Assert.Equal("Рыбинский район", area.Name);
            Assert.Equal("посёлок Октябрьский", city.Name);
            Assert.Null(street);
            Assert.Equal("12", house.Name);
            Assert.Equal("39.110132 57.984856", coordinate);
        }

        [Fact]
        public async Task ReverseGeocode()
        {
            var request = new ReverseGeocoderRequest { Latitude = 58.046733, Longitude = 38.841715 };
            var client = new GeocoderClient(apiKey);

            var response = await client.ReverseGeocode(request);

            var firstGeoObject = response.GeoObjectCollection.FeatureMember.FirstOrDefault();
            var addressComponents = firstGeoObject.GeoObject.MetaDataProperty.GeocoderMetaData.Address.Components;
            var country = addressComponents.FirstOrDefault(c => c.Kind.Equals(AddressComponentKind.Country));
            var province = addressComponents.LastOrDefault(c => c.Kind.Equals(AddressComponentKind.Province));
            var area = addressComponents.FirstOrDefault(c => c.Kind.Equals(AddressComponentKind.Area));
            var city = addressComponents.FirstOrDefault(c => c.Kind.Equals(AddressComponentKind.Locality));
            var street = addressComponents.FirstOrDefault(c => c.Kind.Equals(AddressComponentKind.Street));
            var house = addressComponents.FirstOrDefault(c => c.Kind.Equals(AddressComponentKind.House));

            Assert.Equal("Россия", country.Name);
            Assert.Equal("Ярославская область", province.Name);
            Assert.Equal("городской округ Рыбинск", area.Name);
            Assert.Equal("Рыбинск", city.Name);
            Assert.Equal("улица Бородулина", street.Name);
            Assert.Equal("23", house.Name);
        }

        [Fact]
        public async Task RestrictionOfObjectsReturnedByReverseGeocoding()
        {
            var request = new ReverseGeocoderRequest { Latitude = 58.046733, Longitude = 38.841715, Kind = AddressComponentKind.Locality };
            var client = new GeocoderClient(apiKey);

            var response = await client.ReverseGeocode(request);

            var firstGeoObject = response.GeoObjectCollection.FeatureMember.FirstOrDefault();
            var addressComponents = firstGeoObject.GeoObject.MetaDataProperty.GeocoderMetaData.Address.Components;
            var province = addressComponents.FirstOrDefault(c => c.Kind.Equals(AddressComponentKind.Province));
            var city = addressComponents.FirstOrDefault(c => c.Kind.Equals(AddressComponentKind.Locality));
            var street = addressComponents.FirstOrDefault(c => c.Kind.Equals(AddressComponentKind.Street));

            Assert.Equal("Центральный федеральный округ", province.Name);
            Assert.Equal("Рыбинск", city.Name);
            Assert.Null(street);
        }

        [Fact]
        public async Task ParseCoordinates()
        {
            var request = new GeocoderRequest { Request = "Ярославская область Октябрьский 12" };
            var client = new GeocoderClient(apiKey);

            var response = await client.Geocode(request);

            var firstGeoObject = response.GeoObjectCollection.FeatureMember.FirstOrDefault();
            var strCoordinate = firstGeoObject.GeoObject.Point.Pos;
            var coordinate = strCoordinate.ToCoordinate();

            Assert.Equal("39.110132 57.984856", strCoordinate);
        }

        [Fact]
        public async Task RestrictSearchArea()
        {
            var firstRequest = new GeocoderRequest
            {
                Request = "Песочное",
                SearchArea = new Area { Latitude = 59.300000, Longitude = 39.500000, LatitudeSpan = 0.500000, LongitudeSpan = 0.500000 },
                IsRestrictArea = true
            };
            var client = new GeocoderClient(apiKey);
            var firstResponse = await client.Geocode(firstRequest);
            var firstResponseGeoObject = firstResponse.GeoObjectCollection.FeatureMember.FirstOrDefault();
            var firstResponseAddressComponents = firstResponseGeoObject.GeoObject.MetaDataProperty.GeocoderMetaData.Address.Components;
            var firstResponseProvince = firstResponseAddressComponents.LastOrDefault(c => c.Kind.Equals(AddressComponentKind.Province));
            var firstResponseLocality = firstResponseAddressComponents.FirstOrDefault(c => c.Kind.Equals(AddressComponentKind.District));

            var secondRequest = new GeocoderRequest
            {
                Request = "Песочное",
                SearchArea = new Area { Latitude = 58.00400, Longitude = 39.200000, LatitudeSpan = 0.500000, LongitudeSpan = 0.500000 },
                IsRestrictArea = true
            };
            var secondResponse = await client.Geocode(secondRequest);
            var secondResponseGeoObject = secondResponse.GeoObjectCollection.FeatureMember.FirstOrDefault();
            var secondResponseAddressComponents = secondResponseGeoObject.GeoObject.MetaDataProperty.GeocoderMetaData.Address.Components;
            var secondResponseProvince = secondResponseAddressComponents.LastOrDefault(c => c.Kind.Equals(AddressComponentKind.Province));
            var secondResponseLocality = secondResponseAddressComponents.FirstOrDefault(c => c.Kind.Equals(AddressComponentKind.Locality));

            Assert.Equal("Вологодская область", firstResponseProvince.Name);
            Assert.Equal("посёлок Песочное", firstResponseLocality.Name);

            Assert.Equal("Ярославская область", secondResponseProvince.Name);
            Assert.Equal("посёлок Песочное", secondResponseLocality.Name);
        }

        [Fact]
        public async Task FindGeoObjectInBordersArea()
        {
            var firstRequest = new GeocoderRequest
            {
                Request = "Песочное",
                BordersArea = new BoxArea { LowerLatitude = 59.440733, LowerLongitude = 39.641063, UpperLatitude = 59.457845, UpperLongitude = 39.666459 },
                IsRestrictArea = true
            };
            var client = new GeocoderClient(apiKey);
            var firstResponse = await client.Geocode(firstRequest);
            var firstResponseGeoObject = firstResponse.GeoObjectCollection.FeatureMember.FirstOrDefault();
            var firstResponseAddressComponents = firstResponseGeoObject.GeoObject.MetaDataProperty.GeocoderMetaData.Address.Components;
            var firstResponseProvince = firstResponseAddressComponents.LastOrDefault(c => c.Kind.Equals(AddressComponentKind.Province));
            var firstResponseLocality = firstResponseAddressComponents.FirstOrDefault(c => c.Kind.Equals(AddressComponentKind.District));

            Assert.Equal("Вологодская область", firstResponseProvince.Name);
            Assert.Equal("посёлок Песочное", firstResponseLocality.Name);
        }

        [Fact]
        public async Task NotFindGeoObjectInBordersArea()
        {
            var firstRequest = new GeocoderRequest
            {
                Request = "Песочное",
                BordersArea = new BoxArea { LowerLatitude = 59.600000, LowerLongitude = 41.641063, UpperLatitude = 59.700000, UpperLongitude = 41.666459 },
                IsRestrictArea = true
            };

            var client = new GeocoderClient(apiKey);
            var firstResponse = await client.Geocode(firstRequest);
            var firstResponseGeoObject = firstResponse.GeoObjectCollection.FeatureMember.FirstOrDefault();

            Assert.Null(firstResponseGeoObject);
        }

        [Fact]
        public async Task LimitNumberOfReturnedGeoObjects()
        {
            var client = new GeocoderClient(apiKey);

            var unlimitedResponse = await client.Geocode(new GeocoderRequest { Request = "Серова" });
            var unlimitedGeoObjectCount = unlimitedResponse.GeoObjectCollection.FeatureMember.Count;

            var limitedResponse = await client.Geocode(new GeocoderRequest { Request = "Серова", MaxCount = 1 });
            var limitedGeoObjectCount = limitedResponse.GeoObjectCollection.FeatureMember.Count;

            Assert.True(unlimitedGeoObjectCount > limitedGeoObjectCount);
        }

        [Fact]
        public async Task FindByUriFromGeoSuggest()
        {
            var client = new GeocoderClient(apiKey);
            var result = await client.Geocode(new UriGeocoderRequest
            {
                Uri = "ymapsbm1://geo?data=CgoxNTI1OTU2MDQ1Ev0B0KDQtdGB0L_Rg9Cx0LvQuNC60LAg0KLQsNGC0LDRgNGB0YLQsNC9LCDQmtGD0LrQvNC-0YDRgdC60LjQuSDRgNCw0LnQvtC9LCDQnNCw0L3Qt9Cw0YDQsNGB0YHQutC-0LUg0YHQtdC70YzRgdC60L7QtSDQv9C-0YHQtdC70LXQvdC40LUsINGB0LXQu9C-INCc0LDQvdC30LDRgNCw0YEsINC80LjQutGA0L7RgNCw0LnQvtC9INCu0LbQvdGL0LktNiwg0YPQu9C40YbQsCDQktCw0LvQtdC90YLQuNC90LAg0JrQtdC70YzQvNCw0LrQvtCy0LAsIDE1IA,,"
            });
            Assert.NotNull(result);
            var firstResponseGeoObject = result.GeoObjectCollection.FeatureMember.FirstOrDefault();
            var firstResponseAddressComponents = firstResponseGeoObject.GeoObject.MetaDataProperty.GeocoderMetaData.Address.Components;
            var firstResponseLocality = firstResponseAddressComponents.FirstOrDefault(c => c.Kind.Equals(AddressComponentKind.Locality));
            Assert.Equal("село Манзарас", firstResponseLocality.Name);
        }
    }
}