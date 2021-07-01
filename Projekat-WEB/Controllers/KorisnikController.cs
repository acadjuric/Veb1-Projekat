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

    public class KorisnikController : ApiController
    {
        //[HttpPost]
        //[Route("api/korisnik/registracija")]
        [ResponseType(typeof(string))]
        public HttpResponseMessage Post([FromBody]Korisnik korisnik)
        {
            string temp = PomocneMetode.ValidacijaKorisnika(korisnik);
            if (temp.ToLower() != "uspesna registracija")
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, temp);
            }

            Dictionary<string, Korisnik> kupci = (Dictionary<string, Korisnik>)HttpContext.Current.Application["kupci"];
            Dictionary<string, Korisnik> admini = (Dictionary<string, Korisnik>)HttpContext.Current.Application["admini"];
            Dictionary<string, Korisnik> prodavci = (Dictionary<string, Korisnik>)HttpContext.Current.Application["prodavci"];

            Korisnik stari = (Korisnik)HttpContext.Current.Session["zaIzmenu"];
            if (stari != null)
            {
                if (stari.Uloga.Equals(EUloga.Kupac))
                {
                    if (PomocneMetode.ProveriKorisnickoIme(admini.Keys.ToList(), prodavci.Keys.ToList(), kupci.Keys.ToList(), korisnik) || stari.Korisnicko_Ime.Equals(korisnik.Korisnicko_Ime))
                    {
                        kupci.Remove(stari.Korisnicko_Ime);


                        stari.Korisnicko_Ime = korisnik.Korisnicko_Ime;
                        stari.Ime = korisnik.Ime;
                        stari.Lozinka = korisnik.Lozinka;
                        stari.Prezime = korisnik.Prezime;
                        stari.Pol = korisnik.Pol;
                        stari.Datum_Rodjenja = korisnik.Datum_Rodjenja;

                        kupci.Add(stari.Korisnicko_Ime, stari);
                        PomocneMetode.UpisiKupceUFajl(kupci);
                        HttpContext.Current.Session["zaIzmenu"] = stari;
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Korisnicko ime je zauzeto");
                    }

                }
                else if (stari.Uloga.Equals(EUloga.Administrator))
                {
                    if (PomocneMetode.ProveriKorisnickoIme(admini.Keys.ToList(), prodavci.Keys.ToList(), kupci.Keys.ToList(), korisnik) || stari.Korisnicko_Ime.Equals(korisnik.Korisnicko_Ime))
                    {

                        admini.Remove(stari.Korisnicko_Ime);

                        stari.Korisnicko_Ime = korisnik.Korisnicko_Ime;
                        stari.Ime = korisnik.Ime;
                        stari.Lozinka = korisnik.Lozinka;
                        stari.Prezime = korisnik.Prezime;
                        stari.Pol = korisnik.Pol;
                        stari.Datum_Rodjenja = korisnik.Datum_Rodjenja;

                        admini.Add(stari.Korisnicko_Ime, stari);
                        PomocneMetode.UpisiAdmineUFajl(admini);
                        HttpContext.Current.Session["zaIzmenu"] = stari;
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Korisnicko ime je zauzeto");
                    }


                }
                return Request.CreateResponse(HttpStatusCode.OK, "Izmena uspesna");

            }
            else
            {
                if (admini.ContainsKey(korisnik.Korisnicko_Ime))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Korisnicko ime je zauzeto");
                }
                else if (prodavci.ContainsKey(korisnik.Korisnicko_Ime))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Korisnicko ime je zauzeto");
                }
                else if (kupci.ContainsKey(korisnik.Korisnicko_Ime))
                {

                    return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Korisnicko ime je zauzeto");
                }


                korisnik.Uloga = EUloga.Kupac;
                korisnik.TipKorisnika = new TipKorisnika(ETipKorisnika.Pocetnik, 0, 1000);
                korisnik.Broj_Sakupljenih_Bodova = 0;
                korisnik.Blokiran = false;
                korisnik.LogickiObrisan = false;
                korisnik.Sumnjiv = false;
                korisnik.Karte = new List<Karta>();
                korisnik.datumiOtkazivanja = new List<DateTime>();
                kupci.Add(korisnik.Korisnicko_Ime, korisnik);
                PomocneMetode.UpisiKupceUFajl(kupci);
               
                return Request.CreateResponse(HttpStatusCode.OK, "Uspesna registracija");

            }


        }

        [HttpGet, Route("api/korisnik/karte")]
        [ResponseType(typeof(List<Karta>))]
        public HttpResponseMessage VratiKarteKupca()
        {
            Korisnik k = (Korisnik)HttpContext.Current.Session["user"];
            if (k != null)
            {
                Dictionary<Karta, string> karte = (Dictionary<Karta, string>)HttpContext.Current.Application["karte"];
                List<Karta> povratna = new List<Karta>();
                if (k.Uloga.Equals(EUloga.Kupac))
                {
                    povratna.AddRange(k.Karte.FindAll(karta => karta.Status_Karte.Equals(EStatusKarte.Rezervisana)));
                    return Request.CreateResponse(HttpStatusCode.OK, povratna);
                }
                else if (k.Uloga.Equals(EUloga.Prodavac))
                {
                    Dictionary<Manifestacija, string> manifestcije = (Dictionary<Manifestacija, string>)HttpContext.Current.Application["manifestacije"];
                    List<Manifestacija> prodavacManifestacije = new List<Manifestacija>();

                    foreach (KeyValuePair<Manifestacija, string> par in manifestcije)
                    {
                        if (par.Value.Equals(k.Korisnicko_Ime))
                        {
                            prodavacManifestacije.Add(par.Key);
                        }
                    }

                    foreach (Karta karta in karte.Keys)
                    {
                        if (karta.Status_Karte.Equals(EStatusKarte.Rezervisana))
                        {
                            foreach (Manifestacija m in prodavacManifestacije)
                            {
                                if (m.Naziv.Equals(karta.Manifestacija.Naziv) &&
                                    m.Tip_Manifestacije.Equals(karta.Manifestacija.Tip_Manifestacije) &&
                                    m.Datum_i_Vreme_Odrzavanja.Equals(karta.Manifestacija.Datum_i_Vreme_Odrzavanja) &&
                                    m.Lokacija.Mesto_Odrzavanja.Ulica.Equals(karta.Manifestacija.Lokacija.Mesto_Odrzavanja.Ulica) &&
                                    m.Lokacija.Mesto_Odrzavanja.Grad.Equals(karta.Manifestacija.Lokacija.Mesto_Odrzavanja.Grad) &&
                                    m.Lokacija.Mesto_Odrzavanja.Broj.Equals(karta.Manifestacija.Lokacija.Mesto_Odrzavanja.Broj) &&
                                    m.Lokacija.Mesto_Odrzavanja.Postanski_Broj.Equals(karta.Manifestacija.Lokacija.Mesto_Odrzavanja.Postanski_Broj)
                                     )
                                {
                                    povratna.Add(karta);
                                }
                            }
                        }

                    }
                    //if (povratna.Count > 0)
                    //{
                    return Request.CreateResponse(HttpStatusCode.OK, povratna);
                    //}
                    //else
                    //{
                    //    return Request.CreateResponse(HttpStatusCode.NoContent, "Prodavac nema rezervisanih karti za njegove manifestacije");
                    //}
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, karte.Keys.ToList());
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Niste autorizovani");
            }
        }

        [HttpPost, Route("api/korisnik/pretrazi")]
        [ResponseType(typeof(List<Karta>))]
        public HttpResponseMessage PretragaKarata([FromBody]PretragaModel model)
        {
            Dictionary<Karta, string> sveKarte = (Dictionary<Karta, string>)HttpContext.Current.Application["karte"];
            Korisnik k = (Korisnik)HttpContext.Current.Session["user"];
            if (k != null)
            {
                if (k.Uloga.Equals(EUloga.Administrator) || k.Uloga.Equals(EUloga.Prodavac) || k.Uloga.Equals(EUloga.Kupac))
                {
                    string temp = PomocneMetode.ValidacijaZaPretraguKarata(model);
                    if (temp.ToLower() == "uspesna pretraga")
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, PomocneMetode.PretragaKarata(sveKarte.Keys.ToList(), k, model));
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Polja za pretragu nisu dobro popunjena");
                    }
                }
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Niste autorizovani");
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Niste autorizovani");
            }

        }


        [HttpPost, Route("api/korisnik/sortiraj")]
        [ResponseType(typeof(List<Karta>))]
        public HttpResponseMessage SortirajKarte([FromBody]SortiranjeFiltriranjeKarteModel model)
        {
            Korisnik k = (Korisnik)HttpContext.Current.Session["user"];
            if (k != null)
            {
                if (k.Uloga.Equals(EUloga.Administrator) || k.Uloga.Equals(EUloga.Prodavac) || k.Uloga.Equals(EUloga.Kupac))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, PomocneMetode.SortirajKarte(model));
                }

                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Niste autorizovani");
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Niste autorizovani");
            }
        }

        [HttpPost, Route("api/korisnik/filtriraj")]
        [ResponseType(typeof(List<Karta>))]
        public HttpResponseMessage FiltrirajKarte([FromBody]SortiranjeFiltriranjeKarteModel model)
        {
            Korisnik k = (Korisnik)HttpContext.Current.Session["user"];
            if (k != null)
            {
                if (k.Uloga.Equals(EUloga.Administrator) || k.Uloga.Equals(EUloga.Prodavac) || k.Uloga.Equals(EUloga.Kupac))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, PomocneMetode.FiltrirajKarte(model));
                }

                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Niste autorizovani");
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Niste autorizovani");
            }
        }

        [HttpGet, Route("api/korisnik/vratikorisnika")]
        public Korisnik VratiKorisnika()
        {
            Korisnik k = (Korisnik)HttpContext.Current.Session["user"];
            return k;
        }

        [HttpGet, Route("api/korisnik/rezervisi/{brojKarata}/{tipKarte}")]
        [ResponseType(typeof(string))]
        public HttpResponseMessage RezervacijaKarata(int brojKarata, string tipKarte)
        {
            Korisnik korisnik = (Korisnik)HttpContext.Current.Session["user"];
            if (korisnik != null && korisnik.Uloga.Equals(EUloga.Kupac))
            {
                //DODATI ZA KUPCA U NJEGOVU LISTU KARATA, KARTE KOJE JE REZERVISAO
                Manifestacija m = (Manifestacija)HttpContext.Current.Session["ManifestacijaRezervacija"];
                Dictionary<Karta, string> sveKarte = (Dictionary<Karta, string>)HttpContext.Current.Application["karte"];
                Dictionary<string, Korisnik> kupci = (Dictionary<string, Korisnik>)HttpContext.Current.Application["kupci"];
                Dictionary<Manifestacija, string> manifestacije = (Dictionary<Manifestacija, string>)HttpContext.Current.Application["manifestacije"];

                if (brojKarata <= 0)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Greska. Broj karata za rezervaciju mora biti pozitivan ceo broj i veci od nule.");
                }

                double cena;
                //provera da li u manifestaciji preostali broj karata >= zeljenom broju karata (zavisno od tipa)
                if (PomocneMetode.PorveriDaLiImaDovoljnoKarata(m, brojKarata, tipKarte))
                {
                    for (int i = 0; i < brojKarata; i++)
                    {
                        //if (korisnik.Broj_Sakupljenih_Bodova >= korisnik.TipKorisnika.Trazeni_Broj_Bodova)
                        //{
                        //    if (korisnik.TipKorisnika.Ime_Tipa_Korisnika.Equals(ETipKorisnika.Pocetnik))
                        //    {
                        //        korisnik.TipKorisnika.Ime_Tipa_Korisnika = ETipKorisnika.Bronzani;
                        //        korisnik.TipKorisnika.Popust = 10;
                        //        korisnik.TipKorisnika.Trazeni_Broj_Bodova = 2000;
                        //    }
                        //    else if (korisnik.TipKorisnika.Ime_Tipa_Korisnika.Equals(ETipKorisnika.Bronzani))
                        //    {
                        //        korisnik.TipKorisnika.Ime_Tipa_Korisnika = ETipKorisnika.Srebrni;
                        //        korisnik.TipKorisnika.Popust = 20;
                        //        korisnik.TipKorisnika.Trazeni_Broj_Bodova = 3000;

                        //    }
                        //    else if (korisnik.TipKorisnika.Ime_Tipa_Korisnika.Equals(ETipKorisnika.Srebrni))
                        //    {
                        //        korisnik.TipKorisnika.Ime_Tipa_Korisnika = ETipKorisnika.Zlatni;
                        //        korisnik.TipKorisnika.Popust = 30;
                        //        korisnik.TipKorisnika.Trazeni_Broj_Bodova = int.MaxValue;
                        //    }
                        //    PomocneMetode.UpisiKupceUFajl(kupci);
                        //}

                        //ovde vidi da li id karte ima sadrzi karaktere (! $ # @ - + / )
                        string id_karte = Guid.NewGuid().ToString().Substring(0, 8) + Guid.NewGuid().ToString().Substring(0, 2);

                        if (tipKarte.ToLower().Equals("vip"))
                        {

                            int ukupanPopust = 100 - korisnik.TipKorisnika.Popust;
                            cena = (m.Cena_Regularne_Karte * 4 * ukupanPopust) / 100;
                            Karta k = new Karta(id_karte, m, m.Datum_i_Vreme_Odrzavanja, cena, new Kupac(korisnik.Ime, korisnik.Prezime), EStatusKarte.Rezervisana, ETipKarte.VIP, false);
                            double broj_bodova = (cena / 1000) * 133;
                            if ((korisnik.Broj_Sakupljenih_Bodova += broj_bodova) > double.MaxValue)
                            {
                                korisnik.Broj_Sakupljenih_Bodova = double.MaxValue;
                            }
                            else
                            {
                                korisnik.Broj_Sakupljenih_Bodova += broj_bodova;
                            }
                            //Manifestacija manifestacija = manifestacije.Keys.FirstOrDefault(mani => mani.Datum_i_Vreme_Odrzavanja.Equals(k.Datum_i_Vreme_Manifestacije) && mani.Naziv.Equals(k.Manifestacija.Naziv));
                            //smanjiti broj karata(zavisno od tip karte)
                            m.BrojVipMesta--;
                            m.Broj_Mesta--;

                            korisnik.Karte.Add(k);
                            sveKarte.Add(k, korisnik.Korisnicko_Ime);
                        }
                        else if (tipKarte.ToLower().Equals("fan pit"))
                        {
                            int ukupanPopust = 100 - korisnik.TipKorisnika.Popust;
                            cena = (m.Cena_Regularne_Karte * 2 * ukupanPopust) / 100;
                            Karta k = new Karta(id_karte, m, m.Datum_i_Vreme_Odrzavanja, cena, new Kupac(korisnik.Ime, korisnik.Prezime), EStatusKarte.Rezervisana, ETipKarte.FanPit, false);
                            double broj_bodova = (cena / 1000) * 133;
                            if ((korisnik.Broj_Sakupljenih_Bodova += broj_bodova) > double.MaxValue)
                            {
                                korisnik.Broj_Sakupljenih_Bodova = double.MaxValue;
                            }
                            else
                            {
                                korisnik.Broj_Sakupljenih_Bodova += broj_bodova;
                            }
                            // Manifestacija manifestacija = manifestacije.Keys.FirstOrDefault(mani => mani.Datum_i_Vreme_Odrzavanja.Equals(k.Datum_i_Vreme_Manifestacije) && mani.Naziv.Equals(k.Manifestacija.Naziv));
                            //smanjiti broj karata(zavisno od tip karte)
                            m.BrojFanPitMesta--;
                            m.Broj_Mesta--;

                            korisnik.Karte.Add(k);
                            sveKarte.Add(k, korisnik.Korisnicko_Ime);
                        }
                        else if (tipKarte.ToLower().Equals("regular"))
                        {

                            int ukupanPopust = 100 - korisnik.TipKorisnika.Popust;
                            cena = (m.Cena_Regularne_Karte * ukupanPopust) / 100;
                            Karta k = new Karta(id_karte, m, m.Datum_i_Vreme_Odrzavanja, cena, new Kupac(korisnik.Ime, korisnik.Prezime), EStatusKarte.Rezervisana, ETipKarte.Regular, false);
                            double broj_bodova = (cena / 1000) * 133;
                            if ((korisnik.Broj_Sakupljenih_Bodova += broj_bodova) > double.MaxValue)
                            {
                                korisnik.Broj_Sakupljenih_Bodova = double.MaxValue;
                            }
                            else
                            {
                                korisnik.Broj_Sakupljenih_Bodova += broj_bodova;
                            }
                            //Manifestacija manifestacija = manifestacije.Keys.FirstOrDefault(mani => mani.Datum_i_Vreme_Odrzavanja.Equals(k.Datum_i_Vreme_Manifestacije) && mani.Naziv.Equals(k.Manifestacija.Naziv));
                            //smanjiti broj karata(zavisno od tip karte)
                            m.BrojRegularnihMesta--;
                            m.Broj_Mesta--;

                            korisnik.Karte.Add(k);
                            sveKarte.Add(k, korisnik.Korisnicko_Ime);
                        }

                        //provera za tip korisnika
                        if (korisnik.Broj_Sakupljenih_Bodova >= korisnik.TipKorisnika.Trazeni_Broj_Bodova)
                        {
                            if (korisnik.TipKorisnika.Ime_Tipa_Korisnika.Equals(ETipKorisnika.Pocetnik))
                            {
                                korisnik.TipKorisnika.Ime_Tipa_Korisnika = ETipKorisnika.Bronzani;
                                korisnik.TipKorisnika.Popust = 10;
                                korisnik.TipKorisnika.Trazeni_Broj_Bodova = 2000;
                            }
                            else if (korisnik.TipKorisnika.Ime_Tipa_Korisnika.Equals(ETipKorisnika.Bronzani))
                            {
                                korisnik.TipKorisnika.Ime_Tipa_Korisnika = ETipKorisnika.Srebrni;
                                korisnik.TipKorisnika.Popust = 20;
                                korisnik.TipKorisnika.Trazeni_Broj_Bodova = 3000;

                            }
                            else if (korisnik.TipKorisnika.Ime_Tipa_Korisnika.Equals(ETipKorisnika.Srebrni))
                            {
                                korisnik.TipKorisnika.Ime_Tipa_Korisnika = ETipKorisnika.Zlatni;
                                korisnik.TipKorisnika.Popust = 30;
                                korisnik.TipKorisnika.Trazeni_Broj_Bodova = int.MaxValue;
                            }
                            PomocneMetode.UpisiKupceUFajl(kupci);
                        }
                        //upis ide posle svake karte iz bezbedonosnih razloga
                        kupci[korisnik.Korisnicko_Ime] = korisnik;
                        PomocneMetode.UpisiKupceUFajl(kupci);
                        PomocneMetode.upisiKarteUFajl(sveKarte);
                        PomocneMetode.UpisiManifestacijeUFajl(manifestacije);
                    }
                    return Request.CreateResponse(HttpStatusCode.OK, "Karte su rezervisane i upisane u fajl");
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Conflict, "Nije moguca rezervacija. Nema dovoljno mesta za izabrani tip karte (Server salje)");
                }
                //kupci[korisnik.Korisnicko_Ime] = korisnik;
                //PomocneMetode.UpisiKupceUFajl(kupci);
                //PomocneMetode.upisiKarteUFajl(sveKarte);
                //PomocneMetode.UpisiManifestacijeUFajl(manifestacije);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Niste autorizovani");
            }

        }

        [HttpPost, Route("api/korisnik/otkazikartu")]
        [ResponseType(typeof(string))]
        public HttpResponseMessage OtkaziKartu([FromBody]string identifikator)
        {
            Korisnik korisnik = (Korisnik)HttpContext.Current.Session["user"];
            if (korisnik != null && korisnik.Uloga.Equals(EUloga.Kupac))
            {
                Dictionary<Karta, string> sveKarte = (Dictionary<Karta, string>)HttpContext.Current.Application["karte"];
                // List<Karta> pomocna = new List<Karta>();
                Dictionary<string, Korisnik> kupci = (Dictionary<string, Korisnik>)HttpContext.Current.Application["kupci"];
                Dictionary<Manifestacija, string> manifestacije = (Dictionary<Manifestacija, string>)HttpContext.Current.Application["manifestacije"];
                ////Radi se kopiranje podataka, jer ne dozvoljava da se obrise iz liste kroz koju se prolazi
                //foreach(Karta karta in sveKarte)
                //{
                //    pomocna.Add(karta);
                //}
                string[] delovi = identifikator.Split(';');
                foreach (Karta k in sveKarte.Keys)
                {
                    if (delovi[0].Equals(k.ID_Karte))
                    {
                        if (delovi[1].ToLower().Equals(k.Manifestacija.Naziv.ToLower()))
                        {
                            DateTime datum = DateTime.Parse(delovi[2]);
                            DateTime date = DateTime.Parse(k.Datum_i_Vreme_Manifestacije);
                            if (date.Equals(datum))
                            {
                                DateTime danas = DateTime.Now;
                                if ((date - danas).TotalDays >= 7)
                                {
                                    double broj_bodova = (k.Cena / 1000) * 133 * 4;
                                    //sigurnosna provera
                                    if ((korisnik.Broj_Sakupljenih_Bodova -= broj_bodova) < double.MinValue)
                                    {
                                        korisnik.Broj_Sakupljenih_Bodova = double.MinValue;
                                    }
                                    else
                                    {
                                        korisnik.Broj_Sakupljenih_Bodova -= broj_bodova;
                                    }
                                    //provera za tip korisnika 
                                    if (korisnik.Broj_Sakupljenih_Bodova < (korisnik.TipKorisnika.Trazeni_Broj_Bodova - 1000))
                                    {
                                        if (korisnik.TipKorisnika.Ime_Tipa_Korisnika.Equals(ETipKorisnika.Zlatni))
                                        {
                                            korisnik.TipKorisnika.Ime_Tipa_Korisnika = ETipKorisnika.Srebrni;
                                            korisnik.TipKorisnika.Popust = 20;
                                            korisnik.TipKorisnika.Trazeni_Broj_Bodova = 3000;
                                        }
                                        else if (korisnik.TipKorisnika.Ime_Tipa_Korisnika.Equals(ETipKorisnika.Srebrni))
                                        {
                                            korisnik.TipKorisnika.Ime_Tipa_Korisnika = ETipKorisnika.Bronzani;
                                            korisnik.TipKorisnika.Popust = 10;
                                            korisnik.TipKorisnika.Trazeni_Broj_Bodova = 2000;

                                        }
                                        else if (korisnik.TipKorisnika.Ime_Tipa_Korisnika.Equals(ETipKorisnika.Bronzani))
                                        {
                                            korisnik.TipKorisnika.Ime_Tipa_Korisnika = ETipKorisnika.Pocetnik;
                                            korisnik.TipKorisnika.Popust = 0;
                                            korisnik.TipKorisnika.Trazeni_Broj_Bodova = 1000;
                                        }
                                        PomocneMetode.UpisiKupceUFajl(kupci);
                                    }

                                    Manifestacija m = manifestacije.Keys.FirstOrDefault(mani => mani.Datum_i_Vreme_Odrzavanja.Equals(k.Datum_i_Vreme_Manifestacije) &&
                                                                                                mani.Naziv.Equals(k.Manifestacija.Naziv) &&
                                                                                                mani.Tip_Manifestacije.Equals(k.Manifestacija.Tip_Manifestacije) &&
                                                                                                mani.Lokacija.Mesto_Odrzavanja.Ulica.Equals(k.Manifestacija.Lokacija.Mesto_Odrzavanja.Ulica) &&
                                                                                                mani.Lokacija.Mesto_Odrzavanja.Grad.Equals(k.Manifestacija.Lokacija.Mesto_Odrzavanja.Grad) &&
                                                                                                mani.Lokacija.Mesto_Odrzavanja.Broj.Equals(k.Manifestacija.Lokacija.Mesto_Odrzavanja.Broj) &&
                                                                                                mani.Lokacija.Mesto_Odrzavanja.Postanski_Broj.Equals(k.Manifestacija.Lokacija.Mesto_Odrzavanja.Postanski_Broj)
                                                                                                );
                                    //za manifestcaiju M uvecaj broj karata (zavisno od tipa karte)
                                    if (k.TipKarte.Equals(ETipKarte.FanPit))
                                    {
                                        m.BrojFanPitMesta++;
                                    }
                                    else if (k.TipKarte.Equals(ETipKarte.VIP))
                                    {
                                        m.BrojVipMesta++;
                                    }
                                    else if (k.TipKarte.Equals(ETipKarte.Regular))
                                    {
                                        m.BrojRegularnihMesta++;
                                    }
                                    m.Broj_Mesta++;
                                    //promeniti i status karte kod korisnika u listi karata
                                    Karta korisnikovaKarta = korisnik.Karte.Find(karta => karta.Equals(k));
                                    korisnikovaKarta.Status_Karte = EStatusKarte.Odustanak;
                                    k.Status_Karte = EStatusKarte.Odustanak;

                                    Dictionary<DateTime, string> datumi = (Dictionary<DateTime, string>)HttpContext.Current.Application["datumi"];

                                    //PROVERI DA LI SE I DATUMI AZURIRAJU KAD I KORISNIKOVA LISTA OTKAZANIH DATUMA;
                                    //MOZDA TREBA DA SE DODA U U LISTU DATUMI 
                                    datumi.Add(DateTime.Now, korisnik.Korisnicko_Ime);
                                    korisnik.datumiOtkazivanja.Add(DateTime.Now);
                                    PomocneMetode.UpisiDatumeOtkazivanjaUFajl(datumi);
                                    if (korisnik.datumiOtkazivanja.Count > 5)
                                    {
                                        if (PomocneMetode.ProveriDatumeOtkazivanja(korisnik.datumiOtkazivanja))
                                        {
                                            //korisnik ima vise od 5 otkazanih karata u roku od mesec dana
                                            //korisnik postaje sumnjiv;
                                            korisnik.Sumnjiv = true;
                                            //vidi da li se promenli u listi kupaca, MORA OMDAH UPIS U FAJL

                                        }
                                    }

                                    //ide break jer se brise samo jedna karta, moguce je da ima vise karata (korisnik unnosi broj karata koje rezervise) 
                                    // ali samo jednu po jednu moze da otkazuje
                                    break;
                                }
                                else
                                {
                                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Rok za otkazivanje karte prosao.");
                                }

                            }
                        }
                    }
                }
                kupci[korisnik.Korisnicko_Ime] = korisnik;
                PomocneMetode.upisiKarteUFajl(sveKarte);
                PomocneMetode.UpisiKupceUFajl(kupci);
                PomocneMetode.UpisiManifestacijeUFajl(manifestacije);
                return Request.CreateResponse(HttpStatusCode.OK, "Uspelo otkazivanje karte");
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Niste autorizovani");
            }

        }

    }
}
