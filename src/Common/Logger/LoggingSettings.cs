using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    public class LoggingSettings
    {
        public string ElasticSearchUrl { get; set; }
        public string ElasticSearchUsername { get; set; }
        public string ElasticSearchPassword { get; set; }
        public string ElasticIndexFormatRoot { get; set; }
        public string ElasticBufferRoot { get; set; }
        public string RoolingFileName { get; set; }
        public string SeqServerUrl { get; set; }

        public bool IsElkActive => !String.IsNullOrWhiteSpace(ElasticSearchUrl);
        public bool HasElkCredentials => (!String.IsNullOrWhiteSpace(ElasticSearchPassword) && !String.IsNullOrWhiteSpace(ElasticSearchPassword));
        public bool IsSeqActive => !String.IsNullOrWhiteSpace(SeqServerUrl);
    }
}
