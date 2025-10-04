using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace NotificationService.Services
{
    public class WhatsAppSender : IWhatsAppSender
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly IConfiguration _configuration;
        private readonly ILogger<WhatsAppSender> _logger;

        public WhatsAppSender(
            IConfiguration configuration,
            ILogger<WhatsAppSender> logger)
        {
            _configuration = configuration;
            _logger = logger;

            ConfigureHttpClient();
        }

        private void ConfigureHttpClient()
        {
            var whatsAppSettings = _configuration.GetSection("WhatsAppSettings");
            var baseUrl = whatsAppSettings["BaseUrl"];

            // Ensure base URL is not null and ends with a trailing slash so path combining works correctly
            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                baseUrl = "https://wpapi.pro/api/v1/wp/";
            }
            else if (!baseUrl.EndsWith("/"))
            {
                baseUrl += "/";
            }

            var apiKey = whatsAppSettings["ApiKey"];

            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Clear();

            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
            }
            else
            {
                _logger.LogWarning("WhatsApp API key is missing or empty.");
            }

            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            _logger.LogDebug("WhatsApp HttpClient configured. BaseAddress: {BaseAddress}", _httpClient.BaseAddress);
        }

        public async Task SendAsync(string phoneNumber, string message)
        {
            try
            {
                var cleanPhoneNumber = CleanPhoneNumber(phoneNumber);

                var requestPayload = new
                {
                    phoneNumber = cleanPhoneNumber,
                    message = message
                };

                var json = JsonSerializer.Serialize(requestPayload, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogInformation("WhatsApp mesajı gönderiliyor. Telefon: {PhoneNumber}, Mesaj uzunluğu: {MessageLength}",
                    cleanPhoneNumber, message.Length);

                // Use relative path without leading slash to preserve BaseAddress path segments
                var response = await _httpClient.PostAsync("send-message", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("WhatsApp mesajı başarıyla gönderildi. Telefon: {PhoneNumber}, MessageId: {MessageId}",
                        cleanPhoneNumber, responseContent);
                }
                else
                {
                    _logger.LogError("WhatsApp mesajı gönderilemedi. Status: {StatusCode}, Error: {Error}",
                        response.StatusCode, responseContent);
                    throw new HttpRequestException($"WhatsApp API error: {response.StatusCode} - {responseContent}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WhatsApp mesajı gönderilirken hata oluştu. Telefon: {PhoneNumber}", phoneNumber);
                throw;
            }
        }

        private static string CleanPhoneNumber(string phoneNumber)
        {
            var cleaned = new string(phoneNumber.Where(char.IsDigit).ToArray());

            if (cleaned.StartsWith("0"))
            {
                cleaned = "90" + cleaned[1..];
            }
            else if (!cleaned.StartsWith("90"))
            {
                cleaned = "90" + cleaned;
            }

            return cleaned;
        }
    }
}
