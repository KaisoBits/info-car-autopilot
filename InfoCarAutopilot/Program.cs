using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using AdvancedOAuthAutomationTool;
using InfoCarAutopilot;
using InfoCarAutopilot.DTO;

Win32Helpers.ToggleOSSleepPrevention(true);

AppSettings settings = await LoadAppSettings();

JWTExtractor jwtExtractor = new();
Console.WriteLine("Logging in...");
string? jwt = await jwtExtractor.ExtractJwt(settings.Url, settings.Username, settings.Password);
if (jwt == null)
{
    Console.WriteLine("Failed to get JWT");
    return;
}

Console.WriteLine("Logged in successfully");

DateTime lastLogin = DateTime.UtcNow;

InfoCarClient client = new(jwt);

while (true)
{
    try
    {
        if (lastLogin.AddMinutes(settings.MinutesToReauthenticate) <= DateTime.UtcNow)
        {
            Console.WriteLine("Reauthenticating....");
            string? newJwt = await jwtExtractor.ExtractJwt(settings.Url, settings.Username, settings.Password);
            if (newJwt != null)
            {
                client.UpdateJwt(newJwt);
                lastLogin = DateTime.UtcNow;
                Console.WriteLine("Reauthenticated successfully");
            }
            else
            {
                Console.WriteLine("Failed to reauthenticate!");
            }
        }

        Console.WriteLine("Getting exam dates....");
        ExamScheduleResponse resp = await client.GetExamSchedule(new()
        {
            WordId = WordOffices.Wroclaw,
            Category = "B",
            StartDate = DateTime.UtcNow.Date,
            EndDate = DateTime.UtcNow.Date.AddDays(settings.DaysFromNowToCheck)
        });


        Console.WriteLine("Successfully got exam dates");

        List<PracticeExam> validExams =
            resp.Schedule.ScheduledDays
            .SelectMany(sd => sd.ScheduledHours)
            .SelectMany(sh => sh.PracticeExams)
            .Where(exam => exam.Date < DateTime.UtcNow.Date.AddDays(settings.DaysFromNowToCheck))
            .OrderBy(exam => exam.Date)
            .ToList();

        Console.WriteLine("Possible exam dates fulfilling criteria: {0}", validExams.Count);
        if (validExams.Count > 0)
        {
            PracticeExam examToReserve = validExams.First();

            Console.WriteLine("Attempting to reserve this one: {0}", JsonSerializer.Serialize(examToReserve));

            ReservationRequest reservationDto = new()
            {
                Candidate = new()
                {
                    Category = "B",
                    Email = settings.Username,
                    Firstname = settings.FirstName,
                    Lastname = settings.LastName,
                    Pesel = settings.Pesel,
                    PhoneNumber = settings.PhoneNumber,
                    Pkk = settings.PKK,
                    Pkz = null
                },
                Exam = new()
                {
                    OrganizationUnitId = settings.OrganizationUnitId ?? WordOffices.Wroclaw,
                    PracticeId = examToReserve.Id,
                    TheoryId = null
                },
                LanguageAndOsk = new()
                {
                    Language = "POLISH",
                    OskVehicleReservation = null,
                    SignLanguage = "NONE"
                }
            };

            Console.WriteLine("Making reservation...");
            ReservationResponse reservation = await client.MakeReservation(reservationDto);
            await File.AppendAllTextAsync("./logs.txt", $"Making reservation for {examToReserve.Date}\n");
            Console.WriteLine("Reservation created");

            ReservationStatusResponse? reservationStatus = null;
            do
            {
                await Task.Delay(1500);
                Console.WriteLine("Checking reservation status...");
                reservationStatus = await client.CheckReservationStatus(reservation.ID);

            } while (reservationStatus.Status.Status == "CREATED");

            Console.WriteLine("Process completed");
            Console.WriteLine("Reservation status: {0}", reservationStatus.Status.Status);
            Console.WriteLine("Reservation message: {0}", reservationStatus.Status.Message);

            await File.AppendAllTextAsync("./logs.txt", $"Reservation result: {reservationStatus.Status.Status} ({reservationStatus.Status.Message})\n");
            if (reservationStatus.Status.Message == "Kandydat jest już zapisany na egzamin lub posiada aktywną rezerwację na wybraną kategorię.")
            {
                await Alarm();
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Global exception: {0}", ex.Message);
        await Task.Delay(settings.DelayMs); // double delay
    }
    await Task.Delay(settings.DelayMs);
}

async Task<AppSettings> LoadAppSettings()
{
    string content = await File.ReadAllTextAsync("./appsettings.json");
    AppSettings? settings = JsonSerializer.Deserialize<AppSettings>(content, new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    });

    return settings ?? throw new InvalidOperationException("Failed to load app settings");
}

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
async Task Alarm()
{
    for (int i = 0; i < 20; i++)
    {
        Console.Beep(400, 100);
        await Task.Delay(50);
    }
}