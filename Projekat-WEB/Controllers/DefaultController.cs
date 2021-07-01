using Projekat_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;

namespace Projekat_WEB.Controllers
{
    public class DefaultController : ApiController
    {
        [HttpGet, Route("")]
        public RedirectResult Index()
        {
            var requestUri = Request.RequestUri;
            return Redirect(requestUri.AbsoluteUri + "Views/Pocetna.html");
        }

        [HttpPost, Route("api/default/login")]
        [ResponseType(typeof(string))]
        public HttpResponseMessage Login([FromBody]Kupac korisnik)
        {
            Dictionary<string, Korisnik> admini = (Dictionary<string, Korisnik>)HttpContext.Current.Application["admini"];
            Dictionary<string, Korisnik> kupci = (Dictionary<string, Korisnik>)HttpContext.Current.Application["kupci"];
            Dictionary<string, Korisnik> prodavci = (Dictionary<string, Korisnik>)HttpContext.Current.Application["prodavci"];

            Korisnik k = (Korisnik)HttpContext.Current.Session["user"];
            if (k == null)
            {

                if (admini.ContainsKey(korisnik.Ime))
                {
                    if (admini[korisnik.Ime].Lozinka.Equals(korisnik.Prezime))
                    {
                        HttpContext.Current.Session["user"] = admini[korisnik.Ime];
                        return Request.CreateResponse(HttpStatusCode.OK, "http://" + Request.RequestUri.Authority + "/Views/Admin/AdminPanel.html");
                    }
                    return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Neispravno korisnicko ime ili lozinka");
                }
                else if (kupci.ContainsKey(korisnik.Ime))
                {
                    if (kupci[korisnik.Ime].LogickiObrisan)
                    {
                        // ne moze da se prijavi
                        return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Nemate nalog. Registrujte se.");
                        //return "http://" + Request.RequestUri.Authority + "/Views/Login.html" + ";obrisan";
                    }
                    else if (kupci[korisnik.Ime].Blokiran)
                    {
                        //ne moze da  se prijavi
                        return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Trenutno Vam je zabranjen pristup sajtu.");
                        //return "http://" + Request.RequestUri.Authority + "/Views/Login.html" + ";blokiran";

                    }
                    else if (kupci[korisnik.Ime].Lozinka.Equals(korisnik.Prezime))
                    {
                        HttpContext.Current.Session["user"] = kupci[korisnik.Ime];
                        return Request.CreateResponse(HttpStatusCode.OK, "http://" + Request.RequestUri.Authority + "/Views/Pocetna.html");
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Neispravno korisnicko ime ili lozinka");
                    }
                }
                else if (prodavci.ContainsKey(korisnik.Ime))
                {
                    if (prodavci[korisnik.Ime].LogickiObrisan)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Nemate nalog. Registrujte se.");
                        //ne moze da se prijavi
                        // return "http://" + Request.RequestUri.Authority + "/Views/Login.html" + ";obrisan";
                    }
                    else if (prodavci[korisnik.Ime].Blokiran)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Trenutno Vam je zabranjen pristup sajtu.");
                        //ne moze da se prijavi;
                        // return "http://" + Request.RequestUri.Authority + "/Views/Login.html" + ";blokiran";
                    }
                    else if (prodavci[korisnik.Ime].Lozinka.Equals(korisnik.Prezime))
                    {
                        HttpContext.Current.Session["user"] = prodavci[korisnik.Ime];
                        return Request.CreateResponse(HttpStatusCode.OK, "http://" + Request.RequestUri.Authority + "/Views/Prodavac/Index.html");
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Neispravno korisnicko ime ili lozinka");
                    }
                }
                return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Nemate nalog. Registrujte se.");
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Vec ste ulogovani kao " + k.Korisnicko_Ime);
                // "http://" + Request.RequestUri.Authority + "/Views/Login.html";
            }
        }

        [HttpGet, Route("api/default/manifestacije")]
        public List<Manifestacija> VratiSveManifestacije()
        {
            // Nema autorizacije mogu svi da pristupe
            Dictionary<Manifestacija, string> manifestacije = (Dictionary<Manifestacija, string>)HttpContext.Current.Application["manifestacije"];
            List<Komentar> komentari = (List<Komentar>)HttpContext.Current.Application["komentari"];
            //ovaj return mozda nece trebati ovu metodu izracunaj prosecnu ocenu si pozvao u global.asax fajlu prilikom pokretanja aplikacije svaki put ce se izracunati
            //return PomocneMetode.IzracunajProsecnuOcenuZaManifestacije(manifestacije, komentari).Keys.OrderByDescending(m => m.Datum_i_Vreme_Odrzavanja).ToList();

            //return manifestacije.Keys.ToList().OrderByDescending(m => m.Datum_i_Vreme_Odrzavanja).ToList();
            return manifestacije.Keys.ToList().OrderBy(m => m.Datum_i_Vreme_Odrzavanja).ToList();

        }

        [HttpPost, Route("api/default/pretrazi")]
        public List<Manifestacija> Pretraga([FromBody]PretragaModel pretragaModel)
        {
            // Nema autorizacije mogu svi da pristupe
            Dictionary<Manifestacija, string> manifestacije = (Dictionary<Manifestacija, string>)HttpContext.Current.Application["manifestacije"];
            List<Manifestacija> lista = new List<Manifestacija>();
            string temp = PomocneMetode.ValidacijaZaPretraguManifestacija(pretragaModel);
            if (temp.ToLower() == "uspesna pretraga")
            {
                lista = PomocneMetode.PretragaManifestacija(pretragaModel, manifestacije.Keys.ToList());

            }
            return lista;
        }

        [HttpPost, Route("api/default/sortiraj")]
        public List<Manifestacija> Sortiraj([FromBody]FiltriranjeModel model)
        {
            // Nema autorizacije mogu svi da pristupe
            return PomocneMetode.SortirajManifestacije(model.Lista, model.Izbor);
        }

        [HttpPost, Route("api/default/filtriraj")]
        public List<Manifestacija> Filtriraj([FromBody]FiltriranjeModel model)
        {
            // Nema autorizacije mogu svi da pristupe
            return PomocneMetode.FiltrirajManifestacije(model.Lista, model.Izbor);
        }

        [HttpPost, Route("api/default/manifestacija")]
        public Manifestacija VratiManifestaciju([FromBody]string id)
        {
            // Nema autorizacije mogu svi da pristupe
            Dictionary<Manifestacija, string> manifestacije = (Dictionary<Manifestacija, string>)HttpContext.Current.Application["manifestacije"];
            string[] delovi = id.Split(';');
            string naziv = delovi[0];
            double gsirina = double.Parse(delovi[1]);
            double gduzina = double.Parse(delovi[2]);
            string ulica = delovi[3];
            string broj = delovi[4];
            string grad = delovi[5];
            int pbroj = int.Parse(delovi[6]);
            Manifestacija m = manifestacije.Keys.FirstOrDefault(mani => mani.Naziv.ToLower().Equals(naziv.ToLower()) &&
                                                                        mani.Lokacija.Geografska_Sirina.Equals(gsirina) &&
                                                                        mani.Lokacija.Geografska_Duzina.Equals(gduzina) &&
                                                                        mani.Lokacija.Mesto_Odrzavanja.Ulica.ToLower().Equals(ulica.ToLower()) &&
                                                                        mani.Lokacija.Mesto_Odrzavanja.Broj.ToLower().Equals(broj.ToLower()) &&
                                                                        mani.Lokacija.Mesto_Odrzavanja.Grad.ToLower().Equals(grad.ToLower()) &&
                                                                        mani.Lokacija.Mesto_Odrzavanja.Postanski_Broj.Equals(pbroj)
                                                                        );
            HttpContext.Current.Session["ManifestacijaRezervacija"] = m;
            if (m != null)
            {
                HttpContext.Current.Session["komentar"] = m;
                HttpContext.Current.Session["ManifestacijaZaIzmenu"] = m;
                return m;
            }

            HttpContext.Current.Session["komentar"] = m;
            return m;
        }

        [HttpGet, Route("api/default/profil/{username}")]
        [ResponseType(typeof(Korisnik))]
        public HttpResponseMessage ProfilKorisnika(string username)
        {
            Korisnik k = (Korisnik)HttpContext.Current.Session["user"];
            if (k != null)
            {
                if (k.Uloga.Equals(EUloga.Administrator) || k.Uloga.Equals(EUloga.Prodavac) || k.Uloga.Equals(EUloga.Kupac))
                {
                    Dictionary<string, Korisnik> admini = (Dictionary<string, Korisnik>)HttpContext.Current.Application["admini"];
                    Dictionary<string, Korisnik> kupci = (Dictionary<string, Korisnik>)HttpContext.Current.Application["kupci"];
                    Dictionary<string, Korisnik> prodavci = (Dictionary<string, Korisnik>)HttpContext.Current.Application["prodavci"];

                    if (admini.ContainsKey(username))
                    {
                        HttpContext.Current.Session["zaIzmenu"] = admini[username];
                        return Request.CreateResponse(HttpStatusCode.OK, admini[username]);
                    }
                    else if (kupci.ContainsKey(username))
                    {
                        HttpContext.Current.Session["zaIzmenu"] = kupci[username];
                        return Request.CreateResponse(HttpStatusCode.OK, kupci[username]);
                    }
                    else
                    {
                        HttpContext.Current.Session["zaIzmenu"] = prodavci[username];
                        return Request.CreateResponse(HttpStatusCode.OK, prodavci[username]);
                    }
                }
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Niste autorizovani");
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Niste autorizovani");
            }
        }

        [HttpGet, Route("api/default/odjava")]
        [ResponseType(typeof(string))]
        public HttpResponseMessage Odjava()
        {
            Korisnik k = (Korisnik)HttpContext.Current.Session["user"];
            if (k != null)
            {
                HttpContext.Current.Session["user"] = null;
                HttpContext.Current.Session["zaIzmenu"] = null;
                return Request.CreateResponse(HttpStatusCode.OK, "Pocetna.html");
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Niste ulogovani");
            }
        }

        [HttpGet, Route("api/default/provera")]
        [ResponseType(typeof(bool))]
        public HttpResponseMessage ProveraZaKomentare()
        {
            Korisnik k = (Korisnik)HttpContext.Current.Session["user"];
            if (k != null && k.Uloga.Equals(EUloga.Kupac))
            {
                Manifestacija m = (Manifestacija)HttpContext.Current.Session["komentar"];

                if (m != null)
                {
                    foreach (Karta karta in k.Karte)
                    {
                        if (karta.Status_Karte.Equals(EStatusKarte.Rezervisana) && karta.LogickiObrisan == false)
                        {
                            if (karta.Manifestacija.Naziv.Equals(m.Naziv) && karta.Manifestacija.Tip_Manifestacije.Equals(m.Tip_Manifestacije) &&
                                karta.Manifestacija.Datum_i_Vreme_Odrzavanja.Equals(m.Datum_i_Vreme_Odrzavanja))
                            {
                                return Request.CreateResponse(HttpStatusCode.OK, true);
                            }
                        }
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, false);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Niste autorizovani");
            }

        }
    }
}
