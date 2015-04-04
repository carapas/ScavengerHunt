using System.Linq;
using System.Web.Mvc;

namespace ScavengerHunt.Web.Controllers
{
    public class ScoreboardController : BaseController
    {
        // GET: /Scoreboard
        public ActionResult Index()
        {
            return View(db.Users.ToList().OrderByDescending(x => x.Score));
        }

        //Get: /Scoreboard/Teams
        public ActionResult Teams()
        {
            return View(db.Teams.ToList().OrderByDescending(x => x.Score));
        }
    }
}