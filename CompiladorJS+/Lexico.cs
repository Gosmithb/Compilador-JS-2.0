using System;
using System.Collections.Generic;

namespace Compilador
{
    class Lexico
    {
        public List<Error> listaDeErrorLexico;
        public List<Token> listaDeToken;

        private string codigoFuente;
        private int linea;
        private int estado;
        private int columna;

        private Token nuevoToken;
        private char caracterActual;
        private string lexema;
        private int apuntador;

        public Lexico(string codigoFuenteI)
        {
            codigoFuente = codigoFuenteI + " ";
            listaDeToken = new List<Token>(); // INICIALIZAR
            listaDeErrorLexico = new List<Error>(); // INICIALIZAR
        }

        public List<Token> EjecutarLexico()
        {

            linea = 1;

            do
            {
                caracterActual = SiguienteCaracter(apuntador);

                if (caracterActual.Equals('\n')) linea++;

                lexema += caracterActual;

                columna = RegresarColumna(caracterActual);
                estado = matrizTransicion[estado, columna];


                if (estado < 0 && estado > -499)
                {
                    if (lexema.Length > 1)
                    {
                        lexema = lexema.Remove(lexema.Length - 1);
                        apuntador--;
                    }

                    nuevoToken = new Token() { ValorToken = estado, Lexema = lexema, Linea = linea };

                    if (estado == -1) nuevoToken.ValorToken = esPalabraReservada(nuevoToken.Lexema);

                    nuevoToken.TipoToken = esTipoToken(nuevoToken.ValorToken);

                    listaDeToken.Add(nuevoToken);

                    LimpiarECL();

                }

                else if (estado <= -500)
                {
                    listaDeErrorLexico.Add(Errores(estado));
                    LimpiarECL();
                }
                else if (estado == 0) LimpiarECL();

                apuntador++;

            } while (apuntador < codigoFuente.ToCharArray().Length);

            return listaDeToken;
        }

        private void LimpiarECL()
        {
            estado = 0;
            columna = 0;
            lexema = string.Empty;
        }

