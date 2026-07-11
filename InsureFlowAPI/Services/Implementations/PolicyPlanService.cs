using AutoMapper;
using InsureFlowAPI.DTOs.PolicyPlan;
using InsureFlowAPI.Exceptions;
using InsureFlowAPI.Models;
using InsureFlowAPI.Models.Enums;
using InsureFlowAPI.Repositories.Interfaces;
using InsureFlowAPI.Services.Interfaces;
using Microsoft.Extensions.Logging;
using InsureFlowAPI.DTOs.Common;
namespace InsureFlowAPI.Services.Implementations
{
    public class PolicyPlanService : IPolicyPlanService
    {
        private readonly IPolicyPlanRepository _planRepository;
        private readonly IInsuranceProductRepository _productRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PolicyPlanService> _logger;

        public PolicyPlanService(IPolicyPlanRepository planRepository,
        IInsuranceProductRepository productRepository,IMapper mapper,ILogger<PolicyPlanService> logger)
        {
            _planRepository = planRepository;
            _productRepository = productRepository;
            _mapper = mapper;
            _logger = logger;
        }

        // Get all plans
        public async Task<PagedResponse<PolicyPlanResponseDto>> GetAllPlansAsync(PolicyPlanQueryDto query)
        {
            var pagedPlans = await _planRepository.GetAllAsync(query);

            _logger.LogInformation("Retrieved policy plans. PageNumber: {PageNumber}, PageSize: {PageSize}",
                query.PageNumber,
                query.PageSize);

            return new PagedResponse<PolicyPlanResponseDto>
            {
                Records = _mapper.Map<IEnumerable<PolicyPlanResponseDto>>(pagedPlans.Records),
                CurrentPage = pagedPlans.CurrentPage,
                PageSize = pagedPlans.PageSize,
                TotalRecords = pagedPlans.TotalRecords,
                TotalPages = pagedPlans.TotalPages,
                IsLastPage = pagedPlans.IsLastPage,
                SortField = pagedPlans.SortField,
                SortDirection = pagedPlans.SortDirection
            };
        }

        // Get active plans
        public async Task<IEnumerable<PolicyPlanResponseDto>> GetActivePlansAsync()
        {
            var plans = await _planRepository.GetActivePlansAsync();

            _logger.LogInformation("Retrieved active policy plans.");

            return _mapper.Map<IEnumerable<PolicyPlanResponseDto>>(plans);
        }

        // Get plans by product
        public async Task<IEnumerable<PolicyPlanResponseDto>> GetPlansByProductIdAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
                throw new NotFoundException($"Insurance Product with Id {productId} not found.");

            var plans = await _planRepository.GetByProductIdAsync(productId);

            _logger.LogInformation("Retrieved policy plans for Product Id {ProductId}.",productId);

            return _mapper.Map<IEnumerable<PolicyPlanResponseDto>>(plans);
        }

        // Get active plans by product
        public async Task<IEnumerable<PolicyPlanResponseDto>> GetActivePlansByProductIdAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            if (product == null)
                throw new NotFoundException($"Insurance Product with Id {productId} not found.");

            if (!product.IsActive)
                throw new BadRequestException("Insurance Product is inactive.");

            var plans = await _planRepository.GetActivePlansByProductIdAsync(productId);

