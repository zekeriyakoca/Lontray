using EventBus.Events;
using EventBus.Events.Interfaces;
using Microsoft.Extensions.Logging;
using Ordering.Infrastructure;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ordering.API.Application.IntegrationEvents
{

    public record OrderCreatedIntegrationEvent : IntegrationEvent
    {

    }
}
