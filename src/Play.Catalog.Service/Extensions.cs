using Play.Catalog.Service.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Play.Catalog.Service
{
    public static class Extensions
    {
        //This is where we turn the new entity into an existing DTO in order to return the DTOs in the Rest API we created
        public static ItemDto AsDto(this Item item)
        {
            return new ItemDto(item.Id, item.Name, item.Description, item.Price, item.CreatedDate);
        }
    }
}
