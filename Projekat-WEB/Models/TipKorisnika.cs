using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Projekat_WEB.Models
{
    public class TipKorisnika
    {


        public ETipKorisnika Ime_Tipa_Korisnika { get; set; }
        public int Popust { get; set; }
        public int Trazeni_Broj_Bodova { get; set; }

        public TipKorisnika( ETipKorisnika tip, int popust, int bodovi) 
        {
            Ime_Tipa_Korisnika = tip;
            Popust = popust;
            Trazeni_Broj_Bodova = bodovi;

        }

        public TipKorisnika() { }


    }
}