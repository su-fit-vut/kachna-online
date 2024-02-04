using System;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace KachnaOnline.App.DateHandling;

public class KindAdjustingDateTimeModelBinderProvider : IModelBinderProvider
{
    private const DateTimeStyles SupportedStyles = DateTimeStyles.AllowWhiteSpaces;

    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        if (context.Metadata.ModelType == typeof(DateTime) || context.Metadata.ModelType == typeof(DateTime?))
        {
            var loggerFactory = context.Services.GetRequiredService<ILoggerFactory>();
            return new KindAdjustingDateTimeBinder(SupportedStyles, loggerFactory);
        }

        return null;
    }
}
