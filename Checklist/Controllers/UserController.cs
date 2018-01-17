using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Checklist.Models;
using Checklist.Business;
using System.Data.Entity.Validation;

namespace Checklist.Controllers
{
    public class UserController : Controller
    {
        GeralBusiness geralBusiness = new GeralBusiness();
        UserBusiness userBusiness = new UserBusiness();

        static user userLoggedIn = new user();

        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        //Login
        public ActionResult Login()
        {
            userLoggedIn = null;

            Session.Timeout = 120;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(user user)
        {
            ChecklistEntities db = new ChecklistEntities();

            // Criptografar senha e verificar com a da base
            string passwordCripted = userBusiness.CalculateMD5Hash(user.Password);

            // Verifica se tem o usuário e senha salvos
            userLoggedIn = db.user.Where(e => e.Email == user.Email && e.Password == passwordCripted).Select(e => e).FirstOrDefault();

            // Fazer a verificação do email se é válido
            if (userLoggedIn != null && userLoggedIn.EmailConfirmed == true)
            {
                Session["IdUser"] = userLoggedIn.IdUser.ToString();
                Session["Name"] = userLoggedIn.Name.ToString();
                Session["Email"] = userLoggedIn.Email.ToString();
                Session["Password"] = userLoggedIn.Password.ToString();
                Session["ADM"] = userLoggedIn.ADM.ToString();

                Session.Timeout = 120;

                return RedirectToAction("ListTasks", "Tasks");
            }
            else
            {
                if (userLoggedIn != null && userLoggedIn.EmailConfirmed == false)
                {
                    ModelState.AddModelError("error", "Olá " + userLoggedIn.Name + ", você ainda não confirmou seu email!");
                }
                else
                {
                    ModelState.AddModelError("error", "Usuário ou senha incorretos!");
                }
            }

            return View();
        }

        //Cadastrar novo Usuário
        public ActionResult NewUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult NewUser(user user, string PasswordConfirm)
        {
            try
            {
                // Salvando o novo usuário
                user = userBusiness.SavaNewUser(user, PasswordConfirm);

                //TODO:  Link para Ativação do Email -- Passar um parâmetro diferente do ID
                var linkUrl = Url.Action("EmailConfirmed", "User", new { code = user.Code });

                string url = Url.Action("EmailConfirmed", "User", new System.Web.Routing.RouteValueDictionary(new { code = user.Code }), "http", Request.Url.Host);

                // Enviar email para confirmar email do usuário
                geralBusiness.SendMail(user, url);

                return Json(user, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(400, ex.Message);
            }
        }

        //Alterar Senha
        public ActionResult ChangePassword()
        {
            ValidateUser();
            
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(string oldPass, string newPass, string confirmNewPass)
        {
            ValidateUser();

            try
            {
                userBusiness.ChangePassword(int.Parse(Session["IdUser"].ToString()), oldPass, newPass, confirmNewPass);

                ModelState.AddModelError("success", "Senha alterada com sucesso!");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", ex.Message);
            }

            return View();
        }
        
        public ActionResult EmailConfirmed(string code)
        {
            try
            {
                userBusiness.EmailConfirmed(code);

                return View();
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(400, ex.Message);
            }

            //return RedirectToAction("Login", "User");
        }

        public void ValidateUser()
        {
            if (userLoggedIn == null)
                RedirectToAction("Login", "User");
        }
    }
}