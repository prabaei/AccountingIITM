using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.data.services.dbservice
{
    public interface IDbservice<T>
    {
        void Insert(T Entity);

        void Update(T Entity);

        IList<T> getTable();

        T getByID(int Id);

        void updateByID(int Id);


    }
}
