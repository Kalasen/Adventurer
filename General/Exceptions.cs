using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adventurer
{
    public class GraphicsException : Exception
    {
        public GraphicsException()
        {
        }

        public GraphicsException(string message)
            : base(message)
        {
        }

        public GraphicsException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
