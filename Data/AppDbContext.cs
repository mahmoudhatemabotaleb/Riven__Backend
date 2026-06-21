using Microsoft.EntityFrameworkCore;
using RivenBackend.Models;
namespace RivenBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Case> Cases { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<Ambulance> Ambulances { get; set; }
        public DbSet<VitalSigns> VitalSigns { get; set; }
        public DbSet<Symptoms> Symptoms { get; set; }
        public DbSet<RiskFactors> RiskFactors { get; set; }
        public DbSet<NihssAssessment> NihssAssessments { get; set; }
        public DbSet<AiReport> AiReports { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<EcgResult> EcgResults { get; set; }
        public DbSet<StrokeResult> StrokeResults { get; set; }
        public DbSet<OtpVerification> OtpVerifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.UseIdentityByDefaultColumns();
            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
        protected override void ConfigureConventions(ModelConfigurationBuilder builder)
        {
            builder.Properties<string>().HaveColumnType("text");
            builder.Properties<DateTime>().HaveColumnType("timestamptz");
            builder.Properties<DateTime?>().HaveColumnType("timestamptz");
            builder.Properties<bool>().HaveColumnType("boolean");
            builder.Properties<bool?>().HaveColumnType("boolean");
        }
    }
}