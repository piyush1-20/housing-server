using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUnitofWork
    {
        ICityRepository CityRepository { get; }

        IUserRepository UserRepository { get; }

        IPropertyRepository PropertyRepository { get; }

        IPropertyTypeRepository PropertyTypeRepository { get; } 
        IFurnishingTypeRepository FurnishingTypeRepository { get; }
        Task<bool> SaveAsync();
    }
}
