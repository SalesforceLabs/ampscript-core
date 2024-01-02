using System;
using System.Collections.Generic;

namespace MarketingCloudIntegration.Render
{
    [Serializable]
    public class RenderRequest
    {
        public string? context { get; set; }
        public string? content { get; set; }
        public Dictionary<string, string>? attributes { get; set; }
        public DataExtension? dataExtension { get; set; }
        public Recipient? recipient { get; set; }
    }

    [Serializable]
    public class DataExtension
    {
        public string? customerKey { get; set; }
        public string? objectID { get; set; }
        public string? row { get; set; }

    }

    [Serializable]
    public class Recipient
    {
        public Dictionary<string, string>? attributes { get; set; }
        public string? contactKey { get; set; }
        public string? appID { get; set; }
        public string? deviceID { get; set; }
        public string? mobileNumber { get; set; }
    }

    [Serializable]
    public class RenderResponse
    {
        public long? renderServiceLogID { get; set; }
        public string? renderedContent { get; set; }
        public List<string>? contentFieldsNotFound { get; set; }

    }
}
