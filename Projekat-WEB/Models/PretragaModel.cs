using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Projekat_WEB.Models
{
    public class PretragaModel
    {

        public string Naziv { get; set; }
        public string MestoOdrzavanja { get; set; }
        public string DatumOD { get; set; }
        public string DatumDO { get; set; }
        public int CenaOD { get; set; }
        public int CenaDO { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Kime { get; set; }


        public PretragaModel(string naziv,string mesto,string datumod, string datumdo, int cenaod ,int cenado,string ime,string prezime,string kime)
        {
            this.Naziv = naziv;
            this.MestoOdrzavanja = mesto;
            this.DatumOD = datumod;
            this.DatumDO = datumdo;
            this.CenaOD = cenaod;
            this.CenaDO = cenado;
            this.Ime = ime;
            this.Prezime = prezime;
            this.Kime = kime;
        }

        public PretragaModel() { }
    }
}