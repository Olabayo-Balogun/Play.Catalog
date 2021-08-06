using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

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
        private static readonly List<ItemDto> items = new()
        {
            new ItemDto(Guid.NewGuid(), "Potion", "Restores a small amount of HP", 5, DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(), "Antidote", "Cures poison", 7, DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(), "Bronze sword", "Deals a small amount of damage", 20, DateTimeOffset.UtcNow)
        };

        /// <summary>
        ///     Get all Items
        /// </summary>
        /// <returns>A list of items</returns>
        /// <response code="200">Returns the list items</response>
        [HttpGet]
        public IEnumerable<ItemDto> Get()
        {
            return items;
        }

        /// <summary>
        ///     Get an item by id.
        /// </summary>
        /// <param name="id">The ID of the item</param>
        /// <returns>An ACtionResult of type "Book"</returns>
        /// <response code="200">Returns the requested item</response>
        [HttpGet("{id}")] //GET /Items/{id}
        public ItemDto GetById(Guid id)
        {
            var item = items.Where(item => item.Id == id).SingleOrDefault();
            return item;
        }

        /// <summary>
        ///     Create new item
        /// </summary>
        /// <param name="createItemDto">The createItemDto contract</param>
        /// <returns>new item</returns>
        /// <response code="201">creates the item</response>
        [HttpPost] //Post  /Items
        public ActionResult<ItemDto> Post(CreateItemDto createItemDto)
        {
            //Here we're passing in the parameters that are part of the parameters of ItemDtos in the Dtos class
            var item = new ItemDto(Guid.NewGuid(), createItemDto.Name, createItemDto.Description, createItemDto.Price, DateTimeOffset.UtcNow);
            items.Add(item);

            //This helps to specify that the item was created and where to find it
            return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
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
        [HttpPut("{id}")] //PUT /Items/{id}
        public IActionResult Put(Guid id, UpdateItemDto updateItemDto)
        {
            var existingItem = items.Where(item => item.Id == id).SingleOrDefault();

            //This is where we clone the existing item and pass in the updated details
            var updatedItem = existingItem with
            {
                Name = updateItemDto.Name,
                Description = updateItemDto.Description,
                Price = updateItemDto.Price
            };

            //Here we try to get the position of the existing Item (which can be located by it's Id)
            var index = items.FindIndex(existingItem => existingItem.Id == id);
            //Here's where we assign the content of the existing item to the updated item thus updating the item
            items[index] = updatedItem;

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
        [HttpDelete("{id}")] //DELETE /Items/{id}
        public IActionResult Delete(Guid id)
        {
            //This tries to get the position (id) of the item we want to delete
            var index = items.FindIndex(existingItem => existingItem.Id == id);

            //This is where we delete the item by removing the item
            items.RemoveAt(index);

            //we don't return any content because there's nothing to return.
            return NoContent();

        }
    }
}