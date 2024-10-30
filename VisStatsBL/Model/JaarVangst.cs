using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisStatsBL.Model
{

    public class JaarVangst
    {
        public string Soortnaam { get; set; } //string ipv lijst van vissoort. geeft makkelijker weer. de lijst herkent hij niet zo gemakkelijk
        public double Totaal { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public double Gemiddelde { get; set; }
        public JaarVangst(string soortnaam, double totaal, double min, double max, double gemiddelde)
        {
            Soortnaam = soortnaam;
            Totaal = totaal;
            Min = min;
            Max = max;
            Gemiddelde = gemiddelde;
        }

        //public override string ToString()
        //{ 
        //    return Soortnaam; 
        //}
    }
}
