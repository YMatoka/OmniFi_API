using AutoMapper;
using Microsoft.VisualBasic;
using OmniFi_DTOs.Dtos.Assets;
using OmniFi_DTOs.Dtos.Banks;
using OmniFi_DTOs.Dtos.Cryptos;
using OmniFi_DTOs.Dtos.Identity;
using OmniFi_API.Mapping.Converters;
using OmniFi_API.Models.Assets;
using OmniFi_API.Models.Banks;
using OmniFi_API.Models.Cryptos;
using OmniFi_API.Models.Currencies;
using OmniFi_API.Models.Identity;

namespace OmniFi_API.Mapping
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<ApplicationUser, UserDTO>().ReverseMap();
            CreateMap<Bank, BankDTO>();
            CreateMap<CryptoExchange, CryptoExchangeDTO>();
            CreateMap<CryptoHolding, CryptoHoldingDTO>();
            CreateMap<CryptoHoldingHistory, CryptoHoldingHistoryDTO>();

            CreateMap<FinancialAsset, FinancialAssetDTO>()
                .ForMember(dest => dest.UserName, conf => conf.ConvertUsing(new UserConverter(), src => src.User))
                .ForMember(dest => dest.AssetPlatformName, conf => conf.ConvertUsing(new AssetPlatformConverter(), src => src.AssetPlatform))
                .ForMember(dest => dest.AssetSourceName, conf => conf.ConvertUsing(new AssetSourceConverter(), src => src.AssetSource))
                .ForMember(dest => dest.FiatCurrencyCode, conf => conf.ConvertUsing(new FiatCurrencyConverter(), src => src.FiatCurrency));

            CreateMap<FinancialAssetHistory, FinancialAssetHistoryDTO>()
                .ForMember(dest => dest.UserName, conf => conf.ConvertUsing(new UserConverter(), src => src.User))
                .ForMember(dest => dest.AssetPlatformName, conf => conf.ConvertUsing(new AssetPlatformConverter(), src => src.AssetPlatform))
                .ForMember(dest => dest.AssetSourceName, conf => conf.ConvertUsing(new AssetSourceConverter(), src => src.AssetSource))
                .ForMember(dest => dest.FiatCurrencyCode, conf => conf.ConvertUsing(new FiatCurrencyConverter(), src => src.FiatCurrency));
        }
    }

}
