using ProyectoUnidad2.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Documents;
using GalaSoft.MvvmLight.Command;

namespace ProyectoUnidad2.ViewModels
{
    public enum Ventanas { Agregar, Editar, Eliminar, Lista, Detalles, AgregarCapitulo, EditarCapitulo, EliminarCapitulo, ListaCapitulo, DetallesCapitulo }
    public class SeriesViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<TemporadaModel> Temporadas { get; set; } = new();
        public TemporadaModel? Temporada { get; set; }
        public CapituloModel? Capitulo { get; set; } 
        public ICommand AgregarCommand { get; set; }
        public ICommand EditarCommand { get; set; }
        public ICommand EliminarCommand { get; set; }
        public ICommand CambioVentanaCommand { get; set; }
        public ICommand AgregarCapituloCommand { get; set; }
        public ICommand EditarCapituloCommand { get; set; }
        public ICommand EliminarCapituloCommand { get; set; }
        public Ventanas Ventana { get; set; } = Ventanas.Lista;
        public string Error { get; set; } = "";

        public SeriesViewModel()
        {
            AgregarCommand = new RelayCommand(Agregar);
            EditarCommand = new RelayCommand(Editar);
            EliminarCommand = new RelayCommand(Eliminar);
            AgregarCapituloCommand = new RelayCommand(AgregarCapitulo);
            EditarCapituloCommand = new RelayCommand(EditarCapitulo);
            EliminarCapituloCommand = new RelayCommand(EliminarCapitulo);
            CambioVentanaCommand = new RelayCommand<Ventanas>(CambiarVentana);
            Deserializar();
        }
        int posicionAnterior;
        private void CambiarVentana(Ventanas ventanas)
        {
            if (ventanas == Ventanas.Agregar)
            {
                Temporada = new();
            }
            if(ventanas == Ventanas.AgregarCapitulo)
            {
                Capitulo = new();
            }

            if (ventanas == Ventanas.Editar && Temporada != null)
            {
                var clon = new TemporadaModel
                {
                    NoTemporada = Temporada.NoTemporada,
                    Portada = Temporada.Portada,
                    Sinopsis= Temporada.Sinopsis,
                    Fecha= Temporada.Fecha,
                    Capitulos= Temporada.Capitulos,
                };

                posicionAnterior = Temporadas.IndexOf(Temporada);

                Temporada = clon;
            }
            if (ventanas == Ventanas.EditarCapitulo && Temporada != null && Capitulo != null)
            {
                var clon = new CapituloModel
                {
                    Titulo = Capitulo.Titulo,
                    Duracion = Capitulo.Duracion,
                    Sinopsis = Capitulo.Sinopsis
                };

                posicionAnterior = Temporada.Capitulos.IndexOf(Capitulo);

                Capitulo = clon;
            }

            Error = "";
            
            Actualizar(nameof(Error));

            Ventana = ventanas;
            Actualizar(nameof(Ventana));

            Actualizar(nameof(Temporada));
            Actualizar(nameof(Capitulo));
        }

        private void Eliminar()
        {
            if (Temporada != null)
            {
                Temporadas.Remove(Temporada);
                Guardar();
                CambiarVentana(Ventanas.Lista);
            }
        }
        private void EliminarCapitulo()
        {
            if (Temporada != null && Capitulo != null)
            {
                Temporada.Capitulos.Remove(Capitulo);
                Guardar();
                CambiarVentana(Ventanas.ListaCapitulo);
            }
        }

        private void Editar()
        {
            if (Temporada != null)
            {
                Error = "";

                if (Temporada.NoTemporada <= 0)
                {
                    Error += "- Escriba el numero de temporada.\n";
                }
                if (!Temporada.Portada.StartsWith("http") || !Temporada.Portada.EndsWith(".jpg"))
                {
                    Error += "- Escriba la URL de la portada en JPG.\n";
                }
                Actualizar(nameof(Error));

                if (Error == "")
                {
                    Temporadas[posicionAnterior] = Temporada;
                    Guardar();
                    CambiarVentana(Ventanas.Lista);
                }
            }
        }
        private void EditarCapitulo()
        {
            if (Temporada != null && Capitulo != null)
            {
                if (Error == "")
                {
                    Temporada.Capitulos.Add(Capitulo);
                    Guardar();
                    CambiarVentana(Ventanas.ListaCapitulo);
                }
            }
        }

        private void Agregar()
        {
            if (Temporada != null)
            {
                Error = "";

                if (Temporada.NoTemporada<=0)
                {
                    Error += "- Escriba el numero de temporada.\n";
                }
                if (!Temporada.Portada.StartsWith("http") || !Temporada.Portada.EndsWith(".jpg"))
                {
                    Error += "- Escriba la URL de la portada en JPG.\n";
                }
                Actualizar(nameof(Error));

                if (Error == "")
                {
                    Temporadas.Add(Temporada);
                    Guardar();
                    CambiarVentana(Ventanas.Lista);
                }
            }
        }
        private void AgregarCapitulo()
        {
            if(Temporada != null && Capitulo != null) 
            {
                if(Error == "")
                {
                    Temporada.Capitulos.Add(Capitulo);
                    Guardar();
                    CambiarVentana(Ventanas.ListaCapitulo);
                }
            }
        }
    

        public void Guardar()
        {
            File.WriteAllText("temporadas.json",
            JsonSerializer.Serialize(Temporadas));
        }
        public void Deserializar()
        {
            if (File.Exists("temporadas.json"))
            {
                var datos = JsonSerializer
                    .Deserialize<ObservableCollection<TemporadaModel>>(File.ReadAllText("temporadas.json"));

                if (datos != null)
                {
                    foreach (var n in datos)
                    {
                        Temporadas.Add(n);
                    }
                }
            }
        }
        private void Actualizar(string? propertyName=null)
        {
            PropertyChanged?.Invoke(this, new(propertyName));
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
