using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using AlliedTimbers.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;

namespace AlliedTimbers.Migrations;

internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
{
    public Configuration()
    {
        AutomaticMigrationsEnabled = true;
        AutomaticMigrationDataLossAllowed = AppSettings["app.env"] == "local";
    }

    protected override void Seed(ApplicationDbContext context)
    {
        if (AppSettings["app.env"] != "local") return;
       
        const string adminEmail = "admin@mugonat.com";
        const string adminPassword = "Mugonat#99";
        const string adminRole = "Admin";

        var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
        var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

        var adminUser = userManager.FindByEmail(adminEmail);
        if (adminUser == null)
        {
            var user = new ApplicationUser { UserName = adminEmail, Email = adminEmail,
            RoleName = adminRole};
            var result = userManager.Create(user, adminPassword);

            if (result.Succeeded)
            {
                if (!roleManager.RoleExists(adminRole))
                {
                    roleManager.Create(new IdentityRole(adminRole));
                }

                userManager.AddToRole(user.Id, adminRole);
            }
        }

        //for (var i = 0; i < 5; i++)
        //{
        //    var productFiles = Enumerable.Range(0, Faker.RandomNumber.Next(5))
        //        .Select(n => new ProductFile
        //        {
        //            Name = Faker.Company.CatchPhrase(),
        //            Path = Faker.Internet.Url(),
        //            Type = new List<string> { "image", "pdf", "document" }[Faker.RandomNumber.Next(0, 2)],
        //            Size = Faker.RandomNumber.Next(0L, 1000000L),
        //            IsChecked = Faker.Boolean.Random()
        //        })
        //        .ToList();

        //    var description = Faker.Lorem.Paragraph();
        //    var productInfos = Enumerable.Range(0, Faker.RandomNumber.Next(5))
        //        .Select(n => new ProductInfo()
        //        {
        //            Title = Faker.Lorem.Sentence(),
        //            Description = description,
        //            IsChecked = Faker.Boolean.Random()
        //        })
        //        .ToList();
        //    var ticker = Faker.Finance.Ticker();
        //    var requirements = string.Join(", ", Faker.Lorem.Words(Faker.RandomNumber.Next(2, 10)));

        //    var loan = new LoanApplication
        //    {
        //        CustomerName = Faker.Name.FullName(),
        //        Amount = Faker.RandomNumber.Next(),
        //        IdNo = Faker.Identification.UkPassportNumber(),
        //        PhoneNo = Faker.Phone.Number(),
        //        LoanApproval = Faker.Enum.Random<LoanApproval>(),
        //        DateApplied = DateTime.Now.AddDays(3),
        //    };


        //    var product = context.Products.Add(new Product
        //    {
        //        Name = ticker,
        //        Description = description,
        //        Requirements = requirements,
        //        //Files = productFiles,
        //        IsLoan = Faker.Boolean.Random(),
        //        IsMukando = Faker.Boolean.Random(),
        //        IsSolar = Faker.Boolean.Random(),

        //        Information = productInfos,
        //    });


        //}

        //for (var j = 0; j < 15; j++)
        //{
        //    var address = new Address
        //    {
        //        Name = Faker.Address.City(),
        //        Line1 = Faker.Address.StreetAddress(true),
        //        Line2 = Faker.Address.StreetAddress()
        //    };

        //    var name = Faker.Company.CatchPhrase();
        //    var email = Faker.Internet.FreeEmail();
        //    var phoneNumber = Faker.Phone.Number();

        //    context.CompanyBranches.Add(new CompanyBranch
        //    {
        //        Name = name,
        //        Email = email,
        //        PhoneNumber = phoneNumber,
        //        Address = address
        //    });
        //}

        //for (var z = 0; z < 12; z++)
        //{
        //    var question = Faker.Lorem.Paragraph();
        //    var answer = Faker.Lorem.Sentence();
        //    var tags = Faker.Lorem.GetFirstWord();

        //    context.Faqs.Add(new Faq
        //    {
        //        Question = question,
        //        Answer = answer,
        //        Tags = tags,
        //        CreatedAt = DateTime.Now.Date,
        //        UpdatedAt = DateTime.Now.Date.AddDays(12.0)
        //    });
        //}

        ////for (var k = 0; k < 20; k++)
        ////{
        ////    var name = Faker.Company.Name();
        ////    var startDate = DateTime.Now.Date;
        ////    var endDate = DateTime.Now.Date.AddDays(2);
        ////    var description = Faker.Lorem.Paragraph();

        ////    context.Promotions.Add(new Promotion
        ////    {
        ////        Name = name,
        ////        Description = description,
        ////        StartDate = startDate,
        ////        EndDate = endDate
        ////    });
        ////}

        ////for (var o = 0; o < 7; o++)
        ////{
        ////    var opinion = Faker.Lorem.Sentence();
        ////    var createdAt = DateTime.Now.AddMinutes(2);

        ////    context.Feedbacks.Add(new Feedback
        ////    {
        ////        Opinion = opinion,
        ////        CreatedAt = createdAt
        ////    });
        ////}

        context.SaveChanges();
    }
    }