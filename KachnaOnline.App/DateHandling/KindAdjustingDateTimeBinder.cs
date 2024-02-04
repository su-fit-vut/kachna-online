using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging;

namespace KachnaOnline.App.DateHandling;

public class KindAdjustingDateTimeBinder : DateTimeModelBinder
{
    public KindAdjustingDateTimeBinder(DateTimeStyles supportedStyles, ILoggerFactory loggerFactory)
        : base(supportedStyles, loggerFactory)
    {
    }

    public new Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var result = base.BindModelAsync(bindingContext);

        if (bindingContext.Result.Model is DateTime dateTime)
        {
            bindingContext.Result = ModelBindingResult.Success(dateTime.ToLocalTime());
        }

        return result;
    }
}
