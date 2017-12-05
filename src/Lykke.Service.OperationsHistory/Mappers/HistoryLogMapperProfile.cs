using AutoMapper;
using Lykke.Service.OperationsHistory.Core.Entities;
using Lykke.Service.OperationsHistory.Models;

namespace Lykke.Service.OperationsHistory.Mappers
{
    public class HistoryLogMapperProfile : Profile
    {
        public HistoryLogMapperProfile()
        {
            CreateMap<IHistoryLogEntryEntity, HistoryEntryResponse>()
                .ForMember(dest => dest.Currency, o => o.MapFrom(src => src.AssetId));
        }
    }
}