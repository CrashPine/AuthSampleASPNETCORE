using Microsoft.EntityFrameworkCore;
using Backend.DAL.Entities;

namespace Backend.DAL;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Contract> Contracts => Set<Contract>();
    public DbSet<ContractAnalysis> ContractAnalyses => Set<ContractAnalysis>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Users
        modelBuilder.Entity<User>(b =>
        {
            b.HasKey(u => u.Id);
            b.HasIndex(u => u.Email).IsUnique();
            b.Property(u => u.Email).IsRequired();
            b.Property(u => u.UserName).IsRequired();
            b.Property(u => u.PasswordHash).IsRequired();

            b.HasMany(u => u.RefreshTokens)
             .WithOne(rt => rt.User)
             .HasForeignKey(rt => rt.UserId);

            b.HasMany(u => u.ContractAnalyses)
             .WithOne(ca => ca.User)
             .HasForeignKey(ca => ca.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // RefreshTokens
        modelBuilder.Entity<RefreshToken>(b =>
        {
            b.HasKey(rt => rt.Id);
            b.Property(rt => rt.Token).IsRequired();
            b.Property(rt => rt.ExpiresAt).IsRequired();
        });

        // Contracts
        modelBuilder.Entity<Contract>(b =>
        {
            b.HasKey(c => c.Id);
            b.Property(c => c.Name).IsRequired();
            b.Property(c => c.SourceCode).IsRequired();
            b.Property(c => c.CreatedAt).IsRequired();

            b.HasMany(c => c.Analyses)
             .WithOne(ca => ca.Contract)
             .HasForeignKey(ca => ca.ContractId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // ContractAnalysis
        modelBuilder.Entity<ContractAnalysis>(b =>
        {
            b.HasKey(ca => ca.Id);
            b.Property(ca => ca.Summary).IsRequired();
            b.Property(ca => ca.CreatedAt).IsRequired();

            b.HasOne(ca => ca.User)
             .WithMany(u => u.ContractAnalyses)
             .HasForeignKey(ca => ca.UserId);

            b.HasOne(ca => ca.Contract)
             .WithMany(c => c.Analyses)
             .HasForeignKey(ca => ca.ContractId);
        });
    }
}
