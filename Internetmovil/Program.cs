using System;
using CsvHelper;
using System.IO;
using System.Linq;

namespace Internetmovil
{
    class Program
    {
        static void Main(string[] args)
        {
            var ruta = @"C:\Users\carlo\Downloads\Comparativo_Segundo_Trimestre_vs_Primer_Trimestre_2016.csv";
            var archivo = File.OpenText(ruta);
            var csv = new CsvReader(archivo);
            csv.Configuration.Delimiter = ",";
            var registros = csv.GetRecords<DatosInternetMovil>().ToList();
        }
    }
}
