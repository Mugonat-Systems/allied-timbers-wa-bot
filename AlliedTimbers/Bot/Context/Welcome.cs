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
                        new("Products", "Products in Store", "productsInfo"),
                        //new("Get a quotation","Get a Quotation for products in stock", "quotationInfo"),
                        new("Contact us", "Visit our branches or contact branch directly", "branchCategories"),
                        new("News and Events", "View news and updates", "newsEvents"),
                    }
                },
                new()
                {
                    Title = "Support",
                    Options = new List<ChatMessageModels.ListOption>
                    {
                        new("Let's Chat", "Talk to us directly", "live chat"),
                        new("FAQs", "Frequently asked questions", "faqs"),
                        new("Feedback & Consultations", "For for Feedback and Consultations", "feedbackConsultations"),
                        new("Account", "Manage your account", "account")
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
                    new("Trusses", "Strong timber frames", "trusses"),
                    new("Timber Products", "High-quality timber for all your building needs", "timber"),
                    new("Boards", "Durable boards and stylish doors for every project", "boards"),
                    new("Treated Poles", "Long-lasting, termite-resistant treated poles", "poles"),
                    new("Doors", "Sturdy, elegant doors for any space", "doors")
                }
            }
        };
        return json;
    }
    
    public BotMessageConfig FxGetQuotationInfo(BotMessageConfig json)
    {
        var list = (ListMenuMessage)json.Message;

        list.Items = new List<ChatMessageModels.ListSection>
        {
            new()
            {
                Title = "Categories",
                Options = new List<ChatMessageModels.ListOption>
                {
                    new("Trusses", "Diverse roof truss options cater to project specifications.", "trussesQuotation"),
                    new("Timber Products", "Timber Products", "timberQuotation"),
                    new("Boards and Doors", "Boards and Doors", "boardsQuotation"),
                    new("Treated Poles", "Treated Poles", "polesQuotation"),
                    new("Doors", "Doors", "doorsQuotation")
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
                     new("Branches", "Visit our branches or contact branch directly", "branches"),
                     new("Operating times", "Check our working hours", "operatingTime"),
                }
            }
        };

        return json;
    }

    private List<Product> ProductType()
    {
        if (Thread.CurrentMessage == "trusses")
        {
            Session.Set("category", Thread.CurrentMessage);
            return Database.Products.Where(x => x.IsTrusses)
                .OrderBy(x => x.IsTrusses)
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
           
        if (Thread.CurrentMessage == "next page" ||
         Thread.CurrentMessage == "previous page")
        {
            switch(Session.GetString("category"))
            {
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
                    return Database.Products.Where(d => d.IsTrusses != true &&
                        d.IsTimber != true && d.IsBoards != true && d.IsPoles != true && d.IsDoors != true).OrderBy(z => z.Name).ToList();
            }
        }
        
        return Database.Products.Where(d => d.IsBoards != true &&
       d.IsTrusses != true && d.IsTimber != true && d.IsPoles != true && d.IsDoors != true)
             .OrderBy(x => x.Name).ToList();
    }
    
     private List<Product> QuotationProductType()
    {
        if (Thread.CurrentMessage == "trussesQuotation")
        {
            Session.Set("category", Thread.CurrentMessage);
            return Database.Products.Where(x => x.IsTrusses)
                .OrderBy(x => x.Name)
                .ToList();
        }

        if (Thread.CurrentMessage == "timberQuotation")
        {
            Session.Set("category", Thread.CurrentMessage);
            return Database.Products.Where(x => x.IsTimber)
                .OrderBy(x => x.Name)
                .ToList();
        }

        if (Thread.CurrentMessage == "boardsQuotation")
        {
            Session.Set("category", Thread.CurrentMessage);
            return Database.Products.Where(x => x.IsBoards)
                .OrderBy(x => x.Name)
                .ToList();
        }

        if (Thread.CurrentMessage == "polesQuotation")
        {
            Session.Set("category", Thread.CurrentMessage);
            return Database.Products.Where(x => x.IsPoles)
                .OrderBy(x => x.Name)
                .ToList();
        }

        if (Thread.CurrentMessage == "doorsQuotation")
        {
            Session.Set("category", Thread.CurrentMessage);
            return Database.Products.Where(x => x.IsDoors)
                .OrderBy(x => x.Name)
                .ToList();
        }
           
        if (Thread.CurrentMessage == "next page" ||
         Thread.CurrentMessage == "previous page")
        {
            switch(Session.GetString("category"))
            {
                case "trussesQuotation":
                    return Database.Products.Where(x => x.IsTrusses)
                        .OrderBy(x => x.Name).ToList();
                case "timberQuotation":
                    return Database.Products.Where(x => x.IsTimber)
                        .OrderBy(x => x.Name).ToList();
                case "boardsQuotation":
                    return Database.Products.Where(x => x.IsBoards)
                        .OrderBy(x => x.Name).ToList();
                case "polesQuotation":
                    return Database.Products.Where(x => x.IsPoles)
                        .OrderBy(x => x.Name).ToList();
                case "doorsQuotation":
                    return Database.Products.Where(x => x.IsDoors)
                        .OrderBy(x => x.Name).ToList();
                default:
                    return Database.Products.Where(d => d.IsTrusses != true &&
                        d.IsTimber != true && d.IsBoards != true && d.IsPoles != true && d.IsDoors != true).OrderBy(z => z.Name).ToList();
            }
        }
        
        return Database.Products.Where(d => d.IsBoards != true &&
       d.IsTrusses != true && d.IsTimber != true && d.IsPoles != true && d.IsDoors != true)
             .OrderBy(x => x.Name).ToList();
    }
    
    public string GetStep(string alias)
    {
        if (Thread.AliasChatMessages.ContainsKey(alias))
        {
            return Thread.AliasChatMessages[alias].Step.ToString();
        }
        else
        {
            throw new KeyNotFoundException($"The alias ''{alias}'' was not found in the Thread.AliasChatMessages dictionary.");
        }
    
    }
}