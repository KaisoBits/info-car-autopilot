using System.Net.Http.Json;
using System.Text.Json;
using InfoCarAutopilot.DTO;

namespace InfoCarAutopilot;

public class InfoCarClient
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _serializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private string _jwt;

    public InfoCarClient(string jwt)
    {
        _jwt = jwt;
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri("https://info-car.pl");
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwt);
    }

    public void UpdateJwt(string newJwt)
    {
        _jwt = newJwt;
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _jwt);
    }

    public async Task<ExamScheduleResponse> GetExamSchedule(ExamScheduleRequest request)
    {
        HttpResponseMessage resp = await _httpClient.PutAsJsonAsync("api/word/word-centers/exam-schedule", request);
        return (await resp.Content.ReadFromJsonAsync<ExamScheduleResponse>())!;
    }

    public async Task<ReservationResponse> MakeReservation(ReservationRequest request)
    {
        HttpResponseMessage resp = await _httpClient.PostAsJsonAsync("api/word/reservations", request);
        string content = await resp.Content.ReadAsStringAsync();

        return (await resp.Content.ReadFromJsonAsync<ReservationResponse>())!;
    }

    public async Task<ReservationStatusResponse> CheckReservationStatus(string reservationID)
    {
        return (await _httpClient.GetFromJsonAsync<ReservationStatusResponse>($"api/word/reservations/{reservationID}"))!;
    }
}
