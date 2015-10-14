using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGTLocalizer
{
    public class Client
    {
        public Dictionary<String, String> textVariables { get; set; }

        public Client() {
            this.textVariables = new Dictionary<string, string>();
        }

        public Client(Dictionary<String, String> textVariables) {
            this.textVariables = textVariables;
        }

        public bool AddVar(String key, String value){
            bool valueUpdated = textVariables.Contains(new KeyValuePair<String, String>(key, value));
            this.textVariables.Add(key, value);
            return valueUpdated;
        }


    }
}
