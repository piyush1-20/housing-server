using API.Data;
using API.Dtos;
using API.Interfaces;
using API.Models;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
//using System.Web.Mvc;
namespace API.Controllers
{
    [Authorize]
    public class CityController : BaseController
    {
        private readonly IUnitofWork uow;

        private readonly IMapper mapper;

        public CityController(IUnitofWork uow,IMapper mapper)
        {
            this.uow = uow;
            this.mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCities()
        {
            //throw new UnauthorizedAccessException();
            var cities = await uow.CityRepository.GetCitiesAsync();
            var citiesdto = mapper.Map<IEnumerable<CityDto>>(cities);
            //var citiesdto = from c in cities
            //                select new CityDto()
            //                  {
            //                    Id = c.Id,
            //                    Name = c.Name
            //                };
            return Ok(citiesdto);
        }


        ///Post api/city/add?cityName=Miami

        //[HttpPost("add")]
        //[HttpPost("add/{cityName}")]
        //public async Task<IActionResult> AddCity(string cityName)
        //{
        //    City city = new()
        //    {
        //        Name = cityName
        //    };
        //    await dc.Cities.AddAsync(city);
        //    await dc.SaveChangesAsync();
        //    return Ok(city);
        //}

        ///Post api/city/post ---- Post data in JSON format ,  Ids are not posted they give error 500 Internal server error
        [HttpPost("post")]
        public async Task<IActionResult> AddCity(CityDto citydto)
        {   
            var city = mapper.Map<City>(citydto);
            city.LastUpdatedOn = DateTime.Now;
            city.LastUpdatedBy = 1;
            uow.CityRepository.AddCity(city);
            await uow.SaveAsync();
            return StatusCode(201);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCity(int id,CityDto citydto)
        {
            var cityFromDb = await uow.CityRepository.FindCity(id);
            cityFromDb.LastUpdatedBy = 1;
            cityFromDb.LastUpdatedOn = DateTime.Now;
            mapper.Map(citydto, cityFromDb);
            await uow.SaveAsync();
            return StatusCode(200);
        }


        [HttpPatch("update/{id}")]
        public async Task<IActionResult> UpdateCityPatch(int id,JsonPatchDocument<City> cityToPatch)
        {
            var cityFromDb = await uow.CityRepository.FindCity(id);
            cityFromDb.LastUpdatedBy = 1;
            cityFromDb.LastUpdatedOn = DateTime.Now;
            cityToPatch.ApplyTo(cityFromDb, ModelState);
            await uow.SaveAsync();
            return StatusCode(200);
        }


        //deleting entries from database
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            uow.CityRepository.DeleteCity(id);
            await uow.SaveAsync();
            return Ok(id);
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "Atlanta";
        }
    }
}
