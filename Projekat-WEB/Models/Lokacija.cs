using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Projekat_WEB.Models
{
    public class Lokacija
    {

        public double Geografska_Duzina { get; set; }
        public double Geografska_Sirina { get; set; }
        
        public MestoOdrzavanja Mesto_Odrzavanja { get; set; }

        public Lokacija(double gDuzina, double gSirina, MestoOdrzavanja m)
        {
            Geografska_Duzina = gDuzina;
            Geografska_Sirina = gSirina;
            Mesto_Odrzavanja = m;

        }

        public Lokacija()
        {

        }


    }
}