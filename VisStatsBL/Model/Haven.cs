using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisStatsBL.Exceptions;

namespace VisStatsBL.Model
{
    public class Haven
    {
        public int? ID; //waarom niet een prop? en waarom nullable? omdat het bij de start geen waarde zal hebben 
        private string _naam;
        public string Naam
        {
            get { return _naam; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new DomeinException("Haven_naam");
                _naam = value;
            }
        }
        public Haven(int iD, string naam)
        {
            ID = iD;
            Naam = naam;
        }
        public Haven(string naam)
        {
            Naam = naam;
        }

        public override string ToString()
        {
            return Naam;
        }

    }
}
