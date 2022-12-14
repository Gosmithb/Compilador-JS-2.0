using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Compilador.Semantico;

namespace Compilador
{
    class Arbol
    {
        public enum TipoNodoArbol
        {
            Expresion,
            Sentencia,
            Condiciona,
            NADA
        }
        public enum TipoSentencia
        {
            IF,
            FOR,
            ASIGNACION,
            LEER,
            ESCRIBIR,
            NADA
        }
        public enum tipoExpresion
        {
            Operador,
            Constante,
            Identificador,
            NADA
        }
        public enum tipoOperador
        {
            Suma,
            Resta,
            Multiplicacion,
            Division,
            NADA
        }
        public enum OperacionCondicional
        {
            IgualIgual,
            MenorQue,
            MayorQue,
            MenorIgualQue,
            MayorIgualQue,
            Diferente,
            NADA
        }

        public class NodoArbol
        {                                      //    IF             EXP             ASIG       for
            public NodoArbol hijoIzquierdo;  //  condicional      operando izq    arbol exp   sentencias
            public NodoArbol hijoDerecho;     // if false          operando der                condicional
            public NodoArbol hijoCentro;      // if true                                       cuerpo del for
            public NodoArbol hermano;     // apunta a la siguiente instruccion (arbol)

            public TipoNodoArbol soyDeTipoNodo;
            public TipoSentencia soySentenciaDeTipo;
            public tipoExpresion SoyDeTipoExpresion;
            public tipoOperador soyDeTipoOperacion;
            public OperacionCondicional soyOperacionCondicionaDeTipo;

            public string lexema;

            //reglas semanticas // atributos
            //comprobacion de tipos y generacion de codigo intermedio
            public TipoDato SoyDeTipoDato;
            public TipoDato tipoValorNodoHijoIzquierdo; // valores sintetizados
            public TipoDato tipoValorNodoHijoDerecho;   // valores sintetizados

            public string memoriaAsignada; // conocer la memoria asignada
            public string valor;   //para hacer calculos codigo intermedio
            public string pCode;  // generar el codigo intermedio
            public string pCode1;  // generar el codigo intermedio
            public string pCode2;  // generar el codigo intermedio
            public string pCode3;  // generar el codigo intermedio
            public int linea;
        }

        public class Arbol1
        {
            public NodoClase nombreClaseActiva;
            public string nombreMetodoActivo;

            public int puntero;
            public List<Token> miListaTokenTemporal;

            public Arbol1(List<Token> miListaTokenLexico, NodoClase ClaseActiva, string MetodoActivo)
            {
                puntero = 0;
                miListaTokenTemporal = miListaTokenLexico;
                nombreClaseActiva = ClaseActiva;
                nombreMetodoActivo = MetodoActivo;
            }

            public NodoArbol CrearArbolSintacticoAbstracto()
            {
                NodoArbol nodoraiz = new NodoArbol();
                nodoraiz = ObtenerSiguienteArbol();
                puntero++;
                if (miListaTokenTemporal[puntero].Lexema != "$")
                {
                    nodoraiz.hermano = NodoHermano();
                }
                return nodoraiz;
            }

            private NodoArbol NodoHermano()
            {

                NodoArbol t = ObtenerSiguienteArbol();
                puntero++;

                if (miListaTokenTemporal[puntero].Lexema != "$")
                {
                    t.hermano = NodoHermano();
                }
                return t;
            }

            private NodoArbol ObtenerSiguienteArbol()
            {
                switch (miListaTokenTemporal[puntero].ValorToken)
                {
                    case -57: //if  *si*
                        return CrearArbolIF();

                    case -1: //asignacion
                        return CrearArbolAsignacion();

                    case -55: //for *por*
                        return CrearArbolFor();// por

                    default:
                        return new NodoArbol();
                }
            }


            #region Crear ARBOL Expresion
            public NodoArbol CrearArbolExpresion()
            {
                NodoArbol nodoRaiz = Termino();
                while (miListaTokenTemporal[puntero].Lexema.Equals("+")
                    || miListaTokenTemporal[puntero].Lexema.Equals("-"))
                {
                    NodoArbol nodoTemp = NuevoNodoExpresion(tipoExpresion.Operador);
                    nodoTemp.hijoIzquierdo = nodoRaiz;
                    nodoTemp.soyDeTipoOperacion =
                        miListaTokenTemporal[puntero].Lexema.Equals("+")
                        ? tipoOperador.Suma
                        : tipoOperador.Resta;
                    if (miListaTokenTemporal[puntero].Lexema.Equals("+"))
                    {
                        nodoTemp.pCode = "adi;";
                    }
                    else
                    {
                        nodoTemp.pCode = "sbi;";
                    }
                    nodoTemp.lexema = miListaTokenTemporal[puntero].Lexema;
                    nodoRaiz = nodoTemp;
                    puntero++;
                    nodoRaiz.hijoDerecho = Termino();
                }

                return nodoRaiz;
            }

            private NodoArbol Termino()
            {
                NodoArbol t = Factor();
                while (miListaTokenTemporal[puntero].Lexema.Equals("*")  // cambiar por su token
                     || miListaTokenTemporal[puntero].Lexema.Equals("/"))
                {
                    NodoArbol p = NuevoNodoExpresion(tipoExpresion.Operador);
                    p.hijoIzquierdo = t;
                    p.soyDeTipoOperacion = miListaTokenTemporal[puntero].Lexema.Equals("*")
                        ? tipoOperador.Multiplicacion
                        : tipoOperador.Division;
                    if (miListaTokenTemporal[puntero].Lexema.Equals("*"))
                    {
                        t.pCode = "mpi;";
                    }
                    else
                    {
                        t.pCode = "div;";
                    }

                    t.lexema = miListaTokenTemporal[puntero].Lexema;
                    t = p;
                    puntero++;
                    t.hijoDerecho = Factor();
                }
                return t;
            }

            public NodoArbol Factor()
            {
                NodoArbol t = new NodoArbol();

                if (miListaTokenTemporal[puntero].ValorToken == -2) //ENTERO
                {
                    t = NuevoNodoExpresion(tipoExpresion.Constante);
                    t.pCode = " ldc " + miListaTokenTemporal[puntero].Lexema + ";";
                    t.SoyDeTipoDato = TipoDato.INT;
                    t.lexema = miListaTokenTemporal[puntero].Lexema;
                    puntero++;
                }
                if (miListaTokenTemporal[puntero].ValorToken == -3)  //decimal
                {
                    t = NuevoNodoExpresion(tipoExpresion.Constante);
                    t.pCode = " ldc " + miListaTokenTemporal[puntero].Lexema + ";";
                    t.lexema = miListaTokenTemporal[puntero].Lexema;
                    t.SoyDeTipoDato = TipoDato.DOBLE;
                    puntero++;
                }
                else if (miListaTokenTemporal[puntero].ValorToken == -4) //cadena
                {
                    t = NuevoNodoExpresion(tipoExpresion.Constante);
                    t.pCode = " ldc " + miListaTokenTemporal[puntero].Lexema + ";";
                    t.lexema = miListaTokenTemporal[puntero].Lexema;
                    t.SoyDeTipoDato = TipoDato.STRING;
                    puntero++;
                }
                else if (miListaTokenTemporal[puntero].ValorToken == -1) // identificador
                {
                    t = NuevoNodoExpresion(tipoExpresion.Identificador);
                    t.lexema = miListaTokenTemporal[puntero].Lexema;

                    t.pCode = " lod " + miListaTokenTemporal[puntero].Lexema + ";"; // gramatica con atributos
                    t.SoyDeTipoDato =
                        TablaSimbolos.ObtenerTipoDato(
                            miListaTokenTemporal[puntero].Lexema,
                            nombreClaseActiva, nombreMetodoActivo); // gramatica con atributos

                    puntero++;
                }
                else if (miListaTokenTemporal[puntero].Lexema.Equals("("))
                {
                    puntero++;
                    t = CrearArbolExpresion();
                    puntero++;
                }
                return t;
            }

            #endregion
            #region Crear ARBOL Asignacion
            public NodoArbol CrearArbolAsignacion()
            {
                var sentenciaAsignacion = NuevoNodoSentencia(TipoSentencia.ASIGNACION);
                sentenciaAsignacion.lexema = miListaTokenTemporal[puntero].Lexema;
                sentenciaAsignacion.pCode = "lda " + miListaTokenTemporal[puntero].Lexema + ";";
                sentenciaAsignacion.pCode1 = "sto;";
                puntero += 2;
                sentenciaAsignacion.SoyDeTipoDato = TablaSimbolos.ObtenerTipoDato(sentenciaAsignacion.lexema, nombreClaseActiva, nombreMetodoActivo);
                sentenciaAsignacion.hijoIzquierdo = CrearArbolExpresion();

                return sentenciaAsignacion;

            }


            #endregion
            #region Crear Arbol If
            public NodoArbol CrearArbolIF()
            {
                var nodoArbolIF = NuevoNodoSentencia(TipoSentencia.IF);
                puntero += 2;
                nodoArbolIF.hijoIzquierdo = CrearArbolCondicional();
                puntero += 2;
                nodoArbolIF.pCode = "fjp" + " " + "Ln";

                //error cuando no hay comandos cuando la condicional es verdadera
                // validar que exista codigo en el TRUE
                nodoArbolIF.hijoCentro = ObtenerSiguienteArbol();
                puntero += 2;
                //codigo cuando sea falso


                if (miListaTokenTemporal[puntero].Lexema.Equals("else"))  // cambiar por el numero de token
                {
                    nodoArbolIF.pCode2 = "ujp lx";
                    nodoArbolIF.pCode3 = "lab lx";
                    puntero++;
                    if (miListaTokenTemporal[puntero].Lexema.Equals("if")) // cambiar por el numero de token
                    {
                        CrearArbolIF();
                    }
                    else
                    {
                        puntero++;
                        nodoArbolIF.hijoDerecho = ObtenerSiguienteArbol();
                    }
                }
                nodoArbolIF.pCode1 = "lab Ln";
                return nodoArbolIF;
            }


            #endregion
            #region Crear Arbol Condicional 
            public NodoArbol CrearArbolCondicional()
            {
                NodoArbol nodoRaiz = CrearArbolExpresion();

                if (miListaTokenTemporal[puntero].Lexema.Equals("==")
                    || miListaTokenTemporal[puntero].Lexema.Equals("<=")
                    || miListaTokenTemporal[puntero].Lexema.Equals(">=")
                    || miListaTokenTemporal[puntero].Lexema.Equals(">")
                    || miListaTokenTemporal[puntero].Lexema.Equals("<"))
                {
                    NodoArbol nodoTemp = NuevoNodoCondicional();

                    switch (miListaTokenTemporal[puntero].Lexema)
                    {
                        case "==":
                            nodoTemp.soyOperacionCondicionaDeTipo = OperacionCondicional.IgualIgual;
                            nodoTemp.pCode = "equ" + ";";
                            break;
                        case "<=":
                            nodoTemp.soyOperacionCondicionaDeTipo = OperacionCondicional.MenorIgualQue;
                            nodoTemp.pCode = "" + ";";
                            break;

                        case ">=":
                            nodoTemp.soyOperacionCondicionaDeTipo = OperacionCondicional.MayorIgualQue;
                            break;
                        case ">":
                            nodoTemp.pCode = "grt" + ";";
                            nodoTemp.soyOperacionCondicionaDeTipo = OperacionCondicional.MayorQue;
                            break;
                        case "<":
                            nodoTemp.soyOperacionCondicionaDeTipo = OperacionCondicional.MenorQue;
                            break;
                        default:
                            break;
                    }
                    nodoTemp.hijoIzquierdo = nodoRaiz;
                    nodoRaiz = nodoTemp;
                    puntero++;
                    nodoRaiz.hijoDerecho = CrearArbolExpresion();
                }

                return nodoRaiz;
            }
            #endregion
            #region Crear Arbol For

            private NodoArbol CrearArbolFor()
            {
                var nodoArbotFor = NuevoNodoSentencia(TipoSentencia.FOR);
                puntero += 3;
                nodoArbotFor.hijoIzquierdo = ObtenerSiguienteArbol();
                puntero++;
                nodoArbotFor.hijoDerecho = CrearArbolCondicional();
                puntero += 5;

                int cont = 0;

                while (!(miListaTokenTemporal[puntero].ValorToken == -26))
                {
                    if (cont == 0)
                    {
                        nodoArbotFor.hijoCentro = ObtenerSiguienteArbol();
                    }
                    else
                    {
                        nodoArbotFor.hijoCentro = NodoHermano();
                        puntero++;
                    }
                    cont++;
                }

                return nodoArbotFor;
            }

            #endregion

            #region Crear diferentes tipos de NodoArbol
            public NodoArbol NuevoNodoExpresion(tipoExpresion tipoExpresion)
            {
                NodoArbol nodo = new NodoArbol();

                nodo.soyDeTipoNodo = TipoNodoArbol.Expresion;

                nodo.SoyDeTipoExpresion = tipoExpresion;
                nodo.soyDeTipoOperacion = tipoOperador.NADA;
                nodo.SoyDeTipoDato = TipoDato.NADA;

                nodo.soySentenciaDeTipo = TipoSentencia.NADA;
                nodo.soyOperacionCondicionaDeTipo = OperacionCondicional.NADA;
                nodo.tipoValorNodoHijoDerecho = TipoDato.NADA;
                nodo.soyOperacionCondicionaDeTipo = OperacionCondicional.NADA;
                nodo.tipoValorNodoHijoIzquierdo = TipoDato.NADA;
                return nodo;
            }
            public NodoArbol NuevoNodoSentencia(TipoSentencia tipoSentencia)
            {
                NodoArbol nodo = new NodoArbol();
                nodo.soyDeTipoNodo = TipoNodoArbol.Sentencia;
                nodo.soySentenciaDeTipo = tipoSentencia;

                nodo.SoyDeTipoExpresion = tipoExpresion.NADA;
                nodo.soyDeTipoOperacion = tipoOperador.NADA;
                nodo.SoyDeTipoDato = TipoDato.NADA;


                nodo.soyOperacionCondicionaDeTipo = OperacionCondicional.NADA;
                nodo.tipoValorNodoHijoDerecho = TipoDato.NADA;
                nodo.soyOperacionCondicionaDeTipo = OperacionCondicional.NADA;
                nodo.tipoValorNodoHijoIzquierdo = TipoDato.NADA;
                return nodo;

            }
            public NodoArbol NuevoNodoCondicional()
            {
                NodoArbol nodo = new NodoArbol();
                nodo.soyDeTipoNodo = TipoNodoArbol.Condiciona;

                nodo.SoyDeTipoExpresion = tipoExpresion.NADA;
                nodo.soyDeTipoOperacion = tipoOperador.NADA;
                nodo.SoyDeTipoDato = TipoDato.NADA;

                nodo.soySentenciaDeTipo = TipoSentencia.NADA;
                nodo.soyOperacionCondicionaDeTipo = OperacionCondicional.NADA;
                nodo.soyOperacionCondicionaDeTipo = OperacionCondicional.NADA;
                nodo.tipoValorNodoHijoDerecho = TipoDato.NADA;
                nodo.tipoValorNodoHijoIzquierdo = TipoDato.NADA;
                return nodo;

            }
            #endregion
        }

    }
}
