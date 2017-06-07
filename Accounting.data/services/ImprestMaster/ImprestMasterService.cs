using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accounting.data.Database;


namespace Accounting.data.services.ImprestMaster
{
    public class ImprestMasterService : IImprestMasterService
    {
        private readonly IAccountingDbModel Iaccountingdbmodel;
        public ImprestMasterService(IAccountingDbModel _IAccountingDbmodel)
        {
            this.Iaccountingdbmodel = _IAccountingDbmodel;
        }
        public Database.ImprestMaster getByID(int Id)
        {
            throw new NotImplementedException();
        }

        public IList<Database.ImprestMaster> getTable()
        {
            throw new NotImplementedException();
        }

        public void Insert(Database.ImprestMaster Entity)
        {
            Iaccountingdbmodel.ImprestMasters.Add(Entity);
            Iaccountingdbmodel.SaveChanges();

            //Iaccountingdbmodel.ImprestMasters.

        }

        public void Update(Database.ImprestMaster Entity)
        {
            throw new NotImplementedException();
        }

        public void updateByID(int Id)
        {
            throw new NotImplementedException();
        }
    }
}
