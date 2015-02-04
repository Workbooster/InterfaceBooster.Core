using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sys = System;

namespace InterfaceBooster.Common.Tools.Data.ExceptionHandling
{
    public static class ExceptionHelper
    {
        /// <summary>
        /// Gets a string that contains all Exception.Message texts from the nestest exceptions.
        /// </summary>
        /// <param name="ex">the root exception</param>
        /// <param name="maxDepth">set a maximum level of nested Exceptions</param>
        /// <returns></returns>
        public static string GetNestedExceptionMessages(sys.Exception ex, int maxDepth = 10)
        {
            sys.Exception currentException = ex;
            StringBuilder sb = new StringBuilder();

            do
            {
                // append the message to the string
                sb.AppendLine(currentException.Message);

                // goto next level of exception
                currentException = currentException.InnerException;
                maxDepth--;
            } while (currentException != null && maxDepth > 0);

            return sb.ToString();
        }
    }
}
