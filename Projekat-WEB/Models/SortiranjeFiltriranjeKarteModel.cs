using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Projekat_WEB.Models
{
    public class SortiranjeFiltriranjeKarteModel
    {
        public List<Karta> Lista { get; set; }
        public string Izbor { get; set; }
        public string Nacin { get; set; }
        public List<Korisnik> ListaK { get; set; }

        public SortiranjeFiltriranjeKarteModel(List<Karta> lista, string izbor, string nacin, List<Korisnik> listaa)
        {
            this.Lista = lista;
            this.Nacin = nacin;
            this.Izbor = izbor;
            this.ListaK = listaa;
        }

        public SortiranjeFiltriranjeKarteModel() { }

    }
}