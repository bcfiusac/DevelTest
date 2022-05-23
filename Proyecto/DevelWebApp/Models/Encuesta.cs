using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevelWebApp.Models
{
    public class Encuesta
    {
        public string Nombre { get; set; }
        public string NuevoNombre { get; set; }
        public string Descripcion { get; set; }
        public string NuevaDescripcion { get; set; }
        public List<Campo> Campos { get; set; }
        public class Campo
        {
            public string NombreCampo { get; set; }
            public string TituloCampo { get; set; }
            public string EsRequerido { get; set; }
            public string tipoCampo { get; set; }

            public string respuesta { get; set; }
        }
        public class Respuesta
        {
            public string NombreCampo { get; set; }
            public string respuesta { get; set; }
        }

   
    }
}
