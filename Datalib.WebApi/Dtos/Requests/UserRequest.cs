using System.ComponentModel.DataAnnotations;

namespace Datalib.WebApi.Dtos.Requests;

public record struct UserRequest(
    [property: Required(AllowEmptyStrings = false, ErrorMessage = "Non empty full name is required")] string FullName,
    [property: Required(AllowEmptyStrings = false, ErrorMessage = "Non empty email is required"),
               EmailAddress(ErrorMessage = "Valid email address is required")] string Email);