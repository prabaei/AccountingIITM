using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using PagedList.Mvc;
namespace Accounting.Models
{
    //public class ImprestMasterModel
    //{
    //    [Display(Name = "AprlNo")]
    //    public string ProjectNo { get; set; }
    //    [Display(Name = "Project Start Date")]
    //    public string ProjectStartDate { get; set; }
    //    [Display(Name = "Project Close Date")]
    //    public string ProjectEndDate { get; set; }
    //    [Display(Name = "AmountPaid")]
    //    public decimal? AmountPaid { get; set; }
    //    [Display(Name = "DateOfDrawn")]
    //    public DateTime? DateOfDrawn { get; set; }
    //    [Display(Name = "ImprestCloseDate")]
    //    public DateTime? ImprestCloseDate { get; set; }
    //    [Display(Name = "Amount Refund")]
    //    public decimal? AmountRefund { get; set; }
    //}
    public class ImprestMdl
    {
        public ProjectMdl projectmodel { get; set; }
        
       // public ImprestCordinatorDetails cordDetails { get; set; }
  
        public coordmodel cordmodel { get; set; }
        
        public ImprestProjectDetails imprestprojectdetails { get; set; }
      
        public paymentDetails paymentdetails { get; set; }
        public PaymentMdl paymentmodel { get; set; }
        public AccountMdl accountdetail { get; set; }

       
       
    }
    public class coordmodel
    {
        
        public ImprestCordinatorDetails cordDetails { get; set; }
        public List<ImprestCordinatorDetails> Imprestlist { get; set; }
    }
    public class ImprestCordinatorDetails
    {
        [Display(Name = "InstituteId")]
        [Required(ErrorMessage = "IIRNO is required")]
        public string IIRNO { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Department")]
        public string Department { get; set; }
        //public List<ImprestCordinatorDetails> Imprestlist { get; set; }
    }
    public class ImprestProjectDetails
    {
        [Display(Name = "Project No")]
        public string ProjectNo { get; set; }
        [Display(Name = "Project Start Date")]
        public DateTime? ProjectStartDate { get; set; }
        [Display(Name = "Project Close Date")]
        public DateTime? ProjectCloseDate { get; set; }
    }
    public class ProjectMdl
    {
        public List<ImprestProjectDetails> ProjectList { get; set; }
        [Required(ErrorMessage = "Project No is required")]
        public string projectNo { get; set; }
    }
    public class paymentDetails
    {
        public string VoucherNo { get; set; }
        public DateTime? dateofDrawn { get; set; }
        public double? AmountPayed { get; set; }
    }

    public class PaymentMdl
    {
        public List<paymentDetails> paymentList { get; set; }
        [Required(ErrorMessage = "Voucher No Required")]
        public string VoucherNo { get; set; }
    }

    public class AccountMdl
    {
        [Display(Name = "Bank Account Number")]
        [Required(ErrorMessage ="Account No Required")]
        public string BankAccountNumber { get; set; }
        [Display(Name = "Limit")]
        public string Limit { get; set; }

        public string conf { get; set; }
    }
}