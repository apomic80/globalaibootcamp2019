using System;

namespace demo.Models
{
    public enum CampiDocumento
    {
        Nome = 0,
        Cognome = 1,
        Indirizzo = 2
    }

    public enum TipoDocumento
    {
        CartaIdentitaCartacea = 0,
        CartaIdentitaDigitale = 1,
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
        public string Foto { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Indirizzo { get; set; }
    }
}