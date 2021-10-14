using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RundooApi
{
    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string? message) : base(message)
        {

        }
    }

    public class InvalidQueryTypeException : Exception
    {
        public InvalidQueryTypeException(string? message) : base(message)
        {

        }
    }
}
