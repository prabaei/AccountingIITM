namespace Accounting.data.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("VoucherType")]
    public partial class VoucherType
    {
        [Key]
        public int TypeId { get; set; }

        [StringLength(80)]
        public string VoucherTypeName { get; set; }

        public bool deposit { get; set; }

        public bool ForEntry { get; set; }

        public bool Brs { get; set; }

        public bool cns { get; set; }

        public bool recoup { get; set; }

        public bool cq { get; set; }

        public bool creditoFacc { get; set; }

        public bool depositdd { get; set; }

        public bool wddd { get; set; }
    }
}
