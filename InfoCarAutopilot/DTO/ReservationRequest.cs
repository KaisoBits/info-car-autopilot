namespace InfoCarAutopilot.DTO;

public class Candidate
{
    public required string Category { get; init; }
    public required string Email { get; init; }
    public required string Firstname { get; init; }
    public required string Lastname { get; init; }
    public required string Pesel { get; init; }
    public required string PhoneNumber { get; init; }
    public required string Pkk { get; init; }
    public required string? Pkz { get; init; }
}

public class Exam
{
    public required string OrganizationUnitId { get; init; }
    public required string PracticeId { get; init; }
    public required string? TheoryId { get; init; }
}

public class LanguageAndOsk
{
    public required string Language { get; init; }
    public required string SignLanguage { get; init; }
    public required string? OskVehicleReservation { get; init; }
}

public class ReservationRequest
{
    public required Candidate Candidate { get; init; }
    public required Exam Exam { get; init; }
    public required LanguageAndOsk LanguageAndOsk { get; init; }
}
