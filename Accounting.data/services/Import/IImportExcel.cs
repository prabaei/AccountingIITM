using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.data.services.Import
{
    public interface IImportExcel
    {
        void ImportAccountDetails(Stream stream);
    }
}
