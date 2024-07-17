using System;
using System.ComponentModel;

namespace AlliedTimbers.Models
{
    public enum LoanApproval
    {
        Approved, Pending, Cancelled
    }
    public class LoanApplication
    {
        public int Id { get; set; }
        [DisplayName("Customer Name")]
        public string CustomerName { get; set; }
        public decimal Amount { get; set; }
        [DisplayName("National ID Number")]
        public string IdNo { get; set; }
        [DisplayName("Phone Number")]
        public string PhoneNo { get; set; }
        [DisplayName("Approval Status")]
        public LoanApproval LoanApproval { get; set; }
        [DisplayName("Date Applied")]
        public DateTime DateApplied { get; set; } = DateTime.Now;

        [DisplayName("Payslip Image")]
        public string FilePath { get; set; }
        [DisplayName("Cash Plan")]
        public string CashPlan { get; set; }
        public string Description { get; set; }
        public string ProductName { get; set; }
        //public string SalaryType { get; set; }
        [DisplayName("Customer Photo")]
        public string SelfiePath { get; set; }

        public string BranchName { get; set; }

       
        //public int ProductId { get; set; }
        //[Required]
        //public virtual Product Product { get; set; }

    }
}