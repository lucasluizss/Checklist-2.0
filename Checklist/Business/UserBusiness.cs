using System;
using System.Linq;
using Checklist.Models;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;

namespace Checklist.Business
{
    public class UserBusiness
    {
        private ChecklistEntities userDB = new ChecklistEntities();
        public GeralBusiness geralBusiness = new GeralBusiness();

        #region CRUD USER

        //Salvar usuário
        public user SavaNewUser(user user, string passwordConfirm)
        {
            if (string.IsNullOrEmpty(user.Name))
            {
                throw new Exception("Você não informou seu nome!");
            }

            if (string.IsNullOrEmpty(user.Email))
            {
                throw new Exception("Você não informou seu email!");
            }
            
            Regex rg = new Regex(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$");

            if (!(rg.IsMatch(user.Email)))
            {
                throw new Exception("O email digitado não é um email válido!");
            }

            if (userDB.user.Where(u => u.Email == user.Email).Select(u => u).Any())
            {
                throw new Exception("Este email já esta cadastrado no sitema!");
            }

            if (string.IsNullOrEmpty(user.Password))
            {
                throw new Exception("Faltou preencher sua senha!");
            }

            if (user.Password != passwordConfirm)
            {
                throw new Exception("Ops, suas sennhas não conferem!");
            }

            //Verificar senha se segura e criptografar
            user.Password = SafaPassword(user.Password);

            //Usuário por padrão não é administrador
            user.ADM = false;

            //Atribuindo false ao email
            user.EmailConfirmed = false;

            //Gerando código único do usuário
            user.Code = Guid.NewGuid().ToString();

            try
            {
                userDB.user.Add(user);
                userDB.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
            return user;
        }

        //TODO: completar
        public user EditUser(user user)
        {
            if (string.IsNullOrEmpty(user.Name))
            {
                throw new Exception("Favor informar seu nome!");
            }

            if (string.IsNullOrEmpty(user.Email))
            {
                throw new Exception("Favor informar seu e-mail!");
            }

            return user;
        }
        
        #endregion

        public user GetUserById(int id)
        {
            if (id == null)
                return null;

            return userDB.user.FirstOrDefault(u => u.IdUser == id);
        }

        public user EmailConfirmed(string code)
        {
            user user = userDB.user.Where(u => u.Code == code).Select(u => u).FirstOrDefault();

            user.EmailConfirmed = true;

            try
            {
                userDB.user.Attach(user);
                userDB.Entry(user).Property(x => x.EmailConfirmed).IsModified = true;
                userDB.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
            return user;
        }
        
        public string ChangePassword(int idUser, string oldPassword, string newPassword, string confirmNewPassword)
        {
            user usr = userDB.user.Where(a => a.IdUser == idUser).Select(a => a).FirstOrDefault();

            if (usr.Password != CalculateMD5Hash(oldPassword).ToString())
            {
                throw new Exception("Senha atual incorreta! Tente novamente.");
            }
            
            if (newPassword != confirmNewPassword)
            {
                throw new Exception("Essas senhas não correspondem! Tente novamente.");
            }

            // Verificando segurança da senha e criptografando
            usr.Password = SafaPassword(newPassword);
            
            // Salvando mudanças
            try
            {
                userDB.user.Attach(usr);
                userDB.Entry(usr).Property(x => x.Password).IsModified = true;
                userDB.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
            return usr.Password;
        }
        
        // Verifica segurança da senha e criptografa
        public string SafaPassword(string password)
        {
            string endPassword;

            // Expressão regular para senha forte
            Regex pass = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,40}$", RegexOptions.None);
            
            //Verifica se password passou na expressão e retorna uma excessão se não passou
            if (!pass.IsMatch(password))
            {
                throw new Exception("Senha inválida. Sua senha deve conter no mínimo 8 digitos, sendo no mínimo uma letra minúscula, uma letra maiúscula e um número.");
            }
            else
            {
                // Criptografar senha
                endPassword = CalculateMD5Hash(password);
            }

            return endPassword;
        }

        // Criptografia da senha MD5
        public string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }
    }
}