using InsureFlowAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace InsureFlowAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<InsuranceProduct> InsuranceProducts { get; set; }
        public DbSet<PolicyPlan> PolicyPlans { get; set; }
        public DbSet<Policy> Policies { get; set; }
        public DbSet<PremiumPayment> PremiumPayments { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<ClaimDocument> ClaimDocuments { get; set; }
        public DbSet<ClaimStatusHistory> ClaimStatusHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User -> Customer (One-to-One)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Customer)
                .WithOne(c => c.User)
                .HasForeignKey<Customer>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // InsuranceProduct -> PolicyPlan (One-to-Many)
            modelBuilder.Entity<InsuranceProduct>()
                .HasMany(p => p.PolicyPlans)
                .WithOne(pp => pp.Product)
                .HasForeignKey(pp => pp.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Customer -> Policy (One-to-Many)
            modelBuilder.Entity<Customer>()
                .HasMany(c => c.Policies)
                .WithOne(p => p.Customer)
                .HasForeignKey(p => p.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // PolicyPlan -> Policy (One-to-Many)
            modelBuilder.Entity<PolicyPlan>()
                .HasMany(pp => pp.Policies)
                .WithOne(p => p.Plan)
                .HasForeignKey(p => p.PlanId)
                .OnDelete(DeleteBehavior.Restrict);

            // Policy -> PremiumPayment (One-to-Many)
            modelBuilder.Entity<Policy>()
                .HasMany(p => p.Payments)
                .WithOne(pp => pp.Policy)
                .HasForeignKey(pp => pp.PolicyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Policy -> Claim (One-to-Many)
            modelBuilder.Entity<Policy>()
                .HasMany(p => p.Claims)
                .WithOne(c => c.Policy)
                .HasForeignKey(c => c.PolicyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Claim -> ClaimDocument (One-to-Many)
            modelBuilder.Entity<Claim>()
                .HasMany(c => c.Documents)
                .WithOne(cd => cd.Claim)
                .HasForeignKey(cd => cd.ClaimId)
                .OnDelete(DeleteBehavior.Cascade);

            // Claim -> ClaimStatusHistory (One-to-Many)
            modelBuilder.Entity<Claim>()
                .HasMany(c => c.Histories)
                .WithOne(h => h.Claim)
                .HasForeignKey(h => h.ClaimId)
                .OnDelete(DeleteBehavior.Cascade);

            // User -> ClaimStatusHistory (One-to-Many)
            modelBuilder.Entity<User>()
                .HasMany(u => u.ClaimHistories)
                .WithOne(h => h.User)
                .HasForeignKey(h => h.UpdatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique Constraints

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<InsuranceProduct>()
                .HasIndex(p => p.ProductName)
                .IsUnique();

            modelBuilder.Entity<Policy>()
                .HasIndex(p => p.PolicyNumber)
                .IsUnique();

            modelBuilder.Entity<PremiumPayment>()
                .HasIndex(p => p.TransactionReference)
                .IsUnique();

            modelBuilder.Entity<Claim>()
                .HasIndex(c => c.ClaimNumber)
                .IsUnique();
        }
    }
}