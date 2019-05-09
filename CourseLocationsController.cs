using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinalProject.Data.EF;

namespace FinalProject.UI.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CourseLocationsController : Controller
    {
        private FinalProjectEntities db = new FinalProjectEntities();

        // GET: CourseLocations
        public ActionResult Index()
        {
            var courseLocations = db.CourseLocations.Include(c => c.Cours).Include(c => c.Location);
            return View(courseLocations.ToList());
        }

        // GET: CourseLocations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseLocation courseLocation = db.CourseLocations.Find(id);
            if (courseLocation == null)
            {
                return HttpNotFound();
            }
            return View(courseLocation);
        }

        // GET: CourseLocations/Create
        public ActionResult Create()
        {
            ViewBag.CourseId = new SelectList(db.Courses, "CourseID", "CourseName");
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "StoreNumber");
            return View();
        }

        // POST: CourseLocations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CourseLocationID,CourseId,LocationID,Date")] CourseLocation courseLocation)
        {
            if (ModelState.IsValid)
            {
                db.CourseLocations.Add(courseLocation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CourseId = new SelectList(db.Courses, "CourseID", "CourseName", courseLocation.CourseId);
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "StoreNumber", courseLocation.LocationID);
            return View(courseLocation);
        }

        // GET: CourseLocations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseLocation courseLocation = db.CourseLocations.Find(id);
            if (courseLocation == null)
            {
                return HttpNotFound();
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseID", "CourseName", courseLocation.CourseId);
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "StoreNumber", courseLocation.LocationID);
            return View(courseLocation);
        }

        // POST: CourseLocations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CourseLocationID,CourseId,LocationID,Date")] CourseLocation courseLocation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(courseLocation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseId = new SelectList(db.Courses, "CourseID", "CourseName", courseLocation.CourseId);
            ViewBag.LocationID = new SelectList(db.Locations, "LocationID", "StoreNumber", courseLocation.LocationID);
            return View(courseLocation);
        }

        // GET: CourseLocations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseLocation courseLocation = db.CourseLocations.Find(id);
            if (courseLocation == null)
            {
                return HttpNotFound();
            }
            return View(courseLocation);
        }

        // POST: CourseLocations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CourseLocation courseLocation = db.CourseLocations.Find(id);
            db.CourseLocations.Remove(courseLocation);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
