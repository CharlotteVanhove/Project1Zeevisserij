using VisStatsBL.Enum;
using VisStatsBL.Exceptions;
using VisStatsBL.Interfaces;
using VisStatsBL.Model;

namespace VisStatsBL.Manager
{
    public class VisStatsManager //moet zichtbaar zijn in de UI laag 
    {
        private IFileProcessor _fileProcessor;
        private IVisStatsRepository _visStatsRepository;

        public VisStatsManager(IFileProcessor fileProcessor, IVisStatsRepository visStatsRepository)
        {
            this._fileProcessor = fileProcessor;
            this._visStatsRepository = visStatsRepository;
        }

        public void UploadVissoorten(string fileName)
        {
            try
            {
                List<string> soorten = _fileProcessor.LeesSoorten(fileName);
                List<Vissoort> vissoorten = MaakVissoorten(soorten);
                foreach (Vissoort vis in vissoorten)
                {
                    if (!_visStatsRepository.HeeftVissoort(vis)) //! om te kijken of het niet dubbel is of naamgeving veranderen
                    {
                        _visStatsRepository.SchrijfSoort(vis);
                    }
                }
            }
            catch (Exception ex) { } // te vermijden dat programma zou afsluiten als er iets mis gaat 

        }
        private List<Vissoort> MaakVissoorten(List<string> soorten)
        {
            Dictionary<string, Vissoort> visSoorten = new(); //dictionary omdat het snel moet gaan ipv list of voor grote bestanden, gebruikt dan ook key om te vergelijken jij hebt een string in kijkt of het in vissoort zit en vergelijkt op naam) 
            foreach (string soort in soorten)
            {
                if (!visSoorten.ContainsKey(soort)) //kijken of er geen dubbele vissoorten inzitten  
                {
                    try
                    {
                        visSoorten.Add(soort, new Vissoort(soort));
                    }
                    catch (Exception ex) { throw new ManagerException("UploadVissoorten", ex); }
                }
            }
            return visSoorten.Values.ToList();
        }

        public void UploadHavens(string fileName)
        {
            try
            {
                List<string> soorten = _fileProcessor.LeesHavens(fileName);
                List<Haven> havens = MaakHavens(soorten);
                foreach (Haven haven in havens)
                {
                    if (!_visStatsRepository.HeeftHaven(haven)) //! om te kijken of het niet dubbel is of naamgeving veranderen
                    {
                        _visStatsRepository.SchrijfHaven(haven);
                    }
                }
            }
            catch (Exception ex) { throw new ManagerException("UploadHavens", ex); }

        }
        private List<Haven> MaakHavens(List<string> soorten)
        {
            Dictionary<string, Haven> havens = new();
            foreach (string soort in soorten)
            {
                if (!havens.ContainsKey(soort))
                {
                    try
                    {
                        havens.Add(soort, new Haven(soort));
                    }
                    catch (DomeinException) { }
                }
            }
            return havens.Values.ToList();
        }

        public void UploadStatistieken(string fileName)
        {
            try
            {
                //dit pad opslaan. want is een groter bestand en wil je nie graag dubbel doen. voor is en haven wordt het gecontroleerd vanuit het bestand zelf 
                if (!_visStatsRepository.IsOpgeladen(fileName))
                {
                    List<Vissoort> soorten = _visStatsRepository.LeesVissoorten();
                    List<Haven> havens = _visStatsRepository.LeesHavens();
                    List<VisStatsDataRecord> data = _fileProcessor.LeesStatistieken(fileName, soorten, havens);
                    _visStatsRepository.SchrijfStatistieken(data, fileName); //filename, om te checken of het er al in zit
                }
            }
            catch (Exception ex) { throw new ManagerException("UploadStatistieken", ex); }
        }

        public List<Haven> GeefHavens()
        {
            try
            {
                return _visStatsRepository.LeesHavens();
            }
            catch (Exception ex) {
                 throw new ManagerException("GeefHavens", ex); 
            }
        }

        public List<int> GeefJaartallen()
        {
            try
            {
                return _visStatsRepository.LeesJaartallen();
            }
            catch (Exception ex) {
                throw new ManagerException("GeefJaartallen", ex);
            }
        }
        public List<Vissoort> GeefVissoorten()
        {
            try
            {
                return _visStatsRepository.LeesVissoorten();
            }
            catch (Exception ex) {
                throw new ManagerException("GeefVissoorten", ex);
            } 
        }

        public List<JaarVangst> GeefVangst (int jaar, Haven haven, List<Vissoort> vissoorten, Eenheid eenheid)
        {
            try
            {
                return _visStatsRepository.LeesStatistieken(jaar, haven, vissoorten, eenheid);
            }
            catch (Exception ex) {
                throw new ManagerException("GeefVissoorten", ex);
            } 
        }
    } 
}
