using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;

namespace Projekat_WEB.Models
{
    public class PomocneMetode
    {
        public static Dictionary<string, Korisnik> UcitajKorisnikIzFajla(string path)
        {
            string putanja = HostingEnvironment.MapPath(path);
            Dictionary<string, Korisnik> korisnici = new Dictionary<string, Korisnik>();
            FileStream fs = new FileStream(putanja, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            string linija = "";
            while ((linija = sr.ReadLine()) != null)
            {
                string[] delovi = linija.Split(';');
                EPol pol = (EPol)Enum.Parse(typeof(EPol), delovi[4]);
                ETipKorisnika tipKorisnika = (ETipKorisnika)Enum.Parse(typeof(ETipKorisnika), delovi[7]);
                EUloga uloga = (EUloga)Enum.Parse(typeof(EUloga), delovi[5]);

                if (uloga.Equals(EUloga.Administrator))
                {
                    Korisnik k = new Korisnik(delovi[0], delovi[1], delovi[2], delovi[3], pol, EUloga.Administrator, delovi[6], tipKorisnika, int.Parse(delovi[9]), int.Parse(delovi[10]), bool.Parse(delovi[11]));
                    k.Broj_Sakupljenih_Bodova = double.Parse(delovi[8]);
                    k.Blokiran = bool.Parse(delovi[12]);
                    korisnici.Add(k.Korisnicko_Ime, k);
                }
                else if (uloga.Equals(EUloga.Kupac))
                {
                    Korisnik k = new Korisnik(delovi[0], delovi[1], delovi[2], delovi[3], pol, EUloga.Kupac, delovi[6], tipKorisnika, int.Parse(delovi[9]), int.Parse(delovi[10]), bool.Parse(delovi[11]));
                    k.Broj_Sakupljenih_Bodova = double.Parse(delovi[8]);
                    k.Blokiran = bool.Parse(delovi[12]);
                    k.Sumnjiv = bool.Parse(delovi[13]);
                    korisnici.Add(k.Korisnicko_Ime, k);

                }
                else if (uloga.Equals(EUloga.Prodavac))
                {
                    Korisnik k = new Korisnik(delovi[0], delovi[1], delovi[2], delovi[3], pol, EUloga.Prodavac, delovi[6], tipKorisnika, int.Parse(delovi[9]), int.Parse(delovi[10]), bool.Parse(delovi[11]));
                    k.Broj_Sakupljenih_Bodova = double.Parse(delovi[8]);
                    k.Blokiran = bool.Parse(delovi[12]);
                    korisnici.Add(k.Korisnicko_Ime, k);
                }


            }
            sr.Close();
            fs.Close();
            return korisnici;

        }

        public static void UpisiKupceUFajl(Dictionary<string, Korisnik> kupci)
        {

            string putanja = HostingEnvironment.MapPath("~/App_Data/kupci.txt");
            FileStream fs = new FileStream(putanja, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            foreach (Korisnik k in kupci.Values)
            {
                sw.WriteLine(k.Korisnicko_Ime + ";" + k.Lozinka + ";" + k.Ime + ";" + k.Prezime + ";" + k.Pol + ";" + k.Uloga + ";" + k.Datum_Rodjenja + ";" +
                    k.TipKorisnika.Ime_Tipa_Korisnika + ";" + k.Broj_Sakupljenih_Bodova + ";" + k.TipKorisnika.Trazeni_Broj_Bodova + ";" + k.TipKorisnika.Popust + ";" +
                    k.LogickiObrisan + ";" + k.Blokiran + ";" + k.Sumnjiv);
            }
            sw.Close();
            fs.Close();

        }

        public static void UpisiAdmineUFajl(Dictionary<string, Korisnik> admini)
        {

            string putanja = HostingEnvironment.MapPath("~/App_Data/administratori.txt");
            FileStream fs = new FileStream(putanja, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            foreach (Korisnik k in admini.Values)
            {
                sw.WriteLine(k.Korisnicko_Ime + ";" + k.Lozinka + ";" + k.Ime + ";" + k.Prezime + ";" + k.Pol + ";" + k.Uloga + ";" + k.Datum_Rodjenja + ";" +
                    k.TipKorisnika.Ime_Tipa_Korisnika + ";" + k.Broj_Sakupljenih_Bodova + ";" + k.TipKorisnika.Trazeni_Broj_Bodova + ";" + k.TipKorisnika.Popust + ";" + k.LogickiObrisan + ";" + k.Blokiran);
            }
            sw.Close();
            fs.Close();

        }


        public static void UpisiProdavceUFajl(Dictionary<string, Korisnik> prodavci)
        {

            string putanja = HostingEnvironment.MapPath("~/App_Data/prodavci.txt");
            FileStream fs = new FileStream(putanja, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            foreach (Korisnik k in prodavci.Values)
            {
                sw.WriteLine(k.Korisnicko_Ime + ";" + k.Lozinka + ";" + k.Ime + ";" + k.Prezime + ";" + k.Pol + ";" + k.Uloga + ";" + k.Datum_Rodjenja + ";" +
                    k.TipKorisnika.Ime_Tipa_Korisnika + ";" + k.Broj_Sakupljenih_Bodova + ";" + k.TipKorisnika.Trazeni_Broj_Bodova + ";" + k.TipKorisnika.Popust + ";" + k.LogickiObrisan + ";" + k.Blokiran);
            }
            sw.Close();
            fs.Close();

        }

        public static void UpisiManifestacijeUFajl(Dictionary<Manifestacija, string> manifestacije)
        {
            string putanja = HostingEnvironment.MapPath("~/App_Data/ProdavciManifestacije.txt");
            FileStream fs = new FileStream(putanja, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);

            foreach (KeyValuePair<Manifestacija, string> par in manifestacije)
            {
                sw.WriteLine(par.Value + ";" + par.Key.Naziv + ";" + par.Key.Tip_Manifestacije + ";" + par.Key.BrojRegularnihMesta + ";" + par.Key.BrojVipMesta
                    + ";" + par.Key.BrojFanPitMesta + ";" + par.Key.Datum_i_Vreme_Odrzavanja + ";" + par.Key.Cena_Regularne_Karte
                     + ";" + par.Key.Lokacija.Mesto_Odrzavanja.Ulica + ";" + par.Key.Lokacija.Mesto_Odrzavanja.Grad + ";" +
                    par.Key.Lokacija.Mesto_Odrzavanja.Postanski_Broj + ";" + par.Key.Putanja_do_slike + ";" + par.Key.Status + ";" + par.Key.LogickiObrisan
                    + ";" + par.Key.Lokacija.Geografska_Duzina + ";" + par.Key.Lokacija.Geografska_Sirina + ";" + par.Key.Lokacija.Mesto_Odrzavanja.Broj);
            }

            sw.Close();
            fs.Close();
        }

        public static Dictionary<Manifestacija, string> UcitajManifestacijeIZFajla(string path)
        {
            string putanja = HostingEnvironment.MapPath(path);
            Dictionary<Manifestacija, string> manifestacije = new Dictionary<Manifestacija, string>();
            FileStream fs = new FileStream(putanja, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            string linija = "";
            while ((linija = sr.ReadLine()) != null)
            {
                string[] delovi = linija.Split(';');


                MestoOdrzavanja mesto = new MestoOdrzavanja(delovi[8], delovi[16], delovi[9], int.Parse(delovi[10]));
                ETipManifestacije tip = (ETipManifestacije)Enum.Parse(typeof(ETipManifestacije), delovi[2]);

                bool STATUS = bool.Parse(delovi[12]);
                Lokacija lokacija = new Lokacija(double.Parse(delovi[14]), double.Parse(delovi[15]), mesto);

                Manifestacija m = new Manifestacija(delovi[1], tip, int.Parse(delovi[3]), int.Parse(delovi[4]), int.Parse(delovi[5]), delovi[6], int.Parse(delovi[7]), lokacija, delovi[11], bool.Parse(delovi[13]));
                m.Status = bool.Parse(delovi[12]);
                m.Broj_Mesta = m.BrojRegularnihMesta + m.BrojVipMesta + m.BrojFanPitMesta;
                manifestacije.Add(m, delovi[0]);
            }

            sr.Close();
            fs.Close();
            return manifestacije;
        }

        public static Dictionary<Karta, string> UcitajKarteIzFajla(string path)
        {
            string putanja = HostingEnvironment.MapPath(path);
            Dictionary<Karta, string> karte = new Dictionary<Karta, string>();
            Dictionary<Manifestacija, string> manifestacije = UcitajManifestacijeIZFajla("~/App_Data/ProdavciManifestacije.txt");
            FileStream fs = new FileStream(putanja, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            string linija = "";
            while ((linija = sr.ReadLine()) != null)
            {
                string[] delovi = linija.Split(';');
                Kupac k = new Kupac(delovi[0], delovi[1]);
                ETipManifestacije tip = (ETipManifestacije)Enum.Parse(typeof(ETipManifestacije), delovi[4]);
                EStatusKarte statusKarte = (EStatusKarte)Enum.Parse(typeof(EStatusKarte), delovi[7]);
                ETipKarte tipKarte = (ETipKarte)Enum.Parse(typeof(ETipKarte), delovi[8]);

                Manifestacija m = manifestacije.Keys.FirstOrDefault(mani => mani.Naziv.Equals(delovi[3]) &&
                                                                            mani.Tip_Manifestacije.Equals(tip) &&
                                                                            mani.Lokacija.Mesto_Odrzavanja.Ulica.Equals(delovi[11]) &&
                                                                            mani.Lokacija.Mesto_Odrzavanja.Grad.Equals(delovi[12]) &&
                                                                            mani.Lokacija.Mesto_Odrzavanja.Broj.Equals(delovi[14]) &&
                                                                            mani.Lokacija.Mesto_Odrzavanja.Postanski_Broj.Equals(int.Parse(delovi[13]))
                                                                            );

                Karta karta = new Karta(delovi[2], m, m.Datum_i_Vreme_Odrzavanja, double.Parse(delovi[6]), k, statusKarte, tipKarte, bool.Parse(delovi[9]));
                string korisnicko_ime = delovi[10];
                karte.Add(karta, korisnicko_ime);

            }
            sr.Close();
            fs.Close();
            return karte;
        }

        public static void upisiKarteUFajl(Dictionary<Karta, string> sveKarte)
        {
            string putanja = HostingEnvironment.MapPath("~/App_Data/KupciKarte.txt");
            FileStream fs = new FileStream(putanja, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            foreach (KeyValuePair<Karta, string> k in sveKarte)
            {
                sw.WriteLine(k.Key.Kupac.Ime + ";" + k.Key.Kupac.Prezime + ";" + k.Key.ID_Karte + ";" + k.Key.Manifestacija.Naziv + ";" + k.Key.Manifestacija.Tip_Manifestacije + ";" +
                    k.Key.Manifestacija.Datum_i_Vreme_Odrzavanja + ";" + k.Key.Cena + ";" + k.Key.Status_Karte + ";" + k.Key.TipKarte + ";" + k.Key.LogickiObrisan + ";" + k.Value
                    + ";" + k.Key.Manifestacija.Lokacija.Mesto_Odrzavanja.Ulica + ";" + k.Key.Manifestacija.Lokacija.Mesto_Odrzavanja.Grad + ";" + k.Key.Manifestacija.Lokacija.Mesto_Odrzavanja.Postanski_Broj
                    + ";" + k.Key.Manifestacija.Lokacija.Mesto_Odrzavanja.Broj);
            }
            sw.Close();
            fs.Close();
        }

        public static List<Komentar> UcitajKomentareIzFajla(string path)
        {
            string putanja = HostingEnvironment.MapPath(path);
            List<Komentar> komentari = new List<Komentar>();
            Dictionary<Manifestacija, string> manifestacije = UcitajManifestacijeIZFajla("~/App_Data/ProdavciManifestacije.txt");
            FileStream fs = new FileStream(putanja, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            string linija = "";
            while ((linija = sr.ReadLine()) != null)
            {
                string[] delovi = linija.Split(';');
                Kupac kupac = new Kupac(delovi[0], delovi[1]);
                ETipManifestacije tip = (ETipManifestacije)Enum.Parse(typeof(ETipManifestacije), delovi[3]);
                Manifestacija mani = manifestacije.Keys.FirstOrDefault(m => m.Naziv.Equals(delovi[2]) && m.Tip_Manifestacije.Equals(tip) && m.Datum_i_Vreme_Odrzavanja.Equals(delovi[4]));

                Komentar ko = new Komentar(kupac, mani, delovi[5], int.Parse(delovi[6]));
                ko.Odobren = bool.Parse(delovi[7]);
                ko.Odbijen = bool.Parse(delovi[8]);
                ko.LogickiObrisan = bool.Parse(delovi[9]);

                komentari.Add(ko);
            }
            sr.Close();
            fs.Close();
            return komentari;
        }

        public static void UpisiKomentareUFajl(List<Komentar> komentari)
        {
            string putanja = HostingEnvironment.MapPath("~/App_Data/komentari.txt");
            FileStream fs = new FileStream(putanja, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            foreach (Komentar k in komentari)
            {
                //zbog parsiranja kada ucitavam komentare iz fajla
                Regex sablon = new Regex("[;]");
                k.Tekst_komentara = sablon.Replace(k.Tekst_komentara, ".");

                sw.WriteLine(k.Kupac.Ime + ";" + k.Kupac.Prezime + ";" + k.Manifestacija.Naziv + ";" + k.Manifestacija.Tip_Manifestacije + ";" + k.Manifestacija.Datum_i_Vreme_Odrzavanja
                    + ";" + k.Tekst_komentara + ";" + k.Ocena + ";" + k.Odobren + ";" + k.Odbijen + ";" + k.LogickiObrisan);
            }
            sw.Close();
            fs.Close();
        }

        public static void UpisiDatumeOtkazivanjaUFajl(Dictionary<DateTime, string> datumi)
        {
            string putanja = HostingEnvironment.MapPath("~/App_Data/DatumiOtkazivanja.txt");
            FileStream fs = new FileStream(putanja, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            foreach (KeyValuePair<DateTime, string> par in datumi)
            {
                sw.WriteLine(par.Key + ";" + par.Value);
            }
            sw.Close();
            fs.Close();
        }

        public static Dictionary<DateTime, string> UcitajDatumeOtkazivanjaIzFajla(string path)
        {
            string putanja = HostingEnvironment.MapPath(path);
            FileStream fs = new FileStream(putanja, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            string linija = "";
            Dictionary<DateTime, string> datumi = new Dictionary<DateTime, string>();
            while ((linija = sr.ReadLine()) != null)
            {
                string[] delovi = linija.Split(';');

                datumi.Add(DateTime.Parse(delovi[0]), delovi[1]);

            }
            sr.Close();
            fs.Close();
            return datumi;
        }

        public static List<Manifestacija> PretragaManifestacija(PretragaModel model, List<Manifestacija> lista)
        {
            List<Manifestacija> pomocna = new List<Manifestacija>();
            if (model.Naziv != null && model.MestoOdrzavanja != null && model.DatumOD != null && model.CenaDO > 0)
            {
                model.DatumDO += "T23:59";
                model.DatumOD += "T00:00";
                DateTime datumDOmodel = DateTime.Parse(model.DatumDO);
                DateTime datumODmodel = DateTime.Parse(model.DatumOD);

                pomocna.AddRange(lista.FindAll(m => m.Cena_Regularne_Karte >= model.CenaOD &&
                                            m.Cena_Regularne_Karte <= model.CenaDO &&
                                            m.Naziv.ToLower().Contains(model.Naziv.ToLower()) &&
                                            m.Lokacija.Mesto_Odrzavanja.Grad.ToLower().Contains(model.MestoOdrzavanja.ToLower()) &&
                                            DateTime.Parse(m.Datum_i_Vreme_Odrzavanja) >= datumODmodel &&
                                            DateTime.Parse(m.Datum_i_Vreme_Odrzavanja) <= datumDOmodel
                                            ));
            }
            else if (model.Naziv != null && model.MestoOdrzavanja != null && model.DatumOD != null && model.CenaDO == 0)
            {
                //cena nije uneta
                model.DatumDO += "T23:59";
                model.DatumOD += "T00:00";
                DateTime datumDOmodel = DateTime.Parse(model.DatumDO);
                DateTime datumODmodel = DateTime.Parse(model.DatumOD);

                pomocna.AddRange(lista.FindAll(m => m.Naziv.ToLower().Contains(model.Naziv.ToLower()) &&
                                                    m.Lokacija.Mesto_Odrzavanja.Grad.ToLower().Contains(model.MestoOdrzavanja.ToLower()) &&
                                                    DateTime.Parse(m.Datum_i_Vreme_Odrzavanja) >= datumODmodel &&
                                                    DateTime.Parse(m.Datum_i_Vreme_Odrzavanja) <= datumDOmodel
                                            ));
            }
            else if (model.Naziv != null && model.MestoOdrzavanja != null && model.CenaDO > 0 && model.DatumDO == null)
            {
                //datum nije unet
                pomocna.AddRange(lista.FindAll(m => m.Cena_Regularne_Karte >= model.CenaOD &&
                                           m.Cena_Regularne_Karte <= model.CenaDO &&
                                           m.Naziv.ToLower().Contains(model.Naziv.ToLower()) &&
                                           m.Lokacija.Mesto_Odrzavanja.Grad.ToLower().Contains(model.MestoOdrzavanja.ToLower())
                                           ));

            }
            else if (model.Naziv != null && model.MestoOdrzavanja == null && model.CenaDO > 0 && model.DatumDO != null)
            {
                //grad nije unet
                model.DatumDO += "T23:59";
                model.DatumOD += "T00:00";
                DateTime datumDOmodel = DateTime.Parse(model.DatumDO);
                DateTime datumODmodel = DateTime.Parse(model.DatumOD);

                pomocna.AddRange(lista.FindAll(m => m.Cena_Regularne_Karte >= model.CenaOD &&
                                            m.Cena_Regularne_Karte <= model.CenaDO &&
                                            m.Naziv.ToLower().Contains(model.Naziv.ToLower()) &&
                                            DateTime.Parse(m.Datum_i_Vreme_Odrzavanja) >= datumODmodel &&
                                            DateTime.Parse(m.Datum_i_Vreme_Odrzavanja) <= datumDOmodel
                                            ));
            }
            else if (model.Naziv == null && model.MestoOdrzavanja != null && model.CenaDO > 0 && model.DatumDO != null)
            {
                //naziv nije unet
                model.DatumDO += "T23:59";
                model.DatumOD += "T00:00";
                DateTime datumDOmodel = DateTime.Parse(model.DatumDO);
                DateTime datumODmodel = DateTime.Parse(model.DatumOD);

                pomocna.AddRange(lista.FindAll(m => m.Cena_Regularne_Karte >= model.CenaOD &&
                                            m.Cena_Regularne_Karte <= model.CenaDO &&
                                            m.Lokacija.Mesto_Odrzavanja.Grad.ToLower().Contains(model.MestoOdrzavanja.ToLower()) &&
                                            DateTime.Parse(m.Datum_i_Vreme_Odrzavanja) >= datumODmodel &&
                                            DateTime.Parse(m.Datum_i_Vreme_Odrzavanja) <= datumDOmodel
                                            ));
            }
            else if (model.Naziv != null && model.MestoOdrzavanja != null && model.CenaDO == 0 && model.DatumDO == null)
            {
                //cena i datum nisu uneti
                pomocna.AddRange(lista.FindAll(m => m.Naziv.ToLower().Contains(model.Naziv.ToLower()) &&
                                                    m.Lokacija.Mesto_Odrzavanja.Grad.ToLower().Contains(model.MestoOdrzavanja.ToLower())
                                            ));
            }
            else if (model.Naziv != null && model.MestoOdrzavanja == null && model.DatumDO != null && model.CenaDO == 0)
            {
                //grad i cena nisu uneti
                model.DatumDO += "T23:59";
                model.DatumOD += "T00:00";
                DateTime datumDOmodel = DateTime.Parse(model.DatumDO);
                DateTime datumODmodel = DateTime.Parse(model.DatumOD);

                pomocna.AddRange(lista.FindAll(m => m.Naziv.ToLower().Contains(model.Naziv.ToLower()) &&
                                                    DateTime.Parse(m.Datum_i_Vreme_Odrzavanja) >= datumODmodel &&
                                                    DateTime.Parse(m.Datum_i_Vreme_Odrzavanja) <= datumDOmodel
                                            ));
            }
            else if (model.Naziv != null && model.MestoOdrzavanja == null && model.DatumDO == null && model.CenaDO > 0)
            {
                //grad i datum nisu uneti
                pomocna.AddRange(lista.FindAll(m => m.Cena_Regularne_Karte >= model.CenaOD &&
                                            m.Cena_Regularne_Karte <= model.CenaDO &&
                                            m.Naziv.ToLower().Contains(model.Naziv.ToLower())
                                            ));
            }
            else if (model.Naziv == null && model.MestoOdrzavanja != null && model.DatumDO != null && model.CenaDO == 0)
            {
                //naziv i cena nisu uneti
                model.DatumDO += "T23:59";
                model.DatumOD += "T00:00";
                DateTime datumDOmodel = DateTime.Parse(model.DatumDO);
                DateTime datumODmodel = DateTime.Parse(model.DatumOD);

                pomocna.AddRange(lista.FindAll(m => m.Lokacija.Mesto_Odrzavanja.Grad.ToLower().Contains(model.MestoOdrzavanja.ToLower()) &&
                                                    DateTime.Parse(m.Datum_i_Vreme_Odrzavanja) >= datumODmodel &&
                                                    DateTime.Parse(m.Datum_i_Vreme_Odrzavanja) <= datumDOmodel
                                                ));
            }
            else if (model.Naziv == null && model.MestoOdrzavanja != null && model.DatumDO == null && model.CenaDO > 0)
            {
                //naziv i datum nisu uneti                
                pomocna.AddRange(lista.FindAll(m => m.Cena_Regularne_Karte >= model.CenaOD &&
                                                    m.Cena_Regularne_Karte <= model.CenaDO &&
                                                    m.Lokacija.Mesto_Odrzavanja.Grad.ToLower().Contains(model.MestoOdrzavanja.ToLower())
                                            ));
            }
            else if (model.Naziv == null && model.MestoOdrzavanja == null && model.DatumDO != null && model.CenaDO > 0)
            {
                //naziv i grad nisu uneti
                model.DatumDO += "T23:59";
                model.DatumOD += "T00:00";
                DateTime datumDOmodel = DateTime.Parse(model.DatumDO);
                DateTime datumODmodel = DateTime.Parse(model.DatumOD);

                pomocna.AddRange(lista.FindAll(m => m.Cena_Regularne_Karte >= model.CenaOD &&
                                                    m.Cena_Regularne_Karte <= model.CenaDO &&
                                                    DateTime.Parse(m.Datum_i_Vreme_Odrzavanja) >= datumODmodel &&
                                                    DateTime.Parse(m.Datum_i_Vreme_Odrzavanja) <= datumDOmodel
                                            ));
            }
            else if (model.Naziv != null && model.MestoOdrzavanja == null && model.DatumDO == null && model.CenaDO == 0)
            {
                //samo naziv unet
                pomocna.AddRange(lista.FindAll(m => m.Naziv.ToLower().Contains(model.Naziv.ToLower())));
            }
            else if (model.Naziv == null && model.MestoOdrzavanja != null && model.DatumDO == null && model.CenaDO == 0)
            {
                //samo grad unet
                pomocna.AddRange(lista.FindAll(m => m.Lokacija.Mesto_Odrzavanja.Grad.ToLower().Contains(model.MestoOdrzavanja.ToLower())));
            }
            else if (model.Naziv == null && model.MestoOdrzavanja == null && model.DatumDO != null && model.CenaDO == 0)
            {
                //samo datum unet
                model.DatumDO += "T23:59";
                model.DatumOD += "T00:00";
                DateTime datumDOmodel = DateTime.Parse(model.DatumDO);
                DateTime datumODmodel = DateTime.Parse(model.DatumOD);

                pomocna.AddRange(lista.FindAll(m => DateTime.Parse(m.Datum_i_Vreme_Odrzavanja) >= datumODmodel && DateTime.Parse(m.Datum_i_Vreme_Odrzavanja) <= datumDOmodel));
            }
            else if (model.Naziv == null && model.MestoOdrzavanja == null && model.DatumDO == null && model.CenaDO > 0)
            {
                //samo cena uneta
                pomocna.AddRange(lista.FindAll(m => m.Cena_Regularne_Karte >= model.CenaOD && m.Cena_Regularne_Karte <= model.CenaDO));
            }

            // return pomocna.GroupBy(m => new { m.Datum_i_Vreme_Odrzavanja, m.Mesto_Odrzavanja.Ulica_i_Broj,m.Mesto_Odrzavanja.Grad}).Select(m=> m.First()).ToList();
            return pomocna;

        }

        public static List<Manifestacija> SortirajManifestacije(List<Manifestacija> lista, string izbor)
        {

            if (izbor.ToLower().Equals("naziv"))
            {
                return lista.OrderBy(m => m.Naziv).ToList();
            }
            else if (izbor.ToLower().Equals("cenakarte"))
            {
                return lista.OrderBy(m => m.Cena_Regularne_Karte).ToList();
            }
            else if (izbor.ToLower().Equals("datumvreme"))
            {
                return lista.OrderBy(m => m.Datum_i_Vreme_Odrzavanja).ToList();
            }
            else if (izbor.ToLower().Equals("mestoodrzavanja"))
            {
                return lista.OrderBy(m => m.Lokacija.Mesto_Odrzavanja.Grad).ToList();
            }
            else
            {
                return lista;
            }
        }

        public static List<Manifestacija> FiltrirajManifestacije(List<Manifestacija> lista, string izbor)
        {
            List<Manifestacija> filtrirana = new List<Manifestacija>();
            if (izbor.ToLower().Equals("koncert"))
            {
                foreach (Manifestacija m in lista)
                {
                    if (m.Tip_Manifestacije.Equals(ETipManifestacije.Koncert))
                    {
                        filtrirana.Add(m);
                    }
                }
            }
            else if (izbor.ToLower().Equals("festival"))
            {
                foreach (Manifestacija m in lista)
                {
                    if (m.Tip_Manifestacije.Equals(ETipManifestacije.Festival))
                    {
                        filtrirana.Add(m);
                    }
                }
            }
            else if (izbor.ToLower().Equals("pozoriste"))
            {
                foreach (Manifestacija m in lista)
                {
                    if (m.Tip_Manifestacije.Equals(ETipManifestacije.Pozoriste))
                    {
                        filtrirana.Add(m);
                    }
                }
            }
            else if (izbor.ToLower().Equals("predstava"))
            {
                foreach (Manifestacija m in lista)
                {
                    if (m.Tip_Manifestacije.Equals(ETipManifestacije.Predstava))
                    {
                        filtrirana.Add(m);
                    }
                }
            }
            else if (izbor.ToLower().Equals("nerasprodate"))
            {
                foreach (Manifestacija m in lista)
                {
                    if (m.Broj_Mesta > 0)
                    {
                        if ((DateTime.Now - DateTime.Parse(m.Datum_i_Vreme_Odrzavanja)).TotalDays < 0)
                            filtrirana.Add(m);
                    }
                }
            }
            else
            {
                filtrirana = lista;
            }

            return filtrirana;


        }

        private static List<Karta> FunkcijaPretrageKarata(List<Karta> lista, PretragaModel model)
        {
            List<Karta> finalna = new List<Karta>();

            if (model.Naziv != null && model.DatumOD != null && model.CenaDO > 0)
            {
                //unet svaki parametar u formi za pretragu
                model.DatumDO += "T23:59";
                model.DatumOD += "T00:00";
                DateTime datumDOmodel = DateTime.Parse(model.DatumDO);
                DateTime datumODmodel = DateTime.Parse(model.DatumOD);

                finalna.AddRange(lista.FindAll(karta => karta.Manifestacija.Naziv.ToLower().Contains(model.Naziv.ToLower()) &&
                                                          DateTime.Parse(karta.Datum_i_Vreme_Manifestacije) >= datumODmodel &&
                                                          DateTime.Parse(karta.Datum_i_Vreme_Manifestacije) <= datumDOmodel &&
                                                          karta.Cena <= model.CenaDO && karta.Cena >= model.CenaOD
                                                ));
            }
            else if (model.Naziv != null && model.DatumDO != null && model.CenaDO == 0)
            {
                //uneti naziv i datum
                model.DatumDO += "T23:59";
                model.DatumOD += "T00:00";
                DateTime datumDOmodel = DateTime.Parse(model.DatumDO);
                DateTime datumODmodel = DateTime.Parse(model.DatumOD);

                finalna.AddRange(lista.FindAll(karta => karta.Manifestacija.Naziv.ToLower().Contains(model.Naziv.ToLower()) &&
                                                          DateTime.Parse(karta.Datum_i_Vreme_Manifestacije) >= datumODmodel &&
                                                          DateTime.Parse(karta.Datum_i_Vreme_Manifestacije) <= datumDOmodel
                                                ));

            }
            else if (model.Naziv != null && model.DatumDO == null && model.CenaDO > 0)
            {
                //uneti naziv i cena
                finalna.AddRange(lista.FindAll(karta => karta.Manifestacija.Naziv.ToLower().Contains(model.Naziv.ToLower()) &&
                                                          karta.Cena <= model.CenaDO && karta.Cena >= model.CenaOD
                                                ));
            }
            else if (model.Naziv == null && model.DatumDO != null && model.CenaDO > 0)
            {
                //uneti datum i cena
                model.DatumDO += "T23:59";
                model.DatumOD += "T00:00";
                DateTime datumDOmodel = DateTime.Parse(model.DatumDO);
                DateTime datumODmodel = DateTime.Parse(model.DatumOD);

                finalna.AddRange(lista.FindAll(karta => DateTime.Parse(karta.Datum_i_Vreme_Manifestacije) >= datumODmodel &&
                                                          DateTime.Parse(karta.Datum_i_Vreme_Manifestacije) <= datumDOmodel &&
                                                          karta.Cena <= model.CenaDO && karta.Cena >= model.CenaOD
                                                ));
            }
            else if (model.Naziv != null && model.DatumDO == null && model.CenaDO == 0)
            {
                //samo naziv unet
                finalna.AddRange(lista.FindAll(karta => karta.Manifestacija.Naziv.ToLower().Contains(model.Naziv.ToLower())));
            }
            else if (model.Naziv == null && model.DatumDO != null && model.CenaDO == 0)
            {
                //samo datum unet
                model.DatumDO += "T23:59";
                model.DatumOD += "T00:00";
                DateTime datumDOmodel = DateTime.Parse(model.DatumDO);
                DateTime datumODmodel = DateTime.Parse(model.DatumOD);

                finalna.AddRange(lista.FindAll(karta => DateTime.Parse(karta.Datum_i_Vreme_Manifestacije) >= datumODmodel &&
                                                          DateTime.Parse(karta.Datum_i_Vreme_Manifestacije) <= datumDOmodel
                                                ));
            }
            else if (model.Naziv == null && model.DatumDO == null && model.CenaDO > 0)
            {
                //samo cena uneta
                finalna.AddRange(lista.FindAll(karta => karta.Cena <= model.CenaDO && karta.Cena >= model.CenaOD));
            }
            return finalna;
        }

        public static List<Karta> PretragaKarata(List<Karta> sveKarte, Korisnik k, PretragaModel model)
        {
            List<Karta> pomocna = new List<Karta>();
            List<Karta> finalna = new List<Karta>();
            if (k.Uloga.Equals(EUloga.Kupac))
            {
                // sve karte od kupca
                //pomocna = sveKarte.FindAll(karta => karta.Kupac.Ime.Equals(k.Ime) && karta.Kupac.Prezime.Equals(k.Prezime));
                pomocna = k.Karte;
                return FunkcijaPretrageKarata(pomocna, model);
            }
            else if (k.Uloga.Equals(EUloga.Prodavac))
            {
                Dictionary<Manifestacija, string> manifestcije = (Dictionary<Manifestacija, string>)HttpContext.Current.Application["manifestacije"];
                List<Manifestacija> prodavacManifestacije = new List<Manifestacija>();
                List<Karta> sveKarteProdavca = new List<Karta>();

                //Ovaj foreach izdvaja manifestacije za odredjenog prodavca
                //foreach (KeyValuePair<Manifestacija, string> par in manifestcije)
                //{
                //    if (par.Value.Equals(k.Korisnicko_Ime))
                //    {
                //        prodavacManifestacije.Add(par.Key);
                //    }
                //}
                prodavacManifestacije = k.Manifestacije;

                //ovde izdvajam sve karte za tog prodavca
                foreach (Karta karta in sveKarte)
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
                            sveKarteProdavca.Add(karta);
                        }
                    }
                }
                //pretraga ide kroz sve karte prodavca
                finalna = FunkcijaPretrageKarata(sveKarteProdavca.Where(kartica => kartica.Status_Karte.Equals(EStatusKarte.Rezervisana)).ToList(), model);
                return finalna;

            }
            else
            {
                return FunkcijaPretrageKarata(sveKarte, model);
            }
        }

        public static List<Karta> SortirajKarte(SortiranjeFiltriranjeKarteModel model)
        {
            if (model.Nacin.ToLower().Equals("opadajuce"))
            {
                if (model.Izbor.ToLower().Equals("naziv"))
                {

                    return model.Lista.OrderByDescending(k => k.Manifestacija.Naziv).ToList();
                }
                else if (model.Izbor.ToLower().Equals("datumvreme"))
                {
                    return model.Lista.OrderByDescending(k => k.Datum_i_Vreme_Manifestacije).ToList();
                }
                else if (model.Izbor.ToLower().Equals("cenakarte"))
                {
                    return model.Lista.OrderByDescending(k => k.Cena).ToList();
                }
                else
                {
                    return model.Lista;
                }
            }
            else if (model.Nacin.ToLower().Equals("rastuce"))
            {
                if (model.Izbor.ToLower().Equals("naziv"))
                {

                    return model.Lista.OrderBy(k => k.Manifestacija.Naziv).ToList();
                }
                else if (model.Izbor.ToLower().Equals("datumvreme"))
                {
                    return model.Lista.OrderBy(k => k.Datum_i_Vreme_Manifestacije).ToList();
                }
                else if (model.Izbor.ToLower().Equals("cenakarte"))
                {
                    return model.Lista.OrderBy(k => k.Cena).ToList();
                }
                else
                {
                    return model.Lista;
                }

            }
            else
            {
                return model.Lista;
            }

        }

        public static List<Karta> FiltrirajKarte(SortiranjeFiltriranjeKarteModel model)
        {
            List<Karta> pomocna = new List<Karta>();
            if (model.Izbor.ToLower().Equals("vip"))
            {
                pomocna = model.Lista.FindAll(k => k.TipKarte.Equals(ETipKarte.VIP));
            }
            else if (model.Izbor.ToLower().Equals("regular"))
            {
                pomocna = model.Lista.FindAll(k => k.TipKarte.Equals(ETipKarte.Regular));
            }
            else if (model.Izbor.ToLower().Equals("fanpit"))
            {
                pomocna = model.Lista.FindAll(k => k.TipKarte.Equals(ETipKarte.FanPit));
            }
            else if (model.Izbor.ToLower().Equals("rezervisana"))
            {
                pomocna = model.Lista.FindAll(k => k.Status_Karte.Equals(EStatusKarte.Rezervisana));
            }
            else if (model.Izbor.ToLower().Equals("odustana"))
            {
                pomocna = model.Lista.FindAll(k => k.Status_Karte.Equals(EStatusKarte.Odustanak));
            }
            else
            {
                pomocna = model.Lista;
            }
            return pomocna;
        }

        public static List<Korisnik> PretraziKorisnike(List<Korisnik> lista, PretragaModel model)
        {
            List<Korisnik> pomocna = new List<Korisnik>();
            if (model.Ime != null && model.Prezime != null && model.Kime != null)
            {
                //uneta sva 3 param za pretragu korisnika
                pomocna.AddRange(lista.FindAll(k => k.Ime.ToLower().Contains(model.Ime.ToLower()) &&
                                                    k.Prezime.ToLower().Contains(model.Prezime.ToLower()) &&
                                                    k.Korisnicko_Ime.ToLower().Contains(model.Kime.ToLower())
                                ));
            }
            else if (model.Ime != null && model.Prezime != null && model.Kime == null)
            {
                //uneto ime i prezime
                pomocna.AddRange(lista.FindAll(k => k.Ime.ToLower().Contains(model.Ime.ToLower()) &&
                                                k.Prezime.ToLower().Contains(model.Prezime.ToLower())
                               ));

            }
            else if (model.Ime != null && model.Prezime == null && model.Kime != null)
            {
                //uneto ime i korisnicko ime
                pomocna.AddRange(lista.FindAll(k => k.Ime.ToLower().Contains(model.Ime.ToLower()) &&
                                                    k.Korisnicko_Ime.ToLower().Contains(model.Kime.ToLower())
                                ));
            }
            else if (model.Ime == null && model.Prezime != null && model.Kime != null)
            {
                //uneto prezime i korisnicko ime
                pomocna.AddRange(lista.FindAll(k => k.Prezime.ToLower().Contains(model.Prezime.ToLower()) &&
                                                    k.Korisnicko_Ime.ToLower().Contains(model.Kime.ToLower())
                                ));
            }
            else if (model.Ime != null && model.Prezime == null && model.Kime == null)
            {
                //uneto samo ime
                pomocna.AddRange(lista.FindAll(k => k.Ime.ToLower().Contains(model.Ime.ToLower())));
            }
            else if (model.Ime == null && model.Prezime != null && model.Kime == null)
            {
                //uneto samo  prezime
                pomocna.AddRange(lista.FindAll(k => k.Prezime.ToLower().Contains(model.Prezime.ToLower())));
            }
            else if (model.Ime == null && model.Prezime == null && model.Kime != null)
            {
                //uneto samo korisnicko ime
                pomocna.AddRange(lista.FindAll(k => k.Korisnicko_Ime.ToLower().Contains(model.Kime.ToLower())));
            }

            //return pomocna.GroupBy(k => k.Korisnicko_Ime).Select(k => k.First()).ToList();
            return pomocna;
        }

        public static List<Korisnik> SortirajKorisnike(SortiranjeFiltriranjeKarteModel model)
        {
            if (model.ListaK == null)
            {
                return new List<Korisnik>();
            }

            if (model.Nacin.ToLower().Equals("opadajuce"))
            {
                if (model.Izbor.ToLower().Equals("ime"))
                {
                    return model.ListaK.OrderByDescending(k => k.Ime).ToList();
                }
                else if (model.Izbor.ToLower().Equals("prezime"))
                {
                    return model.ListaK.OrderByDescending(k => k.Prezime).ToList();
                }
                else if (model.Izbor.ToLower().Equals("kime"))
                {
                    return model.ListaK.OrderByDescending(k => k.Korisnicko_Ime).ToList();
                }
                else if (model.Izbor.ToLower().Equals("bodovi"))
                {
                    return model.ListaK.OrderByDescending(k => k.Broj_Sakupljenih_Bodova).ToList();
                }
                else
                {
                    return model.ListaK;
                }
            }
            else if (model.Nacin.ToLower().Equals("rastuce"))
            {
                if (model.Izbor.ToLower().Equals("ime"))
                {
                    return model.ListaK.OrderBy(k => k.Ime).ToList();
                }
                else if (model.Izbor.ToLower().Equals("prezime"))
                {
                    return model.ListaK.OrderBy(k => k.Prezime).ToList();
                }
                else if (model.Izbor.ToLower().Equals("kime"))
                {
                    return model.ListaK.OrderBy(k => k.Korisnicko_Ime).ToList();
                }
                else if (model.Izbor.ToLower().Equals("bodovi"))
                {
                    return model.ListaK.OrderBy(k => k.Broj_Sakupljenih_Bodova).ToList();
                }
                else
                {
                    return model.ListaK;
                }
            }
            else
            {
                return model.ListaK;
            }
        }

        public static List<Korisnik> FiltrirajKorisnike(SortiranjeFiltriranjeKarteModel model)
        {
            if (model.ListaK == null)
            {
                return new List<Korisnik>();
            }

            if (model.Izbor.ToLower().Equals("zlatni"))
            {
                return model.ListaK.FindAll(k => k.TipKorisnika.Ime_Tipa_Korisnika.Equals(ETipKorisnika.Zlatni));
            }
            else if (model.Izbor.ToLower().Equals("srebrni"))
            {
                return model.ListaK.FindAll(k => k.TipKorisnika.Ime_Tipa_Korisnika.Equals(ETipKorisnika.Srebrni));
            }
            else if (model.Izbor.ToLower().Equals("bronzani"))
            {
                return model.ListaK.FindAll(k => k.TipKorisnika.Ime_Tipa_Korisnika.Equals(ETipKorisnika.Bronzani));
            }
            else if (model.Izbor.ToLower().Equals("pocetnik"))
            {
                return model.ListaK.FindAll(k => k.TipKorisnika.Ime_Tipa_Korisnika.Equals(ETipKorisnika.Pocetnik));
            }
            else if (model.Izbor.ToLower().Equals("kupac"))
            {
                return model.ListaK.FindAll(k => k.Uloga.Equals(EUloga.Kupac));
            }
            else if (model.Izbor.ToLower().Equals("prodavac"))
            {
                return model.ListaK.FindAll(k => k.Uloga.Equals(EUloga.Prodavac));
            }
            else if (model.Izbor.ToLower().Equals("administrator"))
            {
                return model.ListaK.FindAll(k => k.Uloga.Equals(EUloga.Administrator));
            }
            else
            {
                return model.ListaK;
            }


        }

        public static bool PorveriDaLiImaDovoljnoKarata(Manifestacija manifestacija, int brojKarti, string tipKarte)
        {
            if (tipKarte.Equals("vip"))
            {
                if (manifestacija.BrojVipMesta >= brojKarti)
                    return true;

                else return false;
            }
            else if (tipKarte.Equals("fan pit"))
            {
                if (manifestacija.BrojFanPitMesta >= brojKarti)
                    return true;

                else return false;
            }
            else if (tipKarte.Equals("regular"))
            {
                if (manifestacija.BrojRegularnihMesta >= brojKarti)
                    return true;

                else return false;
            }
            return false;
        }

        public static string ValidacijaKorisnika(Korisnik korisnik)
        {
            double temp;
            if (string.IsNullOrEmpty(korisnik.Korisnicko_Ime) || string.IsNullOrWhiteSpace(korisnik.Korisnicko_Ime))
            {
                return "Korisnicko ime mora biti popunjeno";
            }
            if (korisnik.Korisnicko_Ime.Length < 3)
            {
                return "Korisnicko ime ne moze biti manje od 3 karaktera";
            }
            if (double.TryParse(korisnik.Korisnicko_Ime, out temp))
            {
                return "Korisnicko ime ne moze da bude samo broj";
            }
            if (string.IsNullOrWhiteSpace(korisnik.Lozinka) || string.IsNullOrEmpty(korisnik.Lozinka))
            {
                return "Morate uneti lozinku";
            }
            if (korisnik.Lozinka.Length < 5)
            {
                return "Lozinka mora imati najmanje 5 karaktera";

            }
            if (string.IsNullOrEmpty(korisnik.Ime) || string.IsNullOrWhiteSpace(korisnik.Ime))
            {
                return "Ime mora biti popunjeno";
            }
            if (korisnik.Ime.Length < 3)
            {
                return "Ime ne moze biti manje od 3 karaktera";
            }
            if (double.TryParse(korisnik.Ime, out temp))
            {
                return "Ime ne moze da bude samo broj";
            }
            if (string.IsNullOrEmpty(korisnik.Prezime) || string.IsNullOrWhiteSpace(korisnik.Prezime))
            {
                return "Prezime mora biti popunjeno";
            }
            if (korisnik.Prezime.Length < 4)
            {
                return "Prezime ne moze biti manje od 4 karaktera";
            }
            if (double.TryParse(korisnik.Prezime, out temp))
            {
                return "Prezime ne moze da bude samo broj";
            }
            if (string.IsNullOrEmpty(korisnik.Pol.ToString()) || string.IsNullOrWhiteSpace(korisnik.Pol.ToString()))
            {
                return "Morate izabrati pol";
            }
            if (korisnik.Pol.ToString().ToLower() != "musko" && korisnik.Pol.ToString().ToLower() != "zensko")
            {
                return "Niste dobro izabrali pol. Izaberite ili Musko ili Zensko";
            }
            if (string.IsNullOrEmpty(korisnik.Datum_Rodjenja) || string.IsNullOrWhiteSpace(korisnik.Datum_Rodjenja))
            {
                return "Morate izabrati Datum rodjennja";
            }
            if (korisnik.Datum_Rodjenja != null)
            {
                if ((DateTime.Now - DateTime.Parse(korisnik.Datum_Rodjenja)).TotalDays < 0)
                {
                    return "Datum Rodjenja ne moze biti u buducnosti";
                }
            }


            return "Uspesna registracija";

        }

        public static string ValidacijaZaPretraguKarata(PretragaModel model)
        {
            double temp;
            if (model.Naziv != null)
            {
                if (double.TryParse(model.Naziv, out temp))
                {
                    return "Naziv za pretragu karata ne moze da bude broj";
                }
            }
            if (model.DatumOD != null && model.DatumDO == null)
            {
                return "Popunite lepo polja za datum kako bi pretraga karata bila uspesna";
            }
            if (model.DatumDO != null && model.DatumOD == null)
            {
                return "Popunite lepo polja za datum kako bi pretraga karata bila uspesna";
            }
            if (model.CenaOD > 0)
            {
                if (model.CenaDO == 0)
                {
                    return "Popunite lepo polja za cenu kako bi pretraga karata bila uspesna";
                }
            }
            if (model.CenaOD < 0 || model.CenaDO < 0)
            {
                return "Cena ne sme da bude negativan broj";
            }
            if (model.CenaDO == 0 && model.DatumOD == null && model.DatumDO == null && model.Naziv == null)
            {
                return "Popunite bar 1 parametar za pretragu kako bi ista bila uspesna";
            }
            return "Uspesna pretraga";
        }

        public static string ValidacijaZaKomentarIOcenau(Komentar komentar)
        {
            if (komentar.Ocena < 1 || komentar.Ocena > 5)
            {
                return "Ocena moze da bude u opsegu od 1 do 5";
            }
            if (string.IsNullOrEmpty(komentar.Tekst_komentara) || string.IsNullOrWhiteSpace(komentar.Tekst_komentara))
            {
                return "Komentar za manifesaciju ne moze da bude prazan";
            }
            if (komentar.Tekst_komentara.Trim().Length < 10)
            {
                return "komentar mora imati bar 10 karaktera";
            }

            return "Komentar uspesno kreiran";
        }

        public static string ValidacijaZaPretraguManifestacija(PretragaModel model)
        {
            double temp;
            if (model.Naziv != null)
            {
                if (double.TryParse(model.Naziv, out temp))
                {
                    return "Naziv za pretragu karata ne moze da bude broj";
                }
            }
            if (model.MestoOdrzavanja != null)
            {
                if (double.TryParse(model.MestoOdrzavanja, out temp))
                {
                    return "Mesto odrzavanja (grad/drzava) za pretragu karata ne moze da bude broj";
                }
            }
            if (model.DatumOD != null && model.DatumDO == null)
            {
                return "Popunite lepo polja za datum kako bi pretraga karata bila uspesna";
            }
            if (model.DatumDO != null && model.DatumOD == null)
            {
                return "Popunite lepo polja za datum kako bi pretraga karata bila uspesna";
            }
            if (model.CenaOD > 0)
            {
                if (model.CenaDO == 0)
                {
                    return "Popunite lepo polja za cenu kako bi pretraga karata bila uspesna";
                }
            }
            if (model.CenaOD < 0 || model.CenaDO < 0)
            {
                return "Cena ne sme da bude negativan broj";
            }
            if (model.Naziv == null && model.MestoOdrzavanja == null && model.DatumOD == null && model.DatumDO == null && model.CenaDO == 0)
            {
                return "Popunite bar 1 parametar za pretragu kako bi ista bila uspesna";
            }
            return "Uspesna pretraga";

        }

        public static string ValidacijaZaPretraguKorisnika(PretragaModel model)
        {
            double temp;
            if (model.Ime != null)
            {
                if (double.TryParse(model.Ime, out temp))
                {
                    return "Ime ne moze da bude broj. Pretraga korisnika";
                }
            }
            if (model.Prezime != null)
            {
                if (double.TryParse(model.Prezime, out temp))
                {
                    return "Prezima ne moze da bude broj. Pretraga korisnika";
                }
            }
            if (model.Kime != null)
            {
                if (double.TryParse(model.Kime, out temp))
                {
                    return "Korisnicko ime ne moze da bude broj. Pretraga korisnika";
                }
            }

            return "Uspesna pretraga";
        }

        public static string ValidacijaZaManifestaciju(Manifestacija manifestacija)
        {
            double temp;
            if (string.IsNullOrEmpty(manifestacija.Naziv) || string.IsNullOrWhiteSpace(manifestacija.Naziv))
            {
                return "Naziv manifestacije ne sme da bude prazan";
            }
            if (manifestacija.Naziv != null)
            {
                if (double.TryParse(manifestacija.Naziv, out temp))
                {
                    return "Naziv manifestacije ne sme da bude broj";
                }
            }
            if (manifestacija.Naziv.Length < 4)
            {
                return "Naziv manifesacije ne sme da bude manji od 4 karaktera";
            }
            if (string.IsNullOrWhiteSpace(manifestacija.Tip_Manifestacije.ToString()) || string.IsNullOrEmpty(manifestacija.Tip_Manifestacije.ToString()))
            {
                return "Izaberite tip manifestacije";
            }
            if (manifestacija.Tip_Manifestacije != ETipManifestacije.Festival && manifestacija.Tip_Manifestacije != ETipManifestacije.Koncert &&
                manifestacija.Tip_Manifestacije != ETipManifestacije.Pozoriste && manifestacija.Tip_Manifestacije != ETipManifestacije.Predstava)
            {
                return "Niste lepo izabrali tip manifestacije";
            }
            if (manifestacija.BrojRegularnihMesta < 0)
            {
                return "Broj regularnih mesta ne moze da bude negativan broj";
            }
            if (manifestacija.BrojVipMesta < 0)
            {
                return "Broj vip mesta ne moze da bude negativan broj";
            }
            if (manifestacija.BrojFanPitMesta < 0)
            {
                return "Broj fan pit mesta ne moze da bude negativan broj";
            }
            if (string.IsNullOrEmpty(manifestacija.Datum_i_Vreme_Odrzavanja) || string.IsNullOrWhiteSpace(manifestacija.Datum_i_Vreme_Odrzavanja))
            {
                return "Datum manifesacije ne sme da bude prazan";
            }
            if (manifestacija.Cena_Regularne_Karte < 0)
            {
                return "Cena regularne karte ne sme da bude negativan broj";
            }
            if (string.IsNullOrWhiteSpace(manifestacija.Putanja_do_slike) || string.IsNullOrEmpty(manifestacija.Putanja_do_slike))
            {
                return "Molimo Vas izaberite sliku manifestacije";
            }
            if (string.IsNullOrEmpty(manifestacija.Lokacija.Mesto_Odrzavanja.Ulica) || string.IsNullOrWhiteSpace(manifestacija.Lokacija.Mesto_Odrzavanja.Ulica))
            {
                return "Polje za ulicu ne moze da bude prazno";
            }
            if (manifestacija.Lokacija.Mesto_Odrzavanja.Ulica.Length < 3)
            {
                return "Ulica mora imati najmanje 3 karaktera";
            }
            if (string.IsNullOrEmpty(manifestacija.Lokacija.Mesto_Odrzavanja.Broj) || string.IsNullOrWhiteSpace(manifestacija.Lokacija.Mesto_Odrzavanja.Broj))
            {
                return "Polje za broj ne moze da bude prazno";
            }
            else
            {
                string filter = @"^[0-9]+[A-Za-z]{0,2}";
                Regex rg = new Regex(filter);
                if (!rg.IsMatch(manifestacija.Lokacija.Mesto_Odrzavanja.Broj))
                {
                    return "Broj ulice nije validan (server salje)";
                }
            }
            if (manifestacija.Lokacija.Mesto_Odrzavanja.Grad.Length < 3)
            {
                return "Naziv grada mora imati najmanje 3 karatera";
            }
            if (string.IsNullOrWhiteSpace(manifestacija.Lokacija.Mesto_Odrzavanja.Grad) || string.IsNullOrEmpty(manifestacija.Lokacija.Mesto_Odrzavanja.Grad))
            {
                return "Morate uneti naziv grada";
            }
            if (manifestacija.Lokacija.Mesto_Odrzavanja.Grad != null)
            {
                if (double.TryParse(manifestacija.Lokacija.Mesto_Odrzavanja.Grad, out temp))
                {
                    return "Naziv grada ne moze da bude broj";
                }
            }
            if (manifestacija.Lokacija.Mesto_Odrzavanja.Postanski_Broj < 0)
            {
                return "Postanski broj ne moze da bude negativan broj";
            }
            if (manifestacija.Lokacija.Mesto_Odrzavanja.Postanski_Broj < 1000 || manifestacija.Lokacija.Mesto_Odrzavanja.Postanski_Broj > 99999)
            {
                return "Postanski broj mora imati 4 ili 5 cifara";
            }
            return "Uspesno kreirana manifestacija";
        }

        public static bool ProveraZaIzmenuManifestacijeIBrojaKarata(Dictionary<Karta, string> sveKarte, Manifestacija stara, Manifestacija nova)
        {

            if (stara.BrojFanPitMesta == nova.BrojFanPitMesta && stara.BrojRegularnihMesta == nova.BrojRegularnihMesta && stara.BrojVipMesta == nova.BrojVipMesta)
            {
                // ako korisnik hoce da izmeni manifestaciju a ne promeni broj mesta za neki od tip karata da moze da se uradi izmena
                // vracalo bi satalno gresku ako bi se menjala manifestacija za koju ima vise karata nego preostalih mesta, a broj mesta prodavac nije menjao
                return true;
            }

            List<Karta> sveKarteManifestacije = new List<Karta>();
            int brojRegularnih = 0, brojVIp = 0, brojFanPit = 0;
            sveKarteManifestacije.AddRange(sveKarte.Keys.ToList().FindAll(k => k.Manifestacija.Naziv.ToLower().Equals(stara.Naziv.ToLower()) &&
                                                                              k.Manifestacija.Datum_i_Vreme_Odrzavanja.Equals(stara.Datum_i_Vreme_Odrzavanja) &&
                                                                              k.Manifestacija.Tip_Manifestacije.Equals(stara.Tip_Manifestacije) &&
                                                                              k.Manifestacija.Lokacija.Mesto_Odrzavanja.Ulica.ToLower().Equals(stara.Lokacija.Mesto_Odrzavanja.Ulica.ToLower()) &&
                                                                              k.Manifestacija.Lokacija.Mesto_Odrzavanja.Broj.ToLower().Equals(stara.Lokacija.Mesto_Odrzavanja.Broj.ToLower()) &&
                                                                              k.Manifestacija.Lokacija.Mesto_Odrzavanja.Grad.ToLower().Equals(stara.Lokacija.Mesto_Odrzavanja.Grad.ToLower()) &&
                                                                              k.Manifestacija.Lokacija.Mesto_Odrzavanja.Postanski_Broj.Equals(stara.Lokacija.Mesto_Odrzavanja.Postanski_Broj)
                                                                           ));
            foreach (Karta karta in sveKarteManifestacije)
            {
                if (karta.TipKarte.Equals(ETipKarte.Regular))
                {
                    brojRegularnih++;
                }
                else if (karta.TipKarte.Equals(ETipKarte.VIP))
                {
                    brojVIp++;
                }
                else if (karta.TipKarte.Equals(ETipKarte.FanPit))
                {
                    brojFanPit++;
                }
            }
            if (brojRegularnih > nova.BrojRegularnihMesta)
            {
                return false;
            }
            if (brojVIp > nova.BrojVipMesta)
            {
                return false;
            }
            if (brojFanPit > nova.BrojFanPitMesta)
            {
                return false;
            }
            return true;

        }

        public static void PromeniDatumNaKartamaZaManifestaciju(Dictionary<Karta, string> sveKarte, Manifestacija stara, Manifestacija nova)
        {
            foreach (Karta k in sveKarte.Keys)
            {
                if (k.Manifestacija.Naziv.ToLower().Equals(stara.Naziv.ToLower()) &&
                     k.Datum_i_Vreme_Manifestacije.Equals(stara.Datum_i_Vreme_Odrzavanja) &&
                     k.Manifestacija.Tip_Manifestacije.Equals(stara.Tip_Manifestacije) &&
                     k.Manifestacija.Lokacija.Mesto_Odrzavanja.Ulica.ToLower().Equals(stara.Lokacija.Mesto_Odrzavanja.Ulica.ToLower()) &&
                     k.Manifestacija.Lokacija.Mesto_Odrzavanja.Broj.ToLower().Equals(stara.Lokacija.Mesto_Odrzavanja.Broj.ToLower()) &&
                     k.Manifestacija.Lokacija.Mesto_Odrzavanja.Grad.ToLower().Equals(stara.Lokacija.Mesto_Odrzavanja.Grad.ToLower()) &&
                     k.Manifestacija.Lokacija.Mesto_Odrzavanja.Postanski_Broj.Equals(stara.Lokacija.Mesto_Odrzavanja.Postanski_Broj)
                  )
                {
                    //ovde se menja za sve karte iz fajla koje su ucitane i za prvu kartu koja je rezervisana u toku rada aplikacije (ako je vise karata rezervisano
                    // onda se zbog reference manifestacije menja na svim kartama koje su rezervisane, na ostalim kartama rezervisanim u toku rada aplikacije
                    //nece se promeniti samo datum na karti(k.datum i vreme manifestaije) jer se promene polja i na STAROJ manifestaciji pa IF uslov nece 
                    //vaziti za ostale, zbog toga ide foreach petlja na kraju ove funckije za promenu karata;
                    k.Datum_i_Vreme_Manifestacije = nova.Datum_i_Vreme_Odrzavanja;
                    k.Manifestacija.Datum_i_Vreme_Odrzavanja = k.Datum_i_Vreme_Manifestacije;
                    // levi deo od znaka jednako je vezan referencom za stara manifestacija pa ce nakon ove promene i na staroj biti novi datum a to ne zelim
                    //jer provera if uslova nece raditi za ostale karte kako se ocekuje. Ako se izmeni naziv ili nesto od ostalog samo ce jednom uci u ovaj if
                    // jer je polje Manifestacija kod karata vezano referencom.
                    //k.Manifestacija.Datum_i_Vreme_Odrzavanja = nova.Datum_i_Vreme_Odrzavanja;
                    k.Manifestacija.Lokacija.Mesto_Odrzavanja.Ulica = nova.Lokacija.Mesto_Odrzavanja.Ulica;
                    k.Manifestacija.Lokacija.Mesto_Odrzavanja.Broj = nova.Lokacija.Mesto_Odrzavanja.Broj;
                    k.Manifestacija.Lokacija.Mesto_Odrzavanja.Grad = nova.Lokacija.Mesto_Odrzavanja.Grad;
                    k.Manifestacija.Lokacija.Mesto_Odrzavanja.Postanski_Broj = nova.Lokacija.Mesto_Odrzavanja.Postanski_Broj;
                    k.Manifestacija.Naziv = nova.Naziv;
                    k.Manifestacija.Tip_Manifestacije = nova.Tip_Manifestacije;
                }
            }

            //ovaj foreach je zbog toga sto se nece na svim kartama promentiti njihov datum, a posto je k.manifestacijaa vezana referencom ona kad se 1 promeni
            // menja se svugde na kartama
            foreach (Karta k in sveKarte.Keys)
            {
                k.Datum_i_Vreme_Manifestacije = k.Manifestacija.Datum_i_Vreme_Odrzavanja;
            }
        }

        public static bool ProveriDatumeOtkazivanja(List<DateTime> lista)
        {
            // treba naci vise od 5 datuma u okviru istog meseca;
            int brojac = 0;
            List<DateTime> datumi = lista.OrderByDescending(d => d.Date).ToList();
            for (int i = 0; i < datumi.Count - 1; i++)
            {
                for (int j = i + 1; j < datumi.Count; j++)
                {
                    TimeSpan period = datumi[i].AddDays(1) - datumi[j];
                    //DateTime date = new DateTime(Math.Abs(period.Ticks));
                    DateTime date = new DateTime(period.Ticks);
                    int meseci = ((date.Year - 1) * 12) + date.Month - 1;
                    if (meseci == 0)
                    {
                        if (brojac == 0)
                        {
                            //prva iteracija, rok jeste mesec dana, to su 2 datuma uvecaj za 2
                            //bez ovoga bi imao 4 datuma a brojac bi na kraju bio 3;
                            brojac += 2;
                        }
                        else
                        {

                            brojac++;
                        }
                        //ako ima vise datuma, a pre toga nadje, odmah vrati true
                        if (brojac > 5) { return true; }
                    }

                }
                //bez ovoga si imao sledeci problem -> 4 datuma otkazivanja a brojac se uvecavao istigao do 6 i korisnik je bio sumnjiv a ne bi trebao
                //oduzme 1-2, 1-3, 1-4  -> brojac =3; sledeca iteracija prve for petlje 2-3, 2-4 ->brojac =5 a ne treba tako, ti datumi su vec provereni 
                //da su u istom mesecu
                brojac = 0;
            }
            return false;
        }

        public static Dictionary<Manifestacija, string> IzracunajProsecnuOcenuZaManifestacije(Dictionary<Manifestacija, string> lista, List<Komentar> komentari, bool IndikatorZaNultuProsecnuOcenuNakonBrisanjaIliDodavanjaKomentara)
        {
            List<Komentar> temp = new List<Komentar>();
            double zbir = 0;
            foreach (Manifestacija m in lista.Keys)
            {
                if ((DateTime.Parse(m.Datum_i_Vreme_Odrzavanja) - DateTime.Now).TotalDays < 0)
                {
                    temp.AddRange(komentari.FindAll(k => k.Manifestacija.Naziv.Equals(m.Naziv) && k.Manifestacija.Tip_Manifestacije.Equals(m.Tip_Manifestacije) &&
                                                        k.Manifestacija.Datum_i_Vreme_Odrzavanja.Equals(m.Datum_i_Vreme_Odrzavanja) &&
                                                        k.Manifestacija.Lokacija.Mesto_Odrzavanja.Ulica.Equals(m.Lokacija.Mesto_Odrzavanja.Ulica) &&
                                                        k.Manifestacija.Lokacija.Mesto_Odrzavanja.Grad.Equals(m.Lokacija.Mesto_Odrzavanja.Grad) &&
                                                        k.Manifestacija.Lokacija.Mesto_Odrzavanja.Postanski_Broj.Equals(m.Lokacija.Mesto_Odrzavanja.Postanski_Broj) &&
                                                        k.Odobren == true && k.LogickiObrisan == false
                                                        ));
                    if (temp.Count > 0)
                    {
                        foreach (Komentar ko in temp)
                        {
                            zbir += ko.Ocena;
                        }

                        m.ProsecnaOcena = zbir / temp.Count;
                        m.ProsecnaOcena = Math.Round(m.ProsecnaOcena, 2);
                    }
                    else if (IndikatorZaNultuProsecnuOcenuNakonBrisanjaIliDodavanjaKomentara)
                    {
                        m.ProsecnaOcena = 0;
                    }
                    zbir = 0;
                    temp = new List<Komentar>();
                }
            }
            return lista;
        }

        public static bool ProveriKorisnickoIme(List<string> admini, List<string> prodavci, List<string> kupci, Korisnik k)
        {
            if (admini.Contains(k.Korisnicko_Ime))
            {
                return false;
            }
            else if (prodavci.Contains(k.Korisnicko_Ime))
            {
                return false;
            }
            else if (kupci.Contains(k.Korisnicko_Ime))
            {
                return false;
            }
            return true;
        }

        public static bool ProveraZaKreiranjeManifestacije(Dictionary<Manifestacija, string> manifestacije, Manifestacija manifestacija)
        {
            foreach (Manifestacija m in manifestacije.Keys)
            {
                if (m.Datum_i_Vreme_Odrzavanja.Equals(manifestacija.Datum_i_Vreme_Odrzavanja))
                {
                    //Izbacujem iz provere sirinu i duzinu jer se razlikuju za  neke lokacije a adresa je ISTA!
                    //if (m.Lokacija.Geografska_Duzina.Equals(manifestacija.Lokacija.Geografska_Duzina))
                    //{
                    //    if (m.Lokacija.Geografska_Sirina.Equals(manifestacija.Lokacija.Geografska_Sirina))
                    //    {
                    if (m.Lokacija.Mesto_Odrzavanja.Grad.ToLower().Equals(manifestacija.Lokacija.Mesto_Odrzavanja.Grad.ToLower()))
                    {
                        if (m.Lokacija.Mesto_Odrzavanja.Postanski_Broj.Equals(manifestacija.Lokacija.Mesto_Odrzavanja.Postanski_Broj))
                        {
                            if (m.Lokacija.Mesto_Odrzavanja.Ulica.ToLower().Equals(manifestacija.Lokacija.Mesto_Odrzavanja.Ulica.ToLower()))
                            {
                                if (m.Lokacija.Mesto_Odrzavanja.Broj.ToLower().Equals(manifestacija.Lokacija.Mesto_Odrzavanja.Broj.ToLower()))
                                {
                                    return true;
                                }
                            }
                        }
                    }

                    //    }
                    //}
                }
            }
            return false;
        }

        public static Manifestacija PretvoriCirilicuULatinicuZaGradoveZbogPretrage(Manifestacija manifestacija)
        {
            Dictionary<string, string> cirilica = new Dictionary<string, string>() {

                { "А","A" }, { "Б","B" },  {"В","V" },{"Г" , "G"}, {"Д", "D"}, {"Ђ", "Dj" },  {"Е", "E"}, {"Ж" , "Z"},  {"З", "Z"},{"И", "I"}, {"Ј","J" },{"К","K" }, {"Л","L" }
                ,{"Љ","Lj" }, {"М" ,"M" },{"Н","N" }, {"Њ","Nj" },{"О","O"}, {"П","P" },{"Р","R" }, {"С","S" } ,{"Т","T" },{"Ћ","C" } ,{"У","U" },  {"Ф","F" } ,{"Х","H" }, {"Ц","C" },
                { "Ч","C" } , { "Џ","Dz" } ,{"Ш","S"},

                 { "а", "a"}, {"б", "b"} ,  {"в","v" }, {"г","g" }  , {"д","d" } ,{"ђ","dj" } ,  {"е","e" } ,{"ж","z" } ,  {"з","z" } ,{"и" ,"i" } , {"ј","j" } ,{"к","k" }  , {"л","l" } ,
                { "љ","lj" }, {"м","m" } , {"н","n" }, {"њ","nj" }, {"о","o" },{"п","p" } ,{"р","r" }, {"с","s" },{"т","t" }, {"ћ","c" }, {"у","u" }, {"ф","f" } ,{"х","h" }, {"ц","c" },
                { "ч" ,"c" },  {"џ","dz" } ,{"ш","s" }

             };
            // А Б   В Г   Д Ђ   Е Ж   З И   Ј К   Л Љ   М Н   Њ О   П Р   С Т   Ћ У   Ф Х   Ц Ч   Џ Ш
            // а б   в г   д ђ   е ж   з и   ј к   л љ   м н   њ о   п р   с т   ћ у   ф х   ц ч   џ ш

            if (Regex.IsMatch(manifestacija.Lokacija.Mesto_Odrzavanja.Grad, @"\p{IsCyrillic}"))
            {
                foreach (KeyValuePair<string, string> par in cirilica)
                {
                    if (manifestacija.Lokacija.Mesto_Odrzavanja.Grad.Contains(par.Key))
                    {
                        manifestacija.Lokacija.Mesto_Odrzavanja.Grad = manifestacija.Lokacija.Mesto_Odrzavanja.Grad.Replace(par.Key, par.Value);

                    }
                }
            }

            return manifestacija;

        }

        public static bool ProverZaKreiranjeManifestacijeUProslosti(Manifestacija manifestacija)
        {
            if ((DateTime.Now - DateTime.Parse(manifestacija.Datum_i_Vreme_Odrzavanja)).TotalDays > 0)
                return true;

            return false;
        }

        public static bool ProveraZaNazivITipManifestacije(List<Manifestacija> sveManifestacije, Manifestacija manifestacija)
        {
            foreach (Manifestacija m in sveManifestacije)
            {
                if (m.Naziv.ToLower().Equals(manifestacija.Naziv.ToLower()) && m.Tip_Manifestacije.Equals(manifestacija.Tip_Manifestacije))
                {
                    return true;
                }
            }

            return false;
        }

    }
}