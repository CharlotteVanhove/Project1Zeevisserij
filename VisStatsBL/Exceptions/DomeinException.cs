using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisStatsBL.Exceptions
{
    public class DomeinException : Exception
    {
        public DomeinException(string? message) //? wanneer het geen waarde moet of kan hebben
            :base(message)
        { 
        }
        public DomeinException(string? message, Exception? innerException) //innerException geeft en exacte fout weer
            : base(message, innerException)
        {
            
        }
    }
}
