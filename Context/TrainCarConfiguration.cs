using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Train_Car_Inventory_App.Models;

namespace Train_Car_Inventory_App.Context
{
    public class TrainCarConfiguration : IEntityTypeConfiguration<TrainCar>
    {
        public void Configure(EntityTypeBuilder<TrainCar> builder)
        {
            builder.Property(t => t.ScrapDate).HasDefaultValue(DateTime.MaxValue);
            builder.HasQueryFilter(t => !t.IsDeleted);

            builder.HasData(
                new TrainCar
                {
                    Id = 1, SerialNumber = "Bhv", ManufacturingYear = 1988, TrackNumber = "51-55 39-30 01-67",
                    Owner = "MÁV",
                    LocationId = 1
                },
                new TrainCar
                {
                    Id = 2, SerialNumber = "BDbhv", ManufacturingYear = 2000, TrackNumber = "50-55 20-05 55-01",
                    Owner = "MÁV",
                    LocationId = 1
                },
                new TrainCar
                {
                    Id = 3, SerialNumber = "AcBc", ManufacturingYear = 2000, TrackNumber = "01-94 87-64 14-66",
                    Owner = "MMV",
                    LocationId = 2
                },
                new TrainCar
                {
                    Id = 4, SerialNumber = "Bhv", ManufacturingYear = 1997, TrackNumber = "91-44 15-49 16-83",
                    Owner = "MÁV",
                    LocationId = 3
                },
                new TrainCar
                {
                    Id = 5, SerialNumber = "BrtE", ManufacturingYear = 2005, TrackNumber = "26-97 27-64 54-20",
                    Owner = "MMV",
                    LocationId = 1
                },
                new TrainCar
                {
                    Id = 6, SerialNumber = "KRtg", ManufacturingYear = 2017, TrackNumber = "49-18 54-47 15-73",
                    Owner = "MÁV",
                    LocationId = 1
                }
            );
        }
    }
}