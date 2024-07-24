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
                        Description = (p.Description.ToEllipsis(20),$"${p.Price}").ToString()
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

        return config;
    }
    
    public Dictionary<string,string> FxGetProduct(BotThread thread)
    {
        int.TryParse(Thread.CurrentMessage.Replace("product ", ""),
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

    public void FxSaveName(BotMessageConfig json)
    {
        int.TryParse(Thread.CurrentMessage.Replace("product ", ""),
           out var id);
    
        Session.Set("fullName", Thread.CurrentMessage);
        var product = Database.Products.Find(Session.GetInteger("ProductId"));
        if(product != null)
        {
            // Session.Set("IsLoan", product.IsLoan);
            // Session.Set("IsImageRequired", product.IsImageRequired);
            
        }
    }

    public void FxSaveIdNo(BotMessageConfig json)
    {
        Session.Set("IdNo", Thread.CurrentMessage);
    }
    
    // for loans
    // public void FxSaveAmount(BotMessageConfig json)
    // {
    //     Session.Set("amount", Thread.CurrentMessage);
    // }

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

    public void FxSaveBusinessDescription(BotMessageConfig json)
    {
        Session.Set("descripton", Thread.CurrentMessage);
    }

    public Dictionary<string, string> FxPreviewFormData(BotThread thread)
    {
        var fullName = Session.GetString("fullName");
        var phoneNumber = Session.GetString("phoneNumber");
        var IdNo = Session.GetString("IdNo");

        var branchName = Database.CompanyBranches.Find(Session.GetInteger("branchId"));

        Session.Set("branchName", branchName.Name);

        var  content = $"Name: {fullName}\n" +
            $"Phone Number: {phoneNumber}\n" +
            $"ID No: {IdNo}" +
            $"Nearest Town: {branchName.Name}";

        // if (Session.GetBool("IsLoan") && Session.GetBool("IsImageRequired"))
        // { 
        //     return new Dictionary<string, string>
        //     {
        //         {"caption", $"{content}\nAmount: {Session.GetDecimal("amount")}"}
        //     };
        // }

        // if(Session.GetBool("IsLoan"))
        // {
        //     string caption = $"Amount: {Session.GetDecimal("amount")}\n" +
        //         $"Description: {Session.GetString("descripton")}";
        //     return new Dictionary<string, string>
        //     {
        //         {"caption", $"{content}\n{caption}"}
        //     };
        // }

        return new Dictionary<string, string>
        {
            {"caption",$"{content}\nCashPlan: {Session.GetString("cashPlan")}"  }
        };
    }

    public BotMessageConfig FxSaveFormData(BotMessageConfig config)
    {
        var product = Database.Products.Find(Session.GetInteger("ProductId"));

        LoanApplication loanApplication = new();
        loanApplication.CustomerName = Session.GetString("fullName");;
        loanApplication.PhoneNo = Session.GetString("phoneNumber");;
        loanApplication.DateApplied = DateTime.Now;
        loanApplication.IdNo = Session.GetString("IdNo");
        loanApplication.SelfiePath = Session.GetString("Selfie");
        loanApplication.BranchName = Session.GetString("branchName");

        loanApplication.ProductName = product.Name;
        
        loanApplication.LoanApproval = LoanApproval.Pending;

        // if (Session.GetBool("IsLoan") && Session.GetBool("IsImageRequired"))
        // {
        //     loanApplication.Amount = Session.GetDecimal("amount");
        //     loanApplication.FilePath = Session.GetString("PaySlip");
        //     //loanApplication.SalaryType = Session.GetString("SalaryType");
        //
        //     Database.LoanApplications.Add(loanApplication);
        //     Database.SaveChanges();
        //     return config;
        // }

        // if (Session.GetBool("IsLoan"))
        // {
        //     loanApplication.Amount = Session.GetDecimal("amount");
        //     loanApplication.Description = Session.GetString("descripton");
        //
        //     Database.LoanApplications.Add(loanApplication);
        //     Database.SaveChanges();
        //     return config;
        // }

        loanApplication.CashPlan = Session.GetString("cashPlan");

        Database.LoanApplications.Add(loanApplication);
        Database.SaveChanges();
        return config;
    }

    public void FxVerificationPic(BotMessageConfig json)
    {
        if(!json.MetaData?.Contains("Picture") ?? false)
        {
            Session.Set(json.MetaData, Thread.CurrentMessage);
            return;
        }

        var downloadKey = json.MetaData?.Split(',').Last();
        var downloadUrl = Thread.CurrentMessage;

        Session.Set(downloadKey, downloadUrl);
        //var fileUrl = FilesHandler.Download(Thread.CurrentMessage);

        //Session.Set("pictureUrl", fileUrl);
    }

    public void FxSelfiePic(BotMessageConfig json)
    {
        if (!json.MetaData?.Contains("Picture") ?? false)
        {
            Session.Set(json.MetaData, Thread.CurrentMessage);
            return;
        }

        var downloadKey2 = json.MetaData?.Split(',').Last();
        var downloadUrl2 = Thread.CurrentMessage;
       
        Session.Set(downloadKey2, downloadUrl2);
    }

    // public void FxSaveCashPan(BotMessageConfig json)
    // {
    //     Session.Set("cashPlan", Thread.CurrentMessage);
    // }

    public void FxSaveBranchId(BotMessageConfig json)
    {
        int.TryParse(Thread.CurrentMessage.Replace("nearBranch ", ""),
            out var id);
        var branch = Database.CompanyBranches.Find(id);
        Session.Set("branchId", id);
        Session.Set("branchName", branch.Name);
    }

    //public Dictionary<string, string> FxSendFile(BotThread thread)
    //{
    //    int.TryParse(Session.GetInteger("ProductId").ToString(), out var id);
    //    var productInfos = Database.ProductInfos.Where(c => c.Product.Id == id).ToList();


    //    if (productInfos.Count == 0)
    //    {
    //        return new Dictionary<string, string>
    //        {
    //            {"title",  "Not found" },
    //            {"caption",  "This product has no files associated with it "}
    //        };
    //    }

    //        StringBuilder stringBuilder = new();

    //        foreach (var productInfo in productInfos)
    //        {
    //            if (productInfo.FilePath != default)
    //            {
    //                foreach (var productFile in productInfos)
    //                {
    //                    SendDocumentMessage(productFile);
    //                }
    //            }

    //        }

    //    return new Dictionary<string, string>
    //        {
    //            {"title", "" },
    //            {"caption", "" }
    //        };
    //}

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

        if (thread.CurrentMessage == "Apply")
            return Thread.AliasChatMessages["applyForm"].Step.ToString();
        if (thread.CurrentMessage == "Purchase")
            return Thread.AliasChatMessages["purchase"].Step.ToString();

        if (thread.CurrentMessage == "Files")
            return Thread.AliasChatMessages["files"].Step.ToString();

         if (Session.GetBool("IsLoan"))
             return Thread.AliasChatMessages["amount"].Step.ToString();

         if (!Session.GetBool("IsLoan"))
             return Thread.AliasChatMessages["cashPlan"].Step.ToString();

        return Thread.AliasChatMessages["menu"].Step.ToString();
    }

    public string FxFilterLoanType(BotThread thread)
    {
        if(Session.GetBool("IsImageRequired"))
            return Thread.AliasChatMessages["postImage"].Step.ToString();
      
        if(!Session.GetBool("IsImageRequired"))
            return Thread.AliasChatMessages["businessDescription"].Step.ToString();
        
        return Thread.AliasChatMessages["menu"].Step.ToString(); ;
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

    //private void SendDocumentMessage(ProductInfo productFile)
    //{
    //    if (string.IsNullOrEmpty(productFile.FilePath)) return;

    //    var fileUrl = AppendToUrl(productFile.FilePath);
    //    var whatsAppService = new WhatsappCloudChatService(BotConfig.WhatsappCloudPhoneNumberId,
    //        BotConfig.WhatsappCloudAccessToken, new AlliedTimbersBot());
    //    whatsAppService.Send(Thread.ThreadId, new DocumentMessage(fileUrl, $"{productFile.Title}"));
    //}

    //private string AppendToUrl(string appendString)
    //{
    //    string siteUrl = Current.Request.Url.GetLeftPart(UriPartial.Authority);
    //    return siteUrl + appendString;
    //}

    public void FxSavePaymentMethod(BotMessageConfig config)
    {
        Session.Set("PaymentMethod", Thread.CurrentMessage);
    }

    // public void FxSaveAmount(BotMessageConfig config)
    // {
    //     var currentMessage = Thread.CurrentMessage;
    //     decimal amount = 0;
    //     if (decimal.TryParse(currentMessage, NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint,
    //             CultureInfo.InvariantCulture, out amount))
    //     {
    //         Session.Set("Amount", amount);
    //     }
    // }

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

public static bool MakePayment(string phone, decimal amount, string channel)
{
    var paynow = InitializePaynow();
    var payment = CreatePayment(paynow, phone, amount);
    
    var responseTask = Task.Run(() =>
        paynow.SendMobileAsync(payment, phone, channel));
    var response = responseTask.Result;

    if (response.Success())
    {
        var link = response.RedirectLink();
        var pollUrl = response.PollUrl();

        var pollPaymentTask = Task.Run(() =>
            PollPaymentStatusAsync(paynow, pollUrl));
        bool pollPaymentResult = pollPaymentTask.Result;
        return pollPaymentResult;
    }
    else
    {
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
    while (loop && count < 60)
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
        Payment.Quantity = Session.GetString("quantity");
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
        Payment.Quantity = Session.GetString("quantity");
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
    }

