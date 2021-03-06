﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Juliusz_Final.Context;
using Juliusz_Final.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace Juliusz_Final.Controllers
{
    public class RentController : Controller
    {
        private EFCDbContext _context { get; }

        public RentController(EFCDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var rents = _context.Rents.Include(x => x.Customer).Include(x => x.Item).ThenInclude(x => x.Category).OrderByDescending(x => x.ID).ToList();
            return View("Rents", rents);
        }

        [HttpGet]
        public IActionResult Add()
        {
            var customers = _context.Customers.ToList();
            var selectListCustomers = customers.Select(x => new SelectListItem()
            {
                Text = $"{x.Name}, {x.Height}cm, Skill: {x.Skill}",
                Value = x.ID.ToString()
            });
            ViewBag.CustomersList = selectListCustomers;

            var items = _context.Items.Include(x => x.Category).ToList();
            var selectListItems = items.Select(x => new SelectListItem()
            {
                Text = $"{x.Category.Type}, {x.Model}, Rozmiar: {x.Size}",
                Value = x.ID.ToString()
            });
            ViewBag.ItemsList = selectListItems;
            return View();
        }


        [HttpPost]
        public IActionResult Add(Rent rent, string rentTime)
        {
            if (ModelState.IsValid)
            {
                rent.RentDate = DateTime.Now;
                var item = _context.Items.Single(x => x.ID == rent.ItemId);
                rent.ItemCondition = item.Condition;
                int hours = Convert.ToInt32(rentTime);
                rent.ExpirationDate = DateTime.Now + new TimeSpan(hours, 0, 0);
                _context.Rents.Add(rent);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rent);
        }

        [HttpGet]
        public IActionResult SingleCustomerAdd(int id)
        {
            var customer = _context.Customers.Single(x => x.ID == id);
            ViewBag.customer = customer;

            var items = _context.Items.Include(x => x.Category).ToList();
            var selectListItems = items.Select(x => new SelectListItem()
            {
                Text = $"{x.Category.Type}, {x.Model}, {x.Size}",
                Value = x.ID.ToString()
            });
            ViewBag.ItemsList = selectListItems;
            return View("SingleCustomerAdd");
        }


        [HttpPost]
        public IActionResult SingleCustomerAdd(Rent rent, string rentTime, string idCustomer)
        {
            if (ModelState.IsValid)
            {
                rent.RentDate = DateTime.Now;
                var item = _context.Items.Single(x => x.ID == rent.ItemId);
                rent.ItemCondition = item.Condition;
                int hours = Convert.ToInt32(rentTime);
                rent.ExpirationDate = DateTime.Now + new TimeSpan(hours, 0, 0);
                rent.CustomerID = Convert.ToInt32(idCustomer);
                rent.ID = 0;
                _context.Rents.Add(rent);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rent);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var rents = _context.Rents.Single(x => x.ID == id);
            return View(rents);
        }

        [HttpPost]
        public IActionResult Edit(Rent rent)
        {
            if (ModelState.IsValid)
            {
                _context.Update(rent);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(rent);
        }

        [HttpGet]
        public IActionResult Remove(int id)
        {
            var rent = _context.Rents.Single(x => x.ID == id);
            return View(rent);
        }

        [HttpPost]
        public IActionResult ConfirmRemove(int id)
        {
            var rent = _context.Rents.Single(x => x.ID == id);
            _context.Remove(rent);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ReturnProduct(int id)
        {
            var rent = _context.Rents.Include(x => x.Customer).Include(x => x.Item).ThenInclude(x => x.Category).Single(x => x.ID == id);
            return View(rent);
        }

        [HttpPost]
        public IActionResult ConfirmReturnProduct(int id)
        {
            var rent = _context.Rents.Single(x => x.ID == id);
            rent.ReturnDate = DateTime.Now;
            _context.Update(rent);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
