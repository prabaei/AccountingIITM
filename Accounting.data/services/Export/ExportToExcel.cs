using Accounting.data.Database;
using OfficeOpenXml;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.data.services.Export
{
    public class ExportToExcel : IExportToExcel
    {
        public byte[] ExportAccountDetails(IEnumerable<ImprestMaster> Accdetail)
        {
            using (var stream = new MemoryStream())
            {
                // ok, we can run the real code of the sample now
                using (var xlPackage = new ExcelPackage(stream))
                {
                    // uncomment this line if you want the XML written out to the outputDir
                    //xlPackage.DebugMode = true; 
                    int row = 5;
                    // get handle to the existing worksheet
                    var worksheet = xlPackage.Workbook.Worksheets.Add("AccountDetails");
                    //create Headers and format them 
                    worksheet.Cells[row, 1].Value = "SlNo";
                    worksheet.Cells[row, 2].Value = "Institute Id";
                    worksheet.Cells[row, 3].Value = "CoordinatorName";
                    worksheet.Cells[row, 4].Value = "AccountNo";
                    worksheet.Cells[row, 5].Value = "CUSTID";
                    worksheet.Cells[row, 6].Value = "Amount";
                    worksheet.Cells[row, 7].Value = "CardNO";
                    worksheet.Cells[row, 8].Value = "Email";
                    int slNO = 1;
                    row += 1;
                    foreach (var acc in Accdetail)
                    {
                        worksheet.Cells[row, 1].Value = slNO;
                        worksheet.Cells[row, 2].Value = acc.InstituteId;
                        worksheet.Cells[row, 3].Value = acc.CoordinatorName;
                        worksheet.Cells[row, 4].Value = acc.BankAccountNo;
                        worksheet.Cells[row, 5].Value = acc.CUSTID;
                        worksheet.Cells[row, 6].Value = acc.Amount;
                        worksheet.Cells[row, 7].Value = acc.CardNO;
                        worksheet.Cells[row, 8].Value = acc.Email;
                        row++;
                        slNO++;
                    }
                    worksheet.Cells["A:I"].AutoFitColumns();
                    xlPackage.Save();
                }
                return stream.ToArray();


                // public const string TextXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //return File(bytes, MimeTypes.TextXlsx, "categories.xlsx");

            }
        }

        public byte[] ExportClimbNS(AccountTrans transdetail)
        {
            using (var stream = new MemoryStream())
            {
                int startrow = 1;
                int row = 0;
                List<TransModel> orlist = transdetail.accoutnTransaction.OrderBy(m => m.BankAccountNO).ToList();
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("Claim Not Submitted");
                    while (orlist.Count > 0)
                    {
                        startrow = row+3;
                        // uncomment this line if you want the XML written out to the outputDir
                        //xlPackage.DebugMode = true; 
                        worksheet.Cells[string.Format("A{0}:C{0}", startrow)].Merge = true; 
                        worksheet.Cells[string.Format("A{0}:C{0}", startrow+1)].Merge = true;
                        worksheet.Cells[string.Format("A{0}:C{0}", startrow+2)].Merge = true;
                        worksheet.Cells[startrow+2, 1].Value = "AccountNO :" + Convert.ToString(orlist.Select(m=>m.BankAccountNO).FirstOrDefault());
                        //worksheet.Cells[1, 2].Value = Convert.ToString(transdetail.AccountNO);
                        worksheet.Cells[startrow + 1, 1].Value = orlist.Select(m => m.CoordinatorName).FirstOrDefault();
                        worksheet.Cells[startrow + 1, 1].Style.Font.Bold = true;
                        worksheet.Cells[startrow + 1, 1].Style.Font.Size = 16;
                        worksheet.Cells[startrow + 2, 1].Style.Font.Bold = true;
                        worksheet.Cells[startrow + 2, 1].Style.Font.Size = 15;
                        // get handle to the existing worksheet

                        startrow += 5;
                        worksheet.Cells[string.Format("A{0}:C{0}", startrow)].Style.Font.Bold = true;
                        worksheet.Cells[string.Format("A{0}:C{0}", startrow)].Style.Font.Size = 12;
                         

                        worksheet.Cells[startrow, 2].Value = "Date";
                        worksheet.Cells[startrow, 3].Value = "ProjectNo";
                        worksheet.Cells[startrow, 4].Value = "Remarks";
                        worksheet.Cells[startrow, 5].Value = "Voucher Type";
                        worksheet.Cells[startrow, 6].Value = "voucherNo";
                        worksheet.Cells[startrow, 7].Value = "CommitmentNo";
                        worksheet.Cells[startrow, 8].Value = "Cheque/ref";
                        worksheet.Cells[startrow, 9].Value = "Debit";
                        worksheet.Cells[startrow, 10].Value = "Credit";
                        worksheet.Cells[startrow, 11].Value = "Balance";
                        //create Headers and format them 
                        //int slNO = 1;
                         row = startrow + 2;
                        var rowdata = orlist.FirstOrDefault();
                        foreach (var acc in orlist.Where(m => m.BankAccountNO == rowdata.BankAccountNO).ToList())
                        {
                            worksheet.Cells[row, 2].Value = acc.bnkdate;
                            worksheet.Cells[row, 3].Value = acc.ProjectNo;
                            worksheet.Cells[row, 4].Value = acc.Remarks;
                            worksheet.Cells[row + 1, 3].Value = "Bank Info";
                            worksheet.Cells[row + 1, 4].Value = acc.bankPart;
                            worksheet.Cells[row, 5].Value = acc.voucherTypeStr;
                            worksheet.Cells[row, 6].Value = acc.voucherNo;
                            worksheet.Cells[row, 7].Value = acc.CommitmentNO;
                            worksheet.Cells[row, 8].Value = acc.ChequeNO;
                            if (acc.voucherType == 1 || acc.voucherType == 4 || acc.voucherType == 11 || acc.voucherType == 14 || acc.voucherType == 8)
                            {
                                worksheet.Cells[row, 9].Value = acc.Amount;
                            }
                            else
                            {
                                worksheet.Cells[row, 10].Value = acc.Amount;
                            }
                            worksheet.Cells[row, 11].Value = acc.currentBal;

                            ///worksheet.Cells[row, 9].Value = acc.narration;
                            //worksheet.Cells[row, 10].Value = acc.Remarks;

                            row = row + 3;
                            //slNO++;
                        }
                        orlist.RemoveAll(m => m.BankAccountNO == rowdata.BankAccountNO);

                    }
                    worksheet.Cells["A:K"].AutoFitColumns();
                    xlPackage.Save();
                }
                return stream.ToArray();

            }
        }

        public byte[] ExportTransaction(AccountTrans transdetail)
        {
            using (var stream = new MemoryStream())
            {
                // ok, we can run the real code of the sample now
                using (var xlPackage = new ExcelPackage(stream))
                {
                    var worksheet = xlPackage.Workbook.Worksheets.Add("TransactionDetails");
                    // uncomment this line if you want the XML written out to the outputDir
                    //xlPackage.DebugMode = true; 
                    worksheet.Cells["A1:C1"].Merge = true;
                    worksheet.Cells["A2:C2"].Merge = true;
                    worksheet.Cells["A3:C3"].Merge = true;
                    worksheet.Cells[3, 1].Value = "AccountNO :" + Convert.ToString(transdetail.AccountNO);
                    //worksheet.Cells[1, 2].Value = Convert.ToString(transdetail.AccountNO);
                    worksheet.Cells[2, 1].Value = transdetail.coordnatorName;
                    worksheet.Cells[2, 1].Style.Font.Bold = true;
                    worksheet.Cells[2, 1].Style.Font.Size = 16;
                    worksheet.Cells[3, 1].Style.Font.Bold = true;
                    worksheet.Cells[3, 1].Style.Font.Size = 15;
                    // get handle to the existing worksheet


                    worksheet.Cells["A5:K5"].Style.Font.Bold = true;
                    worksheet.Cells["A5:C5"].Style.Font.Size = 12;
                    int startrow = 5;

                    worksheet.Cells[startrow, 2].Value = "Date";
                    worksheet.Cells[startrow, 3].Value = "ProjectNo";
                    worksheet.Cells[startrow, 4].Value = "Particular";
                    worksheet.Cells[startrow, 5].Value = "Voucher Type";
                    worksheet.Cells[startrow, 6].Value = "voucherNo";
                    worksheet.Cells[startrow, 7].Value = "CommitmentNo";
                    worksheet.Cells[startrow, 8].Value = "Cheque/ref";
                    worksheet.Cells[startrow, 9].Value = "Debit";
                    worksheet.Cells[startrow, 10].Value = "Credit";
                    worksheet.Cells[startrow, 11].Value = "Balance";
                    //create Headers and format them 
                    //int slNO = 1;
                    int row = startrow + 2;
                    foreach (var acc in transdetail.accoutnTransaction)
                    {
                        worksheet.Cells[row, 2].Value = acc.bnkdate;
                        worksheet.Cells[row, 3].Value = acc.ProjectNo;
                        worksheet.Cells[row, 4].Value = acc.Remarks;
                        worksheet.Cells[row + 1, 3].Value = "Bank Info";
                        worksheet.Cells[row + 1, 4].Value = acc.bankPart;
                        worksheet.Cells[row, 5].Value = acc.voucherTypeStr;
                        worksheet.Cells[row, 6].Value = acc.voucherNo;
                        worksheet.Cells[row, 7].Value = acc.CommitmentNO;
                        worksheet.Cells[row, 8].Value = acc.ChequeNO;
                        if (acc.voucherType == 1 || acc.voucherType == 4 || acc.voucherType == 11 || acc.voucherType == 14 || acc.voucherType == 8)
                        {
                            worksheet.Cells[row, 9].Value = acc.Amount;
                        }
                        else
                        {
                            worksheet.Cells[row, 10].Value = acc.Amount;
                        }
                        worksheet.Cells[row, 11].Value = acc.currentBal;

                        ///worksheet.Cells[row, 9].Value = acc.narration;
                        //worksheet.Cells[row, 10].Value = acc.Remarks;
                        if (acc.voucherType == 1 || acc.voucherType == 4 )
                        {
                            foreach (var ff in transdetail.accoutnTransaction.Where(m => m.voucherType != 1 && m.voucherType != 4 && m.voucherType != 8 && m.voucherType != 11 && m.voucherType != 14 && m.Recoupid == acc.Recoupid).ToList())
                            {
                                row = row + 2;
                                worksheet.Cells[row, 3].Value = ff.ProjectNo + "   " + Convert.ToString(ff.Amount);
                            }
                        }
                        row = row + 3;
                        //slNO++;
                    }
                    worksheet.Cells["A:K"].AutoFitColumns();
                    xlPackage.Save();
                }
                return stream.ToArray();

            }
        }
        
    }
}
