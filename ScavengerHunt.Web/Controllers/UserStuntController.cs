using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

using Microsoft.AspNet.Identity;

using ScavengerHunt.Web.Models;

using WebGrease.Css.Extensions;

namespace ScavengerHunt.Web.Controllers
{
    public class UserStuntController : BaseController
    {
        // GET: /UserStunt/
        public ActionResult Index()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Index", "Stunt");

            // Get current user
            string currentUserId = User.Identity.GetUserId();
            var user = db.Users.Find(currentUserId);

            // Filter and sort stunts
            var stunts =
                user.UserStunts.Where(x => x.Stunt.Published)
                    .OrderByDescending(x => x.Status == UserStuntStatusEnum.Pending)
                    .ThenByDescending(x => x.Status == UserStuntStatusEnum.Available);

            return View(stunts.ToList().Globalize(Language));
        }

        public ActionResult ActivityPartial()
        {
            return PartialView(db.UserStunts.ToList().Globalize(Language));
        }

        // GET: /UserStunt/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UserStunt teamstunt = db.UserStunts.Find(id);
            if (teamstunt == null) return HttpNotFound();

            // Make sure the current user is registered, part of a team and have access to this stunt
            var userid = User.Identity.GetUserId();
            var user = db.Users.Find(userid);

            if (user == null) return RedirectToAction("Login", "Account");
            if (user.Team == null) return RedirectToAction("Start", "Team");
            if (user.UserStunts.All(x => x.Id != id)) return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            return View(teamstunt.Globalize(Language));
        }

        // POST: /UserStunt/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Submission,Status")] UserStunt teamstunt)
        {
            if (ModelState.IsValid)
            {
                // Get previous stunt object
                var teamStunt = db.UserStunts.Find(teamstunt.Id);

                teamStunt.Submission = teamstunt.Submission;
                teamStunt.Status = teamstunt.Status;
                teamStunt.DateUpdated = DateTime.Now;

                // Special logic if it's a flag
                if (teamStunt.Stunt.Type == StuntTypeEnum.Flag && !string.IsNullOrEmpty(teamStunt.Stunt.JudgeNotes) && !string.IsNullOrEmpty(teamStunt.Submission))
                {
                    if (teamstunt.Submission == teamStunt.Stunt.JudgeNotes)
                    {
                        teamStunt.Score = teamStunt.Stunt.MaxScore;
                        teamStunt.Status = UserStuntStatusEnum.Done;
                    }
                    else
                    {
                        // Store the amount of failed tries
                        int tries;
                        int.TryParse(teamStunt.JudgeNotes, out tries);

                        teamStunt.JudgeNotes = (++tries).ToString();
                        db.Entry(teamStunt);
                        db.SaveChanges();
                        
                        ModelState.AddModelError("Submission", "Wrong flag");
                        return View(teamStunt.Globalize(Language));
                    }
                }

                db.Entry(teamStunt).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(teamstunt.Globalize(Language));
        }

        public ActionResult Summary()
        {
            // Filter and sort stunts
            var stunts =
                db.UserStunts.Where(x => x.Stunt.Published)
                    .OrderByDescending(x => x.Status == UserStuntStatusEnum.Pending)
                    .ThenByDescending(x => x.Status == UserStuntStatusEnum.Available);

            return View(stunts.ToList().Globalize(Language));
        }

        // GET: /TeamStunt/Description
        public ActionResult Description(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            UserStunt teamstunt = db.UserStunts.Find(id);
            if (teamstunt == null) return HttpNotFound();

            // Make sure the current user is registered, part of a team and have access to this stunt
            var userid = User.Identity.GetUserId();
            var user = db.Users.Find(userid);

            if (user == null) return RedirectToAction("Description", "Stunt");
            if (user.Team == null) return RedirectToAction("Start", "Team");
            if (user.UserStunts.All(x => x.Id != id)) return new HttpStatusCodeResult(HttpStatusCode.Forbidden);

            return View(teamstunt.Globalize(Language));
        }

        // POST: /TeamStunt/Description/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Description([Bind(Include = "Id,Submission,Status")] UserStunt teamstunt)
        {
            if (ModelState.IsValid)
            {
                // Get previous stunt object
                var teamStunt = db.UserStunts.Find(teamstunt.Id);

                teamStunt.Submission = teamstunt.Submission;
                teamStunt.Status = UserStuntStatusEnum.Pending;
                teamStunt.DateUpdated = DateTime.Now;

                // Special logic if it's a flag
                if (teamStunt.Stunt.Type == StuntTypeEnum.Flag && !string.IsNullOrEmpty(teamStunt.Stunt.JudgeNotes) && !string.IsNullOrEmpty(teamStunt.Submission))
                {
                    if (teamstunt.Submission == teamStunt.Stunt.JudgeNotes)
                    {
                        teamStunt.Score = teamStunt.Stunt.MaxScore;
                        teamStunt.Status = UserStuntStatusEnum.Done;
                    }
                    else
                    {
                        // Store the amount of failed tries
                        int tries;
                        int.TryParse(teamStunt.JudgeNotes, out tries);

                        teamStunt.JudgeNotes = (++tries).ToString();
                        db.Entry(teamStunt);
                        db.SaveChanges();

                        ModelState.AddModelError("Submission", "Wrong flag");
                        return View(teamStunt.Globalize(Language));
                    }
                }

                db.Entry(teamStunt).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(teamstunt.Globalize(Language));
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
