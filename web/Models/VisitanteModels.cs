using System.ComponentModel.DataAnnotations;

namespace PortalMDA.WEB.Models
{
    public class VisitanteModels
    {
        [Key]
        public int VisitanteId { get; set; }
        public string? Nome { get; set; }
        public string? Sobrenome { get; set; }
        public int Idade { get; set; }
        //Inserir estado civil por enum
        public string? NmrTelefone { get; set; }
        public string? Endereco { get; set; }
        public string? Bairro { get; set; }
        public string? NmrResidencia { get; set; }
        public bool? WhatsApp { get; set; }
        public bool? Batizado { get; set; } 
        public string? WhatsAppStr { get; set; }
        public string? BatizadoStr { get; set; }
        public string? DataCadastrado { get; set; }
        public bool? Visita { get; set; }
        public string? VisitaStr { get; set; }
        public bool? Sexo { get; set; }
        public string? msc { get; set; }
        public string? fem { get; set; }
        //Conheceu por enum

    }
}