using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.data.Database
{
    [Table("Countries")]
    public class Countries
    {
        [Key]
        public int Id { get; set; }
        [StringLength(60)]
        public string Country { get; set; }
    }
}
