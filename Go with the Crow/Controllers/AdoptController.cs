using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Go_with_the_Crow.Models;
using Go_with_the_Crow.Models.Database;
using Go_with_the_Crow.Models.Store;
using System.Net;
using System.Collections.Specialized;

namespace Go_with_the_Crow.Controllers
{
    [Authorize(Roles = "User")]
    public class AdoptController : Controller
    {
        // GET: Store
        [AllowAnonymous]
        public ActionResult Index()
        {
            List<Bird> adoptableBirds = new List<Bird>();
            using (Context ctx = new Context())
            {
                adoptableBirds = ctx.Birds
                    .Where(birb => birb.IsAvailable).OrderByDescending(x=>x.ListDate).ToList();
                ViewBag.Title = $"Available birds";
                return View("Index", adoptableBirds);
            }
        }

        public ActionResult ViewAdoptions()
        {
            using (Context ctx = new Context())
            {
                string uid = User.Identity.GetUserId();
                List<Bird> birds = ctx.Birds.Where(x => x.AdoptedBy == uid).ToList();
                ViewBag.Title = $"Birds adopted by {User.Identity.Name}";
                return View("Index", birds);
            }
        }

        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.Action = "Create";
            return View("Manage");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Bird userBird)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(Request.Files[0].FileName))
                {
                    userBird.ImagePath = SavePostedFileToServer(Request.Files[0], HttpContext.User.Identity.Name);
                }
                userBird.IsAvailable = true;
                userBird.ListDate = DateTime.Now;
                using(ApplicationDbContext adbc = new ApplicationDbContext())
                {
                    userBird.PostedBy = User.Identity.GetUserId();
                }
                using (Context ctx = new Context())
                {
                    ctx.Birds.Add(userBird);
                    ctx.SaveChanges();
                }
            return Redirect("Index");
            }
            return View("Manage");
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            using(Context ctx = new Context())
            {
                Bird bird = ctx.Birds.Where(x => x.ID == id).FirstOrDefault();
                return View("Details", bird);
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            using (Context ctx = new Context())
            {
                Bird bird = ctx.Birds.Where(x => x.ID == id).FirstOrDefault();
                if(bird == null)
                {
                    ViewBag.Reason = "The bird you tried to edit does not exist.";
                    return View("Invalid");
                }
                if (User.Identity.GetUserId() != bird.PostedBy && !User.IsInRole("Admin"))
                {
                    ViewBag.Reason = "Post was created by another user.";
                    return View("Invalid");
                }
                ViewBag.Action = "Edit";
                return View("Manage", bird);
            }
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditBird(Bird updBird)
        {
            if (ModelState.IsValid)
            {
                using (Context ctx = new Context())
                {
                    Bird birdToUpdate = ctx.Birds.Where(x => x.ID == updBird.ID).FirstOrDefault();
                    if (birdToUpdate == null) return new HttpNotFoundResult();

                    FormCollection fc = new FormCollection
                    {
                        { "BirdName", updBird.BirdName },
                        { "Age", updBird.Age.ToString() },
                        { "AdoptionFee", updBird.AdoptionFee.ToString() },
                        { "Species", updBird.Species },
                        { "Description", updBird.Description }
                    };

                    //figure out the upload image thing
                    updBird.ImagePath = !string.IsNullOrEmpty(Request.Files[0].FileName) ? SavePostedFileToServer(Request.Files[0], HttpContext.User.Identity.Name) : birdToUpdate.ImagePath;

                    fc.Add("ImagePath", updBird.ImagePath);

                    if(!TryUpdateModel(birdToUpdate, fc))
                    {
                        ViewBag.Reason = "The update could not be processed";
                        return View("Invalid");
                    }
                    ctx.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            return View("Manage", updBird);
        }

        [HttpGet]
        public ActionResult Purchase(int id)
        {
            using(Context ctx = new Context())
            {
                Bird bird = ctx.Birds.Where(x => x.ID == id).FirstOrDefault();
                if (bird == null)
                {
                    ViewBag.Reason = "The bird you requested could not be found.";
                    return View("Invalid");
                }
                if (!bird.IsAvailable)
                {
                    ViewBag.Reason = "The bird you requested is not available.";
                    return View("Invalid");
                }
            return View("Purchase", bird);
            }
        }

        [HttpPost, ActionName("Purchase")]
        public ActionResult PurchaseBird(int birdID)
        {
            using(Context ctx = new Context())
            {
                Bird birdToAdopt = ctx.Birds.Where(x => x.ID == birdID).FirstOrDefault();
                if (birdToAdopt != null)
                {
                    if(birdToAdopt.IsAvailable && User.Identity.GetUserId() != birdToAdopt.PostedBy)
                    {
                        Adoption adopt = new Adoption()
                        {
                            AdopterID = User.Identity.GetUserId(),
                            BirdID = birdToAdopt.ID,
                            AdoptDate = DateTime.Now
                        };

                        ctx.Adoptions.Add(adopt);

                        FormCollection fc = new FormCollection();
                        fc.Add("IsAvailable", "false");
                        fc.Add("AdoptedBy", User.Identity.GetUserId());

                        if(TryUpdateModel(birdToAdopt, new string[] { "IsAvailable", "AdoptedBy" }, fc))
                        {
                            ctx.SaveChanges();
                            return View("Congratulations", birdToAdopt);
                        }
                    }
                    ViewBag.Reason = "You cannot adopt this bird.";
                    return View("Invalid");
                }
                ViewBag.Reason = "The bird you tried to adopt was not available.";
                return View("Invalid");
            }
        }

        private string SavePostedFileToServer(HttpPostedFileBase file, string username)
        {

            string escUsername = Regex.Replace(username, @"[^a-zA-Z\d\s:]", string.Empty);
            string storeExtension = Path.GetExtension(file.FileName);
            string fileID = $"upl-{Guid.NewGuid()}-{escUsername}";

            string outputPath = Path.Combine(Server.MapPath("~/UploadedImages"), $"{fileID}{storeExtension}");

            file.SaveAs(outputPath);

            return $"{fileID}{storeExtension}";
        }
    }
}