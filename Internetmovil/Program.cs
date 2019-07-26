using System;
using CsvHelper;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Internetmovil
{
    class Program
    {
        static void ProcesarJSON()
        {
            var directorio = new Dictionary<string, Int16> {
                { "PROVEEDOR", 8 },
                { "SUSCRIPTORES_2015_2T", 9 },
                { "SUSCRIPTORES_2016_1T", 10 },
                { "SUSCRIPTORES_2016_2T", 11 },
                { "POBLACION_2015_2T", 12 },
                { "POBLACIONS_2016_1T", 13 },
                { "POBLACION_2016_2T", 14 },
            };
            var ruta = @"C:\Users\carlo\Documents\DATOS\INTERNET_MOVIL.json";
            var contenido = File.ReadAllText(ruta);
            var resultado = JsonConvert.DeserializeObject<JObject>(contenido);
            var data = resultado.SelectToken("data") as JArray;
            foreach (JArray datum in data)
            {

                Console.WriteLine(datum[directorio["POBLACION_2015_2T"]]);
            }
        }

        static void ProcesarCsv()
        {
            string FloatToCustomString(double num)
            {
                return num.ToString("0.0000", System.Globalization.CultureInfo.InvariantCulture);
            }
            var ruta = @"C:\Users\carlo\Downloads\Comparativo_Segundo_Trimestre_vs_Primer_Trimestre_2016.csv";
            var rutaSalida = @"C:\Users\carlo\Downloads\datosTransformados.csv";
            var archivo = File.OpenText(ruta);
            var csv = new CsvReader(archivo);
            csv.Configuration.Delimiter = ",";
            var registros1 = csv.GetRecords<DatosInternetMovil>().
                Select(registro =>
                    (registro.Proveedor,
                     porc1: registro.Suscriptores_2015_2T * 100.0 / registro.Poblacion_2015_2T,
                     porc2: registro.Suscriptores_2016_1T * 100.0 / registro.Poblacion_2016_1T));
            var archivoSalida = File.CreateText(rutaSalida);
            archivoSalida.WriteLine("Proveedor,Porcentaje_penetracion_2015,Porcentaje_penetracion_2016");
            foreach (var (proveedor, porc1, porc2) in registros1)
            {
                archivoSalida.WriteLine($"{proveedor},{FloatToCustomString(porc1)},{FloatToCustomString(porc2)}");
            }
            archivoSalida.Close();
            archivo.Close();

        }
        static void Main(string[] args)
        {
            ProcesarJSON();
        }
    }
}
