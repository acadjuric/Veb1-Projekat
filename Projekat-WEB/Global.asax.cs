
using Projekat_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace Projekat_WEB
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            
            Dictionary<string, Korisnik> kupci = PomocneMetode.UcitajKorisnikIzFajla("~/App_Data/kupci.txt");

            Dictionary<string, Korisnik> admini = PomocneMetode.UcitajKorisnikIzFajla("~/App_Data/administratori.txt");
            
            Dictionary<string, Korisnik> prodavci = PomocneMetode.UcitajKorisnikIzFajla("~/App_Data/prodavci.txt");
            
            Dictionary<Manifestacija, string> manifestacije = PomocneMetode.UcitajManifestacijeIZFajla("~/App_Data/ProdavciManifestacije.txt");

            Dictionary<Karta,string> karte = PomocneMetode.UcitajKarteIzFajla("~/App_Data/KupciKarte.txt");

            List<Komentar> komentari = PomocneMetode.UcitajKomentareIzFajla("~/App_Data/komentari.txt");

            Dictionary<DateTime, string> datumi = PomocneMetode.UcitajDatumeOtkazivanjaIzFajla("~/App_Data/DatumiOtkazivanja.txt");


            //OBAVEZNO DODELI KUPCIMA NJIHOVE KARTE A PRODAVCIMA NJIHOVE MANIFESTACIJE
            //dodavanje manifestacija njihovim prodavcima
            foreach(KeyValuePair<Manifestacija,string> par in manifestacije)
            {
                if (prodavci.Keys.Contains(par.Value))
                {
                    Korisnik korisnik = prodavci.Values.FirstOrDefault(k => k.Korisnicko_Ime.Equals(par.Value));
                    korisnik.Manifestacije.Add(par.Key);
                }
            }
            //dodavanje karata njovim kupcima
            foreach(KeyValuePair<Karta,string> par in karte)
            {
                if (kupci.ContainsKey(par.Value))
                {
                    Korisnik korisnik = kupci.Values.FirstOrDefault(k => k.Korisnicko_Ime.Equals(par.Value));
                    korisnik.Karte.Add(par.Key);
                }
            }
            //dodaovanje datumaOtkazivanj njihovim kupcima
            foreach(KeyValuePair<DateTime,string> par in datumi)
            {
                if (kupci.ContainsKey(par.Value))
                {
                    Korisnik korisnik = kupci.Values.FirstOrDefault(k => k.Korisnicko_Ime.Equals(par.Value));
                    korisnik.datumiOtkazivanja.Add(par.Key);
                }
            }

            HttpContext.Current.Application["karte"] = karte;
            HttpContext.Current.Application["manifestacije"] = PomocneMetode.IzracunajProsecnuOcenuZaManifestacije(manifestacije,komentari,false);
            HttpContext.Current.Application["kupci"] = kupci;
            HttpContext.Current.Application["prodavci"] = prodavci;
            HttpContext.Current.Application["admini"] = admini;
            HttpContext.Current.Application["komentari"] = komentari;
            HttpContext.Current.Application["datumi"] = datumi;
        }

        public override void Init()
        {
            this.PostAuthenticateRequest += PodrskaZaSesiju;
            base.Init();
        }

        // Ukljucuje podrsku za sesiju, samo ako URL pocinje sa stringom /api/
        void PodrskaZaSesiju(object sender, EventArgs e)
        {
            if (HttpContext.Current.Request.Url.AbsolutePath.StartsWith("/api/"))
            {
                System.Web.HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
            }
        }
    }
}
