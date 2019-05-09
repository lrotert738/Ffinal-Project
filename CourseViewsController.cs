using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinalProject.Data.EF;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using FinalProject.UI.MVC.Models;

namespace FinalProject.UI.MVC.Controllers
{
    public class CourseViewsController : Controller
    {
        private FinalProjectEntities db = new FinalProjectEntities();

        // GET: CourseViews
        [Authorize]
        public ActionResult Index()
        {
            var courseViews = db.CourseViews.Include(c => c.Cours);

            var coursesViewed = new List<CourseView>();

            var userManager = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();            

            if(Request.IsAuthenticated && !User.IsInRole("Admin") && !User.IsInRole("Manager"))
            {            
                var currentUserID = User.Identity.GetUserId();
                coursesViewed = db.CourseViews.Where(x => x.UserID == currentUserID).ToList();
            }
            else
            {
                coursesViewed = courseViews.ToList();
            }

            foreach (var item in coursesViewed)
            {
                var user = userManager.FindById(item.UserID);
                item.UserID = user.LastName + ", " + user.FirstName;
            }

            return View(coursesViewed);
        }

        //public ActionResult IndexAdmin()
        //{
        //    var courseViews = db.CourseViews.Include(c => c.Cours);

        //    var coursesViewed = new List<CourseView>();

        //    var userManager = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();

        //    if (Request.IsAuthenticated && !User.IsInRole("Admin") && !User.IsInRole("Manager"))
        //    {
        //        var currentUserID = User.Identity.GetUserId();
        //        coursesViewed = db.CourseViews.Where(x => x.UserID == currentUserID).ToList();
        //    }
        //    else
        //    {
        //        coursesViewed = courseViews.ToList();
        //    }

        //    foreach (var item in coursesViewed)
        //    {
        //        var user = userManager.FindById(item.UserID);
        //        item.UserID = user.LastName + ", " + user.FirstName;
        //    }

        //    return View(coursesViewed);
        //}


        // GET: CourseViews/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                Response.Redirect("/error.html");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseView courseView = db.CourseViews.Find(id);
            if (courseView == null)
            {
                Response.Redirect("/error.html");
                //return HttpNotFound();
            }
            return View(courseView);
        }

        // GET: CourseViews/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName");
            ViewBag.UserID =  new SelectList(db.AspNetUsers, "Id", "FullName");
            // LLR
            //ViewBag.UserID = new SelectList(db.AspNetUsers)
            return View();
        }

        // POST: CourseViews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CourseViewID,UserID,CourseID,DateViewed")] CourseView courseView)
        {
            if (ModelState.IsValid)
            {
                db.CourseViews.Add(courseView);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName", courseView.CourseID);
            return View(courseView);
        }

        // GET: CourseViews/Edit/5
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                Response.Redirect("/error.html");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseView courseView = db.CourseViews.Find(id);
            if (courseView == null)
            {
                Response.Redirect("/error.html");
                //return HttpNotFound();
            }
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName", courseView.CourseID, courseView.UserID);
            return View(courseView);
        }

        // POST: CourseViews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin, Manager")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CourseViewID,UserID,CourseID,DateViewed")] CourseView courseView)
        {
            if (ModelState.IsValid)
            {
                db.Entry(courseView).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CourseID = new SelectList(db.Courses, "CourseID", "CourseName", courseView.CourseID);
            return View(courseView);
        }

        // GET: CourseViews/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                Response.Redirect("/error.html");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseView courseView = db.CourseViews.Find(id);
            if (courseView == null)
            {
                Response.Redirect("/error.html");
                //return HttpNotFound();
            }
            return View(courseView);
        }

        // POST: CourseViews/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CourseView courseView = db.CourseViews.Find(id);
            db.CourseViews.Remove(courseView);
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
