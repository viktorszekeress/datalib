namespace Datalib.WebApi.Domain.Models;

public record struct ReminderInfo(string Email, DateTimeOffset IssuedOn, List<string> AuthorsAndTitles);