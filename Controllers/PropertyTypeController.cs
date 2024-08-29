using API.Dtos;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class PropertyTypeController:BaseController
    {
        private readonly IUnitofWork uow;
        private readonly IMapper mapper;

        public PropertyTypeController(IUnitofWork uow,IMapper mapper)
        {
            this.uow = uow;
            this.mapper = mapper;
        }

        [HttpGet("list")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPropertyTypes()
        {
            var Propertytypes = await uow.PropertyTypeRepository.GetPropertyTypesAsync();
            var propertyTypesDto = mapper.Map<IEnumerable<KeyValuePairDto>>(Propertytypes);
            return Ok(propertyTypesDto);
        }

    }
}
