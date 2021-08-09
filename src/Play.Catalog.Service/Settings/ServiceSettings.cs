using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Play.Catalog.Service.Settings
{
    public class ServiceSettings
    {
        //We use the "init" feature to ensure that the specified Host and Port aren't changed after the project is initialized
        public string ServiceName { get; init; }
    }
}
