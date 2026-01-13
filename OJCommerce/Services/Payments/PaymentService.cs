using Microsoft.EntityFrameworkCore;
using OJCommerce.Config;
using OJCommerce.Data;
using OJCommerce.Domain.Events;
using OJCommerce.Dtos.Payments;
using OJCommerce.Enums;
using OJCommerce.Enums.Payments;
using OJCommerce.Exceptions;
using OJCommerce.Models.Orders;
using OJCommerce.Models.Transactions;
using OJCommerce.Models.Webhooks;
using OJCommerce.Services.Users;
using System.Text.Json;
using Microsoft.Extensions.Options;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;
using OJCommerce.Models.PaymentMethods;
using OJCommerce.Dtos.Webhook;
using OJCommerce.Services.QuartzServices;

namespace OJCommerce.Services.Payments
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        private readonly ILogger<PaymentService> _logger;
        private readonly IEnumerable<IPaymentProvider> _providers;
        private readonly PaymentProviderOptions _paymentProviderOptions;
        private readonly IJobScheduler _jobScheduler;
        public PaymentService(AppDbContext context, IUserService userService, ILogger<PaymentService> logger, IEnumerable<IPaymentProvider> providers, IOptions<PaymentProviderOptions> paymentProviderOptions, IJobScheduler jobScheduler)
        {
            _context = context;
            _userService = userService;
            _logger = logger;
            _providers = providers;
            _paymentProviderOptions = paymentProviderOptions.Value;
            _jobScheduler = jobScheduler;
        }

        public PaymentProvider PaymentProvider => throw new NotImplementedException();

        public async Task<PaymentDto> GetPaymentAsync(Guid paymentId)
        {
            var currentUserPublicId = _userService.GetCurrentUser();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PublicUserId == currentUserPublicId);
            if (user == null) throw new NotFoundException("user not found");
            var payyment = await _context.PaymentTransactions.Include(p => p.Order).FirstOrDefaultAsync
                (p => p.PublicPaymentId == paymentId && p.Order.UserId == user.Id);
            if (payyment == null)
            {
                throw new NotFoundException("payment not found");
            }

            return MapToDto(payyment, payyment.Order.PublicOrderId);
        }

        public async Task<PaymentDto> GetPaymentByOrderAsync(Guid orderId)
        {
            var publicUserId = _userService.GetCurrentUser();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PublicUserId == publicUserId);
            if (user == null) throw new NotFoundException("user not found");
            var payment = await _context.PaymentTransactions.Include(p => p.Order).FirstOrDefaultAsync(p => p.Order.PublicOrderId == orderId &&
                p.Order.UserId == user.Id);
            
            if (payment == null)
            {
                throw new NotFoundException("payment not found");
            }
            return MapToDto(payment, payment.Order.PublicOrderId);
        }

        public async Task<List<PaymentDto>> GetUserPaymentsAsync()
        {
            var publicUserId = _userService.GetCurrentUser();
            var user = _context.Users.FirstOrDefault(u => u.PublicUserId == publicUserId);
            if (user == null) throw new NotFoundException("user not found");
            var payments = await _context.PaymentTransactions.Include(p => p.Order)
                .Where(p => p.Order.UserId == user.Id)
                .OrderByDescending(p => p.Order.CreatedAt)
                .ToListAsync();
            return payments.Select(p => MapToDto(p, p.Order.PublicOrderId)).ToList();
                
        }

        
        public async Task<PaymentDto> InitiatePaymentAsync(InitiatePaymentDto request)
        {
            var publicUserId = _userService.GetCurrentUser();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PublicUserId == publicUserId);
            if (user == null)
            {
                throw new NotFoundException("user not found");
            }

            var order = await _context.Orders
                .Include(o => o.Payment)
                .FirstOrDefaultAsync(o => o.PublicOrderId == request.OrderId && o.UserId == user.Id);

            if (order == null)
            {
                throw new NotFoundException("order not found");
            }

            if (order.Payment != null)
            {
                if (order.Payment.Status == PaymentStatus.Completed)
                    throw new BusinessRuleViolationException("Order has already been paid");

                if (order.Payment.Status == PaymentStatus.Pending)
                    return MapToDto(order.Payment, order.PublicOrderId); // reuse pending payment
            }

            // Validate that provider was provided
            if (!request.Provider.HasValue)
            {
                throw new BusinessRuleViolationException("Payment provider must be specified");
            }

            if (!request.Method.HasValue)
            {
                throw new BusinessRuleViolationException("Payment method must be specified");
            }

            // Get the selected provider
            var provider = _providers.FirstOrDefault(p => p.providerType == request.Provider.Value);
            if (provider == null)
                throw new BusinessRuleViolationException($"Payment provider {request.Provider} is not supported");

            // Validate that the selected method is supported by the provider
            if (!provider.SupportedMethods.Contains(request.Method.Value))
            {
                throw new BusinessRuleViolationException(
                    $"Payment method {request.Method} is not supported by {request.Provider}");
            }

            var providerEnum = request.Provider.Value;
            var methodEnum = request.Method.Value;

            var callbackUrl = providerEnum switch
            {
                PaymentProvider.Paystack => _paymentProviderOptions.Paystack.CallbackUrl,
                PaymentProvider.Stripe => _paymentProviderOptions.Stripe.CallbackUrl,
                _ => throw new NotSupportedException($"Callback URL not configured for {providerEnum}")
            };

            var paymentRequest = new PaymentRequest
            {
                OrderId = request.OrderId,
                Amount = order.TotalAmount,
                CustomerEmail = user.Email,
                Currency = order.Currency ?? "NGN",
                Provider = providerEnum,
                Method = methodEnum,
                CallbackUrl = callbackUrl,
                Metadata = new Dictionary<string, string>
        {
            { "order_id", order.PublicOrderId.ToString() },
            { "user_id", user.PublicUserId.ToString() },
            { "customer_name", $"{user.FirstName} {user.LastName}" }
        }
            };

            var providerResponse = await provider.InitializePaymentAsync(paymentRequest);

            if (!providerResponse.Success)
            {
                _logger.LogError($"Payment initialization failed: {providerResponse.Message}");
                throw new BusinessRuleViolationException("Payment initialization failed. Please try again.");
            }

            PaymentTransaction payment;
            if(order.Payment != null)
            {
                payment = order.Payment;
                payment.Status = PaymentStatus.Pending;
                payment.Currency = order.Currency ?? "NGN";
                payment.Provider = providerEnum;
                payment.Method = methodEnum;
                payment.ProviderTransactionReference = providerResponse.TransactionReference;
                payment.AuthorizationUrl = providerResponse.AuthorizationUrl;
                payment.UpdatedAt = DateTime.UtcNow;
                payment.FailureReason = "";
            }

            else
            {
                payment = new PaymentTransaction
                {
                    OrderId = order.Id,
                    Amount = order.TotalAmount,
                    CustomerEmail = user.Email,
                    Provider = providerEnum,
                    Method = methodEnum,
                    Status = PaymentStatus.Pending,
                    ProviderTransactionReference = providerResponse.TransactionReference,
                    AuthorizationUrl = providerResponse.AuthorizationUrl,
                    Currency = order.Currency ?? "NGN",                    
                };
                _context.PaymentTransactions.Add(payment);
            }
            await _context.SaveChangesAsync();
            _logger.LogInformation($"payment initiated: {payment.PublicPaymentId} for {order.PublicOrderId}");

            return MapToDto(payment, order.PublicOrderId);

        }

        public async Task<PaymentDto> VerifyPaymentAsync(string transactionReference)
        {
            var payment = await _context.PaymentTransactions.Include(p => p.Order).FirstOrDefaultAsync(p => p.ProviderTransactionReference == transactionReference);
            if (payment == null) throw new NotFoundException("payment not found");

            if (payment.Status == PaymentStatus.Completed)
                return MapToDto(payment, payment.Order.PublicOrderId);

            //GET PROVIDER
            var provider = _providers.FirstOrDefault(p => p.providerType == payment.Provider);
            if (provider == null) throw new BusinessRuleViolationException($"payment provider: {payment.Provider} not supported");

            var verificationResponse = await provider.VerifyPaymentAsync(transactionReference);
            payment.Status = verificationResponse.Status;
            payment.UpdatedAt = DateTime.UtcNow;
            if (verificationResponse.Status == PaymentStatus.Completed)
            {
                payment.CompletedAt = DateTime.UtcNow;
                payment.Order.Status = OrderStatus.Processing;
                _logger.LogInformation($"Payment completed: {payment.PublicPaymentId} for order {payment.Order.PublicOrderId}");
            }

            else if (verificationResponse.Status == PaymentStatus.Failed)
            {
                payment.FailureReason = verificationResponse.Message;
                _logger.LogInformation($"Payment Failed for: {payment.PublicPaymentId} - {verificationResponse.Message}");
            }
            await _context.SaveChangesAsync();
            return MapToDto(payment, payment.Order.PublicOrderId);
        }

        //EVERYTHING WEBHOOK

        public async Task<bool> HandleWebhookAsync(PaymentProvider provider, string payload, string signature)
        {
            var paymentProvider = Resolve(provider);

            // Validate signature & parse event
            var validation = await paymentProvider.ValidateAndProcessWebhookAsync(payload, signature);

            if (!validation.IsValid)
            {
                _logger.LogWarning($"Invalid webhook signature from {provider}");
                return false;
            }
            var dto = JsonSerializer.Deserialize<PaystackWebhookDto>(payload, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (dto?.Data == null)
            {
                _logger.LogError("Webhook payload deserialization failed - Data is null");
                return false;
            }
            // Save the webhook for audit / idempotency
            var webhookEvent = new PaymentWebhookEvent
            {
                Provider = PaymentProvider.Paystack,
                EventId = dto.Data.Id.ToString(),
                TransactionReference = dto.Data.Reference,
                Status = dto.Data.Status == "success" ? PaymentStatus.Completed : PaymentStatus.Failed,
                Payload = payload,
                CustomerCode = dto.Data.Customer.CustomerCode,
                AuthorizationCode = dto.Data.Authorization?.AuthorizationCode,
                CardLast4 = dto.Data.Authorization?.Last4,
                CardBrand = dto.Data.Authorization?.Brand,
                CardReusable = dto.Data.Authorization?.Reusable ?? false
            };

            try
            {
                _context.PaymentWebhookEvents.Add(webhookEvent);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Already received this webhook event
                _logger.LogInformation($"Duplicate webhook event {validation.EventId} ignored");
                return true; // acknowledge anyway
            }

            // Process payment based on the webhook
            if (!string.IsNullOrEmpty(dto.Data.Reference))
            {
                await _jobScheduler.EnqueueWebhookProcessingAsync(webhookEvent.Id);
            }

            return true; // ALWAYS acknowledge webhook receipt
        }


        public async Task ProcessPendingWebhookAsync(long webhookEventId)
        {
            using var tx = await _context.Database.BeginTransactionAsync();

            //  Load the webhook event
            var evt = await _context.PaymentWebhookEvents
                .FirstOrDefaultAsync(e => e.Id == webhookEventId && !e.Processed);

            if (evt == null) return;

            // Load the payment and related order + user
            var payment = await _context.PaymentTransactions
                .Include(p => p.Order)
                .ThenInclude(o => o.User)
                .FirstOrDefaultAsync(p =>
                    p.Provider == evt.Provider &&
                    p.ProviderTransactionReference == evt.TransactionReference);

            if (payment == null)
            {
                // No payment found -> mark event processed and exit
                evt.Processed = true;
                evt.ProcessedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                await tx.CommitAsync();
                return;
            }

            //  Already completed or failed -> mark event processed and exit
            if (payment.Status is PaymentStatus.Completed or PaymentStatus.Failed)
            {
                evt.Processed = true;
                evt.ProcessedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                await tx.CommitAsync();
                return;
            }

            // Deserialize webhook payload
            var dto = JsonSerializer.Deserialize<PaystackWebhookDto>(evt.Payload, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Update payment status based on webhook
            if (dto.Data.Status == "success")
            {
                payment.Status = PaymentStatus.Completed;
            }
            else
            {
                payment.Status = PaymentStatus.Failed;
            }

            payment.UpdatedAt = DateTime.UtcNow;

            if (payment.Status == PaymentStatus.Completed)
            {
                payment.CompletedAt = DateTime.UtcNow;
                payment.Order.Status = OrderStatus.Processing;

                // Save reusable payment method if available
                var auth = dto.Data.Authorization;
                var customerCode = dto.Data.Customer.CustomerCode;

                if (auth != null && auth.Reusable)
                {
                    _logger.LogInformation("Authorization is reusable. Last4: {Last4}, CustomerCode: {CustomerCode}",
                        auth.Last4, customerCode);

                    var exists = await _context.SavedPaymentMethods.AnyAsync(m =>
                        m.UserId == payment.Order.User.PublicUserId &&
                        m.Provider == evt.Provider &&
                        m.ProviderCustomerId == customerCode &&
                        m.Last4Digits == auth.Last4
                    );

                    _logger.LogInformation("Payment method exists check: {Exists} for UserId: {UserId}",
                        exists, payment.Order.User.PublicUserId);

                    if (!exists)
                    {
                        var savedMethod = new SavedPaymentMethod
                        {
                            SavedPaymentMethodId = Guid.NewGuid(),
                            UserId = payment.Order.User.PublicUserId,
                            Provider = evt.Provider,
                            Method = PaymentMethod.Card,
                            ProviderCustomerId = customerCode,
                            Last4Digits = auth.Last4,
                            CardBrand = auth.Brand,
                            IsDefault = true,
                            CreatedAt = DateTime.UtcNow
                        };

                        _context.SavedPaymentMethods.Add(savedMethod);
                        _logger.LogInformation("Added SavedPaymentMethod with ID: {Id}", savedMethod.SavedPaymentMethodId);
                    }
                }

                // Raise domain event (if any)
                try
                {
                    RaiseDomainEvent(new PaymentCompletedEvent(payment.Id));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to raise PaymentCompletedEvent for payment {PaymentId}", payment.Id);
                    // Don't rethrow - so, we don't want to rollback the transaction
                }
            }

            // Mark webhook as processed
            evt.Processed = true;
            evt.ProcessedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(); // ← This should save everything
            _logger.LogInformation("SaveChanges completed for webhook {WebhookId}", webhookEventId);

            await tx.CommitAsync();
            _logger.LogInformation("Transaction committed for webhook {WebhookId}", webhookEventId);
        }



        public async Task<PaymentDto> PayWithSavedMethodAsync(
            Guid orderId,
            Guid savedPaymentMethodId)
        {
            var publicUserId = _userService.GetCurrentUser();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PublicUserId == publicUserId);

            if (user == null)
                throw new NotFoundException("User not found");

            var savedMethod = await _context.SavedPaymentMethods
                .FirstOrDefaultAsync(s => s.SavedPaymentMethodId == savedPaymentMethodId && s.UserId == user.PublicUserId);

            if (savedMethod == null)
                throw new NotFoundException("Saved payment method not found");

            // Use the saved method to initiate payment
            return await InitiatePaymentAsync(new InitiatePaymentDto
            {
                OrderId = orderId,
                Provider = savedMethod.Provider,
                Method = savedMethod.Method
            });
        }


        public async Task<IEnumerable<PaymentOptionDto>> GetAvailablePaymentOptionsAsync(
            string currency,
            decimal amount)
        {
            // Get country internally
            var userId = _userService.GetCurrentUser();
            var country = await _context.Users
                .Where(u => u.PublicUserId == userId)
                .Select(u => u.Country)
                .FirstOrDefaultAsync() ?? "NG";

            var availableProviders = _providers
                .Where(p => p.SupportedCurrencies.Contains(currency))
                .Select(p => new PaymentOptionDto
                {
                    Provider = p.providerType,
                    ProviderName = p.providerType.ToString(),
                    AvailableMethods = p.SupportedMethods,
                    IsRecommended = IsRecommendedProvider(p.providerType, currency, country)
                })
                .ToList();

            return availableProviders;
        }



        private bool IsRecommendedProvider(PaymentProvider provider, string currency, string country)
        {
            // Logic to determine recommended provider
            if (currency == "NGN" && provider == PaymentProvider.Paystack)
                return true;

            if ((currency == "USD" || currency == "EUR") && provider == PaymentProvider.Stripe)
                return true;

            return false;
        }


        public async Task<PaymentDto> GetPaymentByOrderIdAsync(Guid orderId)
        {
            var userId = _userService.GetCurrentUser();

            var payment = await _context.PaymentTransactions
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p =>
                    p.Order.PublicOrderId == orderId &&
                    p.Order.User.PublicUserId == userId);

            if (payment == null)
                return null;

            return MapToDto(payment, payment.Order.PublicOrderId);
        }

        public async Task<IEnumerable<SavedPaymentMethodDto>> GetUserSavedPaymentMethodsAsync()
        {
            var publicUserId = _userService.GetCurrentUser();

            var methods = await _context.SavedPaymentMethods
                .Where(m => m.UserId == publicUserId)
                .OrderByDescending(m => m.IsDefault)
                .ThenByDescending(m => m.CreatedAt)
                .Select(m => new SavedPaymentMethodDto
                {
                    Id = m.SavedPaymentMethodId,
                    Provider = m.Provider,
                    Method = m.Method,
                    Last4Digits = m.Last4Digits,
                    CardBrand = m.CardBrand,
                    IsDefault = m.IsDefault,
                    CreatedAt = m.CreatedAt
                })
                .ToListAsync();

            return methods;
        }

        public async Task DeleteSavedPaymentMethodAsync(Guid methodId)
        {
            var userId = _userService.GetCurrentUser();

            var method = await _context.SavedPaymentMethods
                .FirstOrDefaultAsync(m => m.SavedPaymentMethodId == methodId && m.UserId == userId);

            if (method == null)
                throw new NotFoundException("Payment method not found");

            _context.SavedPaymentMethods.Remove(method);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Payment method {methodId} deleted for user {userId}");
        }

        public async Task SetDefaultPaymentMethodAsync(Guid methodId)
        {
            var userId = _userService.GetCurrentUser();

            var method = await _context.SavedPaymentMethods
                .FirstOrDefaultAsync(m => m.SavedPaymentMethodId == methodId && m.UserId == userId);

            if (method == null)
                throw new NotFoundException("Payment method not found");

            // Unset all other defaults
            var userMethods = await _context.SavedPaymentMethods
                .Where(m => m.UserId == userId)
                .ToListAsync();

            foreach (var m in userMethods)
            {
                m.IsDefault = (m.SavedPaymentMethodId == methodId);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation($"Payment method {methodId} set as default for user {userId}");
        }


        private PaymentDto MapToDto(PaymentTransaction payment, Guid orderId)
        {
            return new PaymentDto
            {
                PublicPaymentId = payment.PublicPaymentId,
                OrderId = orderId,
                Amount = payment.Amount,
                Currency = payment.Currency,
                Provider = payment.Provider,
                Method = payment.Method,
                Status = payment.Status,
                TransactionReference = payment.ProviderTransactionReference,
                AuthorizationUrl = payment.AuthorizationUrl,
                FailureReason = payment.FailureReason,
                CreatedAt = payment.CreatedAt,
                CompletedAt = payment.CompletedAt
            };
        }

        private IPaymentProvider Resolve(PaymentProvider provider)
        {
            var paymentProvider = _providers.FirstOrDefault(p => p.providerType == provider);
            if (paymentProvider == null)
                throw new InvalidOperationException($"Payment provider {provider} not registered");

            return paymentProvider;
        }

        private void RaiseDomainEvent(object domainEvent)
        {
            var outbox = new OutboxMessage
            {
                Type = domainEvent.GetType().Name,
                Payload = JsonSerializer.Serialize(domainEvent)
            };

            _context.OutboxMessages.Add(outbox);
        }

    }
}