        private int[,] matrizTransicion =
        {

                //  0       1        2        3         4        5        6        7        8        9       10       11       12       13       14       15       16       17       18       19       20       21       22       23       24       25       26        27        28         29         30          31
                // dig ||  pal  ||   "   ||   '   ||    +   ||   -   ||   *   ||   /   ||   %   ||   <   ||   >   ||   =   ||   !   ||   &   ||   |   ||   {   ||   }   ||   (   ||   )   ||   [   ||   ]   ||   .   ||   ,   ||   ;   ||   :   ||   ?   ||   _   ||  espa  ||  enter  ||   eof   ||   tab  ||   Desco   ||
                /*  0 */{  2  ,    1   ,    5   ,     6  ,    8    ,    9   ,   10   ,   11   ,   -10  ,   13   ,   12   ,   14   ,   15   ,   17   ,   16   ,  -22   ,  -23   ,  -20   ,  -21   ,  -24   ,  -25   ,  -30   ,  -26   ,  -27   ,  -28   ,  -29   ,    21  ,    0    ,     0    ,     0    ,     0   ,      0   , },
                /*  1 */{  1  ,    1   ,  -500  ,   -500 ,   -1    ,   -1   ,   -1   ,   -1   ,    -1  ,   -1   ,   -1   ,   -1   ,  -500  ,   -1   ,   -1   ,   -1   ,   -1   ,   -1   ,   -1   ,   -1   ,   -1   ,   -1   ,   -1   ,   -1   ,   -1   ,  -500  ,    1   ,   -1    ,    -1    ,    -1    ,    -1   ,     -1   , },
                /*  2 */{  2  ,  -501  ,  -501  ,   -501 ,   -2    ,   -2   ,   -2   ,   -2   ,    -2  ,   -2   ,   -2   ,   -2   ,  -501  ,   -2   ,   -2   ,  -501  ,   -2   ,  -501  ,   -2   ,  -501  ,   -2   ,    3   ,   -2   ,   -2   ,  -501  ,  -501  ,  -501  ,   -2    ,    -2    ,    -2    ,    -2   ,     -2   , },
                /*  3 */{  4  ,  -502  ,  -502  ,   -502 ,  -502   ,  -502  ,  -502  ,  -502  ,   -502 ,  -502  ,  -502  ,  -502  ,  -502  ,  -502  ,  -502  ,  -502  ,  -502  ,  -502  ,  -502  ,  -502  ,  -502  ,  -502  ,  -502  ,  -502  ,  -502  ,  -502  ,  -502  ,   -502  ,   -502   ,   -502   ,   -502  ,    -502  , },
                /*  4 */{  4  ,  -502  ,  -502  ,   -502 ,   -3    ,   -3   ,   -3   ,   -3   ,    -3  ,   -3   ,   -3   ,   -3   ,  -502  ,   -3   ,   -3   ,  -502  ,   -3   ,  -502  ,   -3   ,  -502  ,   -3   ,  -502  ,   -3   ,   -3   ,  -502  ,  -502  ,  -502  ,    -3   ,    -3    ,    -3    ,    -3   ,     -3   , },
                /*  5 */{  5  ,    5   ,   35   ,     5  ,    5    ,    5   ,    5   ,    5   ,     5  ,    5   ,    5   ,    5   ,    5   ,    5   ,    5   ,    5   ,    5   ,    5   ,    5   ,    5   ,    5   ,    5   ,    5   ,    5   ,    5   ,    5   ,    5   ,     5   ,   -503   ,   -503   ,     5   ,    -503  , },
                /*  6 */{  7  ,    7   ,    7   ,   -504 ,    7    ,    7   ,    7   ,    7   ,     7  ,    7   ,    7   ,    7   ,    7   ,    7   ,    7   ,    7   ,    7   ,    7   ,    7   ,    7   ,    7   ,    7   ,    7   ,    7   ,    7   ,    7   ,    7   ,     7   ,   -504   ,   -504   ,   -504  ,    -504  , },
                /*  7 */{-504 ,  -504  ,  -504  ,    36  ,  -504   ,  -504  ,  -504  ,  -504  ,   -504 ,  -504  ,  -504  ,  -504  ,  -504  ,  -504  ,  -504  ,  -504  ,  -504  ,  -504  ,  -504  ,  -504  ,  -504  ,  -504  ,  -504  ,  -504  ,  -504  ,  -504  ,  -504  ,   -504  ,   -504   ,   -504   ,   -504  ,    -504  , },
                /*  8 */{ -6  ,   -6   ,   -6   ,    -6  ,   22    ,   -6   ,   -6   ,   -6   ,    -6  ,   -6   ,   -6   ,   25   ,   -6   ,   -6   ,   -6   ,   -6   ,   -6   ,   -6   ,   -6   ,   -6   ,   -6   ,   -6   ,   -6   ,   -6   ,   -6   ,   -6   ,   -6   ,    -6   ,    -6    ,    -6    ,    -6   ,     -6   , },
                /*  9 */{ -7  ,   -7   ,   -7   ,    -7  ,   -7    ,   24   ,   -7   ,   -7   ,    -7  ,   -7   ,   -7   ,   26   ,   -7   ,   -7   ,   -7   ,   -7   ,   -7   ,   -7   ,   -7   ,   -7   ,   -7   ,   -7   ,   -7   ,   -7   ,   -7   ,   -7   ,   -7   ,    -7   ,    -7    ,    -7    ,    -7   ,     -7   , },
                /* 10 */{ -8  ,   -8   ,   -8   ,    -8  ,   -8    ,   -8   ,   -8   ,   -8   ,    -8  ,   -8   ,   -8   ,   27   ,   -8   ,   -8   ,   -8   ,   -8   ,   -8   ,   -8   ,   -8   ,   -8   ,   -8   ,   -8   ,   -8   ,   -8   ,   -8   ,   -8   ,   -8   ,    -8   ,    -8    ,    -8    ,    -8   ,     -8   , },
                /* 11 */{ -9  ,   -9   ,   -9   ,    -9  ,   -9    ,   -9   ,   20   ,   18   ,    -9  ,   -9   ,   -9   ,   28   ,   -9   ,   -9   ,   -9   ,   -9   ,   -9   ,   -9   ,   -9   ,   -9   ,   -9   ,   -9   ,   -9   ,   -9   ,   -9   ,   -9   ,   -9   ,    -9   ,    -9    ,    -9    ,    -9   ,     -9   , },
                /* 12 */{ -11 ,   -11  ,  -11   ,   -11  ,  -11    ,  -11   ,  -11   ,  -11   ,    -11 ,   -11  ,  -11   ,   30   ,   -11  ,  -11   ,   -11  ,  -11   ,   -11  ,  -11   ,   -11  ,   -11  ,  -11   ,   -11  ,  -11   ,   -11  ,   -11  ,  -11   ,  -11   ,    -11  ,   -11    ,   -11    ,    -11  ,    -11   , },
                /* 13 */{ -12 ,   -12  ,  -12   ,   -12  ,  -12    ,  -12   ,  -12   ,  -12   ,    -12 ,   -12  ,  -12   ,   29   ,   -12  ,  -12   ,   -12  ,  -12   ,   -12  ,  -12   ,   -12  ,   -12  ,  -12   ,   -12  ,  -12   ,   -12  ,   -12  ,  -12   ,  -12   ,    -12  ,   -12    ,   -12    ,    -12  ,    -12   , },
                /* 14 */{-107 ,  -107  , -107   ,  -107  ,  -107   ,  -107  ,  -107  ,  -107  ,   -107 ,  -107  ,  -107  ,   31   ,  -107  ,  -107  ,  -107  ,  -107  ,  -107  ,  -107  ,  -107  ,  -107  ,  -107  ,  -107  ,  -107  ,   -107 ,   -107 ,  -107  ,  -107  ,   -107  ,   -107   ,   -107   ,   -107  ,    -107  , },
                /* 15 */{ -17 ,   17   ,  -17   ,   -17  ,  -17    ,  -17   ,  -17   ,  -17   ,   -17  ,   -17  ,  -17   ,   32   ,   -17  ,  -17   ,   -17  ,  -17   ,   -17  ,  -17   ,  -17   ,   -17  ,  -17   ,   -17  ,  -17   ,   -17  ,   -17  ,  -17   ,  -17   ,    -17  ,   -17    ,   -17    ,   -107  ,    -17   , },
                /* 16 */{-507 ,  -507  , -507   ,  -507  ,  -507   ,  -507  ,  -507  ,  -507  ,  -507  ,  -507  ,  -507  ,  -507  ,  -507  ,  -507  ,   33   ,  -507  ,  -507  ,  -507  ,  -507  ,  -507  ,  -507  ,  -507  ,  -507  ,   -507 ,   -507 ,  -507  ,  -507  ,   -507  ,   -507   ,   -507   ,   -507  ,    -507  , },
                /* 17 */{-508 ,  -508  , -508   ,  -508  ,  -508   ,  -508  ,  -508  ,  -508  ,  -508  ,  -508  ,  -508  ,  -508  ,  -508  ,   34   ,  -508  ,  -508  ,  -508  ,  -508  ,  -508  ,  -508  ,  -508  ,  -508  ,  -508  ,   -508 ,   -508 ,  -508  ,  -508  ,   -508  ,   -508   ,   -508   ,   -509  ,    -509  , },
                /* 18 */{ 18  ,   18   ,   18   ,   18   ,   18    ,   18   ,   18   ,   18   ,   18   ,   18   ,   18   ,   18   ,   18   ,   18   ,   18   ,   18   ,   18   ,   18   ,   18   ,   18   ,   18   ,   18   ,   18   ,    18  ,    18  ,   18   ,   18   ,    18   ,   -108   ,   -108   ,    18   ,    -108  , },
                /* 19 */{ 19  ,   19   ,   19   ,   19   ,   19    ,   19   ,   20   ,  -505  ,   19   ,   19   ,   19   ,   19   ,   19   ,   19   ,   19   ,   19   ,   19   ,   19   ,   19   ,   19   ,   19   ,   19   ,   19   ,    19  ,    19  ,   19   ,   19   ,    19   ,    19    ,    19    ,    19   ,     19   , },
                /* 20 */{ 19  ,   19   ,   19   ,   19   ,   19    ,   19   ,   19   ,   23   ,   19   ,   19   ,   19   ,   19   ,   19   ,   19   ,   19   ,   19   ,   19   ,   19   ,   19   ,   19   ,   19   ,   19   ,   19   ,    19  ,    19  ,   19   ,   19   ,    19   ,    19    ,    19    ,    19   ,     19   , },
                /* 21 */{-506 ,   1    , -506   ,  -506  ,  -506   ,  -506  ,  -506  ,  -506  ,  -506  ,  -506  ,  -506  ,  -506  ,  -506  ,  -506  ,  -506  ,  -506  ,  -506  ,  -506  ,  -506  ,  -506  ,  -506  ,  -506  ,  -506  ,   -506 ,   -506 ,  -506  ,  -506  ,   -506  ,   -506   ,   -506   ,   -506  ,    -506  , },
                /* 22 */{-33  ,  -33   , -33    ,  -33   ,  -33    ,  -33   ,  -33   ,  -33   ,  -33   ,  -33   ,  -33   ,  -33   ,  -33   ,  -33   ,  -33   ,  -33   ,   -33  ,  -33   ,  -33   ,  -33   ,  -33   ,  -33   ,  -33   ,   -33  ,   -33  ,  -33   ,  -33   ,   -33   ,   -33    ,   -33    ,   -33   ,    -33   , },
                /* 23 */{-109 ,  -109  , -109   ,  -109  ,  -109   ,  -109  ,  -109  ,  -109  ,  -109  ,  -109  ,  -109  ,  -109  ,  -109  ,  -109  ,  -109  ,  -109  ,   -109 ,  -109  ,  -109  ,  -109  ,  -109  ,  -109  ,  -109  ,   -109 ,   -109 ,  -109  ,  -109  ,   -109  ,   -109   ,   -109   ,   -109  ,    -109  , },
                /* 24 */{-34  ,  -34   ,  -34   ,  -34   ,  -34    ,  -34   ,  -34   ,  -34   ,  -34   ,  -34   ,  -34   ,  -34   ,  -34   ,  -34   ,  -34   ,  -34   ,   -34  ,  -34   ,  -34   ,  -34   ,  -34   ,  -34   ,  -34   ,   -34  ,   -34  ,  -34   ,  -34   ,   -34   ,   -34    ,   -34    ,   -34   ,    -34   , },
                /* 25 */{-35  ,  -35   ,  -35   ,  -35   ,  -35    ,  -35   ,  -35   ,  -35   ,  -35   ,  -35   ,  -35   ,  -35   ,  -35   ,  -35   ,  -35   ,  -35   ,   -35  ,  -35   ,  -35   ,  -35   ,  -35   ,  -35   ,  -35   ,   -35  ,   -35  ,  -35   ,  -35   ,   -35   ,   -35    ,   -35    ,   -35   ,    -35   , },
                /* 26 */{-36  ,  -36   ,  -36   ,  -36   ,  -36    ,  -36   ,  -36   ,  -36   ,  -36   ,  -36   ,  -36   ,  -36   ,  -36   ,  -36   ,  -36   ,  -36   ,   -36  ,  -36   ,  -36   ,  -36   ,  -36   ,  -36   ,  -36   ,   -36  ,   -36  ,  -36   ,  -36   ,   -36   ,   -36    ,   -36    ,   -36   ,    -36   , },
                /* 27 */{-37  ,  -37   ,  -37   ,  -37   ,  -37    ,  -37   ,  -37   ,  -37   ,  -37   ,  -37   ,  -37   ,  -37   ,  -37   ,  -37   ,  -37   ,  -37   ,   -37  ,  -37   ,  -37   ,  -37   ,  -37   ,  -37   ,  -37   ,   -37  ,   -37  ,  -37   ,  -37   ,   -37   ,   -37    ,   -37    ,   -37   ,    -37   , },
                /* 28 */{-38  ,  -38   ,  -38   ,  -38   ,  -38    ,  -38   ,  -38   ,  -38   ,  -38   ,  -38   ,  -38   ,  -38   ,  -38   ,  -38   ,  -38   ,  -38   ,   -38  ,  -38   ,  -38   ,  -38   ,  -38   ,  -38   ,  -38   ,   -38  ,   -38  ,  -38   ,  -38   ,   -38   ,   -38    ,   -38    ,   -38   ,    -38   , },
                /* 29 */{-13  ,  -13   ,  -13   ,  -13   ,  -13    ,  -13   ,  -13   ,  -13   ,  -13   ,  -13   ,  -13   ,  -13   ,  -13   ,  -13   ,  -13   ,  -13   ,   -13  ,  -13   ,  -13   ,  -13   ,  -13   ,  -13   ,  -13   ,   -13  ,   -13  ,  -13   ,  -13   ,   -13   ,   -13    ,   -13    ,   -13   ,    -13   , },
                /* 30 */{-14  ,  -14   ,  -14   ,  -14   ,  -14    ,  -14   ,  -14   ,  -14   ,  -14   ,  -14   ,  -14   ,  -14   ,  -14   ,  -14   ,  -14   ,  -14   ,   -14  ,  -14   ,  -14   ,  -14   ,  -14   ,  -14   ,  -14   ,   -14  ,   -14  ,  -14   ,  -14   ,   -14   ,   -14    ,   -14    ,   -14   ,    -14   , },
                /* 31 */{-15  ,  -15   ,  -15   ,  -15   ,  -15    ,  -15   ,  -15   ,  -15   ,  -15   ,  -15   ,  -15   ,  -15   ,  -15   ,  -15   ,  -15   ,  -15   ,   -15  ,  -15   ,  -15   ,  -15   ,  -15   ,  -15   ,  -15   ,   -15  ,   -15  ,  -15   ,  -15   ,   -15   ,   -15    ,   -15    ,   -15   ,    -15   , },
                /* 32 */{-16  ,  -16   ,  -16   ,  -16   ,  -16    ,  -16   ,  -16   ,  -16   ,  -16   ,  -16   ,  -16   ,  -16   ,  -16   ,  -16   ,  -16   ,  -16   ,   -16  ,  -16   ,  -16   ,  -16   ,  -16   ,  -16   ,  -16   ,   -16  ,   -16  ,  -16   ,  -16   ,   -16   ,   -16    ,   -16    ,   -16   ,    -16   , },
                /* 33 */{-18  ,  -18   ,  -18   ,  -18   ,  -18    ,  -18   ,  -18   ,  -18   ,  -18   ,  -18   ,  -18   ,  -18   ,  -18   ,  -18   ,  -18   ,  -18   ,   -18  ,  -18   ,  -18   ,  -18   ,  -18   ,  -18   ,  -18   ,   -18  ,   -18  ,  -18   ,  -18   ,   -18   ,   -18    ,   -18    ,   -18   ,    -18   , },
                /* 34 */{-19  ,  -19   ,  -19   ,  -19   ,  -19    ,  -19   ,  -19   ,  -19   ,  -19   ,  -19   ,  -19   ,  -19   ,  -19   ,  -19   ,  -19   ,  -19   ,   -19  ,  -19   ,  -19   ,  -19   ,  -19   ,  -19   ,  -19   ,   -19  ,   -19  ,  -19   ,  -19   ,   -19   ,   -19    ,   -19    ,   -19   ,    -19   , },
                /* 35 */{-4   ,  -4    ,  -4    ,  -4    ,  -4     ,  -4    ,  -4    ,  -4    ,  -4    ,  -4    ,  -4    ,  -4    ,  -4    ,  -4    ,  -4    ,  -4    ,   -4   ,  -4    ,  -4    ,  -4    ,  -4    ,  -4    ,  -4    ,   -4   ,   -4   ,  -4    ,  -4    ,   -4    ,   -4     ,   -4     ,   -4    ,    -4    , },
                /* 36 */{-5   ,  -5    ,  -5    ,  -5    ,  -5     ,  -5    ,  -5    ,  -5    ,  -5    ,  -5    ,  -5    ,  -5    ,  -5    ,  -5    ,  -5    ,  -5    ,   -5   ,  -5    ,  -5    ,  -5    ,  -5    ,  -5    ,  -5    ,   -5   ,   -5   ,  -5    ,  -5    ,   -5    ,   -5     ,   -5     ,   -5    ,    -5    , },
                /* 37 */{-120 ,  -120  , -120   ,  -120  ,  -120   , -120   , -120   , -120   , -120   , -120   , -120   , -120   , -120   , -120   , -120   , -120   ,  -120  , -120   , -120   , -120   , -120   , -120   , -120   ,  -120  ,  -120  , -120   , -120   ,  -120   ,  -120    ,  -120    ,  -120   ,   -120   , },
        };

