using AutoMapper;
using Lykke.Service.OperationsHistory.Core.Entities;
using Lykke.Service.OperationsHistory.Models;

namespace Lykke.Service.OperationsHistory.Mappers
{
    public class HistoryLogMapperProfile : Profile
    {
        public HistoryLogMapperProfile()
        {
            CreateMap<IHistoryLogEntryEntity, HistoryEntryWalletResponse>();
            CreateMap<IHistoryLogEntryEntity, HistoryEntryClientResponse>()
                .ForMember(x => x.WalletId, o => o.MapFrom(x => x.ClientId));
        }
    }
}