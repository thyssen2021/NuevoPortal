using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bitacoras.Util
{
    public class TimeSpanUtil
    {
        public static Boolean CalculateDalUren(DateTime datum, TimeSpan? dalStart, TimeSpan? dalEnd)
        {
            Boolean isDal = false;
            DateTime StartDate = DateTime.Today;
            DateTime EndDate = DateTime.Today;

            //Check whether the dalEnd is lesser than dalStart
            if (dalStart >= dalEnd)
            {
                //Increase the date if dalEnd is timespan of the Nextday 
                EndDate = EndDate.AddDays(1);
            }

            //Assign the dalStart and dalEnd to the Dates
            StartDate = StartDate.Date + dalStart.Value;
            EndDate = EndDate.Date + dalEnd.Value;

            if ((datum >= StartDate) && (datum <= EndDate))
            {
                isDal = true;
            }
            return isDal;
        }
    }
}
