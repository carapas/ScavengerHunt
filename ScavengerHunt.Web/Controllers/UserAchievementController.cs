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
    public class UserAchievementController : BaseController
    {

        // GET: UserAchievement
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            foreach (UserAchievement achievement in user.UserAchievement)
            {
                achievement.Achievement = db.Achievement.Find(achievement.AchievementId);
            }
            return View(user.UserAchievement);
            //return View(db.UserAchievement.ToList());
        }

        // GET: UserAchievement
        public ActionResult IndexPartial()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            foreach (UserAchievement achievement in user.UserAchievement)
            {
                achievement.Achievement = db.Achievement.Find(achievement.AchievementId);
            }
            return View(user.UserAchievement);
            //return View(db.UserAchievement.ToList());
        }

        // GET: UserAchievement/Create
        [Authorize(Roles = "Judge,Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // GET: UserAchievement/Create
        [Authorize(Roles = "Judge,Admin")]
        public ActionResult Asign(int? id)
        {
            Achievement achievement = db.Achievement.Find(id);
            if (achievement == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userAchievement = new UserAchievement();
            userAchievement.Achievement = achievement;
            userAchievement.allUser = db.Users.Where(x => !x.UserAchievement.Any(y => y.AchievementId == achievement.Id)).ToList();

            return View(userAchievement);
        }

        // Get: UserAchievement/Asign
        [HttpPost]
        [Authorize(Roles = "Judge,Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Asign([Bind(Include = "AchievementId,UserId")] UserAchievement userAchievement)
        {
            if (ModelState.IsValid)
            {
                userAchievement.allUser = null;
                db.UserAchievement.Add(userAchievement);

                var user = db.Users.Find(userAchievement.UserId);
                user.UserAchievement.Add(userAchievement);
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("IndexJudge", "Achievement");
            }
            return View("Asign", userAchievement);
        }

        // POST: UserAchievement/Create
        [HttpPost]
        [Authorize(Roles = "Judge,Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Achievement,UserId")] UserAchievement userAchievement)
        {
            if (ModelState.IsValid)
            {
                db.UserAchievement.Add(userAchievement);
                db.SaveChanges();

                return RedirectToAction("CreateDone");
            }
            return View("Create", userAchievement);
        }


        // GET: UserAchievement/Delete/5
        [Authorize(Roles = "Judge,Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserAchievement userAchievement = db.UserAchievement.Find(id);
            if (userAchievement == null)
            {
                return HttpNotFound();
            }
            return View(userAchievement);
        }

        // POST: UserAchievement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Judge,Admin")]
        public ActionResult Delete(int id)
        {
            UserAchievement userAchievement = db.UserAchievement.Find(id);


            db.UserAchievement.Remove(userAchievement);
            db.SaveChanges();
            return RedirectToAction("IndexAdmin");
        }
    }
}
