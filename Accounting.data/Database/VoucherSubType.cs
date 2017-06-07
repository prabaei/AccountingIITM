namespace Accounting.data.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("VoucherSubType")]
    public partial class VoucherSubType
    {
        [Key]
        public int SubTypeID { get; set; }

        public int? VoucherTypeId { get; set; }

        [StringLength(100)]
        public string VoucherType { get; set; }
    }
}
