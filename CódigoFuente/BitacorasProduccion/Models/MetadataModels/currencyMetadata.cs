using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Portal_2_0.Models
{
    public class currencyMetadata
    {
    }

    [MetadataType(typeof(currencyMetadata))]
    public partial class currency
    {
        //Concatena clave y nombre
        [NotMapped]
        public string CocatCurrency
        {
            get
            {
                return "(" + CurrencyISO + ") " + CurrencyName;
            }
        }
    }
}