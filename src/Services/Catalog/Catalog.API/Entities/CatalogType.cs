﻿using DomainHelper.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catalog.API.Entities
{
    public class CatalogType : Entity
    {
        public string Name { get; set; }
    }
}