using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.data.Database
{
    [Table("ProjectTypeTable")]
   public class ProjectType
    {
        [Key]
        public int ID { get; set; }

        [StringLength(15)]
        public string PrjType { get; set; }
    }
}
