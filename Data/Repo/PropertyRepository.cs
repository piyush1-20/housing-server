using API.Interfaces;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data.Repo
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly DataContext dc;

        public PropertyRepository(DataContext dc)
        {
            this.dc = dc;
        }
        public void AddProperty(Property property)
        {
            throw new NotImplementedException();
        }

        public void DeleteProperty(int Id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Property>> GetPropertiesAsync(int sellRent)
        {
            var properties = await dc.Properties                
                .Include(p=>p.PropertyType)
                .Include(p=>p.City)
                .Include(p=>p.FurnishingType)
                .Where(p => p.SellRent == sellRent)
                .ToListAsync();
            return properties;
        }

        public  async Task<Property> GetPropertyDetailAsync(int id)
        {
            var properties = await dc.Properties
                .Include(p => p.PropertyType)
                .Include(p => p.City)
                .Include(p => p.FurnishingType)
                .Where(p=>p.Id == id)
                .FirstAsync();
            return properties;
        }
    }
}
