using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Projekat_WEB.Models
{
    public class FiltriranjeModel
    {
        public List<Manifestacija> Lista { get; set; } = new List<Manifestacija>();
        public string Izbor { get; set; }

        public FiltriranjeModel(List<Manifestacija> lista,string izbor)
        {
            this.Izbor = izbor;
            Lista = lista;
        }

        public FiltriranjeModel() { }

    }
}