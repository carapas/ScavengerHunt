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
            if(user == null)
            {
               return RedirectToAction("Index", "Achievement");
            }

            var userAchivements = db.UserAchievement.Where(x => x.User.Id == user.Id);
            var list = userAchivements.Where(x => x.IsAssigned).ToList();
            list.AddRange(userAchivements.Where(x => !x.IsAssigned).OrderBy(x => x.Achievement.IsSecret).ToList());
            return View(list);
        }

        // GET: UserAchievement
        public ActionResult PartialIndex(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = db.Users.Find(id);

            return PartialView(db.UserAchievement.Where(x => x.IsAssigned && user.Id == x.User.Id).ToList());
        }

        // GET: UserAchievement/Create
        [Authorize(Roles = "Judge,Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // GET: UserAchievement/Assign
        [Authorize(Roles = "Judge,Admin")]
        public ActionResult Asign(int? id)
        {
            Achievement achievement = db.Achievement.Find(id);
            if (achievement == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userAchievement = new UserAchievementsAssignViewModels();
            userAchievement.Achievement = achievement;
            userAchievement.AllUser = db.Users.Where(x => !x.UserAchievement.FirstOrDefault(y => y.Achievement.Id == achievement.Id).IsAssigned).ToList();

            return View(userAchievement);
        }

        // Post: UserAchievement/Asign
        [HttpPost]
        [Authorize(Roles = "Judge,Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult Asign([Bind(Include = "AchievementId,UserId")] UserAchievementsAssignViewModels userAchievement)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(userAchievement.UserId);
                user.UserAchievement.FirstOrDefault(x => x.Achievement.Id == userAchievement.AchievementId).IsAssigned = true;
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
