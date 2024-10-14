using Mugonat.Chat.BotEngine.Messages.Internal;
using Mugonat.Chat.BotEngine.Messages;
using Mugonat.Chat.BotEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlliedTimbers.Models;
using AlliedTimbers.Controllers;
using System.Threading;
using Mugonat.Chat.BotEngine.Attributes;
using System.Linq.Dynamic.Core;
using Faker;
using Mugonat.Chat.BotEngine;

namespace AlliedTimbers.Bot.Context;

    public partial class AlliedTimbersBotContext
    {
      public BotMessageConfig FxGetAccount(BotMessageConfig json)
      {
          var list = (ListMenuMessage)json.Message;

          list.Items = new List<ChatMessageModels.ListSection>
              {
                  new()
                  {
                      Title = "Account Management",
                      Options = new List<ChatMessageModels.ListOption>
                      {
                          new("Register", "Create a new account", "register/account"),
                          new("Update/Verify", "Modify your existing account", "update/verify"),
                          new("Delete", "Remove your account", "delete/account")
                      }
                  },
              };

          return json;
      }

    public void FxSaveName(BotMessageConfig config)
    {
        Session.Set("Name", Thread.CurrentMessage);
    }

    public void FxSaveSurname(BotMessageConfig config)
    {
        Session.Set("Surname", Thread.CurrentMessage);
    }

    public void FxSaveEmail(BotMessageConfig config)
    {
        Session.Set("Email", Thread.CurrentMessage);
    }

    public void FxSaveRegion(BotMessageConfig config)
    {
        Session.Set("Region", Thread.CurrentMessage);
    }

    public BotMessageConfig FxSaveRegistrationInfomation(BotMessageConfig config)
    {

        var threadId = Thread.ThreadId;
        using var dbContext = new ApplicationDbContext();
        var customer = dbContext.Customers.FirstOrDefault(c => c.PhoneNumber == threadId);
        
        // Check if an account already exists for the given CustomerId
        var existingAccount = dbContext.Accounts.FirstOrDefault(a => a.CustomerId == customer.Id);
        if (existingAccount != null)
        {
            // Return or print a message indicating that the user already exists
            config.Messages = CreateAccountExistsTextMessageTextMessage();
            return config;
            //return GetStep("register/already");
        }

        else 
        { 

            var account = new Account
            {    
               Name = Session.Get<string>("Name"),
               Surname = Session.Get<string>("Surname"),
               Email = Session.Get<string>("Email"),
               Region = Session.Get<string>("Region"),
               CustomerId = customer.Id,
               CreatedAt = DateTime.Now
            };

            Database.Accounts.Add(account);
            Database.SaveChanges();

            config.Messages = CreateAccountTextMessageTextMessage();
            return config;
            //return GetStep("register/save");

        }
    }


    private List<BotMessage> CreateAccountExistsTextMessageTextMessage()
    {
        return new List<BotMessage>()
            {
                new TextMessage("User already exists, type hi to return to menu")
            };
    }

    private List<BotMessage> CreateAccountTextMessageTextMessage()
    {
        return new List<BotMessage>()
        {
            new TextMessage("✅ You have created your account successfully")
        };
    }

    private List<BotMessage> UpdateAccountTextMessageTextMessage()
    {
        return new List<BotMessage>()
        {

        };
    }

    /*public BotMessageConfig FxGetAccountDetails(BotMessageConfig config)
    {

        var threadId = Thread.ThreadId;
        using var dbContext = new ApplicationDbContext();
        var customer = dbContext.Customers.FirstOrDefault(c => c.PhoneNumber == threadId);

        // Check if an account already exists for the given CustomerId
        var existingAccount = dbContext.Accounts.FirstOrDefault(a => a.CustomerId == customer.Id);
        if (existingAccount != null)
        {
            // Return or print a message indicating that the user already exists
            
            config.Messages = CreateAccountExistsTextMessageTextMessage();
            return config;
            //return GetStep("register/already");
        }

        else
        {

            var account = new Account
            {
                Name = Session.Get<string>("Name"),
                Surname = Session.Get<string>("Surname"),
                Email = Session.Get<string>("Email"),
                Region = Session.Get<string>("Region"),
                CustomerId = customer.Id,
                CreatedAt = DateTime.Now
            };

            Database.Accounts.Add(account);
            Database.SaveChanges();

            config.Messages = CreateAccountTextMessageTextMessage();
            return config;
            //return GetStep("register/save");

        }

    }*/

    //

    public string FxCheckAccountDetails(BotThread thread)
    {
        var threadId = Thread.ThreadId;
        using var dbContext = new ApplicationDbContext();
        var customer = dbContext.Customers.FirstOrDefault(c => c.PhoneNumber == threadId);
        var existingAccount = dbContext.Accounts.FirstOrDefault(a => a.CustomerId == customer.Id);

        if (existingAccount != default)
        {
            return GetStep("account/view/details");
        }
        else
        {
            return GetStep("account/details/missing");
        }
    }
    public Dictionary<string, string> FxGetAccountDetails(BotThread thread)
    {

        var threadId = Thread.ThreadId;
        using var dbContext = new ApplicationDbContext();
        var customer = dbContext.Customers.FirstOrDefault(c => c.PhoneNumber == threadId);

        // Check if an account already exists for the given CustomerId
        var existingAccount = dbContext.Accounts.FirstOrDefault(a => a.CustomerId == customer.Id);
        //if (existingAccount != null)
        //{ }
            

        string title = "", caption = "";

        if (existingAccount == default)
        {
            title = "No valid account found";
            caption = "Please register your account. Type hi to return to menu";

        }
        else
        {
            
            title = $"Please confirm your account details below: \n\n";
            
            caption = $"We have your details as follows: \n\n" +
                      $"Name: {existingAccount.Name} \n\n" +
                      $"Surname: {existingAccount.Surname} \n\n" +
                      $"Email: {existingAccount.Email} \n\n " +
                      $"Region: {existingAccount.Region}";
        }

        return new Dictionary<string, string>
        {
            {"title", title },
            {"caption", caption }
        };
    }
    //

    public string FxDeleteAccount(BotMessageConfig config)
    {
        var threadId = Thread.ThreadId;
        using var dbContext = new ApplicationDbContext();

        // Find the customer by PhoneNumber
        var customer = dbContext.Customers.FirstOrDefault(c => c.PhoneNumber == threadId);
        if(customer == null)
        {
            // Handle if customer is not found
            
            return GetStep("register/doesntExist");
        }

        // Check if an account already exists for the given CustomerId
        var existingAccount = dbContext.Accounts.FirstOrDefault(a => a.CustomerId == customer.Id);

        if (existingAccount != null)
        {
            // If account exists, delete it
            dbContext.Accounts.Remove(existingAccount);
            dbContext.SaveChanges();

            return GetStep("register/deleted");
        }

        else
        {
            // If no account found
            return GetStep("register/doesntExist");
        }
        
    }


    public void FxUpdateName(BotMessageConfig config)
    {
        Session.Set("updateName", Thread.CurrentMessage);
    }

    public void FxUpdateSurname(BotMessageConfig config)
    {
        Session.Set("updateSurname", Thread.CurrentMessage);
    }

    public void FxUpdateEmail(BotMessageConfig config)
    {
        Session.Set("updateEmail", Thread.CurrentMessage);
    }

    public void FxUpdateRegion(BotMessageConfig config)
    {
        Session.Set("updateRegion", Thread.CurrentMessage);
    }

    public void FxUpdateRegistrationInfomation(BotMessageConfig config)
    {

        var threadId = Thread.ThreadId;
        using var dbContext = new ApplicationDbContext();
        var customer = dbContext.Customers.FirstOrDefault(c => c.PhoneNumber == threadId);

        // Check if an account already exists for the given CustomerId
        var existingAccount = dbContext.Accounts.FirstOrDefault(a => a.CustomerId == customer.Id);
        if (existingAccount != null)
        {

            existingAccount.Name = Session.Get<string>("updateName");
            existingAccount.Surname = Session.Get<string>("updateSurname");
            existingAccount.Email = Session.Get<string>("updateEmail");
            existingAccount.Region = Session.Get<string>("updateRegion");
            existingAccount.UpdatedAt = DateTime.Now;
            
            Database.Entry(existingAccount).State = System.Data.Entity.EntityState.Modified;
            Database.SaveChanges();
            
        }

        else
        {

        }
    }


}


