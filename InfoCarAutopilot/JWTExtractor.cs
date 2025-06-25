using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.Support.UI;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace InfoCarAutopilot;

public class JWTExtractor
{
    public async Task<string?> ExtractJwt(string url, string username, string password)
    {
        string usernameSelector = "input[type='email'],input[type='text']";
        string passwordSelector = "input[type='password']";

        var chromeOptions = new ChromeOptions();
        chromeOptions.AddArgument("--headless");

        chromeOptions.AddArgument("--disable-blink-features=AutomationControlled");
        chromeOptions.AddArgument("--disable-dev-shm-usage");
        chromeOptions.AddArgument("--no-sandbox");

        var service = ChromeDriverService.CreateDefaultService();
        service.EnableVerboseLogging = false;
        service.SuppressInitialDiagnosticInformation = true;
        service.HideCommandPromptWindow = true;

        Console.WriteLine("Initializing browser...");

        string? capturedBearerToken = null;
        var tokenFound = new ManualResetEvent(false);

        using IWebDriver driver = new ChromeDriver(service, chromeOptions);
        try
        {
            if (driver is IDevTools devToolsDriver)
            {
                var devTools = devToolsDriver.GetDevToolsSession();

                await devTools.SendCommand("Network.enable", JsonNode.Parse("{\"maxTotalBufferSize\": 0, \"maxResourceBufferSize\": 0}")!);

                devTools.DevToolsEventReceived += (sender, e) =>
                {
                    if (e.EventName == "requestWillBeSentExtraInfo")
                    {
                        try
                        {
                            var eventData = JsonDocument.Parse(e.EventData.GetRawText());
                            if (eventData.RootElement.TryGetProperty("headers", out var headers))
                            {
                                if (headers.TryGetProperty("Authorization", out var authHeader))
                                {
                                    var authValue = authHeader.GetString();
                                    if (authValue != null && authValue.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                                    {
                                        capturedBearerToken = authValue.Substring("Bearer ".Length);
                                        Console.WriteLine("Bearer token found in request headers!");
                                        tokenFound.Set();
                                    }
                                }
                            }
                        }
                        catch
                        {
                            // Silently ignore parsing errors
                        }
                    }
                };
            }
            else
            {
                Console.WriteLine("This browser doesn't support DevTools Protocol for network monitoring");
                return null;
            }

            driver.Navigate().GoToUrl(url);
            Console.WriteLine($"Navigated to {url}");

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            var usernameField = wait.Until(d => d.FindElement(By.CssSelector(usernameSelector)));
            usernameField.Clear();
            usernameField.SendKeys(username);

            var passwordField = wait.Until(d => d.FindElement(By.CssSelector(passwordSelector)));
            passwordField.Clear();
            passwordField.SendKeys(password);

            var submitButton = driver.FindElement(By.CssSelector("button[type='submit'],input[type='submit']"));
            submitButton.Click();

            Console.WriteLine("Waiting for OAuth flow to complete and token to be captured...");

            const int MaxWaitTimeSeconds = 30;
            bool tokenCaptured = tokenFound.WaitOne(TimeSpan.FromSeconds(MaxWaitTimeSeconds));

            if (tokenCaptured)
            {
                Console.WriteLine("Bearer token captured successfully");
                return capturedBearerToken;
            }
            else
            {
                Console.WriteLine("Timeout: No bearer token captured within the time limit");
                return null;
            }
        }
        finally
        {
            driver?.Quit();
        }
    }
}
