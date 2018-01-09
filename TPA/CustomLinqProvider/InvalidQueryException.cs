using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomLinqProvider
{
    class InvalidQueryException : Exception
    {
        private string message;

        public InvalidQueryException(string message) => this.message = message + " ";

        public override string Message => "The client query is invalid: " + message;
    }
}
