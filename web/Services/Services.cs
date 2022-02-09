using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PortalMDA.WEB.Models;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace PortalMDA.WEB
{
    public class Services
    {
        public async Task Login(HttpContext ctx, UserModels user, string Token)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim(ClaimTypes.Role, user.Cargo));
            claims.Add(new Claim("TokenApi", Token));
            claims.Add(new Claim("Nome", user.Nome));
            claims.Add(new Claim("Grupo", user.Grupo));
            var claimsIdentity =
                new ClaimsPrincipal(
                    new ClaimsIdentity(
                        claims,
                        CookieAuthenticationDefaults.AuthenticationScheme
                    )
                );

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTime.Now.AddMinutes(30),
                IssuedUtc = DateTime.Now
            };


            await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsIdentity, authProperties);
        }

        public async Task Logoff(HttpContext ctx)
        {
            await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        //TODA COMUNICAÇÃO COM A API
        public HttpClient FactoryHttp(HttpContext ctx)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://localhost:5001/api/");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var token = ctx.User.Claims.Where(x => x.Type == "TokenApi").Select(x => x.Value).FirstOrDefault();
            httpClient.DefaultRequestHeaders.Add("Authorization", "bearer " + token);
            return httpClient;
        }

        public async Task<dynamic> DadosDash(HttpContext ctx)
        {
            try
            {
                HttpClient http = FactoryHttp(ctx);
                HttpResponseMessage resp = await http.GetAsync($"Dashboard");
                var _dados = await resp.Content.ReadAsStringAsync();
                if (_dados != null)
                {
                    string dado= (string)JsonConvert.DeserializeObject(_dados);
                    var dados = JsonConvert.DeserializeObject<DashModel>(dado);
                    return dados;
                }
            }
            catch(Exception e)
            {
                return "Erro " + e.ToString();
            }
            return "Sem execução";
        }
        public async Task<string> VisitanteSave(HttpContext ctx,VisitanteModels visitante)
        {
            HttpClient http = FactoryHttp(ctx);
            HttpResponseMessage resp = await http.PostAsJsonAsync($"Visitante/Criar",visitante);
            var dados = await resp.Content.ReadAsStringAsync();
            return dados.ToString().Replace('"', ' ');
        }
        public async Task<string> UserSave(HttpContext ctx, UserModels user)
        {
            HttpClient http = FactoryHttp(ctx);
            HttpResponseMessage resp = await http.PostAsJsonAsync($"Users/Criar", user);
            var dados = await resp.Content.ReadAsStringAsync();
            return dados.ToString().Replace('"', ' ');
        }

        public async Task<string> ResetSenha(HttpContext ctx, string username, string senhaNew, string chave)
        {
            HttpClient http = FactoryHttp(ctx);

            var resp = http.PutAsync($"Users/ResetSenha?_username={username}&_chave={chave}&_senha={senhaNew}",null);
            var dados = await resp.Result.Content.ReadAsStringAsync();

            return dados.ToString().Replace('"', ' ');
        }
        public async Task<string> ResetChave(HttpContext ctx, string username, string senha, string chaveNew)
        {
           HttpClient http = FactoryHttp(ctx);

           var resp = http.PutAsync($"Users/ResetChave?_username={username}&_chave={chaveNew}&_senha={senha}",null);
           var dados = await resp.Result.Content.ReadAsStringAsync();

           return dados.ToString().Replace('"', ' ');
        }

        public async Task<IEnumerable?> UsersAll(HttpContext ctx)
        {
            HttpClient http = FactoryHttp(ctx);
            HttpResponseMessage resp = await http.GetAsync("Users/GetAll");
            var dados = await resp.Content.ReadAsStringAsync();
            if (dados != "")
            {
                JArray dadosLista = (JArray)JsonConvert.DeserializeObject(dados);
                var _lista = new List<dynamic>();
                IEnumerable lista;
                if(dadosLista != null)
                {
                    for(int x = 0; x < dadosLista.Count; x++)
                    {
                        var elemento = dadosLista[x].ToString();
                        var dado = JsonConvert.DeserializeObject<UserModels>(elemento);
                        _lista.Add(dado);
                    }
                }
                if (_lista.Count() > 0)
                {
                    lista = _lista.AsEnumerable();
                    return lista;
                }
            }
            return null;
        }

        public async Task<string> UserDell(HttpContext ctx, int Id)
        {
            HttpClient http = FactoryHttp(ctx);
            HttpResponseMessage resp = await http.PostAsync($"Users/Deletar?DeletarID={Id}",null);
            var dados = await resp.Content.ReadAsStringAsync();
            return dados.ToString().Replace('"', ' ');
        }

        public async Task<IEnumerable?> VisitAll(HttpContext ctx)
        {
            HttpClient http = FactoryHttp(ctx);
            HttpResponseMessage resp = await http.GetAsync("Visitante/BuscaAll");
            var dados = await resp.Content.ReadAsStringAsync();
            if (dados != "")
            {
                JArray dadosLista = (JArray)JsonConvert.DeserializeObject(dados);
                var _lista = new List<dynamic>();
                IEnumerable lista;
                if (dadosLista != null)
                {
                    for (int x = 0; x < dadosLista.Count; x++)
                    {
                        var elemento = dadosLista[x].ToString();
                        var dado = JsonConvert.DeserializeObject<VisitanteModels>(elemento);
                        _lista.Add(dado);
                    }
                }
                if (_lista.Count() > 0)
                {
                    lista = _lista.AsEnumerable();
                    return lista;
                }
            }
            return null;
        }

        public async Task<IEnumerable?> MembrosAll(HttpContext ctx)
        {
            HttpClient http = FactoryHttp(ctx);
            HttpResponseMessage resp = await http.GetAsync("Membros/BuscaAll");
            var dados = await resp.Content.ReadAsStringAsync();
            if (dados != "")
            {
                JArray dadosLista = (JArray)JsonConvert.DeserializeObject(dados);
                var _lista = new List<dynamic>();
                IEnumerable lista;
                if (dadosLista != null)
                {
                    for (int x = 0; x < dadosLista.Count; x++)
                    {
                        var elemento = dadosLista[x].ToString();
                        var dado = JsonConvert.DeserializeObject<MembrosModels>(elemento);
                        _lista.Add(dado);
                    }
                }
                if (_lista.Count() > 0)
                {
                    lista = _lista.AsEnumerable();
                    return lista;
                }
            }
            return null;
        }

        public async Task<string> TornouMembro(HttpContext ctx,int id)
        {
            HttpClient http = FactoryHttp(ctx);
            HttpResponseMessage resp = await http.PostAsync($"Visitante/TornouMembro/?id={id}",null);
            var dados = await resp.Content.ReadAsStringAsync();
            return dados.ToString().Replace('"', ' ');
        }

        public async Task<string> DeletarVisitante(HttpContext ctx,int id)
        {
            HttpClient http = FactoryHttp(ctx);
            HttpResponseMessage resp = await http.PostAsync($"Visitante/Deletar?Id={id}",null);
            var dados = await resp.Content.ReadAsStringAsync();
            return dados.ToString().Replace('"', ' ');
        }
        public async Task<string> DeletarMembro(HttpContext ctx, int id)
        {
            HttpClient http = FactoryHttp(ctx);
            HttpResponseMessage resp = await http.PostAsync($"Membros/Deletar?Id={id}", null);
            var dados = await resp.Content.ReadAsStringAsync();
            return dados.ToString().Replace('"', ' ');
        }
    }
}
