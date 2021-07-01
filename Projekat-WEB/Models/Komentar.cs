using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Projekat_WEB.Models
{
    public class Komentar
    {

        public Kupac Kupac { get; set; }
        public Manifestacija Manifestacija { get; set; }
        public string Tekst_komentara { get; set; }
        public int Ocena { get; set; }
        public bool Odobren { get; set; }
        public bool Odbijen { get; set; }
        public bool LogickiObrisan { get; set; }

        public Komentar( Kupac k, Manifestacija m , string tekst, int o)
        {
            Kupac = k;
            Manifestacija = m;
            Tekst_komentara = tekst;
            Ocena = o;
        }

        public Komentar() { }
    }
}