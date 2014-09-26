using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HavprognoseTjekker
{
    public class PrognoseBillede
    {
        public string VindUrl { get; private set; }
        public string VindRetningsUrl { get; private set; }
        public DateTime Tidspunkt { get; private set; }
        public DateTime PrognoseTidspunkt { get; private set; }
        public string DagTekst
        {
            get
            {
                return  System.Web.HttpUtility.HtmlEncode(Ugedag) +" " +  Tidspunkt.ToShortDateString();
            }
        }

        public string TidTekst
        {
            get
            {
                return  Tidspunkt.ToShortTimeString();
            }

        }

        public PrognoseBillede(DateTime prognoseTidspunkt, DateTime tidspunkt)
        {
            Tidspunkt = tidspunkt;
            PrognoseTidspunkt = prognoseTidspunkt;
            VindUrl = string.Format("http://www.dmi.dk/fileadmin/SeaForecast/farvandet_nord_for_fyn/Wind.{4}{5}{6}_{7}00.{0}{1}{2}_{3}00.png", Tidspunkt.Year, Tidspunkt.Month.ToString().PadLeft(2, '0'), Tidspunkt.Day.ToString().PadLeft(2, '0'), (Tidspunkt.Hour).ToString().PadLeft(2, '0'), PrognoseTidspunkt.Year, PrognoseTidspunkt.Month.ToString().PadLeft(2, '0'), PrognoseTidspunkt.Day.ToString().PadLeft(2, '0'), PrognoseTidspunkt.Hour.ToString().PadLeft(2, '0'));
            VindRetningsUrl = string.Format("http://www.dmi.dk/fileadmin/SeaForecast/farvandet_nord_for_fyn/WindDirection.{4}{5}{6}_{7}00.{0}{1}{2}_{3}00.png", Tidspunkt.Year, Tidspunkt.Month.ToString().PadLeft(2, '0'), Tidspunkt.Day.ToString().PadLeft(2, '0'), (Tidspunkt.Hour).ToString().PadLeft(2, '0'), PrognoseTidspunkt.Year, PrognoseTidspunkt.Month.ToString().PadLeft(2, '0'), PrognoseTidspunkt.Day.ToString().PadLeft(2, '0'), PrognoseTidspunkt.Hour.ToString().PadLeft(2, '0'));

        }

        public string Ugedag
        {
            get
            {
                switch (Tidspunkt.DayOfWeek)
                {
                    case DayOfWeek.Friday:
                        return "Fredag";
                        break;
                    case DayOfWeek.Monday:
                        return "Mandag";
                        break;
                    case DayOfWeek.Saturday:
                        return "Lørdag";
                        break;
                    case DayOfWeek.Sunday:
                        return "Søndag";
                        break;
                    case DayOfWeek.Thursday:
                        return "Torsdag";
                        break;
                    case DayOfWeek.Tuesday:
                        return "Tirsdag";
                        break;
                    case DayOfWeek.Wednesday:
                        return "Onsdag";
                        break;
                    default:
                        throw new NotImplementedException();
                        break;
                }
            }       
        }
    }
}
