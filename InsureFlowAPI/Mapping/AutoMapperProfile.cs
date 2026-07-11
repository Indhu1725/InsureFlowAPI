using AutoMapper;
using InsureFlowAPI.DTOs.Claim;
using InsureFlowAPI.DTOs.Customer;
using InsureFlowAPI.DTOs.Payment;
using InsureFlowAPI.DTOs.Policy;
using InsureFlowAPI.DTOs.PolicyPlan;
using InsureFlowAPI.DTOs.Product;
using InsureFlowAPI.DTOs.User;
using InsureFlowAPI.Models;

namespace InsureFlowAPI.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        { 
            // User
            CreateMap<User, UserResponseDto>()
                 .ForMember(d => d.Role,o => o.MapFrom(s => s.Role.ToString()));

            // Customer
            CreateMap<Customer, CustomerResponseDto>()
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.Email,
                    opt => opt.MapFrom(src => src.User.Email))
                .ForMember(dest => dest.MobileNumber,
                    opt => opt.MapFrom(src => src.User.MobileNumber));

            // Insurance Product
            CreateMap<InsuranceProduct, ProductResponseDto>();

            // Policy Plan
            CreateMap<PolicyPlan, PolicyPlanResponseDto>()
                .ForMember(dest => dest.ProductName,
                    opt => opt.MapFrom(src => src.Product.ProductName));

            // Policy
            // Policy
            CreateMap<Policy, PolicyResponseDto>()
                .ForMember(dest => dest.CustomerName,
                    opt => opt.MapFrom(src => src.Customer.User.FullName))
                .ForMember(dest => dest.PlanName,
                    opt => opt.MapFrom(src => src.Plan.PlanName))
                .ForMember(dest => dest.ProductType,
                    opt => opt.MapFrom(src => src.Plan.Product.ProductType))
                .ForMember(dest => dest.CoverageAmount,
                    opt => opt.MapFrom(src => src.Plan.CoverageAmount))
                .ForMember(dest => dest.PremiumAmount,
                    opt => opt.MapFrom(src => src.Plan.PremiumAmount))
                .ForMember(dest => dest.PremiumType,
                    opt => opt.MapFrom(src => src.Plan.PremiumType));

            // Premium Payment
            CreateMap<PremiumPayment, PremiumPaymentResponseDto>()
                .ForMember(dest => dest.PolicyNumber,
                    opt => opt.MapFrom(src => src.Policy.PolicyNumber));

            // Claim
            CreateMap<Claim, ClaimResponseDto>()
                .ForMember(dest => dest.PolicyNumber,
                    opt => opt.MapFrom(src => src.Policy.PolicyNumber))
                .ForMember(dest => dest.CustomerName,
                    opt => opt.MapFrom(src => src.Customer.User.FullName))
                .ForMember(dest => dest.InternalStaffRemarks,
                    opt => opt.MapFrom(src => src.InternalStaffRemarks));

            // Claim Document
            CreateMap<ClaimDocument, ClaimDocumentResponseDto>()
                .ForMember(dest => dest.FilePath,
                    opt => opt.MapFrom(src => src.DocumentReference));

            // Claim Status History
            CreateMap<ClaimStatusHistory, ClaimStatusHistoryResponseDto>()
                .ForMember(dest => dest.OldStatus,
                    opt => opt.MapFrom(src => src.PreviousStatus))
                .ForMember(dest => dest.ChangedDate,
                    opt => opt.MapFrom(src => src.UpdatedDate))
                .ForMember(dest => dest.ChangedBy,
                    opt => opt.MapFrom(src => src.User.FullName));
        }
    }
}