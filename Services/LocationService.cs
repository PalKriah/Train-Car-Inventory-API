using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Train_Car_Inventory_App.Models;
using Train_Car_Inventory_App.Models.Payloads;
using Train_Car_Inventory_App.UnitOfWork;

namespace Train_Car_Inventory_App.Services
{
    public interface ILocationService
    {
        IEnumerable<Location> GetAll();

        Location Get(int id);
        LocationData GetByCodeNumber(int codeNumber);

        Location Create(Location newLocation);
        Location Update(int locationId, Location updatedLocation);
        IEnumerable<Location> Delete(int locationId);
    }

    public class LocationService : AbstractService, ILocationService
    {
        private readonly IMemoryCache _memoryCache;

        public LocationService(IUnitOfWork unitOfWork, IMemoryCache memoryCache) : base(unitOfWork)
        {
            _memoryCache = memoryCache;
        }

        public IEnumerable<Location> GetAll()
        {
            return _memoryCache.TryGetValue("locations", out IEnumerable<Location> locations)
                ? locations
                : RefreshLocations();
        }

        public Location Get(int id)
        {
            return GetAll().FirstOrDefault(location => location.Id == id);
        }

        public LocationData GetByCodeNumber(int codeNumber)
        {
            var location = GetAll().FirstOrDefault(location => location.CodeNumber == codeNumber);

            var data = UnitOfWork.GetRepository<TrainCar>()
                .GetAsQueryable()
                .IgnoreQueryFilters()
                .Where(t => t.LocationId == location.Id)
                .GroupBy(t => t.SerialNumber)
                .Select(g => new TrainCarSerialData()
                {
                    SerialNumber = g.Key
                })
                .ToList();

            var trains = UnitOfWork.GetRepository<TrainCar>()
                .GetAsQueryable()
                .IgnoreQueryFilters()
                .Where(t => t.LocationId == location.Id)
                .ToList();

            foreach (var entry in data)
            {
                entry.AverageAge = trains.Where(t => t.SerialNumber == entry.SerialNumber && !t.IsDeleted)
                    .Average(t => DateTime.Now.Year - t.ManufacturingYear);
                entry.TotalNumber = trains.Count(t => t.SerialNumber == entry.SerialNumber && !t.IsDeleted);
                entry.ScrapedCount = trains.Count(t => t.SerialNumber == entry.SerialNumber && t.IsDeleted);
            }

            return new LocationData()
            {
                Name = location.Name,
                Owner = location.Owner,
                TrainCarSerialData = data
            };
        }

        public Location Create(Location newLocation)
        {
            newLocation.Id = 0;
            UnitOfWork.GetRepository<Location>().Create(newLocation);
            UnitOfWork.SaveChanges();
            RefreshLocations();
            return newLocation;
        }

        public Location Update(int locationId, Location updatedLocation)
        {
            var location = Get(locationId);
            location.Name = updatedLocation.Name;
            location.Owner = updatedLocation.Owner;
            location.CodeNumber = updatedLocation.CodeNumber;
            location.PostalAddress = updatedLocation.PostalAddress;

            UnitOfWork.GetRepository<Location>().Update(locationId, location);
            UnitOfWork.SaveChanges();
            RefreshLocations();
            return updatedLocation;
        }

        public IEnumerable<Location> Delete(int locationId)
        {
            var location = Get(locationId);
            location.IsDeleted = true;
            UnitOfWork.GetRepository<Location>().Update(locationId, location);
            UnitOfWork.SaveChanges();
            RefreshLocations();
            return GetAll();
        }

        private IEnumerable<Location> RefreshLocations()
        {
            var locations = UnitOfWork.GetRepository<Location>().GetAll().ToList();
            _memoryCache.Set("locations", locations);
            return locations;
        }
    }
}