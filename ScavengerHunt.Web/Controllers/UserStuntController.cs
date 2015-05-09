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

            if (user.Team == null) return RedirectToAction("Join", "Team");

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

        public ActionResult IndexPartial(string id)
        {
            return PartialView(db.UserStunts.Where(x => x.User.Id == id && x.Status == UserStuntStatusEnum.Done).ToList().Globalize(Language));
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
        public ActionResult Description(int? stuntId)
        {
            if (stuntId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userid = User.Identity.GetUserId();
            var user = db.Users.Find(userid);

            UserStunt userStunt = db.UserStunts.FirstOrDefault(x => x.Stunt.Id == stuntId && x.User.Id == user.Id);
            if (userStunt == null) return HttpNotFound();

            if (user == null) return RedirectToAction("Description", "Stunt");
            if (user.Team == null) return RedirectToAction("Start", "Team");

            var test = userStunt.Globalize(Language);
            return View(userStunt.Globalize(Language));
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
                if (teamStunt.Stunt.Type == StuntTypeEnum.Flag)
                {
                    if (teamstunt.Submission == teamStunt.Stunt.JudgeNotes)
                    {
                        teamStunt.Score = teamStunt.Stunt.MaxScore;
                        teamStunt.Status = UserStuntStatusEnum.Done;
                        teamStunt.Stunt.CompletedNumber++;
                        foreach (var teamUser in db.Users.Where(x => x.Team.Id == teamStunt.User.Team.Id).ToList())
                        {
                            teamUser.Rank = teamUser.Team.GetRank(teamUser.Score);
                        }
                    }
                    else
                    {
                        // Store the amount of failed tries
                        int tries;
                        int.TryParse(teamStunt.JudgeNotes, out tries);
                        teamStunt.Status = UserStuntStatusEnum.Available;

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
