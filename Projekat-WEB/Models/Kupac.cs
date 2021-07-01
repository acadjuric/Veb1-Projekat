using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Projekat_WEB.Models
{
    public class Kupac
    {
        public string Ime { get; set; }
        public string Prezime { get; set; }

        public Kupac(string ime,string prezime)
        {
            Ime = ime;
            Prezime = prezime;
        }

        public Kupac() { }
    }
}