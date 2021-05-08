using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Catalog.API.Infrastructure.Exceptions
{
    public class CatalogDomainException : Exception
    {
        protected CatalogDomainException() { }

        public CatalogDomainException(string message) : base(message) { }

        public CatalogDomainException(string message, Exception innerException) : base(message, innerException) { }

    }
}
