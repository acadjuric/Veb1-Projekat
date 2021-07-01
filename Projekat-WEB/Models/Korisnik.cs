using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Projekat_WEB.Models
{
    public class Korisnik
    {

        public string Korisnicko_Ime { get; set; }
        public string Lozinka { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Datum_Rodjenja { get; set; }
        public double Broj_Sakupljenih_Bodova { get; set; }
        public EPol Pol { get; set; }
        public EUloga Uloga { get; set; }
        public TipKorisnika TipKorisnika { get; set; }
        public List<Karta> Karte { get; set; }
        public List<Manifestacija> Manifestacije { get; set; }
        public bool LogickiObrisan { get; set; }
        public bool Blokiran { get; set; }
        public List<DateTime> datumiOtkazivanja { get; set; }
        public bool Sumnjiv { get; set; }

        public Korisnik(string kIme,string lozinka,string ime,string prezime, EPol pol, EUloga uloga, string DatumRodjenja, ETipKorisnika tip, int trazeniBodovi,int popust,bool obrisan  )
        {
            Korisnicko_Ime = kIme;
            Lozinka = lozinka;
            Ime = ime;
            Prezime = prezime;
            Pol = pol;
            Datum_Rodjenja = DatumRodjenja;
            if (uloga.Equals(EUloga.Kupac))
            {
                Uloga = uloga;
                Karte = new List<Karta>();
                datumiOtkazivanja = new List<DateTime>();
                Manifestacije = null;
                
            }
            else if(uloga.Equals(EUloga.Prodavac))
            {
                Uloga = uloga;
                Manifestacije = new List<Manifestacija>();
                Karte = null;
                datumiOtkazivanja = null;
                
            }
            else if (uloga.Equals(EUloga.Administrator))
            {
                Uloga = uloga;
                Karte = null;
                Manifestacije = null;
                datumiOtkazivanja = null;
            }
                TipKorisnika = new TipKorisnika();
                TipKorisnika.Ime_Tipa_Korisnika = tip;
                TipKorisnika.Trazeni_Broj_Bodova = trazeniBodovi;
                TipKorisnika.Popust = popust;
                LogickiObrisan = obrisan;
        }

        public Korisnik() { }

    }
}