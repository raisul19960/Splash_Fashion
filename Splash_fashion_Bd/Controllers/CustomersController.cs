using Splash_fashion_Bd.Models;
using Splash_fashion_Bd.Repositories.Interfaces;
using Splash_fashion_Bd.Repositories;
using Splash_fashion_Bd.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using X.PagedList;

namespace Splash_fashion_Bd.Controllers
{
    [Authorize]
    public class CustomersController : Controller
    {
        private readonly CustomerDbContext db = new CustomerDbContext();
        IGenericRepo<Customer> repo;
        public CustomersController()
        {
            this.repo = new GenericRepo<Customer>(db);
        }
        // GET: Customers
        [AllowAnonymous]
        public ActionResult Index(int pg = 1)
        {
            var data = this.repo.GetAll("Orders").ToPagedList(pg, 5);
            return View(data);
        }
        public ActionResult Create()
        {
            CustomersViewModel c = new CustomersViewModel();
            c.Orders.Add(new Order { });
            return View(c);
        }
        [HttpPost]
        public ActionResult Create(CustomersViewModel data, string act = "")
        {
            if (act == "add")
            {
                data.Orders.Add(new Order { OrderDate=DateTime.Today, DeliveryDate=DateTime.Today });

                foreach (var item in ModelState.Values)
                {
                    item.Errors.Clear();
                }
            }
            if (act.StartsWith("remove"))
            {
                int index = int.Parse(act.Substring(act.IndexOf("_") + 1));
                data.Orders.RemoveAt(index);
                foreach (var item in ModelState.Values)
                {
                    item.Errors.Clear();
                }
            }
            if (act == "insert")
            {
                if (ModelState.IsValid)
                {
                    var c = new Customer
                    {
                        CustomerName = data.CustomerName,
                        Address = data.Address,
                        Email = data.Email,
                        DoYouBuyOurProducts = data.DoYouBuyOurProducts

                    };
                    string ext = Path.GetExtension(data.Picture.FileName);
                    string fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                    string savePath = Server.MapPath("~/Pictures/") + fileName;
                    data.Picture.SaveAs(savePath);
                    c.Picture = fileName;
                    foreach (var o in data.Orders)
                    {
                        c.Orders.Add(o);
                    }
                    this.repo.Insert(c);
                    return RedirectToAction("Index");
                }

            }

            ViewBag.Act = act;
            return PartialView("_CustomerPartial", data);
        }


        public ActionResult Edit(int id)
        {
            var x = this.repo.Get(id, "Orders"); ;
            var c = new CustomerEditModel
            {
                Id = x.Id,
                CustomerName = x.CustomerName,
                Address = x.Address,
                Email = x.Email,
                DoYouBuyOurProducts = x.DoYouBuyOurProducts,
                Orders = x.Orders.ToList()

            };
            ViewData["CurrentPic"] = x.Picture;
            return View(c);
        }
        [HttpPost]
        public ActionResult Edit(CustomerEditModel data, string act = "")
        {
            if (act == "add")
            {
                data.Orders.Add(new Order { DeliveryDate=DateTime.Today, OrderDate=DateTime.Today });
            }
            if (act.StartsWith("remove"))
            {
                int index = int.Parse(act.Substring(act.IndexOf("_") + 1));
                data.Orders.RemoveAt(index);
            }
            if (act == "update")
            {
                if (ModelState.IsValid)
                {
                    //var c = db.Customers.First(x => x.Id == data.Id);
                    var c = this.repo.Get(data.Id);


                    c.CustomerName = data.CustomerName;
                    c.Address = data.Address;
                    c.Email = data.Email;
                    c.DoYouBuyOurProducts = data.DoYouBuyOurProducts;
                      
                   
                    if (data.Picture != null)
                    {
                        string ext = Path.GetExtension(data.Picture.FileName);
                        string fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                        string savePath = Server.MapPath("~/Pictures/") + fileName;
                        data.Picture.SaveAs(savePath);
                        c.Picture = fileName;
                    }
                    this.repo.Update(c);
                    this.repo.ExecuteCommand($"DELETE FROM Orders WHERE CustomerId={c.Id}");
                    foreach (var item in data.Orders)
                    {
                        this.repo.ExecuteCommand($"INSERT INTO Orders (OrderDate, DeliveryDate, [Status], CustomerId) VALUES ('{item.OrderDate}', '{item.DeliveryDate}', {(int)item.Status}, {c.Id})");
                    }
                   
                    return RedirectToAction("Index");
                }
            }
            ViewData["CurrentPic"] = db.Customers.First(x => x.Id == data.Id).Picture;
            return View(data);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            this.repo.ExecuteCommand($"dbo.DeleteCustomer {id}");
            return Json(new { success = true, deleted = id });
        }

    }   
    
}

    
