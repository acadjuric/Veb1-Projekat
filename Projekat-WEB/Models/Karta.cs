using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Projekat_WEB.Models
{
    public class Karta
    {

        public string ID_Karte { get; set; }
        public Manifestacija Manifestacija { get; set; }
        public string Datum_i_Vreme_Manifestacije { get; set; }
        public double Cena { get; set; }
        public Kupac Kupac { get; set; }
        public EStatusKarte Status_Karte { get; set; }
        public ETipKarte TipKarte { get; set; }
        public bool LogickiObrisan { get; set; }

        public Karta(string id, Manifestacija m, string datumVreme, double cena, Kupac k , EStatusKarte status, ETipKarte tip,bool obrisan)
        {
            ID_Karte = id;
            Manifestacija = m;
            Datum_i_Vreme_Manifestacije = datumVreme;
            Cena = cena;
            Kupac = k;
            Status_Karte = status;
            TipKarte = tip;
            LogickiObrisan = obrisan;
        }

        public Karta() { }
       

        


    }
}