using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KachnaOnline.App.DateHandling;

public class CustomDateTimeConverter : IsoDateTimeConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value is DateTime dateTime)
        {
            dateTime = dateTime.ToUniversalTime();
            base.WriteJson(writer, dateTime, serializer);
        }
        else
        {
            base.WriteJson(writer, value, serializer);
        }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var dateTime = base.ReadJson(reader, objectType, existingValue, serializer);
        if (dateTime == null)
            return null;

        var unboxedDateTime = (DateTime)dateTime;
        return unboxedDateTime.ToLocalTime();
    }
}
