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

        public bool IsElkActive => !string.IsNullOrWhiteSpace(ElasticSearchUrl);
        public bool HasElkCredentials => (!string.IsNullOrWhiteSpace(ElasticSearchPassword) && !string.IsNullOrWhiteSpace(ElasticSearchPassword));
        public bool IsSeqActive => !string.IsNullOrWhiteSpace(SeqServerUrl);
    }
}
