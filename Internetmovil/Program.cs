using System;
using CsvHelper;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using OfficeOpenXml;
using Dapper;

namespace Internetmovil
{
    class Program
    {
        static DatosInternetMovil JArrayToDIM(JArray arreglo)
        {
            var directorio = new Dictionary<string, Int16> {
                { "PROVEEDOR", 8 }, // del archivo original se capturaron los datos. se asosica el nombre que nosotros queremos con el índice en el que se encuentra este archivo
                { "SUSCRIPTORES_2015_2T", 9 },
                { "SUSCRIPTORES_2016_1T", 10 },
                { "SUSCRIPTORES_2016_2T", 11 },
                { "POBLACION_2015_2T", 12 },
                { "POBLACIONS_2016_1T", 13 },
                { "POBLACION_2016_2T", 14 },
            };
            var objetoARetornar = new DatosInternetMovil
            {
                Proveedor = (arreglo[directorio["PROVEEDOR"]] as JValue).Value as string, 
                Suscriptores_2015_2T = Convert.ToInt32((arreglo[directorio["SUSCRIPTORES_2015_2T"]] as JValue).Value), //se convierte de string a entero
                Suscriptores_2016_1T = Convert.ToInt32((arreglo[directorio["SUSCRIPTORES_2016_1T"]] as JValue).Value),
                Poblacion_2015_2T = Convert.ToInt32((arreglo[directorio["POBLACION_2015_2T"]] as JValue).Value ),
                Poblacion_2016_1T = Convert.ToInt32((arreglo[directorio["POBLACIONS_2016_1T"]] as JValue).Value),
            };
            return objetoARetornar; 
        }

        static List<DatosInternetMovil> ProcesarJSON()
        {
            
            var ruta = @"C:\Users\carlo\Documents\DATOS\INTERNET_MOVIL.json";
            var contenido = File.ReadAllText(ruta);
            var resultado = JsonConvert.DeserializeObject<JObject>(contenido);
            var data = resultado.SelectToken("data") as JArray;
            var datosSalida = data.Select(x => JArrayToDIM(x as JArray));
            return datosSalida.ToList();
            //var jsonAGuardar = JsonConvert.SerializeObject(datosSalida); // se realiza la trasnformaci{on de datos
            //var rutaSalida = @"C:\Users\carlo\Downloads\jsonreducido.json"; // en esta ruta se guarda el archivo procesado
            //var archivoSalida = File.CreateText(rutaSalida); // se crea el archivo de salida
            //archivoSalida.Write(jsonAGuardar); // se escribe la representaci{on json de los datose
            //archivoSalida.Close();
        }

        public static void CrearExcel(List<DatosInternetMovil> lista)
        {
            using (var p = new ExcelPackage())
            {
                var ws = p.Workbook.Worksheets.Add("DIM");
                var numFila = 2;
                ws.Cells[1, 1].Value = "Proveedor";
                ws.Cells[1, 2].Value = "Suscriptores_2015_2T";
                ws.Cells[1, 3].Value = "Suscriptores_2016_1T";
                ws.Cells[1, 4].Value = "Poblacion_2015_2T";
                ws.Cells[1, 5].Value = "Poblacion_2016_1T";
                foreach (var registro in lista)
                {
                    ws.Cells[numFila, 1].Value = registro.Proveedor;
                    ws.Cells[numFila, 2].Value = registro.Suscriptores_2015_2T;
                    ws.Cells[numFila, 3].Value = registro.Suscriptores_2016_1T;
                    ws.Cells[numFila, 4].Value = registro.Poblacion_2015_2T;
                    ws.Cells[numFila, 5].Value = registro.Poblacion_2016_1T;
                    numFila++;
                }

                p.SaveAs(new FileInfo(@"C:\Users\carlo\Downloads\Comparativo_Segundo_Trimestre_vs_Primer_Trimestre_2016.xlsx"));
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

        static void procesarXml()
        {
            var ruta = @"C:\Users\carlo\Documents\DATOS\internetmovilporsuscriptor.xml";
            var nodo = XElement.Load(ruta);
            var filas = nodo.Element("row").Elements("row");
            var totalSubs =  filas.
                                Select(x => x.Element("suscriptores_2015_2t").Value).
                                Select(int.Parse).
                                Sum();
            Console.WriteLine(totalSubs);
        }
        static void Main(string[] args)
        {
            //procesarXml();
            CrearExcel(ProcesarJSON());// se est{a llamando el m{etodo para porcesar el json
            Console.WriteLine("He terminado");
            Console.ReadKey();
        }
    }
}
