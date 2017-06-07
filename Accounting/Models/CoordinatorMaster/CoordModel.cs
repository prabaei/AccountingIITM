using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Accounting.Models.CoordinatorMaster
{
    public class CoordModel
    {

        public bool showcoordetail { get; set; }
        public bool showprojectdetail { get; set; }

        public Search search { get; set; }
        public string Designation { get; set; }
        public string Name { get; set; }
        [Display(Name = "IIRNO")]
        [Required(ErrorMessage = "IIRNO is required")]
        public string IIRNO { get; set; }
        public string Department { get; set; }
        public string ProjectNo { get; set; }

        public string AccountNo { get; set; }
    }

    public class projectList
    {
        public string NPRNO { get; set; }

        public string STARTDATE { get; set; }

        public string CLOSEDATE { get; set; }
        public DateTime? START_DATE
        {

            set
            {
                if (value == null)
                {
                    this.STARTDATE = string.Empty;
                }
                else
                {
                    DateTime dt = Convert.ToDateTime(value);
                    STARTDATE = dt.ToShortDateString();
                }
            }
        }
        public DateTime? CLOSE_DATE
        {
            set
            {
                if (value == null)
                {
                    this.CLOSEDATE = string.Empty;
                }
                else
                {
                    DateTime dt = Convert.ToDateTime(value);
                    CLOSEDATE = dt.ToShortDateString();
                }
            }
        }

    }
    public class Search
    {
        
        private string _IIRONO;
        [Display(Name = "IIRNO")]
        public string IIRNO { get {return _IIRONO; }
            set {
                _IIRONO = value;
                setModelstate();
            }
        }

        private string _Name;
        [Display(Name = "Name")]
        public string Name { get { return _Name; } set { _Name = value;setModelstate(); } }

        public string Department { get; set; }
        [Required(ErrorMessage ="Any one of the above field is required")]
        public string valid { get; set; }
        public void setModelstate()
        {
            if(!string.IsNullOrEmpty(IIRNO) || !string.IsNullOrEmpty(Name))
            {
                valid = "hasvalue";
            }
        }
    }

    public class gridMdl
    {
        public string Name { get; set; }
        public string Dept { get; set; }
        public string IIRNO { get; set; }
    }
}