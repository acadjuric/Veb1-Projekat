using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Projekat_WEB.Models
{
    public class Manifestacija
    {
        public string Naziv { get; set; }
        public ETipManifestacije Tip_Manifestacije { get; set; }
        public int Broj_Mesta { get; set; }
        public string Datum_i_Vreme_Odrzavanja { get; set; }
        public int Cena_Regularne_Karte { get; set; }
        public bool Status { get; set; }
        public Lokacija Lokacija { get; set; }
        public string Putanja_do_slike { get; set; }
        public bool LogickiObrisan { get; set; }
        public int BrojRegularnihMesta { get; set; }
        public int BrojVipMesta { get; set; }
        public int BrojFanPitMesta { get; set; }
        public double ProsecnaOcena { get; set; }



        public Manifestacija(string naziv, ETipManifestacije tip, int brojMestaRegular,int brojMestaVip,int brojMestaFanPit,string datumVreme, int cenaRegularneKarte,Lokacija m, string putanjaSLika,bool obrisan)
        {
            Naziv = naziv;
            Tip_Manifestacije = tip;
            BrojRegularnihMesta = brojMestaRegular;
            BrojVipMesta = brojMestaVip;
            BrojFanPitMesta = brojMestaFanPit;
            Datum_i_Vreme_Odrzavanja = datumVreme;
            Cena_Regularne_Karte = cenaRegularneKarte;
            Lokacija = m;
            Putanja_do_slike = putanjaSLika;
            LogickiObrisan = obrisan;

        }

        public Manifestacija()
        {

        }
        
        
    }
}