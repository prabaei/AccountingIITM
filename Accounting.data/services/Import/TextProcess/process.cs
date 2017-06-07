using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProcess
{
    public class process
    {
        public List<stringData> ProcessValue(char[] line)
        {
            List<stringData> datalist = new List<stringData>();
            stringData sindata = new stringData();
            bool madestarind = false;
            bool wordstarted = false;
            int spacecount =0;
            string tempstr = string.Empty;
            //int spacecount = 0;
            for (int i = 0; i < line.Length; i++)
            {
                if (!Equals(line[i],' '))
                {
                    wordstarted = true;
                    tempstr += line[i];
                    if (spacecount == 1)
                    {
                        spacecount = 0;
                    }
                    if (madestarind == false)
                    {
                        sindata.starInd = i;
                        madestarind = true;
                    }
                    
                }
                if (line[i] == ' ' && wordstarted==true)
                {
                    tempstr += line[i];
                    spacecount++;
                }

                if (spacecount>1 || i==line.Length-1)
                {
                    if (wordstarted == true)
                    {
                        sindata.data = tempstr.Trim();
                        sindata.endInd = i;
                        datalist.Add(sindata);
                        spacecount = 0;
                        madestarind = false;
                        tempstr = string.Empty;
                        sindata = new stringData();
                        wordstarted = false;
                    }
                    
                }

            }

            return datalist;
        }
    }
}
