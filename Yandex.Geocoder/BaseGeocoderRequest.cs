using Yandex.Geocoder.Enums;

namespace Yandex.Geocoder
{
    public abstract class BaseGeocoderRequest
    {
        public const ResponseLanguage DefaultLanguage = ResponseLanguage.ru_RU;
        public const byte DefaultMaxCount = 10;

        protected BaseGeocoderRequest()
        {
            Language = DefaultLanguage;
            MaxCount = DefaultMaxCount;
        }

        public ResponseLanguage Language { get; set; }

        public byte MaxCount { get; set; }
    }
}