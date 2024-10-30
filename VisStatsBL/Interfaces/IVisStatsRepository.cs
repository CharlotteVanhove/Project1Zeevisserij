using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisStatsBL.Enum;
using VisStatsBL.Model;

namespace VisStatsBL.Interfaces
{
    public interface IVisStatsRepository
    {
        bool HeeftHaven(Haven haven);
        bool HeeftVissoort(Vissoort vis);
        bool IsOpgeladen(string fileName);
        List<Haven> LeesHavens(); 
        List<Vissoort> LeesVissoorten(); 
        void SchrijfHaven(Haven haven);
        void SchrijfSoort(Vissoort vis);
        void SchrijfStatistieken(List<VisStatsDataRecord> data, string fileName);
        List<int> LeesJaartallen();
        List<JaarVangst> LeesStatistieken(int jaar, Haven haven, List<Vissoort> vissoorten, Eenheid eenheid);
    }
}
