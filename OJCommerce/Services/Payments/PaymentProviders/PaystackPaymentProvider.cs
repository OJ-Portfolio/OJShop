using Microsoft.Extensions.Options;
using OJCommerce.Config;
using OJCommerce.Dtos.Payments;
using OJCommerce.Dtos.Payments.PaymentProviders.Paystack;
using OJCommerce.Enums.Payments;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace OJCommerce.Services.Payments.PaymentProviders
{
    public class PaystackPaymentProvider : IPaymentProvider
    {
        public PaymentProvider providerType => PaymentProvider.Paystack;
        private readonly HttpClient _httpClient;
        private readonly string _secretKey;
        private readonly PaystackSettings _settings;
        private readonly ILogger<PaystackPaymentProvider> _logger;

        public IEnumerable<string> SupportedCurrencies => new[]
          {
                "NGN", "GHS", "ZAR", "KES"
          };

        public IEnumerable<PaymentMethod> SupportedMethods => new[]
        {
            PaymentMethod.Card,
            PaymentMethod.BankTransfer,
            PaymentMethod.USSD,
            PaymentMethod.MobileMoney
        };

        public PaystackPaymentProvider(HttpClient httpClient, IOptions<PaymentProviderOptions> options, ILogger<PaystackPaymentProvider> logger)
        {
            _httpClient = httpClient;
            _settings = options.Value.Paystack;
            _secretKey = _settings.SecretKey;
            _logger = logger;

            if (_httpClient.BaseAddress == null)
            {
                _httpClient.BaseAddress = new Uri(_settings.BaseUrl);
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", _secretKey);
                _httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
            }
        }

        //INITIALIZE PAYMENT
        public async Task<PaymentInitializationResponse> InitializePaymentAsync(PaymentRequest request)
        {
            var payload = new
            {
                email = request.CustomerEmail,
                amount = (int)(request.Amount * 100),
                currency = request.Currency,
                callback_url = request.CallbackUrl,
                metadata = request.Metadata,
            };
            var response = await _httpClient.PostAsJsonAsync("transaction/initialize", payload);
            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                return new PaymentInitializationResponse
                {
                    Success = false,
                    Message = content
                };
            }
            var result = JsonSerializer.Deserialize<PaystackInitializeResponse>(content);
            return new PaymentInitializationResponse
            {
                Success = true,
                AuthorizationUrl = result.data.authorization_url,
                TransactionReference = result.data.reference,
            };
        }

        //VERIFY PAYMENT
        public async Task<PaymentVerificationResponse> VerifyPaymentAsync(string transactionReference)
        {
            var response = await _httpClient.GetAsync($"transaction/verify/{transactionReference}");
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new PaymentVerificationResponse
                {
                    Success = false,
                    Message = content
                };
            }
            var result = JsonSerializer.Deserialize<PaystackVerifyResponse>(content);
            return new PaymentVerificationResponse
            {
                Status = result.data.status == "success" ? PaymentStatus.Completed : PaymentStatus.Failed,
            };
        }

        //WEBHOOK VALIDATION for PAYSTACK
        public Task<WebhookValidationResult> ValidateAndProcessWebhookAsync(string payload, string signature)
        {
            _logger.LogWarning("🔐 Validating signature");
            _logger.LogWarning("Secret key starts with: {Start}", _secretKey?.Substring(0, 7));
            _logger.LogWarning("Received signature: {Signature}", signature);

            var computedHash = ComputeHmacSha512(payload, _secretKey);

            _logger.LogWarning("Computed signature: {Computed}", computedHash);

            if (!string.Equals(computedHash, signature, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("❌ Signatures don't match!");
                return Task.FromResult(new WebhookValidationResult { IsValid = false });
            }

            _logger.LogWarning("✅ Signature valid!");

            using var doc = JsonDocument.Parse(payload);
            var data = doc.RootElement.GetProperty("data");
            var reference = data.GetProperty("reference").GetString();
            var status = data.GetProperty("status").GetString();

            return Task.FromResult(new WebhookValidationResult
            {
                IsValid = true,
                TransactionReference = reference,
                Status = status == "success"
                    ? PaymentStatus.Completed
                    : PaymentStatus.Failed
            });

        }


        private static string ComputeHmacSha512(string payload, string secret)
        {
            var keyBytes = Encoding.UTF8.GetBytes(secret);
            var payloadBytes = Encoding.UTF8.GetBytes(payload);

            using var hmac = new HMACSHA512(keyBytes);
            var hash = hmac.ComputeHash(payloadBytes);

            return BitConverter.ToString(hash)
                .Replace("-", "")
                .ToLowerInvariant();
        }
    }
}
