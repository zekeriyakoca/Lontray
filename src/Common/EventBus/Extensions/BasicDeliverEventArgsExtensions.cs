

using Newtonsoft.Json;
using System.Text;

namespace RabbitMQ.Client.Events
{
    public static class BasicDeliverEventArgsExtensions
    {
        public static T DeserializeBody<T>(this BasicDeliverEventArgs arguments)
        {
            if (arguments == null)
                return default;

            return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(arguments.Body.ToArray()));
        }
    }
}
