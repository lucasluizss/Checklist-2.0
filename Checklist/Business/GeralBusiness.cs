using System;
using Checklist.Models;
using System.Net.Mail;
using System.Net;
using System.Configuration;

namespace Checklist.Business
{
    public class GeralBusiness
    {
        public void SendMail(user userMail, string link)
        {
            MailMessage mail = new MailMessage();

            var email = ConfigurationManager.AppSettings["Email"];
            var password = ConfigurationManager.AppSettings["Password"];
            
            mail.From = new MailAddress(email);
            mail.To.Add(userMail.Email);
            mail.Subject = "Suporte Checklist - Tasks";
            mail.Body = "<html xmlns='http://www.w3.org/1999/xhtml'>"+
                        "< head >" +
                        "< title >Email confirmation</ title >" +
                        "</ head >" +
                        "< body >" +
                        "<h1>Olá " + userMail.Name + ", click <a href='" + link + "' title='click aqui para confirmar'>aqui</a> para confirmar seu email.</h1> " +
                        "</ body >" +
                        "</ html >"; 
            //Olá " + userMail.Name + ", click <a href='" + link + "' title='click aqui para confirmar'>aqui</a> para confirmar seu email.
            mail.IsBodyHtml = true;
            
            // em caso de anexos
            mail.Attachments.Add(new Attachment(@"D:\teste.txt"));

            using (var smtp = new SmtpClient("smtp.gmail.com"))
            {
                smtp.EnableSsl = true; // GMail requer SSL
                smtp.Port = 587;       // porta para SSL
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network; // modo de envio
                smtp.UseDefaultCredentials = false; // vamos utilizar credencias especificas

                // seu usuário e senha para autenticação
                smtp.Credentials = new NetworkCredential(email, password);

                // envia o e-mail
                smtp.Send(mail);
            }
        }
    }
}