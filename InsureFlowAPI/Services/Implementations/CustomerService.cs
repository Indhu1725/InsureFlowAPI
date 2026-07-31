using AutoMapper;
using InsureFlowAPI.DTOs.Common;
using InsureFlowAPI.DTOs.Customer;
using InsureFlowAPI.Exceptions;
using InsureFlowAPI.Models;
using InsureFlowAPI.Models.Enums;
using InsureFlowAPI.Repositories.Interfaces;
using InsureFlowAPI.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace InsureFlowAPI.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerService> _logger;
        private readonly ICloudinaryService _cloudinaryService;

        public CustomerService(ICustomerRepository customerRepository,IUserRepository userRepository,IMapper mapper,ILogger<CustomerService> logger, ICloudinaryService cloudinaryService)
        {
            _customerRepository = customerRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
            _cloudinaryService = cloudinaryService;
        }

        // Get all customers
        public async Task<PagedResponse<CustomerResponseDto>> GetAllCustomersAsync(CustomerQueryDto query)
        {
            // Validate Page Number
            if (query.PageNumber < 1)
            {
                _logger.LogWarning("Invalid page number received: {PageNumber}", query.PageNumber);
                throw new BadRequestException("Page number must be greater than zero.");
            }

            // Validate Page Size
            if (query.PageSize < 1)
            {
                _logger.LogWarning("Invalid page size received: {PageSize}", query.PageSize);
                throw new BadRequestException("Page size must be greater than zero.");
            }

            // Validate Maximum Page Size
            if (query.PageSize > 100)
            {
                _logger.LogWarning("Page size exceeded maximum limit: {PageSize}", query.PageSize);
                throw new BadRequestException("Maximum page size is 100.");
            }

            // Validate Sort Field
            var allowedSortFields = new[]
            {
              "city",
              "state",
              "createddate"
            };

            if (!allowedSortFields.Contains(query.SortBy.ToLower()))
            {
                _logger.LogWarning("Invalid sort field: {SortField}", query.SortBy);

                throw new BadRequestException($"Invalid sort field. Allowed fields are: {string.Join(", ", allowedSortFields)}");
            }

            // Validate Sort Direction
            var allowedDirections = new[]
            {
                "asc",
                "desc"
            };

            if (!allowedDirections.Contains(query.SortDirection.ToLower()))
            {
                _logger.LogWarning("Invalid sort direction: {SortDirection}", query.SortDirection);

                throw new BadRequestException("Sort direction must be either 'asc' or 'desc'.");
            }

            var result = await _customerRepository.GetAllAsync(query);
            _logger.LogInformation("Customer list retrieved. Page: {Page}, Size: {Size}",query.PageNumber,query.PageSize);

            var customerDtos = _mapper.Map<IEnumerable<CustomerResponseDto>>(result.Items);

            var totalPages = (int)Math.Ceiling((double)result.TotalRecords / query.PageSize);

            return new PagedResponse<CustomerResponseDto>
            {
                Records = customerDtos,
                CurrentPage = query.PageNumber,
                PageSize = query.PageSize,
                TotalRecords = result.TotalRecords,
                TotalPages = totalPages,
                IsLastPage = query.PageNumber >= totalPages,
                SortField = query.SortBy,
                SortDirection = query.SortDirection
            };
        }


        // Get all active customers
        public async Task<IEnumerable<CustomerResponseDto>> GetActiveCustomersAsync()
        {
            var customers = await _customerRepository.GetActiveCustomersAsync();

            return _mapper.Map<IEnumerable<CustomerResponseDto>>(customers);
        }

        // Get customer by Id
        public async Task<CustomerResponseDto?> GetCustomerByIdAsync(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);

            if (customer == null)
            {
                _logger.LogWarning("Customer not found. CustomerId: {CustomerId}", id);
                return null;
            }

            _logger.LogInformation("Customer profile retrieved. CustomerId: {CustomerId}", id);

            return _mapper.Map<CustomerResponseDto>(customer);
        }

        // Get customer by User Id
        public async Task<CustomerResponseDto?> GetCustomerByUserIdAsync(int userId)
        {
            var customer = await _customerRepository.GetByUserIdAsync(userId);

            if (customer == null)
            {
                _logger.LogWarning("Customer not found for UserId: {UserId}", userId);
                return null;
            }

            _logger.LogInformation("Customer retrieved successfully for UserId: {UserId}", userId);

            return _mapper.Map<CustomerResponseDto>(customer);
        }
        public async Task<CustomerResponseDto?> GetMyProfileAsync(int loggedInUserId)
        {
            var customer = await _customerRepository.GetByUserIdAsync(loggedInUserId);

            if (customer == null)
            {
                _logger.LogWarning("Customer profile not found for UserId: {UserId}",loggedInUserId);

                return null;
            }

            if (!customer.User.IsActive)
            {
                _logger.LogWarning("Inactive user attempted to access profile. UserId: {UserId}",loggedInUserId);

                throw new BadRequestException("Inactive user cannot access profile.");
            }

            _logger.LogInformation("Customer profile retrieved successfully. UserId: {UserId}",loggedInUserId);

            return _mapper.Map<CustomerResponseDto>(customer);
        }

        // Create customer profile
        public async Task<CustomerResponseDto> CreateCustomerAsync(int userId,CustomerRequestDto requestDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning("User not found. UserId: {UserId}", userId);
                throw new NotFoundException($"User with Id {userId} not found.");
            }

            if (user.Role != Role.Customer)
            {
                _logger.LogWarning("User {UserId} is not a customer.", userId);
                throw new BadRequestException("Only users with Customer role can have a customer profile.");
            }

            if (!user.IsActive)
            {
                _logger.LogWarning("Inactive user {UserId} attempted to create profile.", userId);
                throw new BadRequestException("Inactive user cannot create a customer profile.");
            }

            var existingCustomer = await _customerRepository.GetByUserIdAsync(userId);

            if (existingCustomer != null)
            {
                _logger.LogWarning("Customer profile already exists for UserId {UserId}", userId);
                throw new ConflictException("Customer profile already exists.");
            }

            if (requestDto.DateOfBirth > DateOnly.FromDateTime(DateTime.UtcNow))
            {
                _logger.LogWarning("Future date of birth entered for UserId {UserId}", userId);
                throw new BadRequestException("Date of Birth cannot be in the future.");
            }

            // Validate age
            var age = DateTime.Today.Year - requestDto.DateOfBirth.Year;

            if (requestDto.DateOfBirth > DateOnly.FromDateTime(DateTime.Today.AddYears(-age)))
            {
                age--;
            }

            if (age < 18)
            {
                _logger.LogWarning("Customer age validation failed. UserId {UserId}", userId);
                throw new BadRequestException("Customer must be at least 18 years old.");
            }

            if (string.IsNullOrWhiteSpace(requestDto.Address))
                throw new BadRequestException("Address is required.");

            if (string.IsNullOrWhiteSpace(requestDto.City))
                throw new BadRequestException("City is required.");

            if (string.IsNullOrWhiteSpace(requestDto.State))
                throw new BadRequestException("State is required.");

            if (string.IsNullOrWhiteSpace(requestDto.PinCode))
                throw new BadRequestException("Pin Code is required.");

            if (string.IsNullOrWhiteSpace(requestDto.NomineeName))
                throw new BadRequestException("Nominee Name is required.");

            if (string.IsNullOrWhiteSpace(requestDto.NomineeRelation))
                throw new BadRequestException("Nominee Relation is required.");

            var customer = new Customer
            {
                UserId = userId,
                DateOfBirth = requestDto.DateOfBirth,
                Address = requestDto.Address.Trim(),
                City = requestDto.City.Trim(),
                State = requestDto.State.Trim(),
                PinCode = requestDto.PinCode.Trim(),
                NomineeName = requestDto.NomineeName.Trim(),
                NomineeRelation = requestDto.NomineeRelation.Trim(),
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            await _customerRepository.AddAsync(customer);
            await _customerRepository.SaveChangesAsync();

            _logger.LogInformation("Customer profile created successfully for UserId {UserId}.",userId);

            var createdCustomer = await _customerRepository.GetByUserIdAsync(userId);

            if (createdCustomer == null)
                throw new BadRequestException("Customer profile could not be created.");

            return _mapper.Map<CustomerResponseDto>(createdCustomer);
        }

        // Update customer profile
        public async Task<CustomerResponseDto> UpdateCustomerAsync(int id,int loggedInUserId,CustomerRequestDto requestDto)
        {
            var customer = await _customerRepository.GetByIdAsync(id);

            if (customer == null)
            {
                _logger.LogWarning("Customer not found. CustomerId: {CustomerId}", id);
                throw new NotFoundException($"Customer with Id {id} not found.");
            }

            if (customer.UserId != loggedInUserId)
            {
                _logger.LogWarning("Unauthorized update attempt. CustomerId: {CustomerId}, UserId: {UserId}",id,loggedInUserId);

                throw new UnauthorizedAccessException("You can update only your own profile.");
            }

            if (!customer.User.IsActive)
            {
                _logger.LogWarning("Inactive customer attempted update. CustomerId: {CustomerId}",id);

                throw new BadRequestException("Inactive customer cannot be updated.");
            }

            if (requestDto.DateOfBirth > DateOnly.FromDateTime(DateTime.UtcNow))
            {
                throw new BadRequestException("Date of Birth cannot be in the future.");
            }

            // Validate age
            var age = DateTime.Today.Year - requestDto.DateOfBirth.Year;

            if (requestDto.DateOfBirth > DateOnly.FromDateTime(DateTime.Today.AddYears(-age)))
            {
                age--;
            }

            if (age < 18)
            {
                throw new BadRequestException("Customer must be at least 18 years old.");
            }

            if (string.IsNullOrWhiteSpace(requestDto.Address))
                throw new BadRequestException("Address is required.");

            if (string.IsNullOrWhiteSpace(requestDto.City))
                throw new BadRequestException("City is required.");

            if (string.IsNullOrWhiteSpace(requestDto.State))
                throw new BadRequestException("State is required.");

            if (string.IsNullOrWhiteSpace(requestDto.PinCode))
                throw new BadRequestException("Pin Code is required.");

            if (string.IsNullOrWhiteSpace(requestDto.NomineeName))
                throw new BadRequestException("Nominee Name is required.");

            if (string.IsNullOrWhiteSpace(requestDto.NomineeRelation))
                throw new BadRequestException("Nominee Relation is required.");

            if (requestDto.ProfileImage != null)
            {
                var imageUrl = await _cloudinaryService.UploadImageAsync(requestDto.ProfileImage);

                customer.User.ProfileImageUrl = imageUrl;
            }

            customer.DateOfBirth = requestDto.DateOfBirth;
            customer.Address = requestDto.Address.Trim();
            customer.City = requestDto.City.Trim();
            customer.State = requestDto.State.Trim();
            customer.PinCode = requestDto.PinCode.Trim();
            customer.NomineeName = requestDto.NomineeName.Trim();
            customer.NomineeRelation = requestDto.NomineeRelation.Trim();
            customer.UpdatedDate = DateTime.UtcNow;

            await _customerRepository.UpdateAsync(customer);
            await _customerRepository.SaveChangesAsync();

            _logger.LogInformation("Customer profile updated successfully. CustomerId: {CustomerId}",id);

            var updatedCustomer = await _customerRepository.GetByIdAsync(id);

            if (updatedCustomer == null)
            {
                throw new BadRequestException("Customer profile could not be updated.");
            }
            return _mapper.Map<CustomerResponseDto>(updatedCustomer);
        }
    }
}