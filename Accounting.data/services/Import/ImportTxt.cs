using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestProcess;

namespace Accounting.data.services.Import
{
    public class ImportTxt : IImportTxt
    {
        public List<transactionDetails> ImportAccountDetails(Stream stream, out string AccoutnNo,out string fromdate,out string todate)
        {
            string Accno=string.Empty;
            string fromDate = string.Empty;
            string ToDate = string.Empty;
            //bool isError = false;
            bool headingdone = false;
            char[] heading;
            List<stringData> Headdata = new List<stringData>();
            List<stringData> transdata = new List<stringData>();

            process processedData = new process();
            transactionDetails td = new transactionDetails();
            var tdtype = typeof(transactionDetails);
            List<transactionDetails> TransactionList = new List<transactionDetails>();
            PropertyInfo[] propertyInfos;
            propertyInfos = typeof(transactionDetails).GetProperties(BindingFlags.Public |BindingFlags.Static | BindingFlags.Instance);
            string line2 = string.Empty;
            using (var streamReader = new StreamReader(stream, Encoding.UTF8))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    string linetrail=string.Empty;
                    linetrail = line.Replace(" ", "").ToLower();
                    line2 = line;
                    if (line2.Replace(" ", "").ToLower().Contains("accountno"))
                    {
                        Accno = line2.Replace(" ", "").Trim();
                        try
                        {
                            Accno = Accno.Substring(Accno.Length - 13, 13);
                        }
                        catch
                        {
                           // isError = true;
                        }
                       
                    }
                    if (line2.Replace(" ", "").ToLower().Contains("statementofaccountfortheperiod"))
                    {
                        fromDate = line2.Replace(" ", "").Trim();
                        ToDate = line2.Replace(" ", "").Trim();

                        try
                        {
                            fromDate = fromDate.Substring(fromDate.Length - 22, 10);
                            ToDate = ToDate.Substring(ToDate.Length - 10, 10);
                        }
                        catch
                        {
                            //isError = true;
                        }
                    }
                    //if (isError) continue;
                    if (headingdone == true)
                    {
                        heading = line.ToArray();
                        // process processedData = new process();
                        transdata = processedData.ProcessValue(heading);
                        // linetrail = line.Replace(" ", "");
                        //linetrail = linetrail.ToLower();
                        if (linetrail.Contains("total:"))
                        {
                            AccoutnNo = Accno;
                            fromdate = fromDate;
                            todate = ToDate;
                            return TransactionList;
                            //headingdone = false;
                        }
                        //if (linetrail.Contains("postingdatevaluedate"))
                        //{
                        //    AccoutnNo = Accno;
                        //    fromdate = fromDate;
                        //    todate = ToDate;
                        //    return TransactionList;
                        //    //headingdone = false;
                        //}
                        if (!line.Contains("________") && !string.IsNullOrEmpty(line.Trim()) && !linetrail.Contains("postingdatevaluedate"))
                        {
                            foreach (var dd in transdata)
                            {
                                headinglist demo = new headinglist();
                                demo.headinglistda = Headdata;
                                string demheading = demo.GetHeading(dd.starInd, dd.endInd);
                                string fil = demheading.ToLower().Substring(0, 3);
                                PropertyInfo ssd = propertyInfos.Where(m => m.Name.ToLower().Contains(fil)).FirstOrDefault();
                                ssd.SetValue(td, Convert.ChangeType(dd.data, ssd.PropertyType), null);
                                Console.WriteLine(dd.data + "  " + demheading);
                            }
                            TransactionList.Add(td);
                            td = new transactionDetails();
                        }
                    }
                    if (linetrail.Contains("postingdatevaluedate") && headingdone!=true)
                    {
                        heading = line.ToArray();
                        Headdata = processedData.ProcessValue(heading);
                        headingdone = true;
                    }
                    if (linetrail.Contains("total:"))
                    {
                        AccoutnNo = Accno;
                        fromdate = fromDate;
                        todate = ToDate;
                        return TransactionList;
                    }
                }
            }
            AccoutnNo = Accno;
            fromdate = fromDate;
            todate = ToDate;
            return TransactionList;
        }
        public  List<transactionDetails> processTransList(List<transactionDetails> translist)
        {
            List<transactionDetails> processedTranslist = new List<transactionDetails>();
            foreach (var df in translist)
            {
                if (!string.IsNullOrEmpty(df.postingDate) && !string.IsNullOrEmpty(df.ValueDate))
                {
                    processedTranslist.Add(df);
                }
                else
                {
                    processedTranslist[processedTranslist.Count - 1].description += df.description == null ? "" : " " + df.description.Trim();
                    processedTranslist[processedTranslist.Count - 1].branch += df.branch == null ? "" : " " + df.branch.Trim();
                    processedTranslist[processedTranslist.Count - 1].deposite += df.deposite == null ? "" : " " + df.deposite.Trim();
                    processedTranslist[processedTranslist.Count - 1].postingDate += df.postingDate == null ? "" : " " + df.postingDate.Trim();
                    processedTranslist[processedTranslist.Count - 1].refchqno += df.refchqno == null ? "" : " " + df.refchqno.Trim();
                    processedTranslist[processedTranslist.Count - 1].ValueDate += df.ValueDate == null ? "" : " " + df.ValueDate.Trim();
                    processedTranslist[processedTranslist.Count - 1].withdrawals += df.withdrawals == null ? "" : " " + df.withdrawals.Trim();
                }
            }
            return processedTranslist;
        }

    }
}
