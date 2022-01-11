using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Train_Car_Inventory_App.Models;

namespace Train_Car_Inventory_App.Context
{
    public class LocationConfiguration: IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.HasData(
                new Location()
                {
                    Id = 1, Name = "Celldömölk", Owner = "MÁV", PostalAddress = "Celldömölk, Arany János út 19.",
                    CodeNumber = 02170
                },
                new Location()
                {
                    Id = 2, Name = "Széksefehérvár", Owner = "MÁV",
                    PostalAddress = "Székesfehérvár, Arany János út 19.",
                    CodeNumber = 19735
                },
                new Location()
                {
                    Id = 3, Name = "Budapest", Owner = "MÁV", PostalAddress = "Budapest, Arany János út 19.",
                    CodeNumber = 34972
                }
            );
        }
    }
}