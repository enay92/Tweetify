using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tweetify.DAL;
using Tweetify.Models;

namespace Tweetify.Controllers
{
    public class AuthController : Controller
    {

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(User u)
        {
            using (var context = new TweetifyContext())
            {
                User temp = context.Users.FirstOrDefault(x => x.Username == u.Username);

                if (temp != null)
                {
                    throw new Exception("Utilisateur existe déja");
                }
                else
                {
                    u.Username = u.Username.ToLower();
                    context.Users.Add(u);
                    await context.SaveChangesAsync();
                    return RedirectToAction("Login");
                }
            }
        }

        public IActionResult Login(User u)
        {
            // Récupérer un user à partir du username (en minuscules)
            // SI User => Verifier Mdp 
            // // Si User.MDP == MDP 
            // //    Je créée la variable de session
            // // SINON
            // //   STFU
            // SINON 
            //   STFU 

            using (var context = new TweetifyContext())
            {
                User temp = context.Users.FirstOrDefault(x => x.Username == u.Username);

                if (temp != null)
                {
                    if (temp.Password == u.Password)
                    {
                        HttpContext.Session.SetInt32(" UserId ", temp.Id);

                        return RedirectToAction(" Home ", " Index ");
                    }
                    else
                    {
                        throw new Exception(" wrong password");
                    }

                }
                else
                {
                    throw new Exception(" wrong User");
                }


            }
            //return View();
        }
        public IActionResult Logout()
        {
            // VARIABLE DE SESSION ? DIE MUTHAFUKA DIE !
            HttpContext.Session.Clear();
            return RedirectToAction(" Home ", " Index ");
        }
    }
}