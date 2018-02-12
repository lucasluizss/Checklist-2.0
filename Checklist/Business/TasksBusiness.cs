using System;
using System.Linq;
using Checklist.Models;
using System.Collections.Generic;

namespace Checklist.Business
{
    public class TasksBusiness
    {
        private ChecklistEntities checkListDB = new ChecklistEntities();
        public UserBusiness userBusiness = new UserBusiness();

        public tasks NewTask(tasks newTask)
        {
            if (string.IsNullOrEmpty(newTask.Title))
            {
                throw new Exception("Faltou o título da tarefa!");
            }

            if (string.IsNullOrEmpty(newTask.Description))
            {
                throw new Exception("Faltou a descrição da tarefa!");
            }

            if (newTask.StartDate == null)
            {
                throw new Exception("Faltou a data de início da tarefa!");
            }

            if (newTask.StartDate < DateTime.Now.Date)
            {
                throw new Exception("A data de ínicio não pode ser anterior à data de hoje!");
            }

            if (newTask.EndDate == null)
            {
                throw new Exception("Faltou a data de fim da tarefa!");
            }

            if (newTask.IdPriority == 0)
            {
                throw new Exception("A prioridade não foi informada!");
            }

            //Quando criada uma tarefa ela recebe o estado (Aguardando início)
            newTask.IdStep = 1;

            try
            {
                checkListDB.tasks.Add(newTask);

                checkListDB.SaveChanges();

                return newTask;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Editar aqui Para diferentes tipos de Status
        public reasons ChangeStep(reasons statusStep)
        {
            checkListDB.Configuration.ProxyCreationEnabled = false;

            //Lambda para buscar uma tarefa pelo ID
            var task = checkListDB.tasks.FirstOrDefault(c => c.IdTask == statusStep.IdTask);
            var step = checkListDB.step.FirstOrDefault(id => id.IdStep == statusStep.IdStep);

            task.IdStep = statusStep.IdStep;
            task.step = step;
            task.LastDescriptionReason = statusStep.Description ?? "-";

            var reason = new reasons()
            {
                IdStep = step.IdStep,
                IdTask = task.IdTask,
                Description = statusStep.Description ?? "-",
                ChangeDate = DateTime.Now,
                step = step,
                tasks = task
            };

            try
            {
                SaveReason(reason);

                checkListDB.tasks.Attach(task);
                checkListDB.Entry(task).Property(x => x.IdStep).IsModified = true;
                checkListDB.Entry(task).Property(x => x.LastDescriptionReason).IsModified = true;
                checkListDB.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return reason;
        }

        public tasks EditTask(tasks editTask)
        {
            //Lambda para buscar uma tarefa pelo ID
            var task = checkListDB.tasks.FirstOrDefault(c => c.IdTask == editTask.IdTask);

            if (string.IsNullOrEmpty(editTask.Title))
            {
                throw new Exception("Favor preencher o campo titúlo!");
            }

            if (string.IsNullOrEmpty(editTask.Description))
            {
                throw new Exception("Favor preencher o campo descrição!");
            }

            //Alterando a descrição
            task.Title = editTask.Title;
            task.Description = editTask.Description;
            task.StartDate = editTask.StartDate;
            task.EndDate = editTask.EndDate;
            task.IdPriority = editTask.IdPriority;

            try
            {
                checkListDB.tasks.Attach(task);
                checkListDB.Entry(task).Property(x => x.Title).IsModified = true;
                checkListDB.Entry(task).Property(x => x.Description).IsModified = true;
                checkListDB.Entry(task).Property(x => x.StartDate).IsModified = true;
                checkListDB.Entry(task).Property(x => x.EndDate).IsModified = true;
                checkListDB.Entry(task).Property(x => x.IdPriority).IsModified = true;
                checkListDB.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return editTask;
        }

        public tasks DeleteTask(tasks task)
        {
            checkListDB.Configuration.ProxyCreationEnabled = false;

            //Lambda para buscar uma tarefa pelo ID
            task = checkListDB.tasks.FirstOrDefault(c => c.IdTask == task.IdTask);

            try
            {
                RemoveAllDependencies(task);
                checkListDB.tasks.Attach(task);
                checkListDB.tasks.Remove(task);
                checkListDB.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return task;
        }

        public tasks GetTaskById(int IdTask)
        {
            checkListDB.Configuration.ProxyCreationEnabled = false;

            return checkListDB.tasks.FirstOrDefault(c => c.IdTask == IdTask);
        }

        public reasons SaveReason(reasons reasons)
        {
            try
            {
                checkListDB.reasons.Add(reasons);

                checkListDB.SaveChanges();

                return reasons;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void RemoveAllDependencies(tasks task)
        {
            var reasonsList = checkListDB.reasons.Where(x => x.IdTask == task.IdTask).ToList();

            try
            {
                reasonsList.ForEach(reason => 
                    checkListDB.reasons.Remove(reason)
                );

                checkListDB.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // Lista de tarefas por usuário
        public List<tasks> GetTasksByIdUser(int idUser)
        {
            return checkListDB.tasks.Where(a => a.IdUser == idUser).OrderByDescending(e => e.IdStep).ToList();
        }

        public reasons GetLastReasonDescription(int? idStep, int idTask)
        {
            var listReasons = checkListDB.reasons.ToList();

            if (!listReasons.Any(a => a.IdStep == idStep))
                return null;

            return listReasons.Where(a => a.IdStep == idStep && a.IdTask == idTask).OrderByDescending(a => a.ChangeDate).FirstOrDefault();
        }

        public List<tasks> GetAllTasks()
        {
            return checkListDB.tasks.ToList();
        }

        public List<step> GetDescriptionSteps()
        {
            return checkListDB.step.ToList();
        }

        public List<priority> GetDescriptionPriority()
        {
            return checkListDB.priority.ToList();
        }
    }
}