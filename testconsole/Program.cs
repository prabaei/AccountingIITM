using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accounting.data.Database;

namespace testconsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("creating database");
            AccountingDbModel db = new AccountingDbModel();
            //db.ImprestMasters.Add(new ImprestMaster()
            //{
            //    AmountPaid = 52,
            //    CoorName = "dfd",
            //    BankAccountNumber = "adf",
            //    DateOfDrawn = DateTime.Now,
            //    Department="che",
            //    InstituteId="33",
            //    Limit="34",
            //    VoucherNo="dfd",
            //    ProjectNo="dfd",
            //    ProjectStartDate=DateTime.Now,
            //    ProjectEndDate=DateTime.Now
            
            //});
            db.SaveChanges();
           var impre = db.Database.SqlQuery<ImprestMaster>("select * from ImprestMaster").ToList();    
        }
    }
}
