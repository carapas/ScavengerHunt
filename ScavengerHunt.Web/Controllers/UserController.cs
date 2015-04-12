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
    public class UserController : BaseController
    {
        // GET: User
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = db.Users.Find(id); ;
            if(user == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            return View(user);
        }
        // GET: User
        public ActionResult Index(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Team team = db.Teams.Find(id); ;
            if (team == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            TeamViewModel teamview = new TeamViewModel();
            teamview.Team = team;
            teamview.Users = db.Users.ToList().Where(x => x.Team != null && x.Team.Id == id).OrderByDescending(x => x.Score).ToList();

            return View(teamview);
        }
    }
}