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
    public class TeamController : BaseController
    {
        // GET: /Team/
        [Authorize(Roles="Admin")]
        public ActionResult IndexAdmin()
        {
            return View(db.Teams.ToList().OrderByDescending(x => x.Score));
        }

        public ActionResult Index()
        {
            Team team = null;

            // Get current user
            var userid = User.Identity.GetUserId();
            var user = db.Users.Find(userid); // Might be null

            // Make sure user is authenticated and has a team
            if (user != null && user.Team != null)
            {
                team = user.Team;
            }

            return View(team);
        }

        public ActionResult IndexPartial()
        {
            return PartialView(db.Teams.ToList().OrderByDescending(x => x.Score));
        }

        public ActionResult ShowToken(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = db.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return this.PartialView(team);
        }

        // GET: /Team/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = db.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        public ActionResult Start()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Account");

            // Redirect to stunts if user is already part of a team
            string currentUserId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(currentUserId);
            if (currentUser.Team != null && !User.IsInRole("Admin")) 
                return RedirectToAction("Index", "UserStunt");

            return View();
        }

        // GET: /Team/Create
        public ActionResult CreatePartial()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Account");

            return PartialView();
        }

        // POST: /Team/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,Name")] Team team)
        {
            if (!Settings.EnableTeamRegistration && !User.IsInRole("Admin"))
            {
                ModelState.AddModelError("Name", "Team creation is disabled");
                return View("Start");
            }
            
            if (ModelState.IsValid)
            {              
                // Make sure the team is not already there
                if (db.Teams.FirstOrDefault(x => x.Name == team.Name) != null)
                {
                    ModelState.AddModelError("Name", "Team name already exists");
                    return View("Start", team);
                }

                // Add current user as leader and member
                string currentUserId = User.Identity.GetUserId();
                var currentUser = db.Users.Find(currentUserId);
                team.ContactUser = currentUser;
                team.Members = new List<ApplicationUser> { currentUser };
                
                // Generate password token
                team.Token = Guid.NewGuid().ToString();

                db.Teams.Add(team);
                db.SaveChanges();

                return RedirectToAction("CreateDone");
            }

            return PartialView("CreatePartial", team);
        }

        public ActionResult CreateDone()
        {
            // TODO: 3 things can go wrong here but we don't really care
            var userid = User.Identity.GetUserId();
            var user = db.Users.Find(userid);
            return View(user.Team);
        }

        // GET: /Team/Join
        public ActionResult Join()
        {
            if (!User.Identity.IsAuthenticated) return RedirectToAction("Login", "Account");
            var jArray = new JArray();
            foreach (var team in db.Teams.OrderBy(x => x.Members.Count()))
            {
                jArray.Add(JObject.FromObject(team));
            }
            return View(jArray);
        }

        // Post: /Team/Join/password
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Join([Bind(Include = "Token")] string token)
        {
            if (!Settings.EnableTeamJoining && !User.IsInRole("Admin"))
            {
                ModelState.AddModelError("Token", "Team joining is disabled");
                return View("Start");
            }

            // Get User
            string currentUserId = User.Identity.GetUserId();
            var user = db.Users.Find(currentUserId);

            // TODO: Qu'est-ce qui arrive quand ce user est déjà membre d'une équipe?

            // Get Team
            var team = db.Teams.FirstOrDefault(t => t.Token == token);
            if (team == null)
            {
                ModelState.AddModelError("Token", "This token does not exist");
                return View("Start");
            }

            user.Rank = team.GetRank(user.Score);
            // Add user to team and save changes
            team.Members.Add(user);
            db.SaveChanges();

            return RedirectToAction("Index", "UserStunt");
        }

        public ActionResult Leave()
        {
            // TODO: Implement
            throw new NotImplementedException();
        }

        // GET: /Team/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = db.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        // POST: /Team/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include="Id,Name,Token,Tagline,Url,LogoUrl,BonusPoints,NumberOfRanks")] Team team)
        {
            if (ModelState.IsValid)
            {
                var t = db.Teams.Find(team.Id);

                t.Name = team.Name;
                t.BonusPoints = team.BonusPoints;
                t.Token = team.Token;
                t.Tagline = team.Tagline;
                t.Url = team.Url;
                t.LogoUrl = team.LogoUrl;
                if (team.NumberOfRanks > t.NumberOfRanks)
                {
                    int diff = team.NumberOfRanks - t.Ranks.Count;
                    for (int i = 0; i < diff; i++)
                    {
                        t.Ranks.Add(new Rank());
                    }
                }
                else if (team.NumberOfRanks < t.NumberOfRanks)
                {
                    int diff = t.Ranks.Count - team.NumberOfRanks;
                    for (int i = 0; i < diff; i++)
                    {
                        var toRemove = t.Ranks.Last();
                        t.Ranks.Remove(toRemove);
                        db.Entry(toRemove).State = EntityState.Deleted;
                    }

                    foreach (var member in t.Members)
                    {
                        member.Rank = t.GetRank(member.Score);
                    }
                }

                t.NumberOfRanks = team.NumberOfRanks;
                db.Entry(t).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("IndexAdmin");
            }
            return View(team);
        }

        // GET: /Team/EditRanks
        [Authorize(Roles = "Admin")]
        public ActionResult EditRanks(int id)
        {
            var team = db.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }

            if (team.Ranks != null)
                return View(team.Ranks.ToList());

            return View(new List<Rank>());
        }

        // POST: /Team/EditRanks/id
        [HttpPost]
        public ActionResult EditRanks([Bind(Include = "Id,Name,ScoreToAchieve")] ICollection<Rank> ranks, int id)
        {
            foreach (var rank in ranks)
            {
                var dbRank = db.Ranks.Find(rank.Id);
                dbRank.Name = rank.Name;
                dbRank.ScoreToAchieve = rank.ScoreToAchieve;
                db.Entry(dbRank).State = EntityState.Modified;
            }
            db.SaveChanges();

            Team team = db.Teams.Find(id);
            if (team != null)
            {
                foreach (var member in team.Members)
                {
                    member.Rank = team.GetRank(member.Score);
                    db.Entry(member).State = EntityState.Modified;
                }
            }

            db.SaveChanges();
            return RedirectToAction("IndexAdmin");
        }

        // GET: /Team/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = db.Teams.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        // POST: /Team/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Team team = db.Teams.Find(id);

            // Kick everyone from that team
            // TODO: Do with CASCADE NULL instead
            team.Members.ForEach(x => x.Team = null);

            db.Teams.Remove(team);
            db.SaveChanges();
            return RedirectToAction("IndexAdmin");
        }

        [HttpPost, ActionName("DeleteAll")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteAll()
        {
            db.Teams.RemoveRange(db.Teams);
            db.SaveChanges();
            return View("Data");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Data()
        {
            return View();
        }

        public ActionResult DataExportPartial()
        {
            return PartialView();
        }

        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult DataExport()
        {
            var teams = db.Teams.ToList();

            ViewBag.ExportData = JsonConvert.SerializeObject(teams);

            return View("Data");
        }

        public ActionResult DataImportPartial()
        {
            return PartialView();
        }

        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult DataImport(string data)
        {
            var teams = JsonConvert.DeserializeObject<List<Team>>(data);

            db.Teams.AddRange(teams);
            db.SaveChanges();

            // Create TeamStunts for the teams
            foreach (var user in db.Users)
            {
                foreach (var stunt in db.Stunts)
                {
                    var ts = new UserStunt() { Stunt = stunt, User = user };
                    db.UserStunts.Add(ts);
                }
            }
            db.SaveChanges();

            return RedirectToAction("IndexAdmin");
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
