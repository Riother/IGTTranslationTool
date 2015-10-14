using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IGTLocalizer
{
    public class Client
    {
        public Dictionary<String, String> TextVariables { get; set; }

        public Client() {
            this.TextVariables = new Dictionary<string, string>();
        }

        public Client(Dictionary<String, String> textVariables) {
            this.TextVariables = textVariables;
        }

        public bool AddVar(String key, String value){
            bool valueUpdated = TextVariables.Contains(new KeyValuePair<String, String>(key, value));
            this.TextVariables.Add(key, value);
            return valueUpdated;
        }


    }
}
