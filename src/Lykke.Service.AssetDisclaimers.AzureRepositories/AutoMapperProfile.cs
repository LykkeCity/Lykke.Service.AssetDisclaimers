using AutoMapper;
using Lykke.Service.AssetDisclaimers.Core.Domain;

namespace Lykke.Service.AssetDisclaimers.AzureRepositories
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ILykkeEntity, LykkeEntityEntity>(MemberList.Source)
                .ForSourceMember(src => src.Id, opt => opt.Ignore());
            CreateMap<LykkeEntityEntity, LykkeEntity>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RowKey));

            CreateMap<IClientDisclaimer, ClientDisclaimerEntity>(MemberList.Source);
            CreateMap<ClientDisclaimerEntity, ClientDisclaimer>(MemberList.Destination);
            
            CreateMap<IDisclaimer, DisclaimerEntity>(MemberList.Source)
                .ForSourceMember(src => src.Id, opt => opt.Ignore());
            CreateMap<DisclaimerEntity, Disclaimer>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.RowKey));
        }
    }
}
