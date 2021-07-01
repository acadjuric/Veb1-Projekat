using Projekat_WEB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Projekat_WEB.Controllers
{
    public class AdminController : ApiController
    {
        [HttpPost, Route("api/admin/kreirajprodavca")]
        [ResponseType(typeof(string))]
        public HttpResponseMessage KreiranjeProdavca([FromBody]Korisnik prodavac)
        {
            Korisnik korisnik = (Korisnik)HttpContext.Current.Session["user"];
            // ovde ide provera za ulogu prodavac jer kada menja svoj profil prodavac on gadja ovu metodu.
            if (korisnik != null && (korisnik.Uloga.Equals(EUloga.Administrator) || korisnik.Uloga.Equals(EUloga.Prodavac)))
            {
                string temp = PomocneMetode.ValidacijaKorisnika(prodavac);
                if (temp.ToLower() != "uspesna registracija")
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, temp);
                }

                Dictionary<string, Korisnik> prodavci = (Dictionary<string, Korisnik>)HttpContext.Current.Application["prodavci"];
                Dictionary<string, Korisnik> admini = (Dictionary<string, Korisnik>)HttpContext.Current.Application["admini"];
                Dictionary<string, Korisnik> kupci = (Dictionary<string, Korisnik>)HttpContext.Current.Application["kupci"];

                Korisnik stari = (Korisnik)HttpContext.Current.Session["zaIzmenu"];
                // dodata provera i za ulogu, jer admin ako se uloguje i ode na svoj profil on ce biti u sessiji zaIzmenu i 
                // prilikom kreiranja novog prodavca bi stalno ulazilo u ovaj if i pokusavalo da izmeni, umesto da kreira novog.
                if (stari != null && stari.Uloga.Equals(EUloga.Prodavac))
                {
                    if (PomocneMetode.ProveriKorisnickoIme(admini.Keys.ToList(), prodavci.Keys.ToList(), kupci.Keys.ToList(), prodavac) || stari.Korisnicko_Ime.Equals(korisnik.Korisnicko_Ime))
                    {
                        prodavci.Remove(stari.Korisnicko_Ime);

                        stari.Korisnicko_Ime = prodavac.Korisnicko_Ime;
                        stari.Ime = prodavac.Ime;
                        stari.Lozinka = prodavac.Lozinka;
                        stari.Prezime = prodavac.Prezime;
                        stari.Pol = prodavac.Pol;
                        stari.Datum_Rodjenja = prodavac.Datum_Rodjenja;

                        prodavci.Add(stari.Korisnicko_Ime, stari);
                        PomocneMetode.UpisiProdavceUFajl(prodavci);
                        HttpContext.Current.Session["zaIzmenu"] = stari;
                        return Request.CreateResponse(HttpStatusCode.OK, "Uspesno izmenjen prodavac");
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Korisnicko ime je zauzeto");
                    }
                }
                else
                {

                    if (PomocneMetode.ProveriKorisnickoIme(admini.Keys.ToList(), prodavci.Keys.ToList(), kupci.Keys.ToList(), prodavac))
                    {
                        prodavac.Uloga = EUloga.Prodavac;
                        prodavac.TipKorisnika = new TipKorisnika(ETipKorisnika.Pocetnik, 0, 0);
                        prodavac.LogickiObrisan = false;
                        prodavac.Blokiran = false;
                        prodavac.Sumnjiv = false;
                        prodavac.Manifestacije = new List<Manifestacija>();
                        prodavci.Add(prodavac.Korisnicko_Ime, prodavac);
                        PomocneMetode.UpisiProdavceUFajl(prodavci);

                        return Request.CreateResponse(HttpStatusCode.OK, "Uspesno kreiran prodavac");
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Korisnicko ime je zauzeto");
                    }
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Niste autorizovani");
            }

        }

        [HttpGet, Route("api/admin/prikazisvekorisnike")]
        [ResponseType(typeof(List<Korisnik>))]
        public HttpResponseMessage VratiSveKorisnike()
        {
            Korisnik k = (Korisnik)HttpContext.Current.Session["user"];
            if (k != null && k.Uloga.Equals(EUloga.Administrator))
            {
                Dictionary<string, Korisnik> kupci = (Dictionary<string, Korisnik>)HttpContext.Current.Application["kupci"];
                Dictionary<string, Korisnik> prodavci = (Dictionary<string, Korisnik>)HttpContext.Current.Application["prodavci"];
                Dictionary<string, Korisnik> admini = (Dictionary<string, Korisnik>)HttpContext.Current.Application["admini"];
                List<Korisnik> sviKorisnici = new List<Korisnik>();
                sviKorisnici.AddRange(kupci.Values);
                sviKorisnici.AddRange(prodavci.Values);
                sviKorisnici.AddRange(admini.Values);
                //return sviKorisnici;
                return Request.CreateResponse(HttpStatusCode.OK, sviKorisnici);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Niste autorizovani");
            }

        }

        [HttpPost, Route("api/admin/pretraga")]
        [ResponseType(typeof(List<Korisnik>))]
        public HttpResponseMessage PretragaKorisnika([FromBody]PretragaModel model)
        {
            Korisnik k = (Korisnik)HttpContext.Current.Session["user"];
            if (k != null && k.Uloga.Equals(EUloga.Administrator))
            {
                Dictionary<string, Korisnik> kupci = (Dictionary<string, Korisnik>)HttpContext.Current.Application["kupci"];
                Dictionary<string, Korisnik> prodavci = (Dictionary<string, Korisnik>)HttpContext.Current.Application["prodavci"];
                Dictionary<string, Korisnik> admini = (Dictionary<string, Korisnik>)HttpContext.Current.Application["admini"];
                List<Korisnik> sviKorisnici = new List<Korisnik>();
                sviKorisnici.AddRange(kupci.Values);
                sviKorisnici.AddRange(prodavci.Values);
                sviKorisnici.AddRange(admini.Values);

                string temp = PomocneMetode.ValidacijaZaPretraguKorisnika(model);
                if (temp.ToLower() == "uspesna pretraga")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, PomocneMetode.PretraziKorisnike(sviKorisnici, model));
                }
                else
                {
                    //validacija nije prosla za pretragu model
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Polja za pretragu nisu lepo popunjena");
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Niste autorizovani");
            }

        }

        [HttpPost, Route("api/admin/sortiraj")]
        [ResponseType(typeof(List<Korisnik>))]
        public HttpResponseMessage SortirajKorisnike([FromBody]SortiranjeFiltriranjeKarteModel model)
        {
            Korisnik k = (Korisnik)HttpContext.Current.Session["user"];
            if (k != null && k.Uloga.Equals(EUloga.Administrator))
            {
                return Request.CreateResponse(HttpStatusCode.OK, PomocneMetode.SortirajKorisnike(model));


            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Niste autorizovani");
            }
        }

        [HttpPost, Route("api/admin/filtriraj")]
        [ResponseType(typeof(List<Korisnik>))]
        public HttpResponseMessage FiltrirajKorisnike([FromBody]SortiranjeFiltriranjeKarteModel model)
        {
            //PomocneMetode.FiltrirajKorisnike(model);
            Korisnik k = (Korisnik)HttpContext.Current.Session["user"];
            if (k != null && k.Uloga.Equals(EUloga.Administrator))
            {
                return Request.CreateResponse(HttpStatusCode.OK, PomocneMetode.FiltrirajKorisnike(model));


            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Niste autorizovani");
            }
        }

        [HttpGet, Route("api/admin/obrisik/{id}")]
        [ResponseType(typeof(string))]
        public HttpResponseMessage ObrisiKorisnika(string id)
        {
            Korisnik korisnik = (Korisnik)HttpContext.Current.Session["user"];
            if (korisnik != null && korisnik.Uloga.Equals(EUloga.Administrator))
            {
                Dictionary<string, Korisnik> kupci = (Dictionary<string, Korisnik>)HttpContext.Current.Application["kupci"];
                Dictionary<string, Korisnik> prodavci = (Dictionary<string, Korisnik>)HttpContext.Current.Application["prodavci"];
                Dictionary<Manifestacija, string> manifestacije = (Dictionary<Manifestacija, string>)HttpContext.Current.Application["manifestacije"];
                Dictionary<Karta, string> sveKarte = (Dictionary<Karta, string>)HttpContext.Current.Application["karte"];
                List<Komentar> sviKomentari = (List<Komentar>)HttpContext.Current.Application["komentari"];

                if (kupci.ContainsKey(id))
                {
                    kupci[id].LogickiObrisan = true;
                    //obrisati i sve karte od kupca;
                    foreach (Karta k in kupci[id].Karte)
                    {
                        if (sveKarte.ContainsKey(k))
                        {
                            k.LogickiObrisan = true;
                        }
                    }

                    //obrisati i sve komentare od kupca

                    foreach (Komentar komentar in sviKomentari)
                    {
                        //kada kreiras komentar cuvas umesto imena korisnicko ime da bih lakse prepoznao koji korisnik je ostavio komentar
                        if (komentar.Kupac.Ime.ToLower().Equals(id.ToLower()))
                        {
                            //Ide provera jer mozda je jedan kupac ostavio vise komentara
                            if (komentar.LogickiObrisan) continue;
                            else
                            {
                                komentar.LogickiObrisan = true;
                                PomocneMetode.UpisiKomentareUFajl(sviKomentari);
                                HttpContext.Current.Application["manifestacije"] = PomocneMetode.IzracunajProsecnuOcenuZaManifestacije(manifestacije, sviKomentari, true);
                            }
                        }
                    }
                    PomocneMetode.upisiKarteUFajl(sveKarte);
                    PomocneMetode.UpisiKupceUFajl(kupci);
                    return Request.CreateResponse(HttpStatusCode.OK, "Kupac sa korisnickim imenom [" + id + "] je uspesno obrisan");
                }
                else if (prodavci.ContainsKey(id))
                {
                    prodavci[id].LogickiObrisan = true;
                    //obrisati i sve manifestacije od prodavca
                    //obrisati i sve karte za tu manifestaciju
                    //obrisati i sve komentare za tu manifestaciju
                    foreach (Manifestacija m in prodavci[id].Manifestacije)
                    {
                        if (manifestacije.Keys.Contains(m))
                        {
                            m.LogickiObrisan = true;
                        }
                        foreach (Karta k in sveKarte.Keys)
                        {
                            if (k.Manifestacija.Naziv.Equals(m.Naziv) &&
                                k.Manifestacija.Tip_Manifestacije.Equals(m.Tip_Manifestacije) &&
                                k.Manifestacija.Datum_i_Vreme_Odrzavanja.Equals(m.Datum_i_Vreme_Odrzavanja) &&
                                k.Manifestacija.Lokacija.Geografska_Duzina.Equals(m.Lokacija.Geografska_Duzina) &&
                                k.Manifestacija.Lokacija.Geografska_Sirina.Equals(m.Lokacija.Geografska_Sirina) &&
                                k.Manifestacija.Lokacija.Mesto_Odrzavanja.Ulica.Equals(m.Lokacija.Mesto_Odrzavanja.Ulica) &&
                                k.Manifestacija.Lokacija.Mesto_Odrzavanja.Broj.Equals(m.Lokacija.Mesto_Odrzavanja.Broj) &&
                                k.Manifestacija.Lokacija.Mesto_Odrzavanja.Grad.Equals(m.Lokacija.Mesto_Odrzavanja.Grad) &&
                                k.Manifestacija.Lokacija.Mesto_Odrzavanja.Postanski_Broj.Equals(m.Lokacija.Mesto_Odrzavanja.Postanski_Broj)
                                )
                            {
                                k.LogickiObrisan = true;
                            }
                        }

                        foreach (Komentar komentar in sviKomentari)
                        {
                            if (komentar.Manifestacija.Naziv.Equals(m.Naziv) &&
                                komentar.Manifestacija.Tip_Manifestacije.Equals(m.Tip_Manifestacije) &&
                                komentar.Manifestacija.Datum_i_Vreme_Odrzavanja.Equals(m.Datum_i_Vreme_Odrzavanja) &&
                                komentar.Manifestacija.Lokacija.Geografska_Duzina.Equals(m.Lokacija.Geografska_Duzina) &&
                                komentar.Manifestacija.Lokacija.Geografska_Sirina.Equals(m.Lokacija.Geografska_Sirina) &&
                                komentar.Manifestacija.Lokacija.Mesto_Odrzavanja.Ulica.Equals(m.Lokacija.Mesto_Odrzavanja.Ulica) &&
                                komentar.Manifestacija.Lokacija.Mesto_Odrzavanja.Broj.Equals(m.Lokacija.Mesto_Odrzavanja.Broj) &&
                                komentar.Manifestacija.Lokacija.Mesto_Odrzavanja.Grad.Equals(m.Lokacija.Mesto_Odrzavanja.Grad) &&
                                komentar.Manifestacija.Lokacija.Mesto_Odrzavanja.Postanski_Broj.Equals(m.Lokacija.Mesto_Odrzavanja.Postanski_Broj)
                                )
                            {
                                komentar.LogickiObrisan = true;
                            }

                        }

                    }

                    PomocneMetode.UpisiKomentareUFajl(sviKomentari);
                    PomocneMetode.UpisiManifestacijeUFajl(manifestacije);
                    PomocneMetode.UpisiProdavceUFajl(prodavci);
                    PomocneMetode.upisiKarteUFajl(sveKarte);
                    return Request.CreateResponse(HttpStatusCode.OK, "Prodavac sa korisnickim imenom [" + id + "] je uspesno obrisan");

                }

                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "korisnik nije ni kupac ni prodavac.Brisanje nije uspelo");
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Niste autorizovani");
            }

        }

        [HttpPost, Route("api/admin/obrisim")]
        [ResponseType(typeof(string))]
        public HttpResponseMessage ObrisiManifestaciju([FromBody]string id)
        {
            Korisnik korisnik = (Korisnik)HttpContext.Current.Session["user"];
            if (korisnik != null && korisnik.Uloga.Equals(EUloga.Administrator))
            {
                // naziv ; tip manifestacije ; datum i vreme odrzavanja; g sirina ;  g duzina; ulica ; broj ; grad ; postanski broj
                string[] delovi = id.Split(';');

                ETipManifestacije tip = (ETipManifestacije)Enum.Parse(typeof(ETipManifestacije), delovi[1]);
                Dictionary<Manifestacija, string> manifestacije = (Dictionary<Manifestacija, string>)HttpContext.Current.Application["manifestacije"];
                Dictionary<Karta, string> sveKarte = (Dictionary<Karta, string>)HttpContext.Current.Application["karte"];
                List<Komentar> sviKomentari = (List<Komentar>)HttpContext.Current.Application["komentari"];

                Manifestacija m = manifestacije.Keys.First(mani => mani.Naziv.ToLower().Equals(delovi[0].ToLower()) &&
                                                                   mani.Tip_Manifestacije.Equals(tip) &&
                                                                   mani.Datum_i_Vreme_Odrzavanja.Equals(delovi[2]) &&
                                                                   mani.Lokacija.Geografska_Sirina.Equals(double.Parse(delovi[3])) &&
                                                                   mani.Lokacija.Geografska_Duzina.Equals(double.Parse(delovi[4])) &&
                                                                   mani.Lokacija.Mesto_Odrzavanja.Ulica.Equals(delovi[5]) &&
                                                                   mani.Lokacija.Mesto_Odrzavanja.Broj.Equals(delovi[6]) &&
                                                                   mani.Lokacija.Mesto_Odrzavanja.Grad.Equals(delovi[7]) &&
                                                                   mani.Lokacija.Mesto_Odrzavanja.Postanski_Broj.Equals(int.Parse(delovi[8]))
                                                            );
                if (m != null)
                {
                    //proveri da li ce se obirsati i u listi manifestacija
                    m.LogickiObrisan = true;
                    foreach (Karta k in sveKarte.Keys)
                    {
                        //if (k.Manifestacija.Naziv.ToLower().Equals(m.Naziv.ToLower()) && k.Manifestacija.Tip_Manifestacije.Equals(m.Tip_Manifestacije)
                        //    && k.Manifestacija.Datum_i_Vreme_Odrzavanja.Equals(m.Datum_i_Vreme_Odrzavanja))
                        if (k.Manifestacija.Naziv.Equals(m.Naziv) &&
                                k.Manifestacija.Tip_Manifestacije.Equals(m.Tip_Manifestacije) &&
                                k.Manifestacija.Datum_i_Vreme_Odrzavanja.Equals(m.Datum_i_Vreme_Odrzavanja) &&
                                k.Manifestacija.Lokacija.Geografska_Duzina.Equals(m.Lokacija.Geografska_Duzina) &&
                                k.Manifestacija.Lokacija.Geografska_Sirina.Equals(m.Lokacija.Geografska_Sirina) &&
                                k.Manifestacija.Lokacija.Mesto_Odrzavanja.Ulica.Equals(m.Lokacija.Mesto_Odrzavanja.Ulica) &&
                                k.Manifestacija.Lokacija.Mesto_Odrzavanja.Broj.Equals(m.Lokacija.Mesto_Odrzavanja.Broj) &&
                                k.Manifestacija.Lokacija.Mesto_Odrzavanja.Grad.Equals(m.Lokacija.Mesto_Odrzavanja.Grad) &&
                                k.Manifestacija.Lokacija.Mesto_Odrzavanja.Postanski_Broj.Equals(m.Lokacija.Mesto_Odrzavanja.Postanski_Broj)
                          )
                        {
                            k.LogickiObrisan = true;
                        }
                    }

                    foreach (Komentar komentar in sviKomentari)
                    {
                        if (komentar.Manifestacija.Naziv.Equals(m.Naziv) &&
                            komentar.Manifestacija.Tip_Manifestacije.Equals(m.Tip_Manifestacije) &&
                            komentar.Manifestacija.Datum_i_Vreme_Odrzavanja.Equals(m.Datum_i_Vreme_Odrzavanja) &&
                            komentar.Manifestacija.Lokacija.Geografska_Duzina.Equals(m.Lokacija.Geografska_Duzina) &&
                            komentar.Manifestacija.Lokacija.Geografska_Sirina.Equals(m.Lokacija.Geografska_Sirina) &&
                            komentar.Manifestacija.Lokacija.Mesto_Odrzavanja.Ulica.Equals(m.Lokacija.Mesto_Odrzavanja.Ulica) &&
                            komentar.Manifestacija.Lokacija.Mesto_Odrzavanja.Broj.Equals(m.Lokacija.Mesto_Odrzavanja.Broj) &&
                            komentar.Manifestacija.Lokacija.Mesto_Odrzavanja.Grad.Equals(m.Lokacija.Mesto_Odrzavanja.Grad) &&
                            komentar.Manifestacija.Lokacija.Mesto_Odrzavanja.Postanski_Broj.Equals(m.Lokacija.Mesto_Odrzavanja.Postanski_Broj)
                            )
                        {
                            komentar.LogickiObrisan = true;
                        }

                    }

                    PomocneMetode.UpisiKomentareUFajl(sviKomentari);
                    PomocneMetode.UpisiManifestacijeUFajl(manifestacije);
                    PomocneMetode.upisiKarteUFajl(sveKarte);
                    return Request.CreateResponse(HttpStatusCode.OK, "Uspesno obrisana manifestacija i sve karte za nju");

                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Greska prilikom brisanja manifestacije. Iz nekog razloga je null ili ne moze da je pronadje u listi manifestacija.");

                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Niste autorizovani");
            }


        }

        [HttpPost, Route("api/admin/odobrim")]
        [ResponseType(typeof(string))]
        public HttpResponseMessage OdobriManifestaciju([FromBody]string id)
        {
            Korisnik korisnik = (Korisnik)HttpContext.Current.Session["user"];
            if (korisnik != null && korisnik.Uloga.Equals(EUloga.Administrator))
            {
                // naziv ; tip manifestacije ; datum i vreme odrzavanja; g sirina ;  g duzina; ulica ; broj ; grad ; postanski broj
                string[] delovi = id.Split(';');
                ETipManifestacije tip = (ETipManifestacije)Enum.Parse(typeof(ETipManifestacije), delovi[1]);
                Dictionary<Manifestacija, string> manifestacije = (Dictionary<Manifestacija, string>)HttpContext.Current.Application["manifestacije"];

                Manifestacija m = manifestacije.Keys.First(mani => mani.Naziv.ToLower().Equals(delovi[0].ToLower()) &&
                                                                   mani.Tip_Manifestacije.Equals(tip) &&
                                                                   mani.Datum_i_Vreme_Odrzavanja.Equals(delovi[2]) &&
                                                                   mani.Lokacija.Geografska_Sirina.Equals(double.Parse(delovi[3])) &&
                                                                   mani.Lokacija.Geografska_Duzina.Equals(double.Parse(delovi[4])) &&
                                                                   mani.Lokacija.Mesto_Odrzavanja.Ulica.Equals(delovi[5]) &&
                                                                   mani.Lokacija.Mesto_Odrzavanja.Broj.Equals(delovi[6]) &&
                                                                   mani.Lokacija.Mesto_Odrzavanja.Grad.Equals(delovi[7]) &&
                                                                   mani.Lokacija.Mesto_Odrzavanja.Postanski_Broj.Equals(int.Parse(delovi[8]))
                                                            );
                if (m != null)
                {
                    m.Status = true;
                    PomocneMetode.UpisiManifestacijeUFajl(manifestacije);
                    return Request.CreateResponse(HttpStatusCode.OK, "Manifestacija uspesno odobrena");

                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Doslo je do greske, ne moze da pronadje manifestaciju za odobrenje");

                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Niste autorizovani");
            }
        }

        [HttpGet, Route("api/admin/svikomentari")]
        [ResponseType(typeof(List<Komentar>))]
        public HttpResponseMessage SviKomentari()
        {
            Korisnik k = (Korisnik)HttpContext.Current.Session["user"];
            if (k != null && k.Uloga.Equals(EUloga.Administrator))
            {
                List<Komentar> sviKomentari = (List<Komentar>)HttpContext.Current.Application["komentari"];
                return Request.CreateResponse(HttpStatusCode.OK, sviKomentari);

            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Niste autorizovani");
            }
        }

        [HttpGet, Route("api/admin/blokiraj/{id}")]
        [ResponseType(typeof(string))]
        public HttpResponseMessage BlokirajKorisnika(string id)
        {
            Korisnik korisnik = (Korisnik)HttpContext.Current.Session["user"];
            if (korisnik != null && korisnik.Uloga.Equals(EUloga.Administrator))
            {
                Dictionary<string, Korisnik> kupci = (Dictionary<string, Korisnik>)HttpContext.Current.Application["kupci"];
                Dictionary<string, Korisnik> prodavci = (Dictionary<string, Korisnik>)HttpContext.Current.Application["prodavci"];

                if (kupci.ContainsKey(id))
                {
                    kupci[id].Blokiran = true;
                    PomocneMetode.UpisiKupceUFajl(kupci);
                    return Request.CreateResponse(HttpStatusCode.OK, "Kupac uspesno blokiran");
                }
                else if (prodavci.ContainsKey(id))
                {
                    prodavci[id].Blokiran = true;
                    PomocneMetode.UpisiProdavceUFajl(prodavci);
                    return Request.CreateResponse(HttpStatusCode.OK, "Prodavac uspesno blokiran");

                }

                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Nije uspelo blokiranje. Nije pornadjen ni kao kupac ni kao prodavac");

            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Niste autorizovani");
            }
        }

        [HttpGet, Route("api/admin/sumnjivikorisnici")]
        [ResponseType(typeof(List<Korisnik>))]
        public HttpResponseMessage VratiSumnjiveKorisnike()
        {
            Korisnik korisnik = (Korisnik)HttpContext.Current.Session["user"];
            if (korisnik != null && korisnik.Uloga.Equals(EUloga.Administrator))
            {
                Dictionary<string, Korisnik> kupci = (Dictionary<string, Korisnik>)HttpContext.Current.Application["kupci"];

                return Request.CreateResponse(HttpStatusCode.OK, kupci.Values.ToList().FindAll(k => k.Sumnjiv == true && k.LogickiObrisan == false));
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Niste autorizovani");
            }
        }

        [HttpGet, Route("api/admin/svekarte")]
        [ResponseType(typeof(List<Karta>))]
        public HttpResponseMessage VrateSveKarteAdminu()
        {
            Korisnik k = (Korisnik)HttpContext.Current.Session["user"];
            if (k != null && k.Uloga.Equals(EUloga.Administrator))
            {
                Dictionary<Karta, string> sveKarte = (Dictionary<Karta, string>)HttpContext.Current.Application["karte"];
                List<Karta> pomocna = new List<Karta>();
                //  return Request.CreateResponse(HttpStatusCode.OK, sveKarte.Keys.ToList());
                // Logicno je da admin ne moze da obrise karte za manifestacije koje su prosle;
                foreach (Karta karta in sveKarte.Keys)
                {
                    if ((DateTime.Now - DateTime.Parse(karta.Datum_i_Vreme_Manifestacije)).TotalDays < 0)
                    {
                        pomocna.Add(karta);
                    }
                }
                if (pomocna.Count > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, pomocna);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "Nema karti za brisanje");
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Niste autorizovani");
            }
        }

        [HttpPost, Route("api/admin/obrisikartu")]
        [ResponseType(typeof(string))]
        public HttpResponseMessage ObrisiKartu([FromBody] string id)
        {
            Korisnik k = (Korisnik)HttpContext.Current.Session["user"];
            if (k != null && k.Uloga.Equals(EUloga.Administrator))
            {
                // id karte; naziv manifestacije; datum i vreme odrzavanja manifestacije;
                string[] delovi = id.Split(';');

                int index = delovi[2].IndexOf(" ");
                delovi[2] = delovi[2].Insert(index, "T");
                delovi[2] = delovi[2].Replace(" ", string.Empty);

                Dictionary<Karta, string> sveKarte = (Dictionary<Karta, string>)HttpContext.Current.Application["karte"];
                foreach (Karta karta in sveKarte.Keys)
                {
                    if (karta.ID_Karte.Equals(delovi[0]) && karta.Manifestacija.Naziv.ToLower().Equals(delovi[1].ToLower()))
                    {
                        if (karta.Datum_i_Vreme_Manifestacije.Equals(delovi[2]) && karta.Manifestacija.Datum_i_Vreme_Odrzavanja.Equals(delovi[2]))
                        {
                            // za svaki slucaj mozda se potrefe dve karte sa istim id-jem i ostalim vrednostima
                            //kada korisnik rezervise vise od 1 karte;
                            if (karta.LogickiObrisan)
                            {
                                continue;
                            }
                            else
                            {
                                karta.LogickiObrisan = true;
                                PomocneMetode.upisiKarteUFajl(sveKarte);
                                return Request.CreateResponse(HttpStatusCode.OK, "Uspesno obrisana karta");
                            }
                        }
                    }
                }

                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Karta nije pronadjena u listi svih karata");
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Niste autorizovani");
            }
        }
    }
}
