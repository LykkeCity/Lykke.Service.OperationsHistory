using AutoMapper;
using Lykke.Service.OperationsHistory.AutorestClient.Models;
using Lykke.Service.OperationsHistory.Core.Entities;

namespace Lykke.Service.OperationsHistory.Tests
{
    public class TestMappingProfile : Profile
    {
        public TestMappingProfile()
        {
            CreateMap<IHistoryLogEntryEntity, HistoryEntryResponse>();
        }
    }
}