using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TestGenerator.DAL.Models;

namespace TestGenerator.DAL.Data;

public class ApplicationDbContext : IdentityDbContext<IdentityUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Test>? Tests { get; set; }
    public DbSet<Question>? Questions { get; set; }
    public DbSet<Answer>? Answers { get; set; }
    public DbSet<Tag>? Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Test>().ToTable("Test");
        modelBuilder.Entity<Question>().ToTable("Question");
        modelBuilder.Entity<Answer>().ToTable("Answer");
        modelBuilder.Entity<Tag>().ToTable("Tag");
    }
}