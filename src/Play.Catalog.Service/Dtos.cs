using System;
using System.ComponentModel.DataAnnotations;

namespace Play.Catalog.Service
{
    public record ItemDto(Guid Id, string Name, string Description, decimal Price, DateTimeOffset CreatedDate);
    //The "Required" attribute ensures that the user can't input null values or skip making an input.
    //The "Range" attribute ensures that the user can't input a number beyond or below what is specified, sometimes, attribute that specifies the minimum value is used instead. 
    public record CreateItemDto([Required] string Name, string Description,[Range(0,1000)] decimal Price);
    public record UpdateItemDto([Required] string Name, string Description, [Range(0, 1000)] decimal Price);
    
}