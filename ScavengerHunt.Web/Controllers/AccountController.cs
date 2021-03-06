﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using ScavengerHunt.Web.Models;
using System.Data.Entity;

namespace ScavengerHunt.Web.Controllers
{
    public class AccountController : BaseController
    {
        public AccountController()
        {
            UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
        }

        public AccountController(UserManager<ApplicationUser> userManager = null)
        {
            UserManager = userManager;
        }

        public UserManager<ApplicationUser> UserManager { get; private set; }


        public async Task<ActionResult> SetMeAdminPls(string returnUrl)
        {
            // Get current user
            var userid = User.Identity.GetUserId();
            if (!String.IsNullOrEmpty(userid) && db.Users.Count() < 2)
            {
                var result = await UserManager.AddToRoleAsync(userid, "Admin");
            }

            return RedirectToLocal(returnUrl);
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public async Task<ActionResult> Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            string ticket = Request.QueryString["ticket"];
            if (String.IsNullOrEmpty(ticket))
            {
                return Redirect(String.Format("https://cas.usherbrooke.ca/login?service={0}", Request.Url));
            }
            else
            {
                returnUrl = returnUrl == null ? Request.Url.GetLeftPart(UriPartial.Authority) : returnUrl;
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(String.Format("https://cas.usherbrooke.ca/serviceValidate?ticket={0}&service={1}", ticket, Request.Url.GetLeftPart(UriPartial.Path)));
                httpRequest.Method = "POST";

                HttpWebResponse response = (HttpWebResponse) httpRequest.GetResponse();
                using (var reader = new StreamReader(response.GetResponseStream(), System.Text.Encoding.UTF8))
                {
                    String resultData = reader.ReadToEnd();
                    XNamespace cas = "http://www.yale.edu/tp/cas";
                    var xDoc = XDocument.Parse(resultData);
                    bool isSuccess = xDoc.Descendants(cas + "authenticationSuccess").Any();
                    if (isSuccess)
                    {
                        string username = xDoc.Descendants(cas + "user").FirstOrDefault().Value;
                        var user = await UserManager.FindAsync(username, username);
                        if (user != null)
                        {
                            await SignInAsync(user, false);
                        }
                        else
                        {
                            //Register
                            var newUser = new ApplicationUser() { UserName = username,
                                                                  FullName = username
                            };
                            newUser.UserStunts = new List<UserStunt>();
                            foreach (var stunt in db.Stunts)
                            {
                                newUser.UserStunts.Add(new UserStunt()
                                {
                                    Stunt = stunt,
                                    Status = UserStuntStatusEnum.Available,
                                    User = user
                                });
                            }

                            newUser.UserAchievement = new List<UserAchievement>();
                            foreach (var achievement in db.Achievement)
                            {
                                newUser.UserAchievement.Add(new UserAchievement()
                                {
                                    Achievement = achievement,
                                    IsAssigned = false,
                                    User = user
                                });
                            }

                            var result = await UserManager.CreateAsync(newUser, username);
                            if (result.Succeeded)
                            {
                                await SignInAsync(newUser, isPersistent: false);
                                ApplicationUser appUser = await UserManager.FindByNameAsync(newUser.UserName);

                                if (db.Users.Count() < 2)
                                {
                                    await UserManager.AddToRoleAsync(appUser.Id, "Admin");
                                }

                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                AddErrors(result);
                            }
                        }
                    }
                }

                return RedirectToLocal(returnUrl);
            }
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.UserName, model.Password);
                if (user != null)
                {
                    await SignInAsync(user, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
            message == ManageMessageId.ChangeSuccess ? "Your account has been updated."
            : message == ManageMessageId.Error ? "An error has occurred."
            : "";
            var user = UserManager.FindById(User.Identity.GetUserId());
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View(user);
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage([Bind(Include = "Id,FullName,Email")] string id, string fullName, string email)
        {
            var user = UserManager.FindById(id);
            if (ModelState.IsValid)
            {
                user.FullName = fullName;
                user.Email = email;
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Manage", new { Message = ManageMessageId.ChangeSuccess });
            }

            // If we got this far, something failed, redisplay form
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.UserName };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var Db = new ScavengerHuntContext();
            var users = Db.Users;
            var model = new List<EditUserViewModel>();
            foreach (var user in users)
            {
                var u = new EditUserViewModel(user);
                model.Add(u);
            }
            return View(model);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Edit(string id, ManageMessageId? Message = null)
        {
            var Db = new ScavengerHuntContext();
            var user = Db.Users.First(u => u.UserName == id);
            var model = new EditUserViewModel(user);
            ViewBag.MessageId = Message;
            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var Db = new ScavengerHuntContext();
                var user = Db.Users.First(u => u.UserName == model.UserName);
                Db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                await Db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        [Authorize(Roles = "Admin")]
        public ActionResult Delete(string id = null)
        {
            var Db = new ScavengerHuntContext();
            var user = Db.Users.First(u => u.UserName == id);
            var model = new EditUserViewModel(user);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(model);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(string id)
        {
            var Db = new ScavengerHuntContext();
            var user = Db.Users.First(u => u.UserName == id);
            Db.Users.Remove(user);
            Db.SaveChanges();
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Admin")]
        public ActionResult UserRoles(string id)
        {
            var Db = new ScavengerHuntContext();
            var user = Db.Users.First(u => u.UserName == id);
            var model = new SelectUserRolesViewModel(user);
            return View(model);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public ActionResult UserRoles(SelectUserRolesViewModel model)
        {
            if (ModelState.IsValid)
            {
                var idManager = new IdentityManager();
                var Db = new ScavengerHuntContext();
                var user = Db.Users.First(u => u.UserName == model.UserName);
                idManager.ClearUserRoles(user.Id);
                foreach (var role in model.Roles)
                {
                    if (role.Selected)
                    {
                        idManager.AddUserToRole(user.Id, role.RoleName);
                    }
                }
                return RedirectToAction("index");
            }
            return View();
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ChangeSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}