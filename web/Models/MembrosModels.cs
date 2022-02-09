using System.ComponentModel.DataAnnotations;
using System;

namespace PortalMDA.WEB.Models
{
    public class MembrosModels
    {
        [Key]
        public int MembroId { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public int Idade { get; set; }
        public string? NmrTelefone { get; set; }
        public bool? WhatsApp { get; set; }
        public string? Endereco { get; set; }
        public string? Bairro { get; set; }
        public string? NmrResidencia { get; set; }
        public bool? Batizado { get; set; }
        public bool? Sexo { get; set; }
        public bool? Visita { get; set; }
        public string? DataCadastrado { get; set; }

    }
}