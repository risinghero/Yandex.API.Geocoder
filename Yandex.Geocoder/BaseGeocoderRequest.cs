using System;
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
        public uint Skip
        {
            get { return Skip; }
            set
            {
                if (value % MaxCount != 0)
                    throw new ArgumentException("Skip value should be fully divided by MaxCount value due to API request");
            }
        }
    }
}