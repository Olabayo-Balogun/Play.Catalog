using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Catalog.Service.Repositories;
using Play.Catalog.Service.Settings;

namespace Play.Catalog.Service
{
    public class Startup
    {
        //We are adding at the class level variables at this position because we're going to be using these resources in multiple places.
        private ServiceSettings serviceSettings;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //The BsonSerializer is what we use to ensure that the data we're serializing (ID and Date created) shows in the database when we check for it.
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

            //Here, we're calling the value of the service settings using the the Configurations settings just outside this method.
            //The ServiceSettings is the same name as the class so it's easy to map to it.
            //With this code snippet below, we have deserialized the ServiceSettings we loaded above.
            serviceSettings = Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

            //Here, we construct the MongoDbClient
            //The AddSingleton service ensures that there is only one instance of our instantiated object in the entire microservice application
            services.AddSingleton(serviceProvider =>
            {
                var mongoDbSettings = Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();

                //Here's where we're actually constructing the MongoDbClient.
                var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);

                //Here's where we get an instance of the database that we have
                //This result will be passed into the database parameter in ItemRepository constructor in line 38 of ItemRepository.cs
                return mongoClient.GetDatabase(serviceSettings.ServiceName);
            });

            //Here, we register the ItemsRepository dependency.
            //The code below is different from the code above because we're registering/declaring the service here while we're constructing a service above
            services.AddSingleton<IItemsRepository, ItemsRepository>();

            //services.AddControllers();

            //When trying to run the create Async, we typically have the programme removing the "Async" suffix in the code and it doesn't allow the creation to run which is why we're disabling that feature here.
            services.AddControllers(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Play.Catalog.Service", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Play.Catalog.Service v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
