using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProcess
{
   
    public class stringData
    {
        public string data { get; set; }
        public int starInd { get; set; }
        public int endInd { get; set; }
    }
    public class headinglist
    {
        public List<stringData> headinglistda { get; set; }
        public string GetHeading(int start,int end)
        {
            foreach(var tt in headinglistda)
            {
                for(int i=start;i<= end; i++)
                {
                    if (i>tt.starInd && i<tt.endInd )
                    {
                        return tt.data;
                    }
                }
            }
            return null;
        }
    }
}
