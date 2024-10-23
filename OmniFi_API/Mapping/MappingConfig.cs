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
using OmniFi_API.Models.Api.Banks;
using OmniFi_API.DTOs.GoCardless;

namespace OmniFi_API.Mapping
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<ApplicationUser, UserDTO>().ReverseMap();
            CreateMap<Bank, BankDTO>();
            CreateMap<CryptoExchange, CryptoExchangeDTO>();

            CreateMap<RequisitionResponse, BankRequisition>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.RedirectUrl, opt => opt.MapFrom(src => src.redirect))
                .ForMember(dest => dest.Link, opt => opt.MapFrom(src => src.link));

            CreateMap<CryptoHolding, CryptoHoldingDTO>()
                .ForMember(dest => dest.CryptoCurrencySymbol, opt => opt.ConvertUsing(new CryptoCurrencyConverter(), src => src.CryptoCurrency));

            CreateMap<CryptoHoldingHistory, CryptoHoldingHistoryDTO>()
                .ForMember(dest => dest.CryptoCurrencySymbol, opt => opt.ConvertUsing(new CryptoCurrencyConverter(), src => src.CryptoCurrency));

            CreateMap<FinancialAsset, FinancialAssetDTO>()
                .ForMember(dest => dest.UserName, opt => opt.ConvertUsing(new UserConverter(), src => src.User))
                .ForMember(dest => dest.AssetPlatformName, opt => opt.ConvertUsing(new AssetPlatformConverter(), src => src.AssetPlatform))
                .ForMember(dest => dest.AssetSourceName, opt => opt.ConvertUsing(new AssetSourceConverter(), src => src.AssetSource))
                .ForMember(dest => dest.FiatCurrencyCode, opt => opt.ConvertUsing(new FiatCurrencyConverter(), src => src.FiatCurrency));

            CreateMap<FinancialAssetHistory, FinancialAssetHistoryDTO>()
                .ForMember(dest => dest.UserName, opt => opt.ConvertUsing(new UserConverter(), src => src.User))
                .ForMember(dest => dest.AssetPlatformName, opt => opt.ConvertUsing(new AssetPlatformConverter(), src => src.AssetPlatform))
                .ForMember(dest => dest.AssetSourceName, opt => opt.ConvertUsing(new AssetSourceConverter(), src => src.AssetSource))
                .ForMember(dest => dest.FiatCurrencyCode, opt => opt.ConvertUsing(new FiatCurrencyConverter(), src => src.FiatCurrency));
        }
    }

}
