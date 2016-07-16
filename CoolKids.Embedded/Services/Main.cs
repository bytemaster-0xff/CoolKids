using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoolKids.Uwp.Embedded.Services
{
    public class Main : SimpleRest
    {

        public event EventHandler<string> MessageReceived;

        public override string Process(string queryString)
        {
            Debug.WriteLine(queryString);

            MessageReceived(this, queryString.Replace("%20"," ").TrimStart('/'));

            return "{'status':'ok'}";
        }
    }
}
