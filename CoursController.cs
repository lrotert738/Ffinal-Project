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
    public class CoursController : Controller
    {
        private FinalProjectEntities db = new FinalProjectEntities();

        // GET: Cours
        public ActionResult Index()
        {
            var currentUser = User.Identity.GetUserId();
            //var courseviews = db.CourseViews
            //        .Where(x => x.UserID == currentUser);
            
            //var courses = db.Courses
            //    .Where(x => !x.CourseViews.Any(y => y.CourseID == x.CourseID));
            // get count of courses
            decimal courseCount = db.Courses.Count();

            // get count of courses completed
            decimal lessonCount = db.CourseViews.
                Where(x => x.UserID == currentUser).Count();

            decimal percentage = (lessonCount / courseCount) * 100;

            percentage = (int)percentage;

            ViewBag.Percentage = percentage;
            
            return View(db.Courses.ToList());
                        
        }
        public ActionResult TopClasses()
        {
            return View();
        }
        [Authorize]
        public ActionResult RemainingCourses()
        {
            var currentUser = User.Identity.GetUserId();
            var courseviews = db.CourseViews
                    .Where(x => x.UserID == currentUser);

            var courses = db.Courses
                .Where(x => !courseviews.Any(y => y.CourseID == x.CourseID));

            #region Calculate percentage completed
            decimal courseCount = db.Courses.Count();

            // get count of courses completed
            decimal lessonCount = db.CourseViews.
                Where(x => x.UserID == currentUser).Count();

            decimal percentage = (lessonCount / courseCount) * 100;

            percentage = (int)percentage;

            ViewBag.Percentage = percentage;
            #endregion
            return View(courses.ToList());

            // return all courses
            //return View(db.Courses.ToList());

        }
        
        [Authorize]
        public ActionResult Congratulations()
        {
            var userManager = System.Web.HttpContext.Current.
                GetOwinContext().GetUserManager<ApplicationUserManager>();
            //var user = userManager.FindById(item.UserID);
            var currentUser = User.Identity.GetUserId();

            ViewBag.Name = Session["FirstName"] + " " + Session["LastName"];

            //ViewBag.UserID = user.LastName + ", " + user.FirstName;
            //ViewBag.UserID = currentUser

            //ViewBag.Name = "Larry";
            return View();
        }

        // GET: Cours/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                Response.Redirect("/error.html");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cours cours = db.Courses.Find(id);
            if (cours == null)
            {
                Response.Redirect("/error.html");
                //return HttpNotFound();
            }

            var userID = User.Identity.GetUserId();
            CourseView cv = new CourseView();
            var hasViewed = db.CourseViews.ToList()
                .Where(x => x.UserID == userID)
                .Where(x => x.CourseID == id).Count();
            cv.UserID = userID;
            cv.CourseID = id ?? default(int);
            cv.DateViewed = DateTime.Now;
            if (hasViewed == 0)
            {
                db.CourseViews.Add(cv);
                db.SaveChanges();
            }

            return View(cours);
        }

        // GET: Cours/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Cours/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CourseID,CourseName,CourseDescription,VideoURL,coursePDF,IsActive")] Cours cours, HttpPostedFileBase coursePDF)
        {
            if (ModelState.IsValid)
            {
                string pdfName = "";
                // if a file was sent
                if(coursePDF != null)
                {
                    // reassign the variable to the filename sent over
                    pdfName = coursePDF.FileName;

                    // create a variable for the extension
                    string ext = pdfName.Substring(pdfName.LastIndexOf('.'));

                    // create a list of valid extensions
                    string[] goodExts = {".pdf"}; 

                    // if our extension is valid, assign a GUID as the name
                    // and concatonate the extension
                    if(goodExts.Contains(ext.ToLower()))
                    {
                        // save the file to the web server
                        coursePDF.SaveAs(Server.MapPath("~/Content/Images/PDF/" + pdfName));
                        cours.PDF = pdfName;
                    }
                    else
                    {
                        // default 
                        pdfName = "";
                    }

                }
                cours.IsActive = true;
                db.Courses.Add(cours);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cours);
        }

        // GET: Cours/Edit/5
        [Authorize(Roles = "Admin, Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                Response.Redirect("/error.html");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cours cours = db.Courses.Find(id);
            if (cours == null)
            {
                Response.Redirect("/error.html");
                //return HttpNotFound();
            }
            return View(cours);
        }

        // POST: Cours/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin, Manager")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CourseID,CourseName,CourseDescription,VideoURL,coursePDF,IsActive")] Cours cours, HttpPostedFileBase coursePDF)
        {
            if (ModelState.IsValid)
            {
                string pdfName = "";
                // if a file was sent
                if (coursePDF != null)
                {
                    // reassign the variable to the filename sent over
                    pdfName = coursePDF.FileName;

                    // create a variable for the extension
                    string ext = pdfName.Substring(pdfName.LastIndexOf('.'));

                    // create a list of valid extensions
                    string[] goodExts = { ".pdf" };

                    // if our extension is valid, assign a GUID as the name
                    // and concatonate the extension
                    if (goodExts.Contains(ext.ToLower()))
                    {
                        // save the file to the web server
                        coursePDF.SaveAs(Server.MapPath("~/Content/Images/PDF/" + pdfName));
                        cours.PDF = pdfName;
                    }
                    else
                    {
                        // default 
                        pdfName = "";
                    }

                }
                db.Courses.Add(cours);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cours);
        }

        // GET: Cours/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                Response.Redirect("/error.html");
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cours cours = db.Courses.Find(id);
            if (cours == null)
            {
                Response.Redirect("/error.html");
                //return HttpNotFound();
            }
            return View(cours);
        }

        // POST: Cours/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cours cours = db.Courses.Find(id);
            db.Courses.Remove(cours);
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
