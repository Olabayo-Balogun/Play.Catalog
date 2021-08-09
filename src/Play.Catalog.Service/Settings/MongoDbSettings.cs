using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Play.Catalog.Service.Settings
{
    public class MongoDbSettings
    {
        //The properties in the class must be a C# replica of what was declared in the "appsettings.json" file.
        //We use the "init" feature to ensure that the specified Host and Port aren't changed after the project is initialized
        public string Host { get; init; }
        public int Port { get; init; }

        //Here's where we declare the path name but using string interpolation so we can pass in the connection string to map to the path of the database
        public string ConnectionString => $"mongodb://{Host}:{Port}";
    }
}