        private int esPalabraReservada(string lexema)
        {
            switch (lexema)
            {
                case "abstract":
                    return -39;
                case "as":
                    return -40;
                case "assert":
                    return -41;
                case "async":
                    return -42;
                case "await":
                    return -43;
                case "break":
                    return -44;
                case "case":
                    return -45;
                case "dynamic":
                    return -46;
                case "else":
                    return -47;
                case "enum":
                    return -48;
                case "export":
                    return -49;
                case "extends":
                    return -50;
                case "external":
                    return -51;
                case "factory":
                    return -52;
                case "implements":
                    return -53;
                case "import":
                    return -54;
                case "in":
                    return -55;
                case "interface":
                    return -56;
                case "is":
                    return -57;
                case "library":
                    return -58;
                case "mixin":
                    return -59;
                case "show":
                    return -60;
                case "static":
                    return -61;
                case "super":
                    return -62;
                case "switch":
                    return -63;
                case "sync":
                    return -64;
                case "this":
                    return -65;
                case "throw":
                    return -66;
                case "catch":
                    return -67;
                case "class":
                    return -68;
                case "const":
                    return -69;
                case "continue":
                    return -70;
                case "covariant":
                    return -71;
                case "default":
                    return -72;
                case "deferred":
                    return -73;
                case "do":
                    return -74;
                case "false":
                    return -75;
                case "final":
                    return -76;
                case "finally":
                    return -77;
                case "for":
                    return -78;
                case "function":
                    return -79;
                case "get":
                    return -80;
                case "hide":
                    return -81;
                case "if":
                    return -82;
                case "new":
                    return -83;
                case "null":
                    return -84;
                case "on1":
                    return -85;
                case "operator":
                    return -86;
                case "part":
                    return -87;
                case "rethrow":
                    return -88;
                case "return":
                    return -89;
                case "set":
                    return -90;
                case "true":
                    return -91;
                case "try":
                    return -92;
                case "typeof":
                    return -93;
                case "var":
                    return -94;
                case "void":
                    return -95;
                case "while":
                    return -96;
                case "with":
                    return -97;
                case "yield":
                    return -98;
                case "int":
                    return -99;
                case "doble":
                    return -100;
                case "num":
                    return -101;
                case "string":
                    return -102;
                case "bool":
                    return -103;
                case "List":
                    return -104;
                case "Map":
                    return -105;
                case "print":
                    return -106;
                case "public":
                    return -110;
                case "protected":
                    return -111;
                case "private":
                    return -112;
                case "read":
                    return -113;
                case "date":
                    return -114;
                case " ":
                    return -120;
                default:
                    return -1;
            }
        }

