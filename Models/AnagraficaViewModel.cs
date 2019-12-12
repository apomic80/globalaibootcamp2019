using System;

namespace demo.Models
{
    public enum TipoDocumento
    {
        CartaIdentità = 1,
        Passaporto = 2,
        Patente = 3
    }

    public enum Sesso
    {
        NonSpecificato = 0,
        Maschile = 1,
        Femminile = 2
    }

    public class AnagraficaViewModel
    {
        public string ImmagineDocumento { get; set; }
        public TipoDocumento Documento { get; set; }
        public string NumeroDocumento { get; set; }
        public DateTime ScadenzaDocumento { get; set; }

        public string Foto { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Indirizzo { get; set; }
        public Sesso Sesso { get; set; }
        public DateTime DataNascita { get; set; }
        public string CodiceFiscale { get; set; }
    }
}