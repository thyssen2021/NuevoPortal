using Portal_2_0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Portal_2_0.Controllers
{
    public class ContactsController : BaseController
    {
        // GET: Contacts
        public ActionResult Index()
        {
            return View();
        }

        // GET: Contacts/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Contacts/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Contacts/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Contacts/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Contacts/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Contacts/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Contacts/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
