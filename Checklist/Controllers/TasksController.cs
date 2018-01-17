using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Checklist.Models;
using Checklist.Business;
using Newtonsoft.Json;

namespace Checklist.Controllers
{
    public class TasksController : Controller
    {
        private ChecklistEntities checkListDB = new ChecklistEntities();
        TasksBusiness taskBusiness = new TasksBusiness();
        UserBusiness userBusiness = new UserBusiness();

        UserController user = new UserController();

        // GET: Tasks injeção de dependência
        public ActionResult ListTasks()
        {
            if (Session["IdUser"] != null)
            {
                List<tasks> list = new List<tasks>();

                list = taskBusiness.GetTasksByIdUser(int.Parse(Session["IdUser"].ToString()));

                if (!list.Any())
                {
                    ModelState.AddModelError("notify", "Você ainda não possui tarefas. Adicione uma nova =)");
                }

                return View(list);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        [HttpPost]
        public ActionResult NewTask(tasks newTask)
        {
            user.ValidateUser();

            try
            {
                newTask.IdUser = int.Parse(Session["IdUser"].ToString());

                taskBusiness.NewTask(newTask);

                return Json(newTask, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(404, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult EditStep(reasons editStep)
        {
            user.ValidateUser();

            try
            {
                taskBusiness.ChangeStep(editStep);

                return Json(editStep, JsonRequestBehavior.AllowGet); //cuidado aqui .step.DescriptionStep
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(404, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Edit(tasks task)
        {
            user.ValidateUser();

            try
            {
                taskBusiness.EditTask(task);
                
                return Json(task, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(404, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult DeleteTask(tasks task)
        {
            user.ValidateUser();

            try
            {
                taskBusiness.DeleteTask(task);

                var list = taskBusiness.GetTasksByIdUser(int.Parse(Session["IdUser"].ToString()));

                if (!list.Any())
                {
                    ModelState.AddModelError("notify", "Você ainda não possui tarefas. Adicione uma nova =)");
                }
                
                return Json(task, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(404, ex.Message);
            }
        }

        [HttpGet]
        public ActionResult GetTaskById(int idTask)
        {
            user.ValidateUser();

            try
            {
                var task = taskBusiness.GetTaskById(idTask);
                task.LastDescriptionReason = taskBusiness.GetLastReasonDescription(task.IdStep, idTask)?.Description ?? "";

                var newTask = new tasks
                {
                    IdTask = task.IdTask,
                    Title = task.Title,
                    Description = task.Description,
                    IdStep = task.IdStep,
                    IdPriority = task.IdPriority,
                    StartDate = task.StartDate,
                    EndDate = task.EndDate,
                    LastDescriptionReason = task.LastDescriptionReason
                };

                return Json(JsonConvert.SerializeObject(newTask), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(404, ex.Message);
            }
        }

        [HttpGet]
        public ActionResult GetDescriptionStep()
        {
            user.ValidateUser();

            try
            {
                var step = taskBusiness.GetDescriptionSteps();

                return Json(step, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(404, ex.Message);
            }
        }

        [HttpGet]
        public ActionResult GetDescriptionPriority()
        {
            user.ValidateUser();

            try
            {
                var priority = taskBusiness.GetDescriptionPriority();
                
                return Json(priority, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(404, ex.Message);
            }
        }

        public ActionResult Teste()
        {
            return View();
        }
    }
}