using Projekat_WEB.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Description;

namespace Projekat_WEB.Controllers
{
    public class ProdavacController : ApiController
    {


        [HttpPost, Route("api/prodavac/kreirajmanifestaciju")]
        [ResponseType(typeof(string))]
        public HttpResponseMessage Post([FromBody]Manifestacija manifestacija)
        {
            Korisnik k = (Korisnik)HttpContext.Current.Session["user"];
            if (k != null && k.Uloga.Equals(EUloga.Prodavac))
            {

                if (PomocneMetode.ProverZaKreiranjeManifestacijeUProslosti(manifestacija))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Ne mozete kreirati manifestaciju u proslosti");
                }

                Dictionary<Manifestacija, string> manifestacije = (Dictionary<Manifestacija, string>)HttpContext.Current.Application["manifestacije"];

                // PREPRAVLJA GRAD DA BUDE LATINICNO ZBOG PRETRAGE
                manifestacija = PomocneMetode.PretvoriCirilicuULatinicuZaGradoveZbogPretrage(manifestacija);

                //provera da li vec postoji manifestacija u zeljenom vremenu i na izabranoj lokaciji
                if (PomocneMetode.ProveraZaKreiranjeManifestacije(manifestacije, manifestacija))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Greska. Postoji vec manifestacija za izabranu lokaciju, datum i vreme");
                }

                if (PomocneMetode.ProveraZaNazivITipManifestacije(manifestacije.Keys.ToList(), manifestacija))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Postoji vec manifestacija sa istim nazivom i tipom");
                }

