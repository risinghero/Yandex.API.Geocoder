namespace Yandex.Geocoder.Enums
{
    public enum AddressComponentKind
    {
        /// <summary>
        /// Не задан
        /// </summary>
        None,

        /// <summary>
        /// Отдельный дом
        /// </summary>
        House,

        /// <summary>
        /// Улица
        /// </summary>
        Street,

        /// <summary>
        /// Станция метро
        /// </summary>
        Metro,

        /// <summary>
        /// Район города
        /// </summary>
        District,

        /// <summary>
        /// Населённый пункт: город / поселок / деревня / село и т. п.
        /// </summary>
        Locality,

        /// <summary>
        /// Район области
        /// </summary>
        Area,

        /// <summary>
        /// Область
        /// </summary>
        Province,

        /// <summary>
        /// Страна
        /// </summary>
        Country,

        /// <summary>
        /// Река / озеро / ручей / водохранилище и т. п.
        /// </summary>
        Hydro,

        /// <summary>
        /// Ж.д. станция
        /// </summary>
        Railway_Station,

        /// <summary>
        /// станции, не относящиеся к железной дороге. Например, канатные станции.
        /// </summary>
        Station,

        /// <summary>
        /// Линия метро / шоссе / ж.д. линия
        /// </summary>
        Route,

        /// <summary>
        /// Лес / парк / сад и т. п.
        /// </summary>
        Vegetation,

        /// <summary>
        /// Аэропорт
        /// </summary>
        Airport,

        /// <summary>
        /// подъезд / вход
        /// </summary>
        Entrance,

        /// <summary>
        /// Прочее
        /// </summary>
        Other
    }
}
