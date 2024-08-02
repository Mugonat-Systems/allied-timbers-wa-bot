using System;
using System.Collections.Generic;
using System.Linq;
using AlliedTimbers.Helpers;
using AlliedTimbers.Models;
using Mugonat.Chat.BotEngine;
using Mugonat.Chat.BotEngine.Messages;
using Mugonat.Chat.BotEngine.Messages.Internal;
using Mugonat.Chat.BotEngine.Models;
using Mugonat.Utils.Extensions;

namespace AlliedTimbers.Bot.Context;

public partial class AlliedTimbersBotContext
{
    public BotMessageConfig FxGetQuotations(BotMessageConfig config)
    {
        var list = (ListMenuMessage)config.Message;
      
        var items = QuotationProductType();

        if(items.Count == 0)
        {
            list.Title = "No products";
            list.Items = new List<ChatMessageModels.ListSection>
            {
                new()
                {
                    Title= "No products.",
                    Options = new List<ChatMessageModels.ListOption>
                    {
                        new ("Menu", "View the main menu", "menu")
                    }
                }
            };

            return config;
        }

        if(items.Count <= 10) {
            list.Items = new List<ChatMessageModels.ListSection>
            {
                new()
                {
                    Title = "Products",
                    Options = items.Select(p => new ChatMessageModels.ListOption
                    {
                        
                        Title = p.Name.ToEllipsis(20),
                        PostbackText = $"quotation {p.Id}",
                        Description = ($"${p.Price}").ToString()
                    }).ToList()
                }
            };
            // not here
            
            //Session.Set("QuotationItem", items);
            return config;
                 
        }
        
        if (!Pagination.Paging(Thread.CurrentMessage, 8)) Pagination.MoveToPage(0);

        list.Items = new List<ChatMessageModels.ListSection>
        {
            new()
            {
                Title = "Products",
                Options = Pagination
                    .GetPaged( items, 8)
                    .Select(p => new ChatMessageModels.ListOption
                    {
                        Title = p.Name.ToEllipsis(20),
                        PostbackText = $"quotation {p.Id}",
                        Description = p.Description.ToEllipsis(20)
                    }).ToList()
            },
            new()
            {
                Title = "Navigation",
                Options = new List<ChatMessageModels.ListOption>
                {
                    new("Next page", "Show more items", "next page"),
                    new("Previous page", "Show previous items", "previous page")
                }
            }
        };
        int.TryParse(Thread.CurrentMessage.Replace("quotation ", ""),
            out var id);
        var product = Database.Products.Find(id);
        return config;
    }
    
    public Dictionary<string,string> FxGetQuotation(BotThread thread)
    {
        
        var postbackText = Session.GetString("PostbackText");
        int.TryParse(postbackText,out var id);
        var product = Database.Products.Find(id);

        string title = "", caption = "";

        if (product == default)
        {
            title = "The product you have specified may have been deleted";
            caption = "Please check again later";

        }
        else
        {
            FxGetProductAmount(product.Id);
            FxGetProductName(product.Id);
            title = $"Allied Timbers Quotation";
            var QuotationItem = Session.GetString("ProductName");
            var QuotationUnitPrice = Session.GetDecimal("productPriceAmount");
            var QuotationQuantity = Session.GetInteger("quotationItemQuantity");
            var QuotationPrice = Session.GetDecimal("productPriceAmount") * Session.GetInteger("quotationItemQuantity");
            bool QuotationPricePayment = Session.Set("QuotationPaymentAmount", QuotationPrice);
            caption = $"Product: {QuotationItem} \n\n" +
                      $"Price: ${QuotationUnitPrice} \n\n" +
                      $"Quantity: {QuotationQuantity} \n\n " +
                      $"Total: ${QuotationPrice}";
        }

        return new Dictionary<string, string>
        {
            {"title", title },
            {"caption", caption }
        };
    }

    public void FxGetProductName(int productId)
    {
        using (var _db = new ApplicationDbContext())
        {
            var product = _db.Products.Find(productId);
            Session.Set("ProductName", product.Name);
        }
    }
    public void FxGetProductAmount(int productId)
    {
        // Initialize the database context
        using (var _db = new ApplicationDbContext())
        {
            // Get the product price from the products table
            var product = _db.Products.Find(productId);

            if (product != null)
            {
                decimal productPriceAmount = product.Price;
                // Store the amount in the session
                Session.Set("productPriceAmount", productPriceAmount);
            }
            else
            {
                // Handle the case where the product is not found
                throw new InvalidOperationException("Product price is not currently available");
            }
        }
    }
    public Dictionary<string,string> FxSubmitQuotation(BotThread thread)
    {
        int.TryParse(Thread.CurrentMessage.Replace("quotation ", ""),
            out var id);
        var product = Database.Products.Find(id);

        string title = "", caption = "";

        if (product == default)
        {
            title = "The product you have specified may have been deleted";
            caption = "Please check again later";

        }
        else
        {
            Session.Set("ProductId", product.Id);

            title = $"{product.Name}\n";
            
            Session.Set("ProductName", product.Name);
            // caption = $"{product.Description} \n\n {product.Requirements}";
            
            FxGetAmount(product.Id);

        }

        return new Dictionary<string, string>
        {
            {"title", title },
            {"caption", caption }
        };
    }
   
    public void FxQuotationSaveQuantity(BotMessageConfig json)
    {
        Session.Set("quotationItemQuantity", Thread.CurrentMessage);
        var amount = Session.GetInteger("quotationItemQuantity") * Session.GetDecimal("productPriceAmount");
        Session.Set("QuotationItemAmount", amount);
    }

    public void FxQuotationSaveProduct(BotMessageConfig json)
    {
        var selectedProduct = Thread.CurrentMessage.Replace("quotation", "");
        Session.Set("SelectedProductId", selectedProduct);
        Session.Set("PostbackText", selectedProduct);
        
    }
    
    public string FxGotoQuotationOption(BotThread thread)
    {
        if (thread.CurrentMessage == "Add Product")
        {
            return Thread.AliasChatMessages["Quotation"].Step.ToString();
        }

        if (thread.CurrentMessage == "Get Quotation")
        {
            return Thread.AliasChatMessages["submitQuotation"].Step.ToString();
        }

        return Thread.AliasChatMessages["menu"].Step.ToString();
    }
    
    public string FxGetPaymentPhone(BotThread thread) => Thread.ThreadId;
    
    public string FxIsQuotationPhoneNumberEntered(BotThread thread)
    {
        bool isPhoneEntered = FileHandler.PhoneNumber.IsLikelyPhoneNumber(Thread.CurrentMessage);
        if (isPhoneEntered)
        {
            return GetStep("quotation/makePayment");
        }
        else
        {
            return GetStep("paymentQuotationPhoneInput");
        }
    }
    
    public void FxSaveQuotationPhoneNumber(BotMessageConfig json)
    {
        Session.Set("phoneNumber", Thread.CurrentMessage);
    }
    
    public BotMessageConfig FxMakeQuotationPayment(BotMessageConfig config)
{
    var amount = Session.GetDecimal("QuotationPaymentAmount");
    var method = Session.GetString("QuotationMethod");

    var isPaymentSuccessful = MakePayment(
        phone: Thread.CurrentMessage,
        amount: amount, 
        method.ToLower()
        );

    BotMessageConfig responseConfig;
    TextMessage text = null;
    ButtonOptionsMessage buttonGroup = null;
    
    if (isPaymentSuccessful)
    {
        var phone = Thread.ThreadId;
        using var dbContext = new ApplicationDbContext();
        var customer = dbContext.Customers.ToList()
            .FirstOrDefault(c => c.PhoneNumber == phone);
        var Payment = Session.Build<Payment>();
        Payment.CustomerId = customer.Id;
        Payment.Date = DateTime.Now;
        Payment.BranchName = Session.GetString("branchName");
        Payment.Status = "Successful";
        Payment.Quantity = Session.GetString("quotationItemQuantity");
        Payment.Product = Session.GetString("ProductName");
        Payment.Amount = amount;
        Payment.PaymentMethod = method;
        Payment.PhoneNumber = phone;
        Payment.PaymentId = Guid.NewGuid().ToString();
        dbContext.Payments.Add(Payment);
        dbContext.SaveChanges();
        
        buttonGroup = new ButtonOptionsMessage("Payment is successful", "Would you like to return to menu?",
            new ChatMessageModels.ButtonOption("Menu"));
        
    }
    else
    {
        
        var phone = Thread.ThreadId;
        using var dbContext = new ApplicationDbContext();
        var customer = dbContext.Customers.ToList()
            .FirstOrDefault(c => c.PhoneNumber == phone);
        var Payment = Session.Build<Payment>();
        Payment.CustomerId = customer.Id;
        Payment.Date = DateTime.Now;
        Payment.BranchName = Session.GetString("branchName");
        Payment.Status = "Failed";
        Payment.Amount = amount;
        Payment.PaymentMethod = method;
        Payment.PhoneNumber = phone;
        Payment.Quantity = Session.GetString("quotationItemQuantity");
        Payment.Product = Session.GetString("ProductName");
        Payment.PaymentId = Guid.NewGuid().ToString();
        dbContext.Payments.Add(Payment);
        dbContext.SaveChanges();
        
        buttonGroup = new ButtonOptionsMessage("Payment failed", "Would you like to retry?",
            new ChatMessageModels.ButtonOption("Purchase"), new ChatMessageModels.ButtonOption("Menu"));
    }
    config.Type = "buttons";
    config.Message = buttonGroup;
    return config;
}
    
    public BotMessageConfig FxQuotationNearestBranches(BotMessageConfig json)
    {
        var items = Database.CompanyBranches.OrderBy(x => x.Name).ToList();
        var list = (ListMenuMessage)json.Message;

        if (items.Count == 0)
        {
            list.Title = "No open branches.";
            list.Items = new List<ChatMessageModels.ListSection>
            {
                new()
                {
                    Title= "No open branches.",
                    Options = new List<ChatMessageModels.ListOption>
                    {
                        new ("Menu", "View the main menu", "menu")
                    }
                }
            };
            return json;
        }

        if (items.Count <= 10)
        {
            list.Items = new List<ChatMessageModels.ListSection>
            {
                new()
                {
                    Title = "Branches",
                    Options = items.Select(f => new ChatMessageModels.ListOption
                    {
                        Title = f.Name,
                        PostbackText = $"nearBranch {f.Id}",
                        Description = f.PhoneNumber

                    }).ToList(),
                },
            };
            return json;
        }

        if (!Pagination.Paging(Thread.CurrentMessage, 8)) Pagination.MoveToPage(0);

        list.Items = new List<ChatMessageModels.ListSection>
            {
                new()
                {
                    Title = "Branches: Page " + Pagination.CurrentHumanPage,
                    Options = Pagination
                        .GetPaged(items, 8)
                        .Select(f => new ChatMessageModels.ListOption
                        {
                            Title = f.Name.ToEllipsis(20),
                            PostbackText = $"nearBranch {f.Id}",
                            Description = f.PhoneNumber.ToEllipsis(20)
                        }).ToList()
                },

            };

        var pagination = new List<ChatMessageModels.ListOption>();

        if (Pagination.CurrentPage > 0)
        {
            pagination.Add(new("Previous page", "Show previous items", "previous page"));
        }

        if (items.Count > (Pagination.CurrentHumanPage * 8)) // 
        {
            pagination.Add(new("Next page", "Show more items", "next page"));
        }

        list.Items.Add(new ChatMessageModels.ListSection("Navigation", pagination.ToArray()));


        return json;
    }
    
    public void FxSaveQuotationBranchId(BotMessageConfig json)
    {
        int.TryParse(Thread.CurrentMessage.Replace("nearBranch ", ""),
            out var id);
        var branch = Database.CompanyBranches.Find(id);
        Session.Set("branchId", id);
        Session.Set("branchName", branch.Name);
    }
    
    public void FxSaveQuotationMethod(BotMessageConfig config)
    {
        Session.Set("QuotationMethod", Thread.CurrentMessage);
    }
}