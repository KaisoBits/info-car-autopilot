namespace InfoCarAutopilot.DTO;

public class CandidateStatus
{
    public required string Pkk { get; init; }
    public required string Firstname { get; init; }
    public required string Lastname { get; init; }
    public required string Pesel { get; init; }
    public string? BirthDate { get; init; }
    public required string PhoneNumber { get; init; }
    public required string Email { get; init; }
    public required string Language { get; init; }
}

public class ExamStatus
{
    public required string OrganizationUnitId { get; init; }
    public required string OrganizationUnitName { get; init; }
    public required string Latitude { get; init; }
    public required string Longitude { get; init; }
    public required string Address { get; init; }
    public required string Province { get; init; }
    public string? ConfirmingOperator { get; init; }
    public string? ConfirmationRecordNumber { get; init; }
    public required string Category { get; init; }
    public string? Theory { get; init; }
    public required PracticeStatus Practice { get; init; }
    public string? OskVehicleNumber { get; init; }
    public required string SignLanguage { get; init; }
    public DateTime? ExamDate { get; init; }
    public DateTime? StartDate { get; init; }
}

public class PracticeStatus
{
    public required string ExamId { get; init; }
    public DateTime? Date { get; init; }
    public required string AdditionalInfo { get; init; }
    public string? Room { get; init; }
}

public class ReservationStatusResponse
{
    public required string Id { get; init; }
    public required string UserId { get; init; }
    public required string WordReservationId { get; init; }
    public required ReservationStatus Status { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public required CandidateStatus Candidate { get; init; }
    public required ExamStatus Exam { get; init; }
    public string? IsReminderSent { get; init; }
    public string? IsFirstReminderSent { get; init; }
    public string? Invoice { get; init; }
    public string? CancellationMessage { get; init; }
    public string? ActivePayment { get; init; }
    public bool? AwaitingReschedule { get; init; }
}

public class ReservationStatus
{
    public required string Status { get; init; }
    public DateTime? Timestamp { get; init; }
    public string? Message { get; init; }
}