                //validacija
                string temp = PomocneMetode.ValidacijaZaManifestaciju(manifestacija);
                if (temp.ToLower() != "uspesno kreirana manifestacija")
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, temp);
                }

                //NOVA MANIFESTACIJA TREBA DA SE UPISE I DODA;
                manifestacija.Status = false;
                List<string> dozvoljeneEkstenzije = new List<string>() { "jpg", "png", "jpeg", "gif" };
                string naziv_slike = manifestacija.Putanja_do_slike.Split('\\')[2];

                var ekstenzija = naziv_slike.Split('.')[1];
                if (!dozvoljeneEkstenzije.Contains(ekstenzija))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Molimo vas izaberite sliku. Dozvoljeni formati: JPG, PNG, JPEG, GIF");
                }
                manifestacija.Putanja_do_slike = naziv_slike;
                manifestacija.Broj_Mesta = manifestacija.BrojRegularnihMesta + manifestacija.BrojVipMesta + manifestacija.BrojFanPitMesta;

                

                manifestacije.Add(manifestacija, k.Korisnicko_Ime);

                //DODATI MANIFESTACIJU U KORISNIKOVU LISTU MANIFESTACIJE AKO JE KORISNIK PRODAVAC
                k.Manifestacije.Add(manifestacija);

                PomocneMetode.UpisiManifestacijeUFajl(manifestacije);
                return Request.CreateResponse(HttpStatusCode.OK, "");
            }
            else
            {

                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Niste autorizovani");
            }

        }
        [HttpPost, Route("api/prodavac/izmenimanifestaciju")]
        [ResponseType(typeof(string))]
        public HttpResponseMessage IzmeniManifestaciju([FromBody]Manifestacija manifestacija)
        {
            Korisnik k = (Korisnik)HttpContext.Current.Session["user"];
            if (k != null && k.Uloga.Equals(EUloga.Prodavac))
            {
                if (PomocneMetode.ProverZaKreiranjeManifestacijeUProslosti(manifestacija))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Ne mozete izmeniti manifestaciju a datum da bude u proslosti");
                }

                Dictionary<Manifestacija, string> manifestacije = (Dictionary<Manifestacija, string>)HttpContext.Current.Application["manifestacije"];

                //validacija
                string temp = PomocneMetode.ValidacijaZaManifestaciju(manifestacija);
                if (temp.ToLower() != "uspesno kreirana manifestacija")
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, temp);
                }
                Manifestacija staraManifestacija = (Manifestacija)HttpContext.Current.Session["ManifestacijaZaIzmenu"];
                if (staraManifestacija != null)
                {

                    //ukoliko je izbrana nova slika mora opet provera za dozvoljene ekstenzije
                    List<string> dozvoljeneEkstenzije = new List<string>() { "jpg", "png", "jpeg", "gif" };
                    string naziv_slike = manifestacija.Putanja_do_slike.Split('\\')[2];
                    manifestacija.Putanja_do_slike = naziv_slike;
                    var ekstenzija = naziv_slike.Split('.')[1];
                    if (!dozvoljeneEkstenzije.Contains(ekstenzija))
                    {
                        //manifestacije.Add(staraManifestacija, k.Korisnicko_Ime);
                        return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Molimo vas izaberite sliku. Dozvoljeni formati: JPG, PNG, JPEG, GIF");
                    }


                    // brisemo odmah da ne bi pravilo problem za lokaciju i datum ako nisu promenjeni, gledace sve ostale mani sto i treba, ova za izmenu se nece 
                    //gledati
                    manifestacije.Remove(staraManifestacija);

                    // PREPRAVLJA GRAD DA BUDE LATINICNO ZBOG PRETRAGE
                    // OVO NEKA IDE PRE PROVERE DA LI JE DOSLO DO PROMENE LOKACIJE ZBOG NAZIVA GRADA NA KARTAMA; // PROVERA TA SAMO OVDE, NE TREBA KOD KERIRANJA
                    manifestacija = PomocneMetode.PretvoriCirilicuULatinicuZaGradoveZbogPretrage(manifestacija);

                    //provera da li vec postoji manifestacija u zeljenom vremenu i na izabranoj lokaciji
                    if (PomocneMetode.ProveraZaKreiranjeManifestacije(manifestacije, manifestacija))
                    {
                        manifestacije.Add(staraManifestacija, k.Korisnicko_Ime);
                        return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Greska. Postoji vec manifestacija za izabranu lokaciju, datum i vreme");
                    }

                    if (PomocneMetode.ProveraZaNazivITipManifestacije(manifestacije.Keys.ToList(), manifestacija))
                    {
                        manifestacije.Add(staraManifestacija, k.Korisnicko_Ime);
                        return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Postoji vec manifestacija sa istim nazivom i tipom");
                    }



                    Dictionary<Karta, string> sveKarte = (Dictionary<Karta, string>)HttpContext.Current.Application["karte"];
                    if (PomocneMetode.ProveraZaIzmenuManifestacijeIBrojaKarata(sveKarte, staraManifestacija, manifestacija))
                    {
                        //moze da se uradi izmena;
                        manifestacija.Broj_Mesta = manifestacija.BrojRegularnihMesta + manifestacija.BrojVipMesta + manifestacija.BrojFanPitMesta;
                        // provera da li je izmenjeno neko polje za manifestaciju koje se nalazi na karti ako jeste zameni i na karti
                        if (!staraManifestacija.Datum_i_Vreme_Odrzavanja.Equals(manifestacija.Datum_i_Vreme_Odrzavanja) ||
                            !staraManifestacija.Lokacija.Mesto_Odrzavanja.Grad.ToLower().Equals(manifestacija.Lokacija.Mesto_Odrzavanja.Grad.ToLower()) ||
                            !staraManifestacija.Lokacija.Mesto_Odrzavanja.Ulica.ToLower().Equals(manifestacija.Lokacija.Mesto_Odrzavanja.Ulica.ToLower()) ||
                            !staraManifestacija.Lokacija.Mesto_Odrzavanja.Broj.ToLower().Equals(manifestacija.Lokacija.Mesto_Odrzavanja.Broj.ToLower()) ||
                            !staraManifestacija.Lokacija.Mesto_Odrzavanja.Postanski_Broj.Equals(manifestacija.Lokacija.Mesto_Odrzavanja.Postanski_Broj) ||
                            !staraManifestacija.Naziv.ToLower().Equals(manifestacija.Naziv.ToLower()) ||
                            !staraManifestacija.Tip_Manifestacije.Equals(manifestacija.Tip_Manifestacije)
                            )
                        {
                            //promenjeno neko polje na manifestaciji koje se nalazi na karti -> promeni ga na svim kartama za tu manifesaciju
                            //PROVERI DA LI SE MENJAJU DATUMI KARTE U sveKarte?????
                            PomocneMetode.PromeniDatumNaKartamaZaManifestaciju(sveKarte, staraManifestacija, manifestacija);
                            //upisi karte u fajl

                            PomocneMetode.upisiKarteUFajl(sveKarte);
                        }


                        
                        //ako je stara bila odobrena bice odmah i izmenjena
                        if (staraManifestacija.Status)
                            manifestacija.Status = true;

                        k.Manifestacije.Remove(staraManifestacija);



                        k.Manifestacije.Add(manifestacija);

                        //obrisano na pocetku prethodnog uslova zbog izmene manifestacije (lokacije i datuma i vremena odrzavanja)
                        //manifestacije.Remove(staraManifestacija);
                        manifestacije.Add(manifestacija, k.Korisnicko_Ime);
                        PomocneMetode.UpisiManifestacijeUFajl(manifestacije);
                        return Request.CreateResponse(HttpStatusCode.OK, "");
                    }
                    else
                    {
                        manifestacije.Add(staraManifestacija, k.Korisnicko_Ime);
                        return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Greska. Izmena nije moguca. Greska kod broja mesta za karte");
                    }

                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Manifestacija nije u sessiji za izmenu");
                }


            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Niste autorizovani");
            }
        }

        [HttpGet, Route("api/prodavac/manifestacije")]
        [ResponseType(typeof(List<Manifestacija>))]
        public HttpResponseMessage ManifestacijeProdavca()
        {
            // kada uvedes sesiju uradi da se vracaju samo manifestacije odredjenog prodavca
            Korisnik k = (Korisnik)HttpContext.Current.Session["user"];
            if (k != null && k.Uloga.Equals(EUloga.Prodavac))
            {
                return Request.CreateResponse(HttpStatusCode.OK, k.Manifestacije.FindAll(m => m.LogickiObrisan == false));
            }
            else
            {
                //Dictionary<Manifestacija, string> manifestacije = (Dictionary<Manifestacija, string>)HttpContext.Current.Application["manifestacije"];
                //return manifestacije.Keys.ToList();
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Niste autorizovani");
            }
            //List<Manifestacija> mani = new List<Manifestacija>();
            //if(k != null)
            //{
            //    foreach(KeyValuePair<Manifestacija,string> par in manifestacije)
            //    {
            //        if (par.Value.Equals(k.Korisnicko_Ime))
            //        {
            //            mani.Add(par.Key);
            //        }
            //    }

            //    return mani;
            //}
            //return manifestacije.Keys.ToList();
        }

        [HttpPost, Route("api/prodavac/komentar")]
        [ResponseType(typeof(string))]
        public HttpResponseMessage KomentarZaManifestaciju([FromBody]Komentar komentar)
        {
            Korisnik k = (Korisnik)HttpContext.Current.Session["user"];
            if (k != null && k.Uloga.Equals(EUloga.Kupac))
            {
                Manifestacija m = (Manifestacija)HttpContext.Current.Session["komentar"];
                List<Komentar> komentari = (List<Komentar>)HttpContext.Current.Application["komentari"];
                if (m != null)
                {
                    string temp = PomocneMetode.ValidacijaZaKomentarIOcenau(komentar);
                    if (temp.ToLower() == "komentar uspesno kreiran")
                    {
                        Kupac kupac = new Kupac(k.Korisnicko_Ime, k.Prezime);
                        komentar.Kupac = kupac;
                        komentar.Manifestacija = m;
                        komentar.Odobren = false;
                        komentar.Odbijen = false;
                        komentar.LogickiObrisan = false;
                        komentari.Add(komentar);
                        PomocneMetode.UpisiKomentareUFajl(komentari);
                        return Request.CreateResponse(HttpStatusCode.OK, "Komentar poslat i ceka odobrenje");
                    }

                    return Request.CreateErrorResponse(HttpStatusCode.NotAcceptable, "Komentar i ocena nisu prosli validaciju");
                }
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Doslo je do greske ne moze da najde korisnika ili manifestaciju za komentar");
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Nite autorizovani");
            }
        }

        [HttpGet, Route("api/prodavac/vratikomentare")]
        [ResponseType(typeof(List<Komentar>))]
        public HttpResponseMessage VratiKomentareZaManifestaciju()
        {
            Korisnik korisnik = (Korisnik)HttpContext.Current.Session["user"];
            if (korisnik != null)
            {
                if (korisnik.Uloga.Equals(EUloga.Administrator) || korisnik.Uloga.Equals(EUloga.Prodavac) || korisnik.Uloga.Equals(EUloga.Kupac))
                {
                    Manifestacija m = (Manifestacija)HttpContext.Current.Session["komentar"];
                    List<Komentar> sviKomentari = (List<Komentar>)HttpContext.Current.Application["komentari"];

                    if (m != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, sviKomentari.FindAll(k => k.Manifestacija.Naziv.Equals(m.Naziv) && k.Manifestacija.Tip_Manifestacije.Equals(m.Tip_Manifestacije) &&
                                                                                                   k.Manifestacija.Datum_i_Vreme_Odrzavanja.Equals(m.Datum_i_Vreme_Odrzavanja)

                                                   ));
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Kod komentara za manifestaciju. Manifestacija je iz nekog razloga null");
                    }
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Niste autorizovani");
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Niste autorizovani");
            }
        }

        [HttpGet, Route("api/prodavac/svikomentari")]
        [ResponseType(typeof(List<Komentar>))]
        public HttpResponseMessage SviKomentariManifestacijaOdredjenogProdavca()
        {
            Korisnik k = (Korisnik)HttpContext.Current.Session["user"];
            List<Komentar> sviKomentari = (List<Komentar>)HttpContext.Current.Application["komentari"];
            List<Komentar> pomocna = new List<Komentar>();
            //ovaj if bi trebao svugde zbog AUTORIZACIJE PRISTUPA METODAMA

            if (k != null && k.Uloga.Equals(EUloga.Prodavac))
            {
                foreach (Manifestacija m in k.Manifestacije)
                {
                    foreach (Komentar kom in sviKomentari)
                    {
                        if (m.Naziv.Equals(kom.Manifestacija.Naziv) && m.Tip_Manifestacije.Equals(kom.Manifestacija.Tip_Manifestacije) &&
                            m.Datum_i_Vreme_Odrzavanja.Equals(kom.Manifestacija.Datum_i_Vreme_Odrzavanja))
                        {
                            pomocna.Add(kom);
                        }
                    }
                }
                if (pomocna.Count > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, pomocna);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NoContent, "");
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Niste autorizovani");
            }
        }

        [HttpPost, Route("api/prodavac/odobri")]
        [ResponseType(typeof(string))]
        public HttpResponseMessage OdobriKomentar([FromBody]string id)
        {
            Korisnik korisnik = (Korisnik)HttpContext.Current.Session["user"];
            if (korisnik != null && korisnik.Uloga.Equals(EUloga.Prodavac))
            {
                //korisnicko ime ; tekst komentara
                string[] delovi = id.Split(';');
                List<Komentar> sviKomentari = (List<Komentar>)HttpContext.Current.Application["komentari"];
                Dictionary<Manifestacija, string> manifestacije = (Dictionary<Manifestacija, string>)HttpContext.Current.Application["manifestacije"];
                foreach (Komentar k in sviKomentari)
                {
                    if (k.Kupac.Ime.Equals(delovi[0]))
                    {
                        if (k.Tekst_komentara.Equals(delovi[1]))
                        {
                            //ova provera mozda je jedan korisnik dao vise istih komentara a jedan od njih je odobren a drugi nije
                            if (k.Odobren) { continue; }
                            else
                            {
                                k.Odobren = true;
                                PomocneMetode.UpisiKomentareUFajl(sviKomentari);
                                HttpContext.Current.Application["manifestacije"] = PomocneMetode.IzracunajProsecnuOcenuZaManifestacije(manifestacije, sviKomentari, true);
                                return Request.CreateResponse(HttpStatusCode.OK, "Komentar uspesno odobren");
                            }
                        }
                    }
                }
                return Request.CreateErrorResponse(HttpStatusCode.Conflict, "Kod odobravanja. Komentar nije pronadjen u listi svih komentara");
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Niste autorizovani");
            }

        }

        [HttpPost, Route("api/prodavac/odbij")]
        [ResponseType(typeof(string))]
        public HttpResponseMessage OdbijKOmentar([FromBody]string id)
        {
            Korisnik korisnik = (Korisnik)HttpContext.Current.Session["user"];
            if (korisnik != null && korisnik.Uloga.Equals(EUloga.Prodavac))
            {
                //korisnicko ime ; tekst komentara
                string[] delovi = id.Split(';');
                List<Komentar> sviKomentari = (List<Komentar>)HttpContext.Current.Application["komentari"];
                foreach (Komentar k in sviKomentari)
                {
                    if (k.Kupac.Ime.Equals(delovi[0]))
                    {
                        if (k.Tekst_komentara.Equals(delovi[1]))
                        {
                            //ova provera mozda je jedan korisnik dao vise istih komentara a jedan od njih je odbijen a drugi nije
                            if (k.Odbijen) { continue; }
                            else
                            {
                                k.Odbijen = true;
                                PomocneMetode.UpisiKomentareUFajl(sviKomentari);
                                return Request.CreateResponse(HttpStatusCode.OK, "Komentar uspesno odbijen");

                            }
                        }
                    }
                }
                return Request.CreateErrorResponse(HttpStatusCode.Conflict, "Kod odbijanja. Komentar nije pronadjen u listi svih komentara");
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Niste autorizovani");
            }

        }


        [HttpPost, Route("api/prodavac/sacuvajsliku")]
        [ResponseType(typeof(string))]
        public HttpResponseMessage SacuvajSliku()
        {
            Korisnik k = (Korisnik)HttpContext.Current.Session["user"];
            if (k != null && k.Uloga.Equals(EUloga.Prodavac))
            {
                var fajl = HttpContext.Current.Request.Files["Fajl"];
                if (fajl != null)
                {

                    string putanja = Path.Combine(HttpContext.Current.Server.MapPath("~/Images/"), fajl.FileName);
                    fajl.SaveAs(putanja);
                    return Request.CreateResponse(HttpStatusCode.OK, "");
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Fajl nije dobro postavljen u form data");
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Niste autorizovani");
            }
        }
    }
}
