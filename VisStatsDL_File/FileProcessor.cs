using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VisStatsBL.Interfaces;
using VisStatsBL.Model;

namespace VisStatsDL_File
{
    public class FileProcessor : IFileProcessor

    {
        public List<string> LeesSoorten(string fileName) // list van string en geen vissoorten omdat we nog controles moeten uitvoeren 
        {
            try
            {
                List<string> soorten = new List<string>();
                using (StreamReader sr = new StreamReader(fileName))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null) //zolang het bestand niet lees is wordt deze ingelezen
                    {
                        soorten.Add(line.Trim());
                    }
                }
                return soorten;
            }
            catch (Exception ex) { throw new Exception("FileProcessor-LeesSoorten", ex); } //geen costum exception, want ze herkennen dit niet in DL
        }
        public List<string> LeesHavens(string fileName)
        {
            try
            {
                List<string> soorten = new List<string>();
                using (StreamReader sr = new StreamReader(fileName))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        soorten.Add(line.Trim());
                    }
                }
                return soorten;
            }
            catch (Exception ex) { throw new Exception("FileProcessor-LeesHavens", ex); }
        }

        public List<VisStatsDataRecord> LeesStatistieken(string fileName, List<Vissoort> vissoorten, List<Haven> havens)
        {
            try
            {
                //sleuten (string) naam van vissoort of haven. want daar zoeken we naar. gebruik de lijst. een LINQ om de lijst naar dictionary om te zetten. neem de naam en neem de vissoort 
                Dictionary<string, Vissoort> soortenD = vissoorten.ToDictionary(x => x.Naam, x => x);
                Dictionary<string, Haven> havensD = havens.ToDictionary(x => x.Naam, x => x);

                Dictionary<(string, int, int, string), VisStatsDataRecord> data = new(); //haven, jaar, maand, soort is een tuble. klasse zonder naam
                using (StreamReader sr = new StreamReader(fileName))
                {
                    string line;
                    int jaar = 0, maand = 0;
                    //bij list ligt de volgorde vast, is lijst van de 3 havens 
                    List<string> havensTXT = new List<string>();
                    while ((line = sr.ReadLine()) != null)
                    {
                        //lees tot het begin van een maand wordt gevonden
                        //------------202201-------------
                        //+kan je vervangen door het aantal als je weet hoeveel - er zijn 
                        if (Regex.IsMatch(line, @"^-+\d{6}-+"))
                        {
                            Console.WriteLine(line);
                            jaar = Int32.Parse(Regex.Match(line, @"\d{4}").Value);
                            maand = Int32.Parse(Regex.Match(line, @"(\d{2})-+").Groups[1].Value); //als je haakjes plaats worden er groepen gemaakt, anders heb je nog de ---- bij .mag je met chatgpt maken, maar controleer of het klopt 
                            Console.WriteLine($"{jaar}, {maand}");
                            havensTXT.Clear(); //clear als je nieuwe maand begint, anders vult hij dezelfde lijst steeds aan
                        }
                        //lees de verschillende havens in
                        //vissoorten|Totaal van de havens||Oostende||Zeebrugge||Niewpoort|

                        else if (line.Contains("Vissoorten|Totaal van de havens"))
                        {
                            // \| cursief zoekt letterlijk het karakter na \
                            string pattern = @"\|([A-Za-z]+)\|";
                            MatchCollection matches = Regex.Matches(line, pattern); //collection, omdat het er meerdere zijn, zal dus 3keer overlopen
                            foreach (Match match in matches) havensTXT.Add(match.Groups[1].Value);
                        }
                        //lees data
                        //Schelvis|5521|11318,0199999999999999997|1828&4987,73000000000005|3693|6330,2900000002|-|-
                        else
                        {
                            string[] element = line.Split('|');
                            //eerste element is de naam van de vissoort
                            if (soortenD.ContainsKey(element[0]))
                            {
                                for (int i = 0; i < havensTXT.Count; i++)
                                {
                                    {
                                        if (havensD.ContainsKey(havensTXT[i]))
                                        {
                                            if (data.ContainsKey((havensTXT[i], jaar, maand, element[0])))
                                            {
                                                // als de sleutel bestaat, herkent hij de andere soort en zal hij de waarde optellen bij de al reeds gekende sleutel 
                                                data[(havensTXT[i], jaar, maand, element[0])].Gewicht += ParseValue(element[(i * 2) + 3]); //positie 3 is voor waarde uit juiste haven. telkens 2 opschuiven voor volgende haven
                                                data[(havensTXT[i], jaar, maand, element[0])].Waarde += ParseValue(element[(i * 2) + 4]);
                                            }
                                            else
                                            {
                                                //de nieuwe toevoegen, naam haven, jaar, maand en vissoort. sleutel. datarecorde: soort, haven, jaar,maand, gewicht, waarde(positie werd bepaald ervoor)
                                                data.Add((havensTXT[i], jaar, maand, element[0]), new VisStatsDataRecord(soortenD[element
                                                    [0]], havensD[havensTXT[i]], jaar, maand, ParseValue(element[(i * 2) + 3]), ParseValue(element[(i * 2) + 4])));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }


                return data.Values.ToList();
            }
            // wanneer er een fout is, stopt de verwerking van het bestand. geen costum ex, want dit herkent hij niet in de datalaag
            catch (Exception ex)
            {
                throw new Exception("FileProcessor-LeesMonthlyResults", ex);
            }
        }

        private double ParseValue(string value)
        {
            // - vervangen door een 0, want dat wil zeggen 'geen waarde'
            if (double.TryParse(value, out double d)) return d;
            else return 0.0;
        }
    }
}
