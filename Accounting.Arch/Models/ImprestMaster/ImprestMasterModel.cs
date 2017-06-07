using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Accounting.Architecture.Models
{
    public class ImprestMasterModel
    {
        public string InstituteId { get; set; }

        [Display(Name ="Coordinator Name")]
        public string CoorName { get; set; }

        public decimal? PaymentNo { get; set; }

        public DateTime? PaymentStartDate { get; set; }

        public DateTime? PaymentEndDate { get; set; }

        public decimal? AmountPaid { get; set; }

        public DateTime? DateOfDrawn { get; set; }

        public DateTime? ImprestCloseDate { get; set; }

        public decimal? AmountRefund { get; set; }

        public decimal? BankAccountNumber { get; set; }

        public decimal? Limit { get; set; }
    }
}