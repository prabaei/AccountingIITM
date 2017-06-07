using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.data.ICSRTallyDb
{
    public interface IICSRDBTALLYEntities
    {
        DbSet<Ledger> Ledgers { get; set; }
        DbSet<Voucher> Vouchers { get; set; }

        
  
         DbChangeTracker ChangeTracker { get; }

        DbContextConfiguration Configuration { get; }

        System.Data.Entity.Database Database { get; }

        void Dispose();

        DbEntityEntry Entry(object entity);

        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        bool Equals(object obj);

        int GetHashCode();

        Type GetType();

        IEnumerable<DbEntityValidationResult> GetValidationErrors();

        int SaveChanges();

        Task<int> SaveChangesAsync();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);


        DbSet Set(Type entityType);


        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        string ToString();

        void Dispose(bool disposing);

        void OnModelCreating(DbModelBuilder modelBuilder);

        bool ShouldValidateEntity(DbEntityEntry entityEntry);

        DbEntityValidationResult ValidateEntity(DbEntityEntry entityEntry, IDictionary<object, object> items);
    }
}
