using System.ComponentModel.DataAnnotations;

namespace PortalMDA.WEB.Models
{
    public class UserModels
    {

        [Key]
        public int UserId { get; set; }
        public string? Nome { get; set; }
        public string? UserName {get; set;}
        public string? Senha { get; set; }
        public string? Cargo { get; set; }
        public string? Grupo { get; set; }
        public string? ChaveSeguranca {get;set;}
    }
}