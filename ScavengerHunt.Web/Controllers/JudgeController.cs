using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ScavengerHunt.Web.Models;

namespace ScavengerHunt.Web.Controllers
{
    [Authorize(Roles = "Judge,Admin")]
    public class JudgeController : BaseController
    {
        // GET: /Judge/
        public ActionResult Index()
        {
            return View(db.UserStunts.ToList().Globalize(Language));
        }

        public ActionResult Flags()
        {
            return View(db.UserStunts.Where(x => x.Stunt.Type == StuntTypeEnum.Flag &&
                (!string.IsNullOrEmpty(x.Submission) || !string.IsNullOrEmpty(x.JudgeNotes)))
                .ToList().Globalize(Language));
        }


        // GET: /Judge/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserStunt teamstunt = db.UserStunts.Find(id);
            if (teamstunt == null)
            {
                return HttpNotFound();
            }
            return View(teamstunt.Globalize(Language));
        }

        // POST: /Judge/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,Score,JudgeNotes,JudgeFeedback")] UserStunt teamstunt)
        {
            if (ModelState.IsValid)
            {
                // Get previous stunt object
                var userStunt = db.UserStunts.Find(teamstunt.Id);
                var user = userStunt.User;

                userStunt.DateUpdated = DateTime.Now;
                userStunt.Score = teamstunt.Score;
                userStunt.Status = UserStuntStatusEnum.Done;
                userStunt.JudgeFeedback = teamstunt.JudgeFeedback;
                userStunt.JudgeNotes = teamstunt.JudgeNotes;
                if (userStunt.Score != 0)
                {
                    userStunt.Stunt.CompletedNumber++;
                }

                foreach (var teamUser in db.Users.Where(x => x.Team.Id == user.Team.Id).ToList())
                {
                    teamUser.Rank = teamUser.Team.GetRank(teamUser.Score);
                }

                db.Entry(userStunt).State = EntityState.Modified;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(teamstunt);
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
