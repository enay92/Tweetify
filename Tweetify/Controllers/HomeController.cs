using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tweetify.DAL;
using Tweetify.Models;

namespace Tweetify.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue)
                return RedirectToAction("Login", "Auth");
            else
            {
                ViewData["UserName"] = HttpContext.Session.GetString("UserName");

                ViewData["NbFollowers"] = HttpContext.Session.GetInt32("NbFollowers");
                using (var con = new TweetifyContext())
                {
                    ViewData["Tweets"] = con.Tweets.Include("Author").Include("Likes").OrderByDescending(x => x.Date).Take(50).ToList();
                }
            };

            return View();
        }
        public ActionResult Like(int id)
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue)
                return RedirectToAction("Login", "Auth");
            else
            {
                using (var con = new TweetifyContext())
                {
                    Like query = con.Likes.FirstOrDefault(x => x.User.Id == HttpContext.Session.GetInt32("UserId") && x.Tweet.Id == id);
                    if (query != null)
                    {
                        con.Likes.Remove(query);
                        con.SaveChanges();
                    }
                    else
                    {
                        Like like = new Like
                        {
                            Tweet = con.Tweets.FirstOrDefault(x => x.Id == id),
                            User = con.Users.FirstOrDefault(x => x.Id == HttpContext.Session.GetInt32("UserId"))
                        };
                        con.Likes.Add(like);
                        con.SaveChanges();

                        return RedirectToAction("Index", "Home");
                    }
                }
                return RedirectToAction("Index", "Home");
            }

        }
        [HttpPost]
        public async Task<ActionResult> Tweeter(Tweet tweeter)
        {
            if (!HttpContext.Session.GetInt32("UserId").HasValue)
                return RedirectToAction("Login", "Auth");
            else
            {
                using (var con = new TweetifyContext())
                {
                    tweeter.Date = DateTime.Now;
                    tweeter.Author = con.Users.FirstOrDefault(a => a.Id == HttpContext.Session.GetInt32("UserId"));
                    await con.Tweets.AddAsync(tweeter);
                    await con.SaveChangesAsync();
                }
                return RedirectToAction("Index", "Home");
            }

        }
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
