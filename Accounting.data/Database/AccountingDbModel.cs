namespace Accounting.data.Database
{
    using System;
    // using System.Data.Entity;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Accounting.data.Database;
    using System.Collections.Generic;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Validation;

    public partial class AccountingDbModel : DbContext,IAccountingDbModel
    {
        public AccountingDbModel()
            : base("name=Accounting")
        {
            Database.SetInitializer<AccountingDbModel>(new CreateDatabaseIfNotExists<AccountingDbModel>());
        }

        public virtual DbSet<VoucherSubType> VoucherSubTypes { get; set; }
        public virtual DbSet<VoucherType> VoucherTypes { get; set; }
        public virtual DbSet<ImprestMaster> ImprestMasters { get; set; }
        public virtual DbSet<Transaction> Transaction { get; set; }
        public virtual DbSet<User> UserTbl { get; set; }
        public virtual DbSet<UserSession> UserSess { get; set; }
        public virtual DbSet<Supplier> supplier { get; set; }
        public virtual DbSet<ErrorLog> ErrorLog { get; set; }
        public virtual DbSet<Countries> countries { get; set; }
        public virtual DbSet<ProjectType> projectType { get; set; }

        public virtual DbSet<TransactionMigrated> migrated { get; set; }
        public virtual DbSet<RecoupRecord> recoupRecord { get; set; }

        public virtual DbSet<ChequeNotDep> cheqnotsub { get; set; }

        public virtual DbSet<ClimNotSubmitted> climNotSubmitted { get; set; }

        public virtual DbSet<MonthlyBalance> monthlyBalance { get; set; }

        public virtual DbSet<MigratedVoucher> migratedvoucher { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transaction>().Ignore(m => m.bnkdate).Ignore(m=>m.projtype).Ignore(m=>m.voucherTypeStr).Ignore(m=>m.engageFilterStr).Ignore(m=>m.filterStr).Ignore(m=>m.WITHDRAWALS).Ignore(m=>m.DEPOSITS).Ignore(m=>m.CoordinatorName).Ignore(m=>m.imported).Ignore(m=>m.voucherTypeList).Ignore(m=>m.recentHistory).Ignore(m=>m.supplierstr).Ignore(m=>m.Cheqdt);
            
          
            modelBuilder.Entity<User>().Ignore(m => m.password);
            //modelBuilder.Entity<User>().Property(m => m.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            //modelBuilder.Entity<Transaction>().Property(m => m.TransNO).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Countries>().Property(m => m.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Supplier>().Property(m => m.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Supplier>().Property(e => e.Amount).HasPrecision(20, 2);

            

            modelBuilder.Entity<ProjectType>().Property(e => e.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            modelBuilder.Entity<Supplier>().Ignore(m => m.CountryList).Ignore(m=>m.countryStr);
            
            modelBuilder.Entity<ImprestMaster>()
                .Property(e => e.Amount)
                .HasPrecision(20, 2);

            modelBuilder.Entity<ImprestMaster>()
                .Property(e => e.Limit)
                .HasPrecision(20, 0);
            
        }

        void IAccountingDbModel.Dispose(bool disposing)
        {
            base.Dispose();
            
        }

        void IAccountingDbModel.OnModelCreating(DbModelBuilder modelBuilder)
        {
            OnModelCreating(modelBuilder);
        }

        bool IAccountingDbModel.ShouldValidateEntity(DbEntityEntry entityEntry)
        {
            return ShouldValidateEntity(entityEntry);
        }

        DbEntityValidationResult IAccountingDbModel.ValidateEntity(DbEntityEntry entityEntry, IDictionary<object, object> items)
        {
            return ValidateEntity(entityEntry, items);
        }
    }
}
