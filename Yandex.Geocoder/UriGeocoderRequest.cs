using System;
using System.Collections.Generic;
using System.Text;
using Yandex.Geocoder;

namespace Yandex.Geocoder
{
    public class UriGeocoderRequest : BaseGeocoderRequest
    {
        public string Uri { get; set; }
    }
}
