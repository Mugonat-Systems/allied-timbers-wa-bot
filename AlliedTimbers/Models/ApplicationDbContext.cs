using System.Data.Entity;
using AlliedTimbers.Migrations;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AlliedTimbers.Models;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext()
        : base("DefaultConnection", false)
    {
        Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
    }

    public DbSet<Product> Products { get; set; }

    public DbSet<Account> Accounts { get; set; }
    
    public DbSet<Event> Events { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<News> News { get; set; }
    
    public DbSet<Educational> Educationals { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<ProductInfo> ProductInfos { get; set; }
    //public DbSet<ProductFile> ProductFiles { get; set; }
    public DbSet<CompanyBranch> CompanyBranches { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Faq> Faqs { get; set; }
    public DbSet<Promotion> Promotions { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Consultation> Consultations { get; set; }
    public DbSet<QuickResponse> QuickResponses { get; set; }
    
    public DbSet<Audit> Audits { get; set; }

    public static ApplicationDbContext Create()
    {
        return new ApplicationDbContext();
    }
}