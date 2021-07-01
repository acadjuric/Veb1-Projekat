using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Projekat_WEB.Models
{
    public class MestoOdrzavanja
    {

        public string Ulica { get; set; }
        public string Broj { get; set; }
        public string Grad { get; set; }
        public int Postanski_Broj { get; set; }

        public MestoOdrzavanja(string ulica,string broj, string grad, int pBroj)
        {
            Ulica = ulica;
            Broj = broj;
            Grad = grad;
            Postanski_Broj = pBroj;

        }

        public MestoOdrzavanja() { }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}