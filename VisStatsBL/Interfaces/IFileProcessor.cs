using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisStatsBL.Model;

namespace VisStatsBL.Interfaces
{
    public interface IFileProcessor //interface om andere klasses te implementeren zonder dat hij gelinkt is aan iets 
    {
        List<string> LeesSoorten(string fileName);
        List<string> LeesHavens(string fileName);
        List<VisStatsDataRecord> LeesStatistieken(string fileName, List<Vissoort> vissoorten, List<Haven> havens);

    }
}
