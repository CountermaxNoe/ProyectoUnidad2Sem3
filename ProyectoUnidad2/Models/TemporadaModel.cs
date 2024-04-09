using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoUnidad2.Models
{

    public class TemporadaModel
    {
        public int NoTemporada { get; set; }
        public string Portada { get; set; } = "";
        public int CapitulosNumero=>(from n in Capitulos select n).Count();
        public string Sinopsis { get; set; } = "";
        public string Genero { get; set; } = "";
        public DateTime Fecha { get; set; }
        public ObservableCollection<CapituloModel> Capitulos { get; set; } = new();

    }
}
