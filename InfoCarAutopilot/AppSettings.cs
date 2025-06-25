namespace InfoCarAutopilot;

public class AppSettings
{
    public required string Url { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required string Pesel { get; init; }
    public required string PKK { get; init; }
    public required string PhoneNumber { get; init; }
    public required string Category { get; init; }
    public required int MinutesToReauthenticate { get; init; }
    public required int DaysFromNowToCheck { get; init; }
    public required int DelayMs { get; init; }
    public string? OrganizationUnitId { get; init; }
}
