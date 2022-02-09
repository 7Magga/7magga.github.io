using Microsoft.AspNetCore.Mvc;
using PortalMDA.WEB.Models;
using System.Text;
using System.Net;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using PortalMDA.WEB.Models;
using Microsoft.AspNetCore.Authorization;
using PortalMDA.WEB;

namespace PortalMDA.WEB.Controllers
{
    public class LoginController : Controller
    {
        public class modelLogin
        {
            public UserModels user { get; set; }
            public string token { get; set; }
        }
        [AllowAnonymous]
        public IActionResult Home(string erroLogin)
        {
            if (erroLogin != "")
            {
                ViewBag.Erro = erroLogin;
            }
            if (HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Index","Home");

            return View();
        }

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> ValidarAsync(string UserName,string Senha)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress= new Uri("https://localhost:5001/api/");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            HttpResponseMessage resp = await httpClient.GetAsync($"Users/Autentica?_username={UserName}&_senha={Senha}");
            var dados = await resp.Content.ReadAsStringAsync();
            if(dados.Contains("Usuario ou senha incorretos"))
                return RedirectToAction("Home", new { erroLogin = dados.ToString().Replace('"',' ')});
            var dadosJson = JsonConvert.DeserializeObject<modelLogin>(dados);
            await new Services().Login(HttpContext, dadosJson.user,dadosJson.token);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Sair()
        {
            await new Services().Logoff(HttpContext);
            return RedirectToAction("Home");
        }
        public async void GetUser(UserModels user)
        {
            var httpCliente  = new HttpClient();
            var response = await httpCliente.GetAsync($"https://localhost:5001/api/Users/GetUser?username={user.UserName}");
            var data = await response.Content.ReadAsStringAsync();
            if (data != null)
            {
                RedirectToAction("Index","Home");
            }
            else
            {
                RedirectToAction("Index","Login");
            }
        }

    }
}
