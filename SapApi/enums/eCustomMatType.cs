using System;
using System.ComponentModel;

namespace SAP2000.enums
{
    // SAP2000 malzemelerinin karşılıkları
    public enum eCustomMatType
    {
        [Description("Beton")]
        Concrete,
        [Description("Donatı Çeliği")]
        Rebar
    }

    public static class EnumHelper
    {
        public static string getDescription(Enum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }
    }
}
