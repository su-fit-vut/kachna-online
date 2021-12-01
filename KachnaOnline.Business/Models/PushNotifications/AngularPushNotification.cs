// AngularPushNotification.cs
// Author: František Nečas

using System.Collections.Generic;
using Lib.Net.Http.WebPush;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace KachnaOnline.Business.Models.PushNotifications
{
    /// <summary>
    /// Represents a Push notification for Angular service worker.
    /// </summary>
    /// <remarks>The WebPush protocol doesn't place any constraints on the format of messages, however Angular
    /// service worker requires this particular format.
    ///
    /// Inspired by: https://www.telerik.com/blogs/push-notifications-in-asp-net-core-with-angular
    /// </remarks>
    public class AngularPushNotification
    {
        private static readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public string Title { get; set; }
        public string Body { get; set; }
        public string Icon { get; set; }

        public PushMessage ToPushMessage()
        {
            return new PushMessage(JsonConvert.SerializeObject(new { notification = this }, _jsonSerializerSettings));
        }
    }
}
