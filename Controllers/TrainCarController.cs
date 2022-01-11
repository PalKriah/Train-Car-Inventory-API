using System;
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
    public class TrainCarController : ControllerBase
    {
        private readonly ITrainCarService _trainCarService;

        public TrainCarController(ITrainCarService trainCarService)
        {
            _trainCarService = trainCarService;
        }

        [AllowAnonymous]
        [HttpGet("{includeDeleted:bool}")]
        public ActionResult<IEnumerable<TrainCar>> GetAll(bool includeDeleted)
        {
            return Ok(_trainCarService.GetAll(includeDeleted));
        }

        [HttpGet("{middleNumber}")]
        public ActionResult<TrainCar> GetByMiddleNumber(string middleNumber)
        {
            return Ok(_trainCarService.GetByMiddleNumber(middleNumber));
        }

        [HttpGet("{serialNumber}")]
        public ActionResult<IEnumerable<TrainCar>> GetBySerialNumber(string serialNumber)
        {
            return Ok(_trainCarService.GetBySerialNumber(serialNumber));
        }

        [HttpGet("{locationName}")]
        public ActionResult<IEnumerable<TrainCar>> GetByLocation(string locationName)
        {
            return Ok(_trainCarService.GetByLocation(locationName));
        }
        
        [HttpGet("{manufacturingYear:int}")]
        public ActionResult<IEnumerable<TrainCar>> GetByManufacturingYear(int manufacturingYear)
        {
            return Ok(_trainCarService.GetByManufacturingYear(manufacturingYear));
        }
        
        [HttpGet]
        public ActionResult<IEnumerable<TrainCar>> GetStatistics()
        {
            return Ok(_trainCarService.GetStatistics());
        }
        
        [HttpGet]
        [Authorize(Roles = "User")]
        [Authorize(Policy = "IsRailwayWorker")]
        public ActionResult<IEnumerable<TrainCar>> GetSecondClassCars()
        {
            return Ok(_trainCarService.GetSecondClassCars());
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public IActionResult Create([FromBody] TrainCar newTrainCar)
        {
            var trainCar = _trainCarService.Create(newTrainCar);
            return Created($"{trainCar.Id}", trainCar);
        }

        [HttpPut("{trainCarId:int}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Update(int trainCarId, [FromBody] TrainCar updatedTrainCar)
        {
            return Ok(_trainCarService.Update(trainCarId, updatedTrainCar));
        }
        
        [HttpPut("{trainCarId:int}/{locationId:int}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult AssignTrainCarToLocation(int trainCarId, int locationId)
        {
            return Ok(_trainCarService.UpdateLocation(trainCarId, locationId));
        }

        [HttpDelete("{trainCarId:int}/{scrapDate:datetime?}")]
        [Authorize(Roles = "Administrator")]
        public IActionResult Delete(int trainCarId, DateTime? scrapDate = null)
        {
            return Ok(_trainCarService.Delete(trainCarId, scrapDate));
        }
    }
}