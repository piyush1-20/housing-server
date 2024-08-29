using API.Dtos;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class PropertyController:BaseController
    {
        private readonly IUnitofWork uow;
        private readonly IMapper mapper;

        public PropertyController(IUnitofWork uow,IMapper mapper)
        {
            this.uow = uow;
            this.mapper = mapper;
        }

        [HttpGet("list/{sellRent}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPropertyList(int sellRent)
        {
            var properties =  await uow.PropertyRepository.GetPropertiesAsync(sellRent);
            var propertydto = mapper.Map<IEnumerable<PropertyListDto>>(properties);
            return Ok(propertydto);
        }

        [HttpGet("detail/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPropertyDetail(int id)
        {
            var property = await uow.PropertyRepository.GetPropertyDetailAsync(id);
            var propertydetaildto = mapper.Map<PropertyDetailDto>(property);
            return Ok(propertydetaildto);
        }
    }
}
