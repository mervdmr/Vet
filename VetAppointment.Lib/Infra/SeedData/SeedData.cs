using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetAppointment.Lib.Domain;

namespace VetAppointment.Lib.Infra.SeedData
{
    public class SeedData
    {
        public static async Task Initialize(VetAppointmentDbContext dbContext)
        {
            string[] roles = { "Admin", "User", "Vet" };
            var dbRoles = await dbContext.Roles.ToListAsync();
            foreach (var role in roles)
            {
                var dbRole = dbRoles.Any(x => x.Name.Equals(role, StringComparison.OrdinalIgnoreCase));
                if (!dbRole)
                {
                    Role data = new Role
                    {
                        Name = role
                    };

                    await dbContext.Roles.AddAsync(data);
                    await dbContext.SaveChangesAsync();

                }
            }


            var admin = await dbContext.Users.FirstOrDefaultAsync(x => x.Email.Equals("admin@example.com"));
            if (admin == null)
            {
                admin = new User
                {
                    Name = "Merve",
                    Surname = "Esra",
                    Email = "admin@example.com",
                    Phone = "05555555555",
                    Password = "Admin123",

                };
                await dbContext.Users.AddAsync(admin);
                await dbContext.SaveChangesAsync();
                var dbRole = await dbContext.Roles.FirstOrDefaultAsync(x => x.Name.Equals("Admin"));

                UserRole userRole = new UserRole
                {
                    UserId = admin.Id,
                    RoleId = dbRole.Id
                };

                await dbContext.UserRoles.AddAsync(userRole);
                await dbContext.SaveChangesAsync();
            }

            var cities = await dbContext.Cities.ToListAsync();

            if (cities.Count == 0)
            {
                var client = new HttpClient();
                var response = await client.GetStringAsync("https://turkiyeapi.dev/api/v1/provinces");
                var result = JsonConvert.DeserializeObject<ApiResponse>(response);
                result.Data.ForEach(x =>
                {
                    x.Id = 0;
                    x.Districts.ForEach(y =>
                    {
                        y.Id = 0;
                    });
                });

                await dbContext.Cities.AddRangeAsync(result.Data);
                await dbContext.SaveChangesAsync();
            }
            //var clinics = await dbContext.Clinics.ToListAsync();
            //if (clinics == null || clinics.Count <= 0)
            //{
            //    var clinic = new Clinic
            //    {
            //        Name = "Örnek Hayvan Veterineri",
            //        DistrictId = 1,
            //        UserId = admin.Id
            //    };
            //    await dbContext.Clinics.AddAsync(clinic);
            //    await dbContext.SaveChangesAsync();
            //}

            //var petTypes = await dbContext.Species.ToListAsync();
            //if (petTypes == null || petTypes.Count <= 0)
            //{
            //    Species species = new Species
            //    {
            //        Name = "Köpek"
            //    };

            //    Species species1 = new Species
            //    {
            //        Name = "Kedi"
            //    };

            //    await dbContext.Species.AddAsync(species);
            //    await dbContext.Species.AddAsync(species1);
            //    await dbContext.SaveChangesAsync();
            //}

        }


        public class ApiResponse
        {
            public string Status { get; set; }
            public List<City> Data { get; set; }
        }
    }
}
