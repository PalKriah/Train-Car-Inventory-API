using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Train_Car_Inventory_App.Models;
using Train_Car_Inventory_App.Services;

namespace Train_Car_Inventory_App.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Location>> GetAll()
        {
            return Ok(_locationService.GetAll());
        }
        
        [HttpGet("{codeNumber:int}")]
        public ActionResult<IEnumerable<Location>> GetByCodeNumber(int codeNumber)
        {
            return Ok(_locationService.GetByCodeNumber(codeNumber));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public IActionResult Create([FromBody] Location newLocation)
        {
            var location = _locationService.Create(newLocation);
            return Created($"{location.Id}", location);
        }

        [HttpPut("{locationId:int}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Update(int locationId, [FromBody] Location updatedLocation)
        {
            return Ok(_locationService.Update(locationId, updatedLocation));
        }

        [HttpDelete("{locationId:int}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Delete(int locationId)
        {
            return Ok(_locationService.Delete(locationId));
        }
    }
}