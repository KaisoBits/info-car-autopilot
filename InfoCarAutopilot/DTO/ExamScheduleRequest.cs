namespace InfoCarAutopilot.DTO;

public class ExamScheduleRequest
{
    public required string WordId { get; init; }
    public required string Category { get; init; }
    public required DateTime StartDate { get; init; }
    public required DateTime EndDate { get; init; }

}
