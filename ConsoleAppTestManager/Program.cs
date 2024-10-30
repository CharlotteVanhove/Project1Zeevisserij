using VisStatsBL.Interfaces;
using VisStatsBL.Manager;
using VisStatsDL_File;
using VisStatsDL_SQL;

namespace ConsoleAppTestManager
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            string conn = @"Data Source=MSI\SQLEXPRESS;Initial Catalog=VisStats;Integrated Security=True;Trust Server Certificate=True";
            string filePath = @"C:\Hogent\ProgrammerenGevorderd\project1\Vis\vissoorten.txt"; //@ gebruiken zodat je geen 2 / hoeft te schrijven
            IFileProcessor fp = new FileProcessor();
            IVisStatsRepository repo = new VisStatsRepository(conn);
            VisStatsManager vm = new VisStatsManager(fp,repo);
            vm.UploadVissoorten(filePath);
        }
    }
}
