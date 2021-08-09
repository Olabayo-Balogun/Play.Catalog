using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Repositories;

//The assembly declaration below triggers default API convention for the entire project
[assembly: ApiConventionType(typeof(DefaultApiConventions))]
namespace Play.Catalog.Service.Controllers
{
    //It is important that you inherit from ControllerBase rather than Controller because it has more functionalities that can be used in the controller and it is more useful for HTTP requests.
    [Produces("application/json", "application/xml")]
    [ApiController]
    [Route("Items")]
    public class ItemsController : ControllerBase
    {
        //"Static" is being used so the list isn't recreated everytime the REST API is called
        //private static readonly List<ItemDto> items = new()
        //{
        //    new ItemDto(Guid.NewGuid(), "Potion", "Restores a small amount of HP", 5, DateTimeOffset.UtcNow),
        //    new ItemDto(Guid.NewGuid(), "Antidote", "Cures poison", 7, DateTimeOffset.UtcNow),
        //    new ItemDto(Guid.NewGuid(), "Bronze sword", "Deals a small amount of damage", 20, DateTimeOffset.UtcNow)
        //};

        //Because the methods in the ItemsRepository are async, we have to make all our methods here aysnc to be able to leverage the async features of the ItemsRepository
        //private readonly ItemsRepository itemsRepository = new();

        //We're initiating the dependency injection by using the interface as an abstraction.
        private readonly IItemsRepository itemsRepository;

        //We introduce this constructor to inject the IItemsrepository dependency.
        public ItemsController(IItemsRepository itemsRepository)
        {
            this.itemsRepository = itemsRepository;
        }

        /// <summary>
        ///     Get all Items
        /// </summary>
        /// <returns>A list of items</returns>
        /// <response code="200">Returns the list items</response>
        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetAsync()
        {
            var items = (await itemsRepository.GetAllAsync()).Select(item => item.AsDto());
            return items;
        }

        /// <summary>
        ///     Get an item by id.
        /// </summary>
        /// <param name="id">The ID of the item</param>
        /// <returns>An ACtionResult of type "Book"</returns>
        /// <response code="200">Returns the requested item</response>
        [HttpGet("{id}")] //GET /Items/{id}

        //We added an "ActionResult" in order to be able return different types of responses, without the "ActionResult" and type (ItemDto in this case) we wouldn't be able to make it work.
        //Any API that needs to find something from the database and return it (and maybe manipulate it), must have an "ActionResult" of the type of item it's trying to find or it won't be able to find the item.
        //We're adding async and Task well as modifying the method used in a bid to leverage the repository pattern.
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {
            //var item = items.Where(item => item.Id == id).SingleOrDefault();
            var item = await itemsRepository.GetAsync(id);
            //The "if" block ensures that we can handle an invalid input.
            if (item == null)
            {
                return NotFound();
            }

            return item.AsDto();
        }

        /// <summary>
        ///     Create new item
        /// </summary>
        /// <param name="createItemDto">The createItemDto contract</param>
        /// <returns>new item</returns>
        /// <response code="201">creates the item</response>
        [HttpPost] //Post  /Items
        //We're adding async and Task well as modifying the method used in a bid to leverage the repository pattern.
        public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
        {
            //Here we're passing in the parameters that are part of the parameters of ItemDtos in the Dtos class
            //var item = new ItemDto(Guid.NewGuid(), createItemDto.Name, createItemDto.Description, createItemDto.Price, DateTimeOffset.UtcNow);
            //items.Add(item);

            var item = new Item
            {
                Name = createItemDto.Name,
                Description = createItemDto.Description,
                Price = createItemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            //This helps to specify that the item was created and where to find it
            //return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);

            await itemsRepository.CreateAsync(item);

            //We changed the "GetById" to "GetByIdAsync" because we changed the method name above to "GetByIdAsync"
            return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
        }
    

        /// <summary>
        ///     Update the existing item
        /// </summary>
        /// <param name="id">The id of the item you want to update</param>
        /// <param name="updateItemDto">The UpdateItemDto contract that will handle most of the details of the item</param>
        /// <returns>It doesn't return anything as it updates an existing item</returns>
        //IActionResult is used when you don't have a return type.
        //Updating a result doesn't require a return type as what you're updating already exists.
        //Here we're passing the id of the item we're updating and calling the UpdateItemDto contract to handle the rest.
        [HttpPut("{id}")] //PUT /Items/{id

        //an IActionResult can suffice in the same was as an ActionResult (for a GET request) for a PUT request.
        //We're adding async and Task well as modifying the method used in a bid to leverage the repository pattern.
        public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto)
        {
            //var existingItem = items.Where(item => item.Id == id).SingleOrDefault();

            //if (existingItem == null)
            //{
            //    return NotFound();
            //}
            ////This is where we clone the existing item and pass in the updated details
            //var updatedItem = existingItem with
            //{
            //    Name = updateItemDto.Name,
            //    Description = updateItemDto.Description,
            //    Price = updateItemDto.Price
            //};

            ////Here we try to get the position of the existing Item (which can be located by it's Id)
            //var index = items.FindIndex(existingItem => existingItem.Id == id);
            ////Here's where we assign the content of the existing item to the updated item thus updating the item
            //items[index] = updatedItem;

            var existingItem = await itemsRepository.GetAsync(id);

            if (existingItem == null)
            {
                return NotFound();
            }

            existingItem.Name = updateItemDto.Name;
            existingItem.Description = updateItemDto.Description;
            existingItem.Price = updateItemDto.Price;

            await itemsRepository.UpdateAsync(existingItem);
            //we don't return any content because there's nothing to return.
            return NoContent(); 
        }

        /// <summary>
        ///     Delete the existing item
        /// </summary>
        /// <param name="id">The id of the item you want to delete</param>
        /// <param name="updateItemDto">The DeleteItemDto contract that will handle most of the details of the item</param>
        /// <returns>It doesn't return anything as it deletes an existing item</returns>
        //IActionResult is used when you don't have a return type.
        //Deleting a result doesn't require a return type as what you're deleting already exists.
        //Here we're passing the id of the item we're deleting and calling the DeleteItemDto contract to handle the rest.
        //We're adding async and Task well as modifying the method used in a bid to leverage the repository pattern.
        [HttpDelete("{id}")] //DELETE /Items/{id}
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            //This tries to get the position (id) of the item we want to delete
            //var index = items.FindIndex(existingItem => existingItem.Id == id);

            //We handle the possibility of an invalid item here using the logic that if the item isn't found, there would be no index position as such less than 0 works.
            //We can't say if index == null because null is a reference type and an int type can't be compared to a reference type, there's also no reason why we should use boxing and/or unboxing in this case.  
            //if (index < 0)
            //{
            //    return NotFound();
            //}

            //This is where we delete the item by removing the item
            //items.RemoveAt(index);

            var item = await itemsRepository.GetAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            await itemsRepository.RemoveAsync(item.Id);

            //we don't return any content because there's nothing to return.
            return NoContent();

        }
    }
}