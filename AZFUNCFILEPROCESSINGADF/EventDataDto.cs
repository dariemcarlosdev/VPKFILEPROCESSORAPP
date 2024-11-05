// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

namespace AZFUNCFILEPROCESSINGADF
{
    /// <summary>
    /// Event Data class is created to deserialize the event data from the event grid trigger.
    /// </summary>
    internal class EventDataDto
    {
        public string FileName { get; set; }
        public string FileUrl { get; set; } = string.Empty;
    }
}