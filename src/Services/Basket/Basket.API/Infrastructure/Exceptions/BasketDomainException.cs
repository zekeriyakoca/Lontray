using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basket.API
{
    public class BasketDomainException : Exception
    {
        public BasketDomainException()
        {
        }

        public BasketDomainException(string message) : base(message)
        {
        }

        public BasketDomainException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
