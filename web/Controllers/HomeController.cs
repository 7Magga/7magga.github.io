using System.Collections;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortalMDA.WEB.Models;

namespace PortalMDA.WEB.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {

        _logger = logger;
    }
    [Authorize]
    public async Task<IActionResult> Index()
    {
        if (HttpContext.User.Identity.IsAuthenticated)
        {
            ViewBag.User = HttpContext.User;
            var dados = await new Services().DadosDash(HttpContext);
            ViewBag.Dados = dados;
            return View();
        }
        else
        {
            return RedirectToAction("Home","Login");
        }
    }
    [Authorize]

    public IActionResult Visitante(string retorno)
    {
        if(retorno!=null && retorno.Contains("Criado"))
        {
            ViewBag.Sucess = retorno;
        }
        else
        {
            ViewBag.Erro = retorno;
        }
        return View();
    }
    
    [Authorize]

    public IActionResult Privacy()
    {
        return View();
    }
    [Authorize]

    public IActionResult billing()
    {
        return View();
    }
    [Authorize]

    public IActionResult notifications()
    {
        return View();
    }
    [Authorize]

    public async Task<IActionResult> profile(string type, string result)
    {
        switch (type)
        {
            case "USR_SAVE":
                if (result == " Usuario cadastrado ")ViewBag.SucessUser = result;
                else ViewBag.ErroUser = result;
                break;
            case "USR_SENHA":
                if(result == " Senha Atualizada ")ViewBag.Sucess = result;
                else ViewBag.Erro = result;
                break;
            case "USR_CHAVE":
                if (result == " Chave Atualizada ")ViewBag.SucessChave = result;
                else ViewBag.ErroChave = result;
                break;
        }
        IEnumerable users = await new Services().UsersAll(HttpContext);
        ViewBag.UsersAll = users;
        ViewBag.Nome = HttpContext.User.Claims.Where(x=>x.Type=="Nome").Select(x=>x.Value).FirstOrDefault();
        ViewBag.Cargo = HttpContext.User.Claims.Where(x=>x.Type==ClaimTypes.Role).Select(x=>x.Value).FirstOrDefault();
        ViewBag.Grupo = HttpContext.User.Claims.Where(x => x.Type == "Grupo").Select(x => x.Value).FirstOrDefault();
        return View();
    }
    [Authorize]

    public IActionResult rtl()
    {
        return View();
    }
    [Authorize]

    public IActionResult signin()
    {
        return View();
    }
    [Authorize]

    public IActionResult signup()
    {
        return View();
    }
    [Authorize]

    public async Task<IActionResult> tables(string type, string result)
    {
        switch (type)
        {
            case "VST_MEMBRO": 
                ViewBag.MembroAdd = result;
                ViewBag.Type = type;
                break;
            case "VST_DELL":
                ViewBag.VisitanteDell = result;
                ViewBag.Type = type;
                break;
            case "MRB_DELL":
                ViewBag.MembroDell = result;
                ViewBag.Type = type;
                break;
        }
        IEnumerable users = await new Services().UsersAll(HttpContext);
        IEnumerable visit = await new Services().VisitAll(HttpContext);
        IEnumerable membros = await new Services().MembrosAll(HttpContext);
        ViewBag.Cargo = HttpContext.User.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value).FirstOrDefault();
        ViewBag.UsersAll = users;
        ViewBag.VisitAll = visit;
        ViewBag.MembrosAll = membros;
        return View();
    }

    public IActionResult Forbidden()
    {
        return View();
    }

    //Métodos
    public async Task<IActionResult> ResetarSenha(string UserName, string Senha, string ChaveSeguranca)
    {
        string result = await new Services().ResetSenha(HttpContext, UserName, Senha, ChaveSeguranca);
        return RedirectToAction("profile", new { result = result, type = "USR_SENHA" });
    }
    public async Task<IActionResult> ResetarChave(string UserName, string Senha, string ChaveSeguranca)
    {
        string result = await new Services().ResetChave(HttpContext, UserName, Senha, ChaveSeguranca);
        return RedirectToAction("profile", new { result = result,type= "USR_CHAVE" });
    }

    [Authorize, HttpPost]
    public async Task<IActionResult> VisitantePost(VisitanteModels visitante)
    {
        visitante.WhatsApp = Boolean.Parse(visitante.WhatsAppStr);
        visitante.Batizado = Boolean.Parse(visitante.BatizadoStr);
        visitante.Visita = Boolean.Parse(visitante.VisitaStr);
        var msc = Boolean.Parse(visitante.msc);
        var fem = Boolean.Parse(visitante.fem);
        if (msc || fem != null)
        {
            if (msc)
            {
                visitante.Sexo = false;
            }
            else
            {
                visitante.Sexo = true;
            }

            string retorno = "";
            if (ModelState.IsValid)
            {
                retorno = await new Services().VisitanteSave(HttpContext, visitante);
                return RedirectToAction("Visitante", new { retorno });
            }
            else
            {
                retorno = "Verifique todos os campos";
                return RedirectToAction("Visitante", new { retorno });

            }
        }
        return RedirectToAction("Visitantes");
    }
    [Authorize,HttpPost]
    public async Task<IActionResult> UserPost(UserModels user)
    {
        if (ModelState.IsValid)
        {
            string retorno = await new Services().UserSave(HttpContext, user);
            if (retorno == "false")
            {
                return RedirectToAction("Sair");
            }
            return RedirectToAction("profile", new { result = retorno,type="USR_SAVE" });
        }
        else
        {
            string retorno = "USR_SAVE Verifique todos os campos";
            return RedirectToAction("profile", new { result = retorno ,type= "USR_SAVE" });

        }
    }
    [Authorize]
    public async Task<IActionResult> VisitanteDell(int id)
    {
        string retorno = await new Services().DeletarVisitante(HttpContext, id);
        return RedirectToAction("tables", new { result = retorno, type = "VST_DELL" });
    }
    [Authorize]
    public async Task<IActionResult> MembroDell(int id)
    {
        string retorno = await new Services().DeletarMembro(HttpContext, id);
        return RedirectToAction("tables", new { result = retorno, type = "MRB_DELL" });
    }
    [Authorize]
    public async Task<IActionResult> UserDell(int id)
    {
        string retorno = await new Services().UserDell(HttpContext, id);
        return RedirectToAction("profile", new { result = retorno, type = "USR_DELL" });
    }
    [Authorize]
    public async Task<IActionResult> Sair()
    {
        await new Services().Logoff(HttpContext);
        return RedirectToAction("Home","Login");
    }

    [Authorize]
    public async Task<IActionResult> TornouMembro(int Id)
    {
        string retorno = await new Services().TornouMembro(HttpContext, Id);
        return RedirectToAction("tables",new {result = retorno,type="VST_MEMBRO"});
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    public IActionResult teste()
    {
        return View();
    }
}
