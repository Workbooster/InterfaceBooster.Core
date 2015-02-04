using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceBooster.RuntimeController.Broadcasting
{
    /// <summary>
    /// can be used to broadcast a question up to the view
    /// </summary>
    /// <param name="question"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public delegate object AskQuestionDelegate(string question, Dictionary<string, object> options = null);
}
