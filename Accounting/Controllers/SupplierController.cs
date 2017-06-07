using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Accounting.data.Database;
using Accounting.Models.Supplier;

namespace Accounting.Controllers
{
    public class SupplierController : Controller
    {
        readonly IAccountingDbModel _accountdbmodel;
        public SupplierController(IAccountingDbModel AccountDb)
        {
            _accountdbmodel = AccountDb;
        }
        // GET: Supplier
        public ActionResult Index()
        {
            //_accountdbmodel.countries.Select(m => new SelectListItem() { Text = m.Country, Value = m.Id.ToString() }).ToList();
            return View(new Supplier() { CountryList=_accountdbmodel.countries.Select(m => new SelectListItem() { Text = m.Country, Value = m.Id.ToString() }).ToList(),Country=77});
        }
        [HttpPost]
        public ActionResult Index(Supplier supplier)
        {
            if(ModelState.IsValid)
            {
                if (supplier.Id == 0)
                {
                    _accountdbmodel.supplier.Add(supplier);
                    _accountdbmodel.SaveChanges();
                    //_accountdbmodel.countries.Select(m => new SelectListItem() { Text = m.Country, Value = m.Id.ToString() }).ToList();
                    return RedirectToAction("List");
                }
                else
                {
                    Supplier supp = _accountdbmodel.supplier.Where(m => m.Id == supplier.Id).FirstOrDefault();
                    supp.Name = supplier.Name;
                    supp.Address1 = supplier.Address1;
                    supp.Address2 = supplier.Address2;
                    supp.Address3 = supplier.Address3;
                    supp.Country = supplier.Country;
                    supp.District = supplier.District;
                    supp.State = supplier.State;
                    supp.PIN = supplier.PIN;
                    supp.Pan = supplier.Pan;
                    supp.Tan = supplier.Tan;
                    supp.Tin = supplier.Tin;
                    _accountdbmodel.SaveChanges();
                    return RedirectToAction("List");
                }
            }
            else
            {
                supplier.CountryList =_accountdbmodel.countries.Select(m => new SelectListItem() { Text = m.Country, Value = m.Id.ToString() }).ToList();
                return View(supplier);
            }
            
        }
        public ActionResult List()
        {
            //IEnumerable <Supplier> supplierList= _accountdbmodel.supplier.Where(m=>m.Id!=1).ToList();
            var supplierList = (from sup in _accountdbmodel.supplier
                       join coun in _accountdbmodel.countries on sup.Country equals coun.Id
                       where sup.Id != 1
                       select new SupplierMdl
                       {
                           Id = sup.Id,
                           Name = sup.Name,
                           Address1 = sup.Address1,
                           Address2 = sup.Address2,
                           Address3 = sup.Address3,
                           countryStr = coun.Country,
                           State = sup.State,
                           District = sup.District,
                           PIN = sup.PIN,
                           Pan = sup.Pan,
                           Tin = sup.Tin,
                           Tan = sup.Tan
                       }).ToList();
            
            return View(supplierList);

        }
        public ActionResult Edit(int? Id=null)
        {
            Supplier singleSup = _accountdbmodel.supplier.SingleOrDefault(m=>m.Id==Id);
            singleSup.CountryList = _accountdbmodel.countries.Select(m=>new SelectListItem() {Text=m.Country,Value=m.Id.ToString() }).ToList();
            return View("Index",singleSup);

        }
        public ActionResult Delete(int? Id = null)
        {
            Supplier singleSup = _accountdbmodel.supplier.SingleOrDefault(m => m.Id == Id);
            _accountdbmodel.supplier.Remove(singleSup);
            _accountdbmodel.SaveChanges();
            return RedirectToAction("List");

        }
    }
}