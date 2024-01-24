using System.ComponentModel.DataAnnotations;

namespace Datalib.WebApi.Dtos.Requests;

public record struct BookRequest(
    [property: Required(AllowEmptyStrings = false, ErrorMessage = "Non empty author is required")] string Author, 
    [property: Required(AllowEmptyStrings = false, ErrorMessage = "Non empty title is required")] string Title);