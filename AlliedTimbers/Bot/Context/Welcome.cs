using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading;
using AlliedTimbers.Models;
using Jil;
using Microsoft.Net.Http.Headers;
using Mugonat.Chat.BotEngine.Messages;
using Mugonat.Chat.BotEngine.Messages.Internal;
using Mugonat.Chat.BotEngine.Models;

namespace AlliedTimbers.Bot.Context;

public partial class AlliedTimbersBotContext
{
    public BotMessageConfig FxGetMenu(BotMessageConfig json)
    {
            var list = (ListMenuMessage)json.Message;

            list.Items = new List<ChatMessageModels.ListSection>
            {
                new()
                {
                    Title = "Information",
                    Options = new List<ChatMessageModels.ListOption>
                    {
                        new("Product Categories", "Products in our catalog", "categories"),
                        new("Contact Us", "Visit our branches or contact them directly", "branchCategories"),
                        new("Promotions", "View running promotions", "promotions"),
                        new("Events", "View upcoming events", "events"),
                        new("Educational Contents", "View educational content", "education"),
                        new("News Updates", "View news and updates", "news"),
                       
                       // new("Promotions", "View running promotions", "promotions")
                    }
                },
                new()
                {
                    Title = "Support",
                    Options = new List<ChatMessageModels.ListOption>
                    {
                        new("FAQs", "Frequently asked questions", "faqs"),
                        new("Let's Chat", "Talk to us directly", "live chat"),
                        new("Feedback", "Give us feedback on how you find our services", "rating"),
                        new("Consultations", "Request a consultation", "consultation"),
                        new("Appointment", "Book an appointment", "appointment")
                    }
                }
            };

            return json;
    }

    public BotMessageConfig FxGetProdCategories(BotMessageConfig json)
    {
        var list = (ListMenuMessage)json.Message;

        list.Items = new List<ChatMessageModels.ListSection>
        {
            new()
            {
                Title = "Categories",
                Options = new List<ChatMessageModels.ListOption>
                {
                    new("Trusses", "Diverse roof truss options cater to project specifications.", "trusses"),
                    new("Timber Products", "Timber Products", "timber"),
                    new("Boards and Doors", "Boards and Doors", "boards"),
                    new("Treated Poles", "Treated Poles", "poles"),
                    new("Doors", "Doors", "doors"),
                    new("Loans", "Affordable Loans", "loans"),
                    new("Mukando", "Save to purchase", "mukando"),
                    new("Solar Systems","No more power cuts", "solar"),
                    new("Funeral Plan", "Have a unique memorial", "funeral")
                }
            }
        };
        return json;
    }

    public BotMessageConfig FxGetContactDetails(BotMessageConfig json)
    {
        var list = (ListMenuMessage)json.Message;

        list.Items = new List<ChatMessageModels.ListSection>
        {
            new()
            {
                Title = "Categories",
                Options = new List<ChatMessageModels.ListOption>
                {
                     new("Branches", "Visit our branches or contact them directly", "branches"),
                     new("Operating times", "Check our working hours", "operatingTime"),
                }
            }
        };

        return json;
    }

    private List<Product> ProductType()
    {
        if (Thread.CurrentMessage == "loans")
        {
            Session.Set("category", Thread.CurrentMessage);
            return Database.Products.Where(x => x.IsLoan &&
            x.IsMukando == false && x.IsSolar == false)
                .OrderBy(x => x.Name)
                .ToList();
        }

        if (Thread.CurrentMessage == "trusses")
        {
            Session.Set("category", Thread.CurrentMessage);
            return Database.Products.Where(x => x.IsTrusses)
                .OrderBy(x => x.Name)
                .ToList();
        }

        if (Thread.CurrentMessage == "timber")
        {
            Session.Set("category", Thread.CurrentMessage);
            return Database.Products.Where(x => x.IsTimber)
                .OrderBy(x => x.Name)
                .ToList();
        }

        if (Thread.CurrentMessage == "boards")
        {
            Session.Set("category", Thread.CurrentMessage);
            return Database.Products.Where(x => x.IsBoards)
                .OrderBy(x => x.Name)
                .ToList();
        }

        if (Thread.CurrentMessage == "poles")
        {
            Session.Set("category", Thread.CurrentMessage);
            return Database.Products.Where(x => x.IsPoles)
                .OrderBy(x => x.Name)
                .ToList();
        }

        if (Thread.CurrentMessage == "doors")
        {
            Session.Set("category", Thread.CurrentMessage);
            return Database.Products.Where(x => x.IsDoors)
                .OrderBy(x => x.Name)
                .ToList();
        }
        
        if(Thread.CurrentMessage == "loans")
        {
            Session.Set("category", Thread.CurrentMessage);
            return Database.Products.Where(x => x.IsLoan &&
            x.IsMukando == false && x.IsSolar == false)
                .OrderBy(x => x.Name)
                .ToList();
        }
               
       if(Thread.CurrentMessage == "mukando")
        {
            Session.Set("category", Thread.CurrentMessage);
            return Database.Products.Where(s => s.IsMukando &&
            s.IsLoan)
                 .OrderBy(x => x.Name)
                .ToList();
        }
            
       if(Thread.CurrentMessage == "solar")
        {
            Session.Set("category", Thread.CurrentMessage);
            return Database.Products.Where(x => x.IsSolar &&
            x.IsLoan)
                .OrderBy(x => x.Name).ToList();
        }
           
        if (Thread.CurrentMessage == "next page" ||
         Thread.CurrentMessage == "previous page")
        {
            switch(Session.GetString("category"))
            {
                case "loans":
                    return Database.Products.Where(x => x.IsLoan && x.IsMukando == false && x.IsSolar == false)
                        .OrderBy(x => x.Name).ToList();
                case "mukando":
                    return Database.Products.Where(s => s.IsMukando && s.IsLoan)
                        .OrderBy(x => x.Name).ToList();
                case "solar":
                    return Database.Products.Where(x => x.IsSolar && x.IsLoan)
                        .OrderBy(x => x.Name).ToList();
                case "trusses":
                    return Database.Products.Where(x => x.IsTrusses)
                        .OrderBy(x => x.Name).ToList();
                case "timber":
                    return Database.Products.Where(x => x.IsTimber)
                        .OrderBy(x => x.Name).ToList();
                case "boards":
                    return Database.Products.Where(x => x.IsBoards)
                        .OrderBy(x => x.Name).ToList();
                case "poles":
                    return Database.Products.Where(x => x.IsPoles)
                        .OrderBy(x => x.Name).ToList();
                case "doors":
                    return Database.Products.Where(x => x.IsDoors)
                        .OrderBy(x => x.Name).ToList();
                default:
                    return Database.Products.Where(d => d.IsLoan != true &&
                        d.IsMukando != true && d.IsSolar != true).OrderBy(z => z.Name).ToList();
            }
        }
        
        return Database.Products.Where(d => d.IsLoan != true &&
       d.IsMukando != true && d.IsSolar != true)
             .OrderBy(x => x.Name).ToList();
    }



    //public void FxOnRegister(BotMessageConfig json)
    //{
    //    if (!Session.GetBool("collecting_name")) return;

    //    var customer = Database.Customers.FirstOrDefault(w => w.PhoneNumber == Thread.ThreadId);
    //    if (customer == null) return;

    //    if(customer != null)
    //    {
    //        customer.Name = Thread.CurrentMessage;
    //        Database.Customers.AddOrUpdate(customer);
    //        Database.SaveChanges();

    //        Session.Set("collecting_name", false);
    //    }
    //}
}