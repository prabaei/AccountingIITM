//using Accounting.data.services.dbservice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accounting.data.Database;

namespace Accounting.data.services.ImprestMaster
{
    public interface IImprestMasterService
    {
        void Insert(Accounting.data.Database.ImprestMaster Entity);

        void Update(Accounting.data.Database.ImprestMaster Entity);

        IList<Accounting.data.Database.ImprestMaster> getTable();

        Accounting.data.Database.ImprestMaster getByID(int Id);

        void updateByID(int Id);
    }
}
