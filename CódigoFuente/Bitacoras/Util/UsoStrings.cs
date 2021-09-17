using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Clases.Util
{
    public class UsoStrings
    {
        public static string ReemplazaCaracteres(string inputString)
        {
            Regex replace_a_Accents = new Regex("[á|à|ä|â]", RegexOptions.Compiled);
            Regex replace_e_Accents = new Regex("[é|è|ë|ê]", RegexOptions.Compiled);
            Regex replace_i_Accents = new Regex("[í|ì|ï|î]", RegexOptions.Compiled);
            Regex replace_o_Accents = new Regex("[ó|ò|ö|ô]", RegexOptions.Compiled);
            Regex replace_u_Accents = new Regex("[ú|ù|ü|û]", RegexOptions.Compiled);
            Regex replace_A_Accents = new Regex("[Á|À|Ä|Â]", RegexOptions.Compiled);
            Regex replace_E_Accents = new Regex("[É|È|Ë|Ê]", RegexOptions.Compiled);
            Regex replace_I_Accents = new Regex("[Í|Ì|Ï|Î]", RegexOptions.Compiled);
            Regex replace_O_Accents = new Regex("[Ó|Ò|Ö|Ô]", RegexOptions.Compiled);
            Regex replace_U_Accents = new Regex("[Ú|Ù|Ü|Û]", RegexOptions.Compiled);
            Regex replace_N_Accents = new Regex("[Ñ]", RegexOptions.Compiled);
            Regex replace_n_Accents = new Regex("[ñ]", RegexOptions.Compiled);
            inputString = replace_a_Accents.Replace(inputString, "a");
            inputString = replace_e_Accents.Replace(inputString, "e");
            inputString = replace_i_Accents.Replace(inputString, "i");
            inputString = replace_o_Accents.Replace(inputString, "o");
            inputString = replace_u_Accents.Replace(inputString, "u");
            inputString = replace_A_Accents.Replace(inputString, "A");
            inputString = replace_E_Accents.Replace(inputString, "E");
            inputString = replace_I_Accents.Replace(inputString, "I");
            inputString = replace_O_Accents.Replace(inputString, "O");
            inputString = replace_U_Accents.Replace(inputString, "U");
            inputString = replace_N_Accents.Replace(inputString, "N");
            inputString = replace_n_Accents.Replace(inputString, "n");
            return inputString;
        }

        public static string RecortaString(string inputString, int limite)
        {
            string valor = inputString;

            if (valor.Length > limite)
            {
                valor.Substring(0, limite - 1);
            }
            return valor;
        }
    }
}
