using ClinicalTrialsApi.Models;
using Microsoft.EntityFrameworkCore;


namespace ClinicalTrialsApi.Data
{
    public class ClinicalTrialsDbContext : DbContext
    {
        public ClinicalTrialsDbContext(DbContextOptions<ClinicalTrialsDbContext> options) : base(options)
        {
        }

        public DbSet<ClinicalTrial> ClinicalTrials => Set<ClinicalTrial>();
        public DbSet<Patient> Patients => Set<Patient>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración fluida: definimos campos obligatorios, longitudes, etc.
            modelBuilder.Entity<ClinicalTrial>(entity =>
            {
                entity.Property(t => t.Name).IsRequired().HasMaxLength(100);
                entity.Property(t => t.Phase).IsRequired().HasMaxLength(10);
                entity.Property(t => t.Status).IsRequired().HasMaxLength(50);
            });

            modelBuilder.Entity<Patient>(entity =>
            {
                entity.Property(p => p.FullName).IsRequired().HasMaxLength(100);

                entity.HasOne(p => p.ClinicalTrial)
                    .WithMany(t => t.Patients)
                    .HasForeignKey(p => p.ClinicalTrialId);
            });
        }
    }
}