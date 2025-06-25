namespace InfoCarAutopilot.DTO;

public class ExamScheduleResponse
{
    public required string OrganizationId { get; init; }
    public bool? IsOskVehicleReservationEnabled { get; init; }
    public bool? IsRescheduleReservation { get; init; }
    public required string Category { get; init; }
    public required Schedule Schedule { get; init; }
}

public class PracticeExam
{
    public required string Id { get; init; }
    public int? Places { get; init; }
    public DateTime? Date { get; init; }
    public int? Amount { get; init; }
    public required string AdditionalInfo { get; init; }
}

public class Schedule
{
    public required List<ScheduledDay> ScheduledDays { get; init; }
}

public class ScheduledDay
{
    public required string Day { get; init; }
    public required List<ScheduledHour> ScheduledHours { get; init; }
}

public class ScheduledHour
{
    public required string Time { get; init; }
    public required List<TheoryExam> TheoryExams { get; init; }
    public required List<PracticeExam> PracticeExams { get; init; }
    public required List<object> LinkedExamsDto { get; init; }
}

public class TheoryExam
{
    public required string Id { get; init; }
    public int? Places { get; init; }
    public DateTime? Date { get; init; }
    public int? Amount { get; init; }
    public required string AdditionalInfo { get; init; }
}

