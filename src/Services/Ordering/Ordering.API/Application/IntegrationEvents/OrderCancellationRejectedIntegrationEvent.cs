﻿using EventBus.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.Application.IntegrationEvents
{
    public record OrderCancellationRejectedIntegrationEvent : IntegrationEvent
    {
    }
}
