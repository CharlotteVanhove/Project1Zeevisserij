using VisStatsBL.Exceptions;

namespace VisStatsBL.Model
{
    public class Vissoort
    {
        public int? ID; //waarom niet een prop? en waarom nullable? omdat het bij de start geen waarde zal hebben 
        private string naam;
        public string Naam
        {
            get { return naam; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new DomeinException("Vissoort_naam");
                naam = value;
            }
        }

        public Vissoort(int iD, string naam)
        {
            ID = iD;
            Naam = naam;
        }
        public Vissoort(string naam)
        {
            Naam = naam;
        } //ctor aanmaken, omdat naam verplicht is 

        public override string ToString()
        {
            return Naam;
        }
    }
}
