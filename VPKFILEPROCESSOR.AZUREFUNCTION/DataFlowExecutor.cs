// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using System;
using Azure.Messaging;
using Azure.ResourceManager.DataFactory.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace VPKFILEPROCESSOR.AZUREFUNCTION
{
    public class DataFlowExecutor
    {
        private readonly ILogger<DataFlowExecutor> _logger;
      

        public DataFlowExecutor(ILogger<DataFlowExecutor> logger)
        {
            _logger = logger;
        }

        [Function(nameof(DataFlowExecutor))]
        public void Run([EventGridTrigger] CloudEvent cloudEvent)
        {
            _logger.LogInformation("Event type: {type}, Event subject: {subject}", cloudEvent.Type, cloudEvent.Subject);
        }
    }
}
