using AutoMapper;
using InsureFlowAPI.DTOs.Payment;
using InsureFlowAPI.Models;
using InsureFlowAPI.Models.Enums;
using InsureFlowAPI.Repositories.Interfaces;
using InsureFlowAPI.Services.Interfaces;
using InsureFlowAPI.Exceptions;
using InsureFlowAPI.DTOs.Common;
using Microsoft.Extensions.Logging;

namespace InsureFlowAPI.Services.Implementations
{
    public class PremiumPaymentService : IPremiumPaymentService
    {
        private readonly IPremiumPaymentRepository _premiumPaymentRepository;
        private readonly IPolicyRepository _policyRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PremiumPaymentService> _logger;
        private readonly ICustomerRepository _customerRepository;

        public PremiumPaymentService(IPremiumPaymentRepository premiumPaymentRepository,IPolicyRepository policyRepository,
    ICustomerRepository customerRepository,IMapper mapper,ILogger<PremiumPaymentService> logger)
        {
            _premiumPaymentRepository = premiumPaymentRepository;
            _policyRepository = policyRepository;
            _customerRepository = customerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        // Get all payments
        public async Task<PagedResponse<PremiumPaymentResponseDto>> GetAllPaymentsAsync(PaginationRequestDto paginationDto)
        {
            _logger.LogInformation("Retrieving premium payments. Page: {Page}, Size: {Size}",
                paginationDto.PageNumber,paginationDto.PageSize);

            var pagedPayments = await _premiumPaymentRepository.GetAllAsync(paginationDto);

            return new PagedResponse<PremiumPaymentResponseDto>
            {
                Records = _mapper.Map<IEnumerable<PremiumPaymentResponseDto>>(pagedPayments.Records),

                CurrentPage = pagedPayments.CurrentPage,

                PageSize = pagedPayments.PageSize,

                TotalRecords = pagedPayments.TotalRecords,

                TotalPages = pagedPayments.TotalPages,

                IsLastPage = pagedPayments.IsLastPage,

                SortField = pagedPayments.SortField,

                SortDirection = pagedPayments.SortDirection
            };
        }

        // Get payments by Policy Id
        public async Task<PagedResponse<PremiumPaymentResponseDto>> GetPaymentsByPolicyIdAsync(int policyId,PaginationRequestDto paginationDto)
        {

            var policy = await _policyRepository.GetByIdAsync(policyId);

            if (policy == null)
                throw new NotFoundException("Policy not found.");

            var pagedPayments = await _premiumPaymentRepository
                .GetPaymentsByPolicyIdAsync(policyId, paginationDto);

            return new PagedResponse<PremiumPaymentResponseDto>
            {
                Records = _mapper.Map<IEnumerable<PremiumPaymentResponseDto>>(pagedPayments.Records),

                CurrentPage = pagedPayments.CurrentPage,

                PageSize = pagedPayments.PageSize,

                TotalRecords = pagedPayments.TotalRecords,

                TotalPages = pagedPayments.TotalPages,

                IsLastPage = pagedPayments.IsLastPage,

                SortField = pagedPayments.SortField,

                SortDirection = pagedPayments.SortDirection
            };
        }

        // Get payments by Customer Id
        public async Task<PagedResponse<PremiumPaymentResponseDto>> GetPaymentsByCustomerIdAsync(int customerId,int userId,string role,PaginationRequestDto paginationDto)
        {
            var policies = await _policyRepository.GetPoliciesByCustomerIdAsync(customerId);

            if (!policies.Any())
                throw new NotFoundException("Customer has no policies.");

            // Customers can view only their own payments
            if (role == "Customer")
            {
                if (policies.First().Customer.UserId != userId)
                {
                    throw new UnauthorizedAccessException(
                        "You can view only your own payment history.");
                }
            }

            var pagedPayments = await _premiumPaymentRepository
                .GetPaymentsByCustomerIdAsync(customerId, paginationDto);

            return new PagedResponse<PremiumPaymentResponseDto>
            {
                Records = _mapper.Map<IEnumerable<PremiumPaymentResponseDto>>(pagedPayments.Records),

                CurrentPage = pagedPayments.CurrentPage,

                PageSize = pagedPayments.PageSize,

                TotalRecords = pagedPayments.TotalRecords,

                TotalPages = pagedPayments.TotalPages,

                IsLastPage = pagedPayments.IsLastPage,

                SortField = pagedPayments.SortField,

                SortDirection = pagedPayments.SortDirection
            };
        }
        // Get payment by Id
        public async Task<PremiumPaymentResponseDto?> GetPaymentByIdAsync(int id)
        {
            _logger.LogInformation(
                "Retrieving payment with ID {PaymentId}",
                id);

            var payment = await _premiumPaymentRepository.GetByIdAsync(id);

            if (payment == null)
                return null;

            return _mapper.Map<PremiumPaymentResponseDto>(payment);
        }
        public async Task<PremiumPaymentResponseDto> MakePaymentAsync(PremiumPaymentRequestDto requestDto,int userId,string role)
        {
            // GET POLICY
            var policy = await _policyRepository.GetByIdAsync(requestDto.PolicyId);

            if (policy == null)
                throw new NotFoundException("Policy not found.");

            // CUSTOMER OWNERSHIP VALIDATION
            if (role == "Customer")
            {
                if (policy.Customer.UserId != userId)
                {
                    _logger.LogWarning(
                        "Unauthorized payment attempt. UserId: {UserId}, PolicyId: {PolicyId}",
                        userId,
                        policy.PolicyId);

                    throw new UnauthorizedAccessException(
                        "You can make payment only for your own policy.");
                }
            }
            // CUSTOMER VALIDATION
            
            if (!policy.Customer.User.IsActive)
            {
                throw new BadRequestException(
                    "Customer account is inactive.");
            }

            // PLAN VALIDATION
            if (!policy.Plan.IsActive)
            {
                throw new BadRequestException(
                    "Policy plan is inactive.");
            }
            // PRODUCT VALIDATION

            if (!policy.InsuranceProduct.IsActive)
            {
                throw new BadRequestException(
                    "Insurance product is inactive.");
            }

            // POLICY STATUS VALIDATION
            if (policy.PolicyStatus == PolicyStatus.Cancelled)
            {
                throw new BadRequestException(
                    "Cancelled policies cannot accept payments.");
            }

            if (policy.PolicyStatus == PolicyStatus.Expired)
            {
                throw new BadRequestException(
                    "Expired policies cannot accept payments.");
            }

            // -------------------------------------------------------
            // CURRENT DATE
            // -------------------------------------------------------

            var now = DateTime.UtcNow;

            var today = DateOnly.FromDateTime(now);

            // -------------------------------------------------------
            // CHECK WHETHER COVERAGE IS ALREADY FULLY PAID
            // -------------------------------------------------------

            if (policy.TotalPremiumPaid >= policy.Plan.CoverageAmount)
            {
                throw new BadRequestException(
                    "The total coverage amount has already been paid.");
            }

            // -------------------------------------------------------
            // ONE-TIME PREMIUM
            // -------------------------------------------------------

            if (policy.Plan.PremiumType == PremiumType.OneTime)
            {
                if (policy.TotalPremiumPaid > 0)
                {
                    throw new BadRequestException(
                        "One-time premium has already been paid for this policy.");
                }
            }

            // -------------------------------------------------------
            // MONTHLY PREMIUM - CHECK NEXT DUE DATE
            // -------------------------------------------------------

            if (policy.Plan.PremiumType == PremiumType.Monthly)
            {
                if (policy.NextPremiumDueDate.HasValue &&
                    today < policy.NextPremiumDueDate.Value)
                {
                    throw new BadRequestException(
                        $"Next premium payment is due on " +
                        $"{policy.NextPremiumDueDate.Value:dd-MM-yyyy}.");
                }
            }

            // -------------------------------------------------------
            // VALIDATE PAYMENT AMOUNT
            // -------------------------------------------------------

            if (requestDto.Amount <= 0)
            {
                throw new BadRequestException(
                    "Payment amount must be greater than zero.");
            }

            // -------------------------------------------------------
            // CALCULATE REMAINING COVERAGE AMOUNT
            // -------------------------------------------------------

            var remainingAmount =
                policy.Plan.CoverageAmount - policy.TotalPremiumPaid;

            // -------------------------------------------------------
            // PAYMENT CANNOT EXCEED REMAINING COVERAGE
            // -------------------------------------------------------

            if (requestDto.Amount > remainingAmount)
            {
                throw new BadRequestException(
                    $"Payment exceeds the remaining coverage amount. " +
                    $"Remaining amount: {remainingAmount}.");
            }

            // -------------------------------------------------------
            // NORMAL PAYMENT VS FINAL PAYMENT
            // -------------------------------------------------------
            //
            // Normal payment:
            //      Must equal Plan.PremiumAmount
            //
            // Final payment:
            //      Can be less than Plan.PremiumAmount
            //
            // Example:
            // Coverage = 100000
            // Premium = 15000
            // Total Paid = 90000
            // Remaining = 10000
            //
            // Final payment of 10000 is allowed.
            // -------------------------------------------------------

            bool isFinalPayment =
                requestDto.Amount == remainingAmount;

            if (!isFinalPayment &&
                requestDto.Amount != policy.Plan.PremiumAmount)
            {
                throw new BadRequestException(
                    $"Payment must be exactly {policy.Plan.PremiumAmount}, " +
                    $"unless this is the final payment. " +
                    $"Final payment amount is {remainingAmount}.");
            }

            // -------------------------------------------------------
            // TRANSACTION REFERENCE VALIDATION
            // -------------------------------------------------------

            if (string.IsNullOrWhiteSpace(requestDto.TransactionReference))
            {
                throw new BadRequestException(
                    "Transaction reference is required.");
            }

            string transactionReference =
                requestDto.TransactionReference.Trim();

            // -------------------------------------------------------
            // DUPLICATE TRANSACTION CHECK
            // -------------------------------------------------------

            var existingPayment =
                await _premiumPaymentRepository
                    .GetByTransactionReferenceAsync(transactionReference);

            if (existingPayment != null)
            {
                _logger.LogWarning(
                    "Duplicate transaction reference detected: {TransactionReference}",
                    transactionReference);

                throw new ConflictException(
                    "Transaction reference already exists.");
            }

            // -------------------------------------------------------
            // CREATE PAYMENT
            // -------------------------------------------------------

            var payment = new PremiumPayment
            {
                CustomerId = policy.CustomerId,

                PolicyId = policy.PolicyId,

                Amount = requestDto.Amount,

                PaymentDate = now,

                PaymentMode = requestDto.PaymentMode,

                TransactionReference = transactionReference,

                PaymentStatus = PaymentStatus.Success,

                CreatedDate = now
            };

            _logger.LogInformation(
                "Recording premium payment. PolicyId: {PolicyId}, " +
                "CustomerId: {CustomerId}, Amount: {Amount}",
                payment.PolicyId,
                payment.CustomerId,
                payment.Amount);

            await _premiumPaymentRepository.AddAsync(payment);

            // -------------------------------------------------------
            // UPDATE TOTAL PREMIUM PAID
            // -------------------------------------------------------

            policy.TotalPremiumPaid += requestDto.Amount;

            // -------------------------------------------------------
            // UPDATE LAST PAYMENT DATE
            // -------------------------------------------------------

            policy.LastPremiumPaymentDate = now;

            // -------------------------------------------------------
            // ACTIVATE POLICY AFTER FIRST PAYMENT
            // -------------------------------------------------------

            if (policy.PolicyStatus == PolicyStatus.PendingPayment)
            {
                policy.PolicyStatus = PolicyStatus.Active;
            }

            // -------------------------------------------------------
            // CALCULATE NEXT PREMIUM DUE DATE
            // -------------------------------------------------------

            if (policy.Plan.PremiumType == PremiumType.Monthly)
            {
                policy.NextPremiumDueDate =
                    today.AddMonths(1);
            }

            // -------------------------------------------------------
            // CHECK WHETHER COVERAGE IS FULLY PAID
            // -------------------------------------------------------

            if (policy.TotalPremiumPaid >= policy.Plan.CoverageAmount)
            {
                // No more premium payments required
                policy.NextPremiumDueDate = null;

                _logger.LogInformation(
                    "Coverage amount fully paid for PolicyId: {PolicyId}.",
                    policy.PolicyId);
            }

            // -------------------------------------------------------
            // UPDATE POLICY
            // -------------------------------------------------------

            policy.UpdatedDate = now;

            await _policyRepository.UpdateAsync(policy);

            // -------------------------------------------------------
            // SAVE PAYMENT + POLICY
            // -------------------------------------------------------

            await _premiumPaymentRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Premium payment recorded successfully. PaymentId: {PaymentId}",
                payment.PaymentId);

            // -------------------------------------------------------
            // RETURN RESPONSE
            // -------------------------------------------------------

            return _mapper.Map<PremiumPaymentResponseDto>(payment);
        }
        public async Task<PremiumDueResponseDto> GetPremiumDueAsync(int userId)
        {
            var customer = await _customerRepository.GetByUserIdAsync(userId);

            if (customer == null)
                throw new NotFoundException("Customer not found.");

            var policies = await _policyRepository.GetPoliciesByCustomerIdAsync(customer.CustomerId);

            var policy = policies.FirstOrDefault(x => x.PolicyStatus == PolicyStatus.Active);

            if (policy == null)
                throw new NotFoundException("No active policy found.");

            string status;

            if (policy.NextPremiumDueDate == null)
                status = "Completed";
            else if (DateOnly.FromDateTime(DateTime.UtcNow) >= policy.NextPremiumDueDate)
                status = "Due";
            else
                status = "Paid";

            return new PremiumDueResponseDto
            {
                PolicyId = policy.PolicyId,
                PolicyNumber = policy.PolicyNumber,
                CoverageAmount = policy.Plan.CoverageAmount,
                PremiumAmount = policy.Plan.PremiumAmount,
                NextPremiumDueDate = policy.NextPremiumDueDate,
                LastPremiumPaymentDate = policy.LastPremiumPaymentDate,
                Status = status
            };
        }
        public async Task<PagedResponse<PremiumPaymentResponseDto>> GetMyPaymentsAsync(
    int userId,
    PaginationRequestDto paginationDto)
        {
            // Find the customer using the logged-in user's ID
            var customer = await _customerRepository.GetByUserIdAsync(userId);

            if (customer == null)
                throw new NotFoundException("Customer not found.");

            // Get the customer's payments
            var pagedPayments = await _premiumPaymentRepository
                .GetPaymentsByCustomerIdAsync(customer.CustomerId, paginationDto);

            return new PagedResponse<PremiumPaymentResponseDto>
            {
                Records = _mapper.Map<IEnumerable<PremiumPaymentResponseDto>>(pagedPayments.Records),

                CurrentPage = pagedPayments.CurrentPage,

                PageSize = pagedPayments.PageSize,

                TotalRecords = pagedPayments.TotalRecords,

                TotalPages = pagedPayments.TotalPages,

                IsLastPage = pagedPayments.IsLastPage,

                SortField = pagedPayments.SortField,

                SortDirection = pagedPayments.SortDirection
            };
        }
    }
}