        private char SiguienteCaracter(int posicion)
        {
            return Convert.ToChar(codigoFuente.Substring(posicion, 1));
        }

        private int RegresarColumna(char caracter)
        {
            if (char.IsDigit(caracter))
            {
                return 0;
            }
            else if (char.IsLetter(caracter))
            {
                return 1;
            }
            else if (caracter.Equals('"'))
            {
                return 2;
            }
            else if (caracter.Equals('\''))
            {
                return 3;
            }
            else if (caracter.Equals('+'))
            {
                return 4;
            }
            else if (caracter.Equals('-'))
            {
                return 5;
            }
            else if (caracter.Equals('*'))
            {
                return 6;
            }
            else if (caracter.Equals('/'))
            {
                return 7;
            }
            else if (caracter.Equals('%'))
            {
                return 8;
            }
            else if (caracter.Equals('<'))
            {
                return 9;
            }
            else if (caracter.Equals('>'))
            {
                return 10;
            }
            else if (caracter.Equals('='))
            {
                return 11;
            }
            else if (caracter.Equals('!'))
            {
                return 12;
            }
            else if (caracter.Equals('&'))
            {
                return 13;
            }
            else if (caracter.Equals('|'))
            {
                return 14;
            }
            else if (caracter.Equals('{'))
            {
                return 15;
            }
            else if (caracter.Equals('}'))
            {
                return 16;
            }
            else if (caracter.Equals('('))
            {
                return 17;
            }
            else if (caracter.Equals(')'))
            {
                return 18;
            }
            else if (caracter.Equals('['))
            {
                return 19;
            }
            else if (caracter.Equals(']'))
            {
                return 20;
            }
            else if (caracter.Equals('.'))
            {
                return 21;
            }
            else if (caracter.Equals(','))
            {
                return 22;
            }
            else if (caracter.Equals(';'))
            {
                return 23;
            }
            else if (caracter.Equals(':'))
            {
                return 24;
            }
            else if (caracter.Equals('?'))
            {
                return 25;
            }
            else if (caracter.Equals('_'))
            {
                return 26;
            }
            else if (caracter.Equals(' '))
            {
                return 27;
            }
            else if (caracter.Equals('\n'))
            {
                return 28;
            }
            /*
            else if (caracter.Equals('EOF'))
            {
                return 29;
            }
            */
            else if (caracter.Equals('\t'))
            {
                return 30;
            }

            else
            {
                return 31;
            }
        }

