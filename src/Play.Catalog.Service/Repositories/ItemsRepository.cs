using MongoDB.Driver;
using Play.Catalog.Service.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Play.Catalog.Service.Repositories
{
    public class ItemsRepository : IItemsRepository
    {
        //A collectionName is to MongoDb as Table name is to SQL
        private const string collectionName = "items";

        //The code below will hold the record instance of the collection
        //It represents the collection in this repository.
        private readonly IMongoCollection<Item> dbCollection;
        //This helps us handle how we query requests and find it through the repository as we search for stuff
        private readonly FilterDefinitionBuilder<Item> filterBuilder = Builders<Item>.Filter;

        //public ItemsRepository()
        //{
        //    //This is where we declare the connection string to the database.
        //    //Note that this shouldn't be declared here in this way, it's not a standard convention.
        //    var mongoClient = new MongoClient("mongodb://localhost:27017");

        //    //Here, we declare the actual database location and name
        //    //It is named "Catalog" as a result of the fact that the microservice is a catalog microservice.

        //    var database = mongoClient.GetDatabase("Catalog");

        //    //Here's where we create an instance of the dbcollection.
        //    //Note that it has to have the same type as the entity.
        //    dbCollection = database.GetCollection<Item>(collectionName);
        //}

        //Here, we inject the IMongoDatabase.
        //This "database" parameter results will come from line 57 of the startup.cs class
        public ItemsRepository(IMongoDatabase database)
        {
            dbCollection = database.GetCollection<Item>(collectionName);
        }

        //We're creating this method to return all the items in the database.
        //We're using readonly because we don't want the customer to be able to modify this list.
        //We're have to make it asynchronous to ensure it works perfectly (I need to do more research on this)
        //<todo><todo>
        public async Task<IReadOnlyCollection<Item>> GetAllAsync()
        {
            //We're making the filterBuilder empty because we want to return all the items.
            //As such, we don't want any filters on the results of our search.
            //We're returning a list of items so it makes sense for us to convert it to a list asynchronously.
            return await dbCollection.Find(filterBuilder.Empty).ToListAsync();
        }

        //Here we're trying to return just one item
        public async Task<Item> GetAsync(Guid id)
        {
            //Here, we're trying to search by Id which is why the filter parameter is Id
            //The Eq ensures that we're searching by equality.
            FilterDefinition<Item> filter = filterBuilder.Eq(entity => entity.Id, id);

            //we're returning the item that matches the Id by inserting the "filter" parameter above into the search query and we want the first item that matches as there should typically be only one item that matches because the Id is Guid and Guid is unique.
            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }

        //Here, we're trying to establish the method that handles creation of an item
        //Because we're not returning anything to the client, we don't need Task of type Item
        public async Task CreateAsync(Item entity)
        {
            //Here we try to handle a possible scenario where the entity we want to create is nonexistent. 
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            //If the item isn't null, we insert the item into the database.
            //Notice that we're inserting one, it follows from the assumption that we're creating one item at a time.
            //We don't need to return anything to the client for this method.
            await dbCollection.InsertOneAsync(entity);
        }

        //Here, we're trying to establish the method that handles updating of an item
        //Because we're not returning anything to the client, we don't need Task of type Item
        public async Task UpdateAsync(Item entity)
        {
            //Here we try to handle a possible scenario where the entity we want to create is nonexistent. 
            if (entity == null)
            {
                throw new ArgumentNullException();
            }

            //We need to find the item we're updating first
            //Because we have an entity labelled above, we don't want to confuse the program so we have to give this one a different name
            FilterDefinition<Item> filter = filterBuilder.Eq(existingEntity => existingEntity.Id, entity.Id);

            //After finding the item we can now begin  replacement
            //Notice that we're replacing one, it follows from the assumption that we're replacing one item at a time.
            //We don't need to return anything to the client for this method.
            await dbCollection.ReplaceOneAsync(filter, entity);
        }

        //Here, we're trying to establish the method that handles removal of an item
        //Because we're not returning anything to the client, we don't need Task of type Item
        public async Task RemoveAsync(Guid id)
        {
            //We need to find the item we're deleting first
            FilterDefinition<Item> filter = filterBuilder.Eq(entity => entity.Id, id);

            //After finding the item we can now delete
            //Notice that we're using DeleteOne, it follows from the assumption that we're deleting one item at a time.
            //We don't need to return anything to the client for this method.
            await dbCollection.DeleteOneAsync(filter);
        }
    }
}
