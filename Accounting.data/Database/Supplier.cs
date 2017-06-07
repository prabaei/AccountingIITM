using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Accounting.data.Database
{
    [Table("SupplierMstr")]
    public class Supplier
    {
        [Key]
        public int Id { get; set; }
        [Index("XI_Name_Supplier")]
        [StringLength(150)]
        [Required(ErrorMessage ="Name Required")]
        public string Name { get; set; }
        [Required(ErrorMessage ="Address1 requierd")]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        [Required(ErrorMessage ="State Required")]
        public string State { get; set; }
        public string District { get; set; }
        public int Country { get; set; }
        public string PIN { get; set; }
        public string BillNO { get; set; }
        public decimal Amount { get; set; }
        public string Projectno { get; set; }
        public string InstituteId { get; set; }
        public string Pan { get; set; }
        public string Tin { get; set; }
        public string Tan { get; set; }
        public int projectType { get; set;}
        public string countryStr { get; set; }
        public List<SelectListItem> CountryList { get; set; }
    }
}
