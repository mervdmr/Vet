using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VetAppointment.Lib.Domain;

namespace VetAppointment.Lib.Infra
{
    public class VetAppointmentDbContext : DbContext
    {

        public VetAppointmentDbContext(DbContextOptions<VetAppointmentDbContext> options)
          : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {

                    if (property.ClrType == typeof(string))
                    {
                        property.SetDefaultValue(null); // Tüm string alanların varsayılan değerini null olarak ayarlar
                        property.IsNullable = true; //String alanlar nullable olabilecek şekilde configre eder.
                    }
                }
            }

            modelBuilder.Entity<Pet>()
           .HasOne(p => p.Appointment)
           .WithOne(a => a.Pet)
           .HasForeignKey<Appointment>(a => a.PetId)
           .OnDelete(DeleteBehavior.ClientCascade);

            // User - Appointment ilişkisinde cascade delete davranışını ayarla
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.User)
                .WithMany(u => u.Appointments)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.ClientCascade); // veya DeleteBehavior.NoAction

            // Clinic - Appointment ilişkisinde cascade delete davranışını ayarla
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Clinic)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.ClinicId)
                .OnDelete(DeleteBehavior.ClientCascade); // veya DeleteBehavior.NoAction

            // District - Clinic ilişkisinde cascade delete davranışını ayarla
            modelBuilder.Entity<Clinic>()
                .HasOne(c => c.District)
                .WithMany(d => d.Clinics)
                .HasForeignKey(c => c.DistrictId)
                .OnDelete(DeleteBehavior.ClientCascade); // veya DeleteBehavior.NoAction

            // City - District ilişkisinde cascade delete davranışını ayarla
            modelBuilder.Entity<District>()
                .HasOne(d => d.City)
                .WithMany(c => c.Districts)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.Restrict); // veya DeleteBehavior.NoAction

            modelBuilder.Entity<UserRole>()
           .HasKey(ur => new { ur.UserId, ur.RoleId });

            // User ile UserRole arasında bir çoka çok ilişki tanımlaması
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);

            // Role ile UserRole arasında bir çoka çok ilişki tanımlaması
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);


            base.OnModelCreating(modelBuilder);
        }
        public DbSet<City> Cities { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Species> Species { get; set; }
        public DbSet<Pet> Pets { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entities = ChangeTracker.Entries().Where(x => x.Entity is BaseEntity<int> && (x.State == EntityState.Added || x.State == EntityState.Modified));
            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    ((BaseEntity<int>)entity.Entity).CreatedDateTime = DateTime.Now;

                }
                ((BaseEntity<int>)entity.Entity).UpdatedDateTime = DateTime.Now;

                if (entity.Entity is User)
                {
                    ((User)entity.Entity).FullName = ((User)entity.Entity).Name + " " + ((User)entity.Entity).Surname;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
