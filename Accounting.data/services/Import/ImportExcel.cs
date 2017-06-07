using Accounting.data.Database;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.data.services.Import
{
    public class ImportExcel: IImportExcel
    {

        public  void ImportAccountDetails(Stream stream)
        {
            using (var xlPackage = new ExcelPackage(stream))
            {
                // get the first worksheet in the workbook
                var worksheet = xlPackage.Workbook.Worksheets.FirstOrDefault();
                if (worksheet != null)
                {
                   var range = worksheet.Dimension;
                    
                    int rw = range.End.Row;
                    int col = range.End.Column;

                    for(int row = 6; row <= rw; row++)
                    {
                        using (AccountingDbModel db = new AccountingDbModel())
                        {
                            string accno = Convert.ToString(worksheet.Cells[row, 4].Value != null ? worksheet.Cells[row, 4].Value : "acc");
                           ImprestMaster accde= db.ImprestMasters.FirstOrDefault(m=>m.BankAccountNo== accno);
                            if (accde != null){
                                ////update record
                                accde.InstituteId = Convert.ToString(worksheet.Cells[row, 2].Value == null ? " " : worksheet.Cells[row, 2].Value).Trim();///institute id
                                accde.CoordinatorName = Convert.ToString(worksheet.Cells[row, 3].Value == null ? " " : worksheet.Cells[row, 3].Value).Trim();///Coordinator Name
                               accde.CUSTID= Convert.ToString(worksheet.Cells[row, 5].Value == null ? " " : worksheet.Cells[row, 5].Value).Trim();///customer Id
                                accde.Amount= Convert.ToDecimal(worksheet.Cells[row, 6].Value == null ? " " : worksheet.Cells[row, 6].Value);///amount
                                accde.CardNO = Convert.ToString(worksheet.Cells[row, 7].Value == null ? " " : worksheet.Cells[row, 7].Value).Trim();///card no
                                accde.Email = Convert.ToString(worksheet.Cells[row, 8].Value == null ? " " : worksheet.Cells[row, 8].Value).Trim();//email

                                db.SaveChanges();


                            }
                            else {
                                ////insert new record
                                accde = new ImprestMaster();
                                accde.InstituteId = Convert.ToString(worksheet.Cells[row, 2].Value == null ? " " : worksheet.Cells[row, 2].Value).Trim();///institute id
                                accde.CoordinatorName = Convert.ToString(worksheet.Cells[row, 3].Value == null ? " " : worksheet.Cells[row, 3].Value).Trim();///Coordinator Name
                                accde.BankAccountNo = Convert.ToString(worksheet.Cells[row, 4].Value == null ? " " : worksheet.Cells[row, 4].Value).Trim();///account no
                                accde.CUSTID = Convert.ToString(worksheet.Cells[row, 5].Value == null ? " " : worksheet.Cells[row, 5].Value).Trim();///customer Id
                                accde.Amount = Convert.ToDecimal(worksheet.Cells[row, 6].Value == null ? " " : worksheet.Cells[row, 6].Value);///amount
                                accde.CardNO = Convert.ToString(worksheet.Cells[row, 7].Value == null ? " " : worksheet.Cells[row, 7].Value).Trim();///card no
                                accde.Email = Convert.ToString(worksheet.Cells[row, 8].Value == null ? " " : worksheet.Cells[row, 8].Value).Trim();//email

                                db.ImprestMasters.Add(accde);
                                db.SaveChanges();
                            }
                        }
                    }
                }
                //throw new NopException("No worksheet found");

                //var iRow = 2;


            }
        }
    }
}
