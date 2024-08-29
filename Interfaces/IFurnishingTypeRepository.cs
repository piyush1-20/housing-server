using API.Models;

namespace API.Interfaces
{
    public interface IFurnishingTypeRepository
    {
        Task<IEnumerable<FurnishingType>>GetFurnishingTypesAsync();
    }
}
