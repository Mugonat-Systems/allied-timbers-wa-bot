using System;
using System.Collections.Generic;
using System.Linq;
using AlliedTimbers.Models;
using Mugonat.Chat.BotEngine;
using Mugonat.Utils.Extensions.Extensions;

namespace AlliedTimbers.Bot.Context;

public partial class AlliedTimbersBotContext : BotExecutionContext, IDisposable
{
    private readonly Dictionary<string, Customer> _customer = new();
    public readonly ApplicationDbContext Database = new();

    public Customer Customer
    {

        get
        {
            if (_customer.TryGetValue(Thread.ThreadId, out var customer) && customer != null)
                return customer;

            customer = _customer[Thread.ThreadId] =
                Database.Customers.FirstOrDefault(g => g.PhoneNumber == Thread.ThreadId);

            if (customer == null)
            {
                customer = new Customer
                {
                    PhoneNumber = Thread.ThreadId,
                    LastAvailableOn = DateTime.Now,
                    IsRegistered = 1,
                    Name = Session.GetString("customerName")
                };
                
                Database.Customers.Add(customer);

                Database.SaveChanges();
            }

            return customer;
        }
    }

    public void Dispose()
    {
        Database.Dispose();
    }

    public override Dictionary<string, string> GlobalTemplate()
    {
        var isMorning = DateTime.Today.AddHours(12) >= DateTime.Now;
        var isAfternoon = !isMorning && DateTime.Today.AddHours(18) >= DateTime.Now;
        
        if (Customer.IsRegistered < 2)
        {
            Customer.IsRegistered = 2;
            Session.Set("registered", "Welcome to");
            Database.SaveChanges();
        }
        else
        {
            Session.Set("registered", "Welcome back to");
        }

        return (Dictionary<string, string>)base.GlobalTemplate().MergeDictionary(new Dictionary<string, string>
        {
            { "user", Session.GetString("customerName") ?? Thread.ThreadId },
            {
                "timeOfDay", 0 switch
                {
                    0 when isMorning => "Good morning",
                    0 when isAfternoon => "Good afternoon",
                    _ => "Good evening"
                }
            },
            {"greet", Session.GetString("registered")}
        });
    }
}