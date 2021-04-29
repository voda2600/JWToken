using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LoginDemo.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace LoginDemo.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult LoginUser(User user)
        {
            //Наш класс в LoginDemo
            TokenProvider _tokenProvider = new TokenProvider();
            var userToken = _tokenProvider.LoginUser(user.USERID.Trim(), user.PASSWORD.Trim());
            if (userToken != null)
            {
                //Помещаем в сессию токен (Можно и в куки, но зачем?)
                HttpContext.Session.SetString("JWToken", userToken);
            }
            return Redirect("~/Home/Index");
        }

        public IActionResult Logoff()
        {
            HttpContext.Session.Clear();
            return Redirect("~/Home/Index");
            
        }
    }
}
