open System
open FSharp.Data


type InternetMovilXML = XmlProvider<"""C:\Users\carlo\Documents\DATOS\internetmovilporsuscriptor.xml""">
let datos = InternetMovilXML.Load("""C:\Users\carlo\Documents\DATOS\internetmovilporsuscriptor.xml""")
for registro in  datos.Rows do
    printfn "%s" registro.Proveedor
