using GajinoAgencies.Models;
using AutoMapper;
using GajinoAgencies.Dtos;

namespace GajinoAgencies.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<AddAgentRequestDto, Agency>();
        CreateMap<Agency, UserAuthorityResponseDto>();
        //.ForMember(dest => dest.Password, opt => opt.MapFrom<PasswordHashingResolver>());

        CreateMap<AddNewVoucherDto, Voucher>()
            .ForMember(dest => dest.PackageDetailIds, opt =>
                    opt.MapFrom(src => string.Join(",", src.PackageDetailIds))
            );

        CreateMap<AddLocationRequestDto, Location>();
        CreateMap<Location, LocationResponseDto>();
        CreateMap<AddPaymentDocumentDto, Payment>();

        CreateMap<PaymentDocumentFromExcelDto, Payment>()
            .ForMember(dest => dest.PaymentDate, opt =>
                opt.MapFrom(src => DateTime.Parse(src.PaymentDate))
            )
            .ForMember(dest => dest.Deposit, opt =>
                opt.MapFrom(src => decimal.Parse(src.Deposit))
            )
            .ForMember(dest => dest.AgencyId, opt =>
                opt.MapFrom(src => int.Parse(src.AgencyId))
            )
            ;

        CreateMap<Payment, PaymentDocumentsResponseDto>();
    }
}