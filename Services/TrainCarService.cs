using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Train_Car_Inventory_App.Models;
using Train_Car_Inventory_App.Models.Payloads;
using Train_Car_Inventory_App.UnitOfWork;

namespace Train_Car_Inventory_App.Services
{
    public interface ITrainCarService
    {
        IEnumerable<TrainCar> GetAll(bool includeDeleted);

        TrainCar Get(int id);
        TrainCar GetByMiddleNumber(string midNum);
        IEnumerable<TrainCar> GetBySerialNumber(string serialNum);
        IEnumerable<TrainCar> GetByLocation(string locationName);
        IEnumerable<TrainCar> GetByManufacturingYear(int manufacturingYear);
        IEnumerable<TrainCarStatisticsBySerialNumber> GetStatistics();
        IEnumerable<TrainCar> GetSecondClassCars();


        TrainCar Create(TrainCar newTrainCar);
        TrainCar Update(int trainCarId, TrainCar updatedTrainCar);
        TrainCar UpdateLocation(int trainCarId, int locationId);
        IEnumerable<TrainCar> Delete(int trainCarId, DateTime? scrapDate);
    }

    public class TrainCarService : AbstractService, ITrainCarService
    {
        public TrainCarService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IEnumerable<TrainCar> GetAll(bool includeDeleted)
        {
            var query = UnitOfWork.GetRepository<TrainCar>().GetAll();
            return includeDeleted ? query.IgnoreQueryFilters() : query;
        }

        public TrainCar Get(int id)
        {
            return UnitOfWork.GetRepository<TrainCar>()
                .GetByIdWithInclude(id, src => src.Include(trainCar => trainCar.Location));
        }

        public TrainCar GetByMiddleNumber(string midNum)
        {
            return UnitOfWork.GetRepository<TrainCar>().GetAsQueryable()
                .Where(trainCar => trainCar.TrackNumber.Substring(6, 5).Equals(midNum))
                .Include(trainCar => trainCar.Location)
                .First();
        }

        public IEnumerable<TrainCar> GetBySerialNumber(string serialNum)
        {
            return UnitOfWork.GetRepository<TrainCar>().GetAsQueryable()
                .Where(trainCar => trainCar.SerialNumber.Equals(serialNum))
                .Include(trainCar => trainCar.Location)
                .ToList();
        }

        public IEnumerable<TrainCar> GetByLocation(string locationName)
        {
            return UnitOfWork.GetRepository<TrainCar>().GetAsQueryable()
                .Where(trainCar => trainCar.Location.Name.Equals(locationName));
        }

        public IEnumerable<TrainCar> GetByManufacturingYear(int manufacturingYear)
        {
            return UnitOfWork.GetRepository<TrainCar>().GetAsQueryable()
                .Where(trainCar => trainCar.ManufacturingYear.Equals(manufacturingYear));
        }

        public IEnumerable<TrainCarStatisticsBySerialNumber> GetStatistics()
        {
            var serialNumbers = UnitOfWork.GetRepository<TrainCar>().GetAsQueryable()
                .IgnoreQueryFilters()
                .GroupBy(t => t.SerialNumber)
                .Select(g => g.Key).ToList();

            return (from serialNumber in serialNumbers
                let manufacturingYears = UnitOfWork.GetRepository<TrainCar>()
                    .GetAsQueryable()
                    .Where(t => t.SerialNumber.Equals(serialNumber))
                    .GroupBy(t => t.ManufacturingYear)
                    .Select(g => new YearCount() {Year = g.Key, Count = g.Count()})
                    .OrderBy(t => t.Year)
                    .ToList()
                let scrapYears = UnitOfWork.GetRepository<TrainCar>()
                    .GetAsQueryable()
                    .IgnoreQueryFilters()
                    .Where(t => t.SerialNumber.Equals(serialNumber) && t.IsDeleted)
                    .GroupBy(t => t.ScrapDate)
                    .Select(g => new YearCount() {Year = g.Key.Year, Count = g.Count()})
                    .OrderBy(t => t.Year)
                    .ToList()
                select GenerateStat(serialNumber, manufacturingYears, scrapYears)).ToList();
        }

        public IEnumerable<TrainCar> GetSecondClassCars()
        {
            return UnitOfWork.GetRepository<TrainCar>().GetAsQueryable()
                .Where(trainCar => trainCar.SerialNumber.Substring(0, 1).Equals("B") &&
                                   trainCar.TrackNumber.Substring(6, 1).Equals("2"));
        }

        public TrainCar Create(TrainCar newTrainCar)
        {
            newTrainCar.Id = 0;
            UnitOfWork.GetRepository<TrainCar>().Create(newTrainCar);
            UnitOfWork.SaveChanges();
            return newTrainCar;
        }

        public TrainCar Update(int trainCarId, TrainCar updatedTrainCar)
        {
            var trainCar = Get(trainCarId);
            trainCar.SerialNumber = updatedTrainCar.SerialNumber;
            trainCar.ManufacturingYear = updatedTrainCar.ManufacturingYear;
            trainCar.TrackNumber = updatedTrainCar.TrackNumber;
            trainCar.Owner = updatedTrainCar.Owner;
            trainCar.LocationId = updatedTrainCar.LocationId;

            UnitOfWork.GetRepository<TrainCar>().Update(trainCarId, trainCar);
            UnitOfWork.SaveChanges();
            return trainCar;
        }

        public TrainCar UpdateLocation(int trainCarId, int locationId)
        {
            var trainCar = Get(trainCarId);
            var location = UnitOfWork.GetRepository<Location>().GetById(locationId).Result;
            trainCar.Location = location;
            UnitOfWork.GetRepository<TrainCar>().Update(trainCarId, trainCar);
            UnitOfWork.SaveChanges();
            return trainCar;
        }

        public IEnumerable<TrainCar> Delete(int trainCarId, DateTime? scrapDate)
        {
            var trainCar = Get(trainCarId);
            trainCar.IsDeleted = true;
            trainCar.ScrapDate = scrapDate ?? DateTime.Now;
            UnitOfWork.GetRepository<TrainCar>().Update(trainCarId, trainCar);
            UnitOfWork.SaveChanges();
            return GetAll(false);
        }

        private static TrainCarStatisticsBySerialNumber GenerateStat(string serialNumber,
            List<YearCount> manufacturingYears,
            List<YearCount> scrapYears)
        {
            var serialStat = new TrainCarStatisticsBySerialNumber
            {
                SerialNumber = serialNumber
            };
            int i = 0, j = 0;

            while (i < manufacturingYears.Count() && j < scrapYears.Count())
            {
                if (manufacturingYears[i].Year == scrapYears[j].Year)
                {
                    serialStat.YearData.Add(new YearData()
                    {
                        Year = manufacturingYears[i].Year,
                        Manufactured = manufacturingYears[i].Count,
                        Scrapped = scrapYears[j].Count
                    });
                    i++;
                    j++;
                }
                else if (manufacturingYears[i].Year < scrapYears[j].Year)
                {
                    serialStat.YearData.Add(new YearData()
                    {
                        Year = manufacturingYears[i].Year,
                        Manufactured = manufacturingYears[i].Count
                    });
                    i++;
                }
                else
                {
                    serialStat.YearData.Add(new YearData()
                    {
                        Year = scrapYears[j].Year,
                        Scrapped = scrapYears[j].Count
                    });
                    j++;
                }
            }

            while (i < manufacturingYears.Count())
            {
                serialStat.YearData.Add(new YearData()
                {
                    Year = manufacturingYears[i].Year,
                    Manufactured = manufacturingYears[i].Count
                });
                i++;
            }

            while (j < scrapYears.Count())
            {
                serialStat.YearData.Add(new YearData()
                {
                    Year = scrapYears[j].Year,
                    Scrapped = scrapYears[j].Count
                });
                j++;
            }

            return serialStat;
        }

        private class YearCount
        {
            public int Year { get; set; }
            public int Count { get; set; }
        }
    }
}