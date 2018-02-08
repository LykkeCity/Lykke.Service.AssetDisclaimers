using AutoMapper;
using Lykke.Service.AssetDisclaimers.Core.Domain;
using Lykke.Service.AssetDisclaimers.Models.ClientDisclaimers;
using Lykke.Service.AssetDisclaimers.Models.Disclaimers;
using Lykke.Service.AssetDisclaimers.Models.LykkeEntities;

namespace Lykke.Service.AssetDisclaimers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ILykkeEntity, LykkeEntityModel>(MemberList.Source);
            CreateMap<CreateLykkeEntityModel, LykkeEntity>(MemberList.Destination);
            CreateMap<EditLykkeEntityModel, LykkeEntity>(MemberList.Destination);
            
            CreateMap<IDisclaimer, DisclaimerModel>(MemberList.Source);
            CreateMap<CreateDisclaimerModel, Disclaimer>(MemberList.Destination)
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<EditDisclaimerModel, Disclaimer>(MemberList.Destination);

            CreateMap<IDisclaimer, ClientDisclaimerModel>(MemberList.Source);
        }
    }
}
