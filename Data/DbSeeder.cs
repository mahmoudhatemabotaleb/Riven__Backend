using Microsoft.EntityFrameworkCore;
using RivenBackend.Models;

namespace RivenBackend.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (!await context.Roles.AnyAsync())
            {
                context.Roles.AddRange(
                    new Role { RoleName = "Admin" },
                    new Role { RoleName = "Doctor" },
                    new Role { RoleName = "Paramedic" });
                await context.SaveChangesAsync();
            }

            if (!await context.Hospitals.AnyAsync())
            {
                context.Hospitals.Add(new Hospital
                {
                    Name = "Riven Stroke Center",
                    Address = "123 Medical Drive",
                    CityStateZip = "Cairo, Egypt",
                    ContactNumber = "+201000000000",
                    StrokeCenterType = "Primary",
                    Status = "Active",
                    AvailableStrokeBeds = 10,
                    Latitude = 30.0444,
                    Longitude = 31.2357,
                    WaitTimeMinutes = 15
                });
                await context.SaveChangesAsync();
            }

            if (!await context.Users.AnyAsync())
            {
                var adminRole = await context.Roles.FirstAsync(r => r.RoleName == "Admin");
                var hospital = await context.Hospitals.FirstAsync();

                context.Users.Add(new User
                {
                    FirstName = "Admin",
                    LastName = "Riven",
                    Email = "admin@riven.com",
                    PhoneNumber = "+201000000001",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    RoleId = adminRole.RoleId,
                    HospitalId = hospital.HospitalId,
                    Status = "Active",
                    AccountCreationDate = DateTime.UtcNow
                });
                await context.SaveChangesAsync();
            }
        }
    }
}
