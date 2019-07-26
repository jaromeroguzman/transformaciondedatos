using System;
using System.Collections.Generic;
using System.Text;
using CsvHelper.Configuration.Attributes;

namespace Internetmovil
{
    class DatosInternetMovil
    {
        [Name("PROVEEDOR")]
        public string Proveedor { get; set; }
        [Name("SUSCRIPTORES_2015_2T")]
        public int Suscriptores_2015_2T { get; set; }
        [Name("SUSCRIPTORES_2016_1T")]
        public int Suscriptores_2016_1T { get; set; }
        [Name("POBLACIONS_2016_1T")]
        public int Poblacion_2016_1T { get; set; }
        [Name("POBLACION_2015_2T")]
        public int Poblacion_2015_2T { get; set; }

    }
}