            _logger.LogInformation("Retrieved active policy plans for Product Id {ProductId}.",productId);
            return _mapper.Map<IEnumerable<PolicyPlanResponseDto>>(plans);
        }

        // Get plan by Id
        public async Task<PolicyPlanResponseDto?> GetPlanByIdAsync(int id)
        {
            var plan = await _planRepository.GetByIdAsync(id);
            _logger.LogInformation("Retrieved policy plan with Id {PlanId}.",id);

            if (plan == null)
                return null;

            return _mapper.Map<PolicyPlanResponseDto>(plan);
        }

        // Create new plan
        public async Task<PolicyPlanResponseDto> CreatePlanAsync(PolicyPlanRequestDto requestDto)
        {
            ValidatePlan(requestDto);
            var product = await _productRepository.GetByIdAsync(requestDto.ProductId);

            if (product == null)
                throw new NotFoundException($"Insurance Product with Id {requestDto.ProductId} not found.");

            if (!product.IsActive)
                throw new BadRequestException("Cannot create a plan for an inactive insurance product.");

            string planName = requestDto.PlanName.Trim();

            var existingPlan = await _planRepository.GetByNameAsync(planName);

            if (existingPlan != null)
                throw new ConflictException("Plan name already exists.");

            var plan = new PolicyPlan
            {
                ProductId = requestDto.ProductId,
                PlanName = planName,
                CoverageAmount = requestDto.CoverageAmount,
                PremiumAmount = requestDto.PremiumAmount,
                PremiumType = requestDto.PremiumType,
                DurationYears = requestDto.DurationYears,
                TermsAndConditions = requestDto.TermsAndConditions.Trim(),
                IsActive = requestDto.IsActive,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            await _planRepository.AddAsync(plan);
            await _planRepository.SaveChangesAsync();

            _logger.LogInformation("Policy plan '{PlanName}' created successfully with Id {PlanId}.",
                plan.PlanName,plan.PlanId);

            var createdPlan = await _planRepository.GetByIdAsync(plan.PlanId);

            if (createdPlan == null)
                throw new BadRequestException("Plan creation failed.");

            return _mapper.Map<PolicyPlanResponseDto>(createdPlan);
        }

        // Update existing plan
        public async Task UpdatePlanAsync(int id, PolicyPlanRequestDto requestDto)
        {
            ValidatePlan(requestDto);
            var plan = await _planRepository.GetByIdAsync(id);

            if (plan == null)
                throw new NotFoundException($"Policy Plan with Id {id} not found.");

            if (!plan.IsActive)
                throw new BadRequestException("Inactive plan cannot be updated.");

            var product = await _productRepository.GetByIdAsync(requestDto.ProductId);

            if (product == null)
                throw new NotFoundException($"Insurance Product with Id {requestDto.ProductId} not found.");

            if (!product.IsActive)
                throw new BadRequestException("Cannot assign an inactive insurance product.");

            string planName = requestDto.PlanName.Trim();

            var existingPlan = await _planRepository.GetByNameAsync(planName);

            if (existingPlan != null && existingPlan.PlanId != id)
                throw new ConflictException("Another plan with the same name already exists.");

            plan.ProductId = requestDto.ProductId;
            plan.PlanName = planName;
            plan.CoverageAmount = requestDto.CoverageAmount;
            plan.PremiumAmount = requestDto.PremiumAmount;
            plan.PremiumType = requestDto.PremiumType;
            plan.DurationYears = requestDto.DurationYears;
            plan.TermsAndConditions = requestDto.TermsAndConditions.Trim();
            plan.IsActive = requestDto.IsActive;
            plan.UpdatedDate = DateTime.UtcNow;

            await _planRepository.UpdateAsync(plan);
            await _planRepository.SaveChangesAsync();

            _logger.LogInformation("Policy plan with Id {PlanId} updated successfully.",id);
        }

        // Soft delete plan
        public async Task SoftDeletePlanAsync(int id)
        {
            var plan = await _planRepository.GetByIdAsync(id);

            if (plan == null)
                throw new NotFoundException($"Policy Plan with Id {id} not found.");

            if (!plan.IsActive)
                throw new BadRequestException("Policy Plan is already inactive.");

            await _planRepository.SoftDeleteAsync(plan);
            await _planRepository.SaveChangesAsync();

            _logger.LogInformation("Policy plan with Id {PlanId} deactivated successfully.",id);
        }
        private static void ValidatePlan(PolicyPlanRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.PlanName))
                throw new BadRequestException("Plan name is required.");

            if (dto.PlanName.Trim().Length < 3)
                throw new BadRequestException("Plan name must contain at least 3 characters.");

            if (dto.PlanName.Trim().Length > 100)
                throw new BadRequestException("Plan name cannot exceed 100 characters.");

            if (dto.CoverageAmount <= 0)
                throw new BadRequestException("Coverage amount must be greater than zero.");

            if (dto.PremiumAmount <= 0)
                throw new BadRequestException("Premium amount must be greater than zero.");

            if (dto.CoverageAmount <= dto.PremiumAmount)
                throw new BadRequestException("Coverage amount must be greater than premium amount.");

            if (dto.DurationYears <= 0)
                throw new BadRequestException("Duration must be greater than zero.");

            if (string.IsNullOrWhiteSpace(dto.TermsAndConditions))
                throw new BadRequestException("Terms and Conditions are required.");

            if (dto.TermsAndConditions.Trim().Length > 1000)
                throw new BadRequestException("Terms and Conditions cannot exceed 1000 characters.");

            if (!Enum.IsDefined(typeof(PremiumType), dto.PremiumType))
                throw new BadRequestException("Invalid premium type.");
        }
    }
}