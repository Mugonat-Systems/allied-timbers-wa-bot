using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlliedTimbers.Helpers;
using AlliedTimbers.Models;
using Mugonat.Chat.BotEngine;
using Mugonat.Chat.BotEngine.Attributes;
using Mugonat.Chat.BotEngine.Messages;
using Mugonat.Chat.BotEngine.Messages.Internal;
using Mugonat.Chat.BotEngine.Models;
using Mugonat.Utils.Extensions;
using Webdev.Payments;
using Payment = AlliedTimbers.Models.Payment;


namespace AlliedTimbers.Bot.Context;

public partial class AlliedTimbersBotContext
{
    public BotMessageConfig FxGetProducts(BotMessageConfig config)
    {
        var list = (ListMenuMessage)config.Message;
      
        var items = ProductType();

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
                        PostbackText = $"product {p.Id}",
                        Description = p.Name.ToEllipsis(47) + $" ${p.Price}"

                    }).ToList()
                }
            };
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
                        PostbackText = $"product {p.Id}",
                        Description = p.Name.ToEllipsis(47) + $" ${p.Price}"
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

        return config;
    }
    
/*    public Dictionary<string,string> FxGetProduct(BotThread thread)
    {
        int.TryParse(Thread.CurrentMessage.Replace("product ", ""),
            out var id);
        var product = Database.Products.Find(id);

        string title = "", caption = "";

        if (product == default)
        {
            var newTitle = "The product you have specified may have been deleted";
            title = newTitle.ToEllipsis(20);
            caption = "Please check again later";

        }
        else
        {
            Session.Set("ProductId", product.Id);

            title = $"{product.Name.ToEllipsis(20)}\n";
            caption = $"{AppendToUrl(product.Image)} \n\n{product.Description} \n\n ${product.Price}";
            
            Session.Set("ProductName", product.Name);
            // caption = $"{product.Description} \n\n {product.Requirements}";
            
            FxGetAmount(product.Id);

        }

        return new Dictionary<string, string>
            {
                {"title", title },
                {"caption", caption }
            };
    }*/

    public BotMessageConfig FxGetProduct(BotMessageConfig json)
    {
        int.TryParse(Thread.CurrentMessage.Replace("product ", ""),
            out var id);
        var product = Database.Products.Find(id);

        if (product == default)
        {
            json.Messages = new List<BotMessage> {
            new TextMessage("The product you have specified may have been deleted"),
            new TextMessage("Please check again later")
        };
        }
        else {

            Session.Set("ProductId", product.Id);
            Session.Set("ProductName", product.Name);
            var url = AppendToUrl(product.Image);

            json.Messages = new List<BotMessage>
        { 
            new ImageMessage(url, $"{product.Name} \n\n {product.Description} \n\n ${product.Price}"),
            new ButtonOptionsMessage("", "Would you like to view other products?",
                    new ChatMessageModels.ButtonOption("Products"), new ChatMessageModels.ButtonOption("Menu")),
        };
            
            FxGetAmount(product.Id);

        }
        return json;

    }

    public BotMessageConfig FxGetProductInfo(BotMessageConfig json)
    {
        int.TryParse(Session.GetInteger("ProductId").ToString(), out var id);
        var productInfo = Database.ProductInfos.Where(g => g.Product.Id == id).ToList();
       

        if (productInfo.Count == 0)
        {
            var error = new TextMessage
            {
                Text = "Information you are looking for may have been deleted\n" +
                "Please check again later"
            };
            json.Message = error;
        }

        else
        {
            StringBuilder stringBuilder = new();
            foreach (var product in productInfo)
            {
                stringBuilder.Append(product.Title + "\n");
                stringBuilder.Append(product.Description + "\n\n");
                stringBuilder.Append("Type hi to go back to the main menu");
            }

            var display = new TextMessage
            {
                Text = stringBuilder.ToString(),
            };
            json.Message = display;
        }

        return json;
    }

    public void FxSaveQuantity1(BotMessageConfig json)
    {
        Session.Set("quantity", Thread.CurrentMessage);
        var amount = Session.GetInteger("quantity") * Session.GetDecimal("productPriceAmount");
        Session.Set("quantityAmount", amount);

    }

    public void FxSaveQuantity(BotMessageConfig json)
    {
        Session.Set("quantity", Thread.CurrentMessage);
        var amount = Session.GetInteger("quantity") * Session.GetDecimal("productPriceAmount");
        Session.Set("quantityAmount", amount);
    }

    public void FxSavePhoneNumber(BotMessageConfig json)
    {
        Session.Set("phoneNumber", Thread.CurrentMessage);
    }

    public void FxSaveBranchId(BotMessageConfig json)
    {
        int.TryParse(Thread.CurrentMessage.Replace("nearBranch ", ""),
            out var id);
        var branch = Database.CompanyBranches.Find(id);
        Session.Set("branchId", id);
        Session.Set("branchName", branch.Name);
    }


    public string FxGotoOption(BotThread thread)
    {
        if (thread.CurrentMessage == "Information")
        {
            return Thread.AliasChatMessages["info"].Step.ToString();
        }

        if (thread.CurrentMessage == "Documents")
        {
            return Thread.AliasChatMessages["documents"].Step.ToString();
        }

        if (thread.CurrentMessage == "Quotation")
        {
            return Thread.AliasChatMessages["Quotation"].Step.ToString();
        }

        if (thread.CurrentMessage == "Apply")
            return Thread.AliasChatMessages["applyForm"].Step.ToString();
        if (thread.CurrentMessage == "Purchase")
            return Thread.AliasChatMessages["purchase"].Step.ToString();


        return Thread.AliasChatMessages["menu"].Step.ToString();
    }

    

    [InvalidResponse("Please enter a valid ID number")]
    public bool FxValidateId(string id)
    {
        return IsValidZimID(Thread.CurrentMessage);
    }

    private bool IsValidZimID(string iDNumber)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(iDNumber, @"\d{2}\d{6,8}?[A-Za-z]?\d{2}");
    }

    [InvalidResponse("Please enter a valid phone number")]
    public bool FxValidatePhoneNo(string phoneNo)
    {
        return IsValidZimNo(Thread.CurrentMessage); 
    }

    private bool IsValidZimNo(string phoneNo) 
    {
        return System.Text.RegularExpressions.Regex.IsMatch(phoneNo, @"^[0-9+]+$");
    }

    [InvalidResponse("Please enter a valid amount e.g 200")]
    public bool FxValidateAmount(string amount)
    {
        return IsValidAmount(Thread.CurrentMessage);
    }

    private bool IsValidAmount(string amount)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(amount, @"^\d+$");
    }
    [InvalidResponse("Please enter a valid quantity e.g 2")]
    public bool FxValidateQuantity(string quantity)
    {
        return IsValidQuantity(Thread.CurrentMessage);
    }


    private bool IsValidQuantity(string quantity)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(quantity, @"^\d+$");
    }
    [InvalidResponse("Please enter a valid email e.g example@gmail.com")]
    public bool FxValidateEmail(string email)
    {
        return IsValidEmail(Thread.CurrentMessage);
    }
    
    private bool IsValidEmail(string email)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^\s@]+@[^\s@]+\.[^\s@]+$");
    }

    public BotMessageConfig FxNearestBranches(BotMessageConfig json)
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
                        Title = f.Name.ToEllipsis(20),
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
                            Description = f.PhoneNumber
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


    public void FxSavePaymentMethod(BotMessageConfig config)
    {
        Session.Set("PaymentMethod", Thread.CurrentMessage);
    }

     public void FxSaveAmount(BotMessageConfig config)
     {
         var currentMessage = Thread.CurrentMessage;
         decimal amount = 0;
         if (decimal.TryParse(currentMessage, NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint,
                 CultureInfo.InvariantCulture, out amount))
         {
             Session.Set("Amount", amount);
         }
     }

    public void FxGetAmount(int productId)
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
    
    public string FxGetCustomerPhone(BotThread thread) => Thread.ThreadId;

public string FxIsPhoneNumberEntered(BotThread thread)
{
    bool isPhoneEntered = FileHandler.PhoneNumber.IsLikelyPhoneNumber(Thread.CurrentMessage);
    if (isPhoneEntered)
    {
        return GetStep("subscription/makePayment");
    }
    else
    {
        return GetStep("paymentPhoneInput");
    }
}

    private static bool paymentInitiated = false;

public static bool MakePayment(string phone, decimal amount, string channel)
{
    if (paymentInitiated)
        {
            // Payment has already been initiated, prevent duplicate payments
            return false;
        }

        var paynow = InitializePaynow();
    var payment = CreatePayment(paynow, phone, amount);

        // ste the flag to indicate payment initiation
        paymentInitiated = true;
    
    // Initiate the payment request
        var responseTask = Task.Run(() =>
        paynow.SendMobileAsync(payment, phone, channel));
    var response = responseTask.Result;

    if (response.Success())
    {
            //var link = response.RedirectLink();
            var pollUrl = response.PollUrl();

            // Poll the payment status for 40 seconds (20 attempts with 2-second intervals)
            var pollPaymentTask = Task.Run(() =>
                PollPaymentStatusAsync(paynow, pollUrl));
            bool pollPaymentResult = pollPaymentTask.Result;

            // Reset the flag after payment processing
            paymentInitiated = false;

            return pollPaymentResult;
    }
        else
        {
            // Reset the flag if payment fails
            paymentInitiated = false;
            return false;
        }
    }

private static Webdev.Payments.Payment CreatePayment(Paynow paynow, string phone, decimal amount)
{
    var reference = GenerateReference(phone);
    var authEmail =
        ConfigurationManager.AppSettings["PaynowAuthEmail"];

    var payment = paynow.CreatePayment(reference, authEmail);
    payment.Add("Allied Timbers Payment", amount);
    return payment;
}

private static bool PollPaymentStatus(Paynow paynow, string pollUrl)
{
    var loop = true;
    var isPaid = false;
    var count = 0;
    while (loop && count < 50)
    {
        var paymentStatus = paynow.PollTransaction(
            pollUrl);
        if (paymentStatus.Paid())
        {
            isPaid = true;
            loop = false;
        }

        count++;
        System.Threading.Thread.Sleep(2000);
    }
    if (isPaid)
    {
        return true;
    }
    else
        return false;
    
}

private static async Task<bool> PollPaymentStatusAsync(
    Paynow paynow, string pollUrl)
{
    const int MaxAttempts = 30; // Changed from 60 to a named constant for clarity
    bool isPaid = false;

    for (int attempt = 0; attempt < MaxAttempts; attempt++)
    {
        var paymentStatus = await paynow.PollTransactionAsync(
            pollUrl);

            if (paymentStatus.Paid())
        {
            isPaid = true;
            break;
        }
           

            await Task.Delay(TimeSpan.FromSeconds(2)); // Using Task.Delay instead of Thread.Sleep for async operation     
        }
    return isPaid;
}


private static string GenerateReference(string uniqueId) => $"{uniqueId}({Guid.NewGuid()})";

public static Paynow InitializePaynow()
{
    return new Paynow(
        ConfigurationManager.AppSettings["PaynowId"],
        ConfigurationManager.AppSettings["PaynowIntegration"]);
}
public BotMessageConfig FxMakePayment(BotMessageConfig config)
{
    var amount = Session.GetDecimal("quantityAmount");
    var method = Session.GetString("PaymentMethod");

    var isPaymentSuccessful = MakePayment(
        phone: Thread.CurrentMessage,
        amount: amount, 
        method.ToLower()
        );

        /*BotMessageConfig responseConfig;
        TextMessage text = null;
        ButtonOptionsMessage buttonGroup = null;*/

        // Prepare payment data
        var phone = Thread.ThreadId;
        using var dbContext = new ApplicationDbContext();
        var customer = dbContext.Customers.FirstOrDefault(c => c.PhoneNumber == phone);
        var payment = Session.Build<Payment>();
        payment.CustomerId = customer.Id;
        payment.Date = DateTime.Now;
        payment.BranchName = Session.GetString("branchName");
        payment.Amount = amount;
        payment.PaymentMethod = method;
        payment.PhoneNumber = phone;
        payment.Quantity = Session.GetString("quantity");
        payment.Product = Session.GetString("ProductName");
        payment.PaymentId = Guid.NewGuid().ToString();

        // Save payment based on success or failure
        if (isPaymentSuccessful)
    {
        
        payment.Status = "Successful";
        dbContext.Payments.Add(payment);
        dbContext.SaveChanges();
        
        var buttonGroup = new ButtonOptionsMessage("Payment is successful", "Would you like to return to menu?",
            new ChatMessageModels.ButtonOption("Menu"));
            config.Type = "buttons";
            config.Message = buttonGroup;
        
    }
    else
    {

        payment.Status = "Failed";
        dbContext.Payments.Add(payment);
        dbContext.SaveChanges();
        
       var buttonGroup = new ButtonOptionsMessage("Payment failed", "Would you like to retry?",
            new ChatMessageModels.ButtonOption("Purchase"), new ChatMessageModels.ButtonOption("Menu"));
            config.Type = "buttons";
            config.Message = buttonGroup;
        }
   
    return config;
}
    }