        private TipoToken esTipoToken(int estado)
        {
            switch (estado)
            {
                case -1:
                    return TipoToken.Identificador;
                case -2:
                    return TipoToken.NumeroEntero;
                case -3:
                    return TipoToken.NumeroDecimal;
                case -4:
                    return TipoToken.Cadena;
                case -5:
                    return TipoToken.Caracter;
                case -6:
                    return TipoToken.OperadorAritmetico;
                case -7:
                    return TipoToken.OperadorAritmetico;
                case -8:
                    return TipoToken.OperadorAritmetico;
                case -9:
                    return TipoToken.OperadorAritmetico;
                case -10:
                    return TipoToken.OperadorAritmetico;
                case -11:
                    return TipoToken.OperadorRelacional;
                case -12:
                    return TipoToken.OperadorRelacional;
                case -13:
                    return TipoToken.OperadorRelacional;
                case -14:
                    return TipoToken.OperadorRelacional;
                case -15:
                    return TipoToken.OperadorRelacional;
                case -16:
                    return TipoToken.OperadorRelacional;
                case -17:
                    return TipoToken.OperadorLogico;
                case -18:
                    return TipoToken.OperadorLogico;
                case -19:
                    return TipoToken.OperadorLogico;
                case -20:
                    return TipoToken.SimboloSimple;
                case -21:
                    return TipoToken.SimboloSimple;
                case -22:
                    return TipoToken.SimboloSimple;
                case -23:
                    return TipoToken.SimboloSimple;
                case -24:
                    return TipoToken.SimboloSimple;
                case -25:
                    return TipoToken.SimboloSimple;
                case -26:
                    return TipoToken.SimboloSimple;
                case -27:
                    return TipoToken.SimboloSimple;
                case -28:
                    return TipoToken.SimboloSimple;
                case -29:
                    return TipoToken.SimboloSimple;
                case -30:
                    return TipoToken.SimboloSimple;
                case -31:
                    return TipoToken.SimboloSimple;
                case -32:
                    return TipoToken.SimboloSimple;
                case -33:
                    return TipoToken.SimboloDoble;
                case -34:
                    return TipoToken.SimboloDoble;
                case -35:
                    return TipoToken.Incremento;
                case -36:
                    return TipoToken.Incremento;
                case -37:
                    return TipoToken.Incremento;
                case -38:
                    return TipoToken.Incremento;
                case -39:
                    return TipoToken.PalabraReservada;
                case -40:
                    return TipoToken.PalabraReservada;
                case -41:
                    return TipoToken.PalabraReservada;
                case -42:
                    return TipoToken.PalabraReservada;
                case -43:
                    return TipoToken.PalabraReservada;
                case -44:
                    return TipoToken.PalabraReservada;
                case -45:
                    return TipoToken.PalabraReservada;
                case -46:
                    return TipoToken.PalabraReservada;
                case -47:
                    return TipoToken.PalabraReservada;
                case -48:
                    return TipoToken.PalabraReservada;
                case -49:
                    return TipoToken.PalabraReservada;
                case -50:
                    return TipoToken.PalabraReservada;
                case -51:
                    return TipoToken.PalabraReservada;
                case -52:
                    return TipoToken.PalabraReservada;
                case -53:
                    return TipoToken.PalabraReservada;
                case -54:
                    return TipoToken.PalabraReservada;
                case -55:
                    return TipoToken.PalabraReservada;
                case -56:
                    return TipoToken.PalabraReservada;
                case -57:
                    return TipoToken.PalabraReservada;
                case -58:
                    return TipoToken.PalabraReservada;
                case -59:
                    return TipoToken.PalabraReservada;
                case -60:
                    return TipoToken.PalabraReservada;
                case -61:
                    return TipoToken.PalabraReservada;
                case -62:
                    return TipoToken.PalabraReservada;
                case -63:
                    return TipoToken.PalabraReservada;
                case -64:
                    return TipoToken.PalabraReservada;
                case -65:
                    return TipoToken.PalabraReservada;
                case -66:
                    return TipoToken.PalabraReservada;
                case -67:
                    return TipoToken.PalabraReservada;
                case -68:
                    return TipoToken.PalabraReservada;
                case -69:
                    return TipoToken.PalabraReservada;
                case -70:
                    return TipoToken.PalabraReservada;
                case -71:
                    return TipoToken.PalabraReservada;
                case -72:
                    return TipoToken.PalabraReservada;
                case -73:
                    return TipoToken.PalabraReservada;
                case -74:
                    return TipoToken.PalabraReservada;
                case -75:
                    return TipoToken.PalabraReservada;
                case -76:
                    return TipoToken.PalabraReservada;
                case -77:
                    return TipoToken.PalabraReservada;
                case -78:
                    return TipoToken.PalabraReservada;
                case -79:
                    return TipoToken.PalabraReservada;
                case -80:
                    return TipoToken.PalabraReservada;
                case -81:
                    return TipoToken.PalabraReservada;
                case -82:
                    return TipoToken.PalabraReservada;
                case -83:
                    return TipoToken.PalabraReservada;
                case -84:
                    return TipoToken.PalabraReservada;
                case -85:
                    return TipoToken.PalabraReservada;
                case -86:
                    return TipoToken.PalabraReservada;
                case -87:
                    return TipoToken.PalabraReservada;
                case -88:
                    return TipoToken.PalabraReservada;
                case -89:
                    return TipoToken.PalabraReservada;
                case -90:
                    return TipoToken.PalabraReservada;
                case -91:
                    return TipoToken.PalabraReservada;
                case -92:
                    return TipoToken.PalabraReservada;
                case -93:
                    return TipoToken.PalabraReservada;
                case -94:
                    return TipoToken.PalabraReservada;
                case -95:
                    return TipoToken.PalabraReservada;
                case -96:
                    return TipoToken.PalabraReservada;
                case -97:
                    return TipoToken.PalabraReservada;
                case -98:
                    return TipoToken.PalabraReservada;
                case -99:
                    return TipoToken.PalabraReservada;
                case -100:
                    return TipoToken.PalabraReservada;
                case -101:
                    return TipoToken.PalabraReservada;
                case -102:
                    return TipoToken.PalabraReservada;
                case -103:
                    return TipoToken.PalabraReservada;
                case -104:
                    return TipoToken.PalabraReservada;
                case -105:
                    return TipoToken.PalabraReservada;
                case -106:
                    return TipoToken.PalabraReservada;
                case -107:
                    return TipoToken.Asignacion;
                case -108:
                    return TipoToken.Comentario;
                case -109:
                    return TipoToken.ComentarioMultilinea;
                case -110:
                    return TipoToken.PalabraReservada;
                case -111:
                    return TipoToken.PalabraReservada;
                case -112:
                    return TipoToken.PalabraReservada;
                case -113:
                    return TipoToken.PalabraReservada;
                case -114:
                    return TipoToken.PalabraReservada;
                case -120:
                    return TipoToken.Lambda;
                default:
                    return TipoToken.Desconocido;
            }
        }

        private Error Errores(int estado)
        {
            string mensajeError = "";

            switch (estado)
            {
                case 31:
                    mensajeError = "Simobolo Desconocido";
                    break;
                case -500:
                    mensajeError = "Identificador no valido";
                    break;
                case -501:
                    mensajeError = "Número entero no valido";
                    break;
                case -502:
                    mensajeError = "Número decimal no valido";
                    break;
                case -503:
                    mensajeError = "Formato de cadena invalida: Se esperaba una comilla";
                    break;
                case -504:
                    mensajeError = "Se esperaba un carácter";
                    break;
                case -505:
                    mensajeError = "Se esperaba un fin de comentario";
                    break;
                case -506:
                    mensajeError = "Palabra desconocida";
                    break;
                case -507:
                    mensajeError = "Formato de operador invalido, falta una |";
                    break;
                case -508:
                    mensajeError = "Formato de operador invalido, falta una &";
                    break;
                default:
                    mensajeError = "Error Desconocido";
                    break;
            }
            return new Error() { Codigo = estado, MensajeError = mensajeError, TipoError = tipoError.Lexico, Linea = linea };
        }

    }
}
