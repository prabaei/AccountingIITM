namespace Accounting.data.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ImprestMaster")]
    public partial class ImprestMaster
    {
        
        [StringLength(4)]
        public string InstituteId { get; set; }
        [StringLength(150)]
        public string CoordinatorName { get; set; }
        [StringLength(15)]
        public string Department { get; set; }
        [StringLength(150)]
        public string Designation { get; set; }
        [Key]
        [StringLength(14)]
        public string BankAccountNo { get; set; }
        [StringLength(9)]
        public string CUSTID { get; set; }
        [StringLength(20)]
        public string CardNO { get; set; }
        public decimal Amount { get; set; }
        public decimal? Limit { get; set; }
        [StringLength(80)]
        public string Email { get; set; }
    }
}
