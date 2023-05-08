using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MoogleEngine
{
    public class Matriz
    {
        public List<Dictionary<string, double>> tfidf;


        public Matriz()
        {

            // se inicializa un objeto tipo Documentos que no es mas que una lista de listas de string 
            //cada lista contiene las palabras de cada documento .
            //es necesaria para inicializar el objeto tipo Matriz 
            Documentos textos = new Documentos();
            
            //se inicializa el objeto tipo matriz que no es mas que  una lista de diccionario
            //cada uno corresponde a un documento , la clave de los mismos es una palabra del texto
            //el valor es su TF-IDF .

            tfidf = CalcularTDIDFDoc(textos);
            


        }

        public List<Dictionary<string, double>> CopiarMatriz()
        {
            //metodo que realiza una copia de objeto tipo Matriz 
            //es producto de un error a la hora de recorrer este tipo de objetos en un bucle
            //por lo cual se hace una copia , que si lo permite
            List<Dictionary<string, double>> copia = new  List<Dictionary<string, double>> (tfidf);

           

            return copia;
        }

        public int Tamaño
        {
            get { return tfidf.Count; }
        }
        public static  int Longitud(Matriz matriz)
        {
            return matriz.Tamaño;

        }
        public static List<Dictionary<string, double>> TFDoc(Documentos textos)
        {
             
             //este metodo recibe como parametro un objeto tipo Documentos .


            //se inicializa una lista de diccionarios c aplicando el metodo Repeticiones de la clase Documentos
            //Cada diccionario contiene las veces que se repite una palabra en un texto
            //Hay un diccionario por cada texto de la coleccion .
            List<Dictionary<string, double>> repeticiones = textos.Repeticiones();
            
            //la siguiente lista va a contener un diccionario para cada texto 
            // cada diccionario con la palabra y su valor de TF en el texto
            List<Dictionary<string, double>> tfs = new List<Dictionary<string, double>>();


            int contador = -1; 

            // en el siguiente bucle se recorre cada diccionario de la lista repeticiones
            foreach (Dictionary<string, double> valores in repeticiones)
            {
                //se inicializa el siguiente diccionario que va a ser añadidio a la lista tfs 
                Dictionary<string, double> resultado = new Dictionary<string, double>();
                //se le aumenta la frecuencia el contador para que inicie en 0 
                //esto es para usarlo como parametro en el metodo Index mas abajo
                contador++;
              
                foreach (KeyValuePair<string, double> palabras in valores)
                {
                    
                    //Index es un metodo de la clase Documentos 
                    //devuelve el Count de cada lista del objeto de la clase Documentos
                    //se calcula el tf , palabras.Value es la cantidad de veces que se repite la palabra
                    //en el texto correspondiente , esto se divide entre la cantidad de palabras del texto
                    //este ultimo dato lo proporciona el metodo Index
                    double tf = (double)palabras.Value / textos.Index(contador);





                    resultado.Add(palabras.Key, tf);



                }

                tfs.Add(resultado);

            }



            return tfs;



        }

        public static Dictionary<string, double> CopiarHash(HashSet<string> unicas)
        {
            //este metodo recibe una lista de Hashsets , en particular recibe el vocabulario de cada texto
            //una lista con todas las palabras de un texto , sin repetir ,  por eso el uso del  HashSet 
            // la intencion del metodo es devolver una lista de diccionarios , siendo las claves...
            //las palabras de cada HashSet , con valor 0 para todas .
            //se va usar en el Metodo NumeroenDocumentos , para tener en un diccionario todas las
            //palabras de cada vocabulario .
            Dictionary<string, double> copiahash = new Dictionary<string, double>();

            foreach (string unic in unicas)
            {
                copiahash.Add(unic , 0);

            }
            

            return copiahash;

        }
        public static Dictionary<string, double> NumeroenDocumentosTXT(Documentos textos)
        {

            //se llama al metodo CogerUna de la clase Documentos,para almacenar en una lista de HashSets
            //el  vocabulario de cada texto , se le va a pasar como parametro  al metodo CopiarHash.

            HashSet<string> words = textos.Vocabulario();
            string[] copiawords = words.ToArray();
            
           
            Dictionary<string, double> frecuencias =  CopiarHash(words);

           
            //se utiliza el metodo GetFiles de la clase Directory para almacenar en un array todas...
            //las rutas de los archivos txt de la carpeta Content.
            
            List<List<string>> copiatextos = textos.CopiarDocumento();  

        

            foreach ( List<string> texto in copiatextos)
            {
                for( int i = 0; i<copiawords.Length; i++ )
                {
                    if (texto.Contains(copiawords[i]))
                    {
                        frecuencias[copiawords[i]]++;
                        continue;

                    }
                    else
                    {
                        continue;
                    }
                }

               

            }


        
           
            


            

           
            

            return frecuencias;
        }

                
        public static List<Dictionary<string, double>> CalcularTDIDFDoc(Documentos textos)
        {
            

            List<Dictionary<string, double>> tds = TFDoc(textos);
            Dictionary<string, double> idfs = IDFdoc(textos);
            List<Dictionary<string, double>> tdidfs = new List<Dictionary<string, double>>();


            for (int i = 0; i < tds.Count; i++)
            {
                //se recorre la lista tds con los valores del TF de las palabras de cada texto

                Dictionary<string, double> valor = new Dictionary<string, double>();

                foreach (KeyValuePair<string, double> valores in tds[i])
                {
                     //se multiplica los valores de ambos diccionarios , uno con el tf y el otro con el idf
                     //se obtiene el TF-IDF
                    double calculo = (double)tds[i][valores.Key] * idfs[valores.Key];


                    valor.Add(valores.Key, calculo);


                }

                tdidfs.Add(valor);


            }





            return tdidfs;



        }




       
        public static Dictionary<string, double> IDFdoc(Documentos textos)
        {

           
            string ruta = @"C:\JOSE\moogle-main\Content";
            string[] documentos = Directory.GetFiles(ruta, "*txt");
            Dictionary<string, double> repeticiones = NumeroenDocumentosTXT(textos);
            Dictionary<string, double> idfs = new Dictionary<string, double>();
            //en esta variable se almacena el valor del logaritmo natural de la cantidad de documetos de la coleccion.
            //la cantidad de documentos de la coleccion se obtiene del array documentos con todas las rutas de los documentos de la carpeta Content
            //como es un valor que se usa en todos los calculos del IDF lo almaceno aca para evitar calcularlo dentro del bucle
            
            double logT = Math.Log(documentos.Length);

           
            
                //se recorre la lista repeticiones , esta es la lista que contiene el resultado del metodo NumeroenDocumentos
                //esta lista tiene en cada diccionario en cuantos textos de la coleccion aparecen las palabras de cada texto .

                

                foreach (KeyValuePair<string, double> elemento in repeticiones)
                {

                    //aunque la formula del idf es logaritmo natural de la division entre el numero total de documentos de la coleccion 
                    //entre la cantidad de textos en los que aparece palabra , voy a utilizar la propiedad de los logaritmos para
                    //calcular el logaritmo de una division como el la resta de logaritmos de igual base 
                    //ya que el numero total de documentos es constante lo guardo afuera del  bucle y solo calculo el logaritmo de la cantidad de textos en los que aparece la palabra;


                     double idf =(double) logT -  Math.Log(elemento.Value);

                     idfs.Add(elemento.Key, idf);
                }




                 return idfs;   
     
            }


    }

}
