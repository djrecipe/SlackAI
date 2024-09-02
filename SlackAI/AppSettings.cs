using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlackAI
{
    public class AppSettings
    {
        public string ApiKey { get; set; }
        public string ApiKeySockets { get; set; }
        public string ModelPath { get; set; }
        public string OpenAIKey { get; set; }
        public string OpenAIEndpoint { get; set; }
    }
}
