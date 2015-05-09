using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ScavengerHunt.Web.Models;

using WebGrease.Css.Extensions;

namespace ScavengerHunt.Web.Controllers
{
    public class AchievementController : BaseController
    {
        // GET: /Achievement/
        [Authorize(Roles = "Admin")]
        public ActionResult IndexAdmin()
        {
            return View(db.Achievement.ToList().OrderBy(x => x.Name));
        }

        // GET: /UserAchievement/
        [Authorize(Roles = "Admin,Judge")]
        public ActionResult IndexJudge()
        {
            return View(db.Achievement.ToList().OrderBy(x => x.Name));
        }

        // GET: Achievement
        public ActionResult Index()
        {
            return View(db.Achievement.OrderBy(x => x.IsSecret).ToList());
        }

        // GET: Achievement
        public ActionResult IndexPartial()
        {
            return View(db.Achievement.OrderBy(x => x.IsSecret).ToList());
        }

        // GET: Achievement/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Achievement/Create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Description,Image,Points,IsSecret")] Achievement achievement)
        {
            if (ModelState.IsValid)
            {               
                db.Achievement.Add(achievement);
                foreach (var user in db.Users)
                {
                    var userAchievement = new UserAchievement()
                    {
                        Achievement = achievement,
                        IsAssigned = false,
                        User = user
                    };

                    db.UserAchievement.Add(userAchievement);
                }

                db.SaveChanges();

                return RedirectToAction("IndexAdmin");
            }
            return View("Create", achievement);
        }

        // GET: Achievement/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Achievement achievement = db.Achievement.Find(id);
            if (achievement == null)
            {
                return HttpNotFound();
            }
            return View(achievement);
        }

        // POST: Achievement/Edit/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Id,Name,Description,Image,Points,IsSecret")] Achievement achievement)
        {
            if (ModelState.IsValid)
            {
                var t = db.Achievement.Find(achievement.Id);

                if(t==null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                t.Name = achievement.Name;
                t.Description = achievement.Description;
                t.Image = achievement.Image;
                t.Points = achievement.Points;
                t.IsSecret = achievement.IsSecret;
                db.Entry(t).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexAdmin");
            }
            return View(achievement);
        }

        // GET: Achievement/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Achievement achievement = db.Achievement.Find(id);
            if (achievement == null)
            {
                return HttpNotFound();
            }
            return View(achievement);
        }

        // POST: Achievement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            Achievement achievement = db.Achievement.Find(id);

            var userAchievements = db.UserAchievement.ToList().Where(x => x.Achievement.Id == id);

            foreach (var userAchievement in userAchievements)
            {
                db.UserAchievement.Remove(userAchievement);
            }
                
            db.Achievement.Remove(achievement);
            db.SaveChanges();
            return RedirectToAction("IndexAdmin");
        }

        // GET: Achievement/Asign
        [Authorize(Roles = "Judge,Admin")]
        public ActionResult Asign(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return RedirectToAction("Asign", "UserAchievement",  new { id = id });
        }
    }
}
