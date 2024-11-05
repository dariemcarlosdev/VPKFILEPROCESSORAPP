// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}

using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Microsoft.Extensions.Logging;
using Azure.Messaging.EventGrid;
using Microsoft.Azure.Functions.Worker;
using Azure.ResourceManager.DataFactory;
using Azure.Messaging;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using System.Text.Json;
using System.Reflection.Metadata;

namespace AZFUNCFILEPROCESSINGADF
{
    public class DataFlowExecutor
    {
        private readonly ILogger<DataFlowExecutor> _logger;
        private readonly string _subscriptionId = Environment.GetEnvironmentVariable("AZURE_SUBSCRIPTION_ID")!; //Subscription Id 1458d008-6c2a-465e-b323-b3a5fef295af
        private readonly string _resourceGroupName = Environment.GetEnvironmentVariable("AZURE_RESOURCE_GROUP")!; //Resource Group name VPKProcessorRG_dev
        private readonly string _dataFactoryName = Environment.GetEnvironmentVariable("AZURE_DATAFACTORY_NAME")!; //Data Factory name ADFSSISProcessor
        private readonly string _userAssignedIdentityClientId = Environment.GetEnvironmentVariable("USER_ASSIGNED_IDENTITY_CLIENT_ID")!; //User assigned identity client id d82ccebf-face-49f8-a4a4-e1b48927d209

        public DataFlowExecutor(ILogger<DataFlowExecutor> logger)
        {
            _logger = logger;
        }

        [Function(nameof(DataFlowExecutor))]
        public async Task RunAsync([EventGridTrigger] EventGridEvent eventGridEvent)
        {
            _logger.LogInformation("Event type: {type}, Event subject: {subject}", eventGridEvent.EventType, eventGridEvent.Subject, eventGridEvent.Data);

            try
            {

                //Deserializing the event data to get the file name
                EventDataDto eventData = JsonSerializer.Deserialize<EventDataDto>(eventGridEvent.Data.ToString());
                var fileName = eventData?.FileName;
                var fileUrl = eventData?.FileUrl;

                _logger.LogInformation($"File Name: {fileName}, File URL: {fileUrl}");

                if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(fileUrl))
                {
                    _logger.LogError("Invalid event data. File name, file type, and file URL are required.");
                    return;
                }


                //Use Managed Identity to authenticate with Azure Data Factory
                //https://docs.microsoft.com/en-us/azure/data-factory/connector-azure-data-lake-storage#managed-identity-authentication

                var credentialOptions = new DefaultAzureCredentialOptions
                {
                    ManagedIdentityClientId = _userAssignedIdentityClientId //Specify the user assigned identity client id
                };

                // Create AzureCredentials using DefaultAzureCredential
                var azureCredentials = new DefaultAzureCredential(credentialOptions);

                //implementation as alternative to keep the code clean

                //initialize the ArmClient with the credentials which is part of the newer version of Azure.ResourceManager, this works with TokenCredential and not AzureCredentials, allows managing Azure resources using Azure SDK.
                var armClient = new ArmClient(azureCredentials);
                //create the data factory pipeline resource id using the subscription id, resource group name, data factory name and pipeline name
                var dataFactoryPipelineResourceId = DataFactoryPipelineResource.CreateResourceIdentifier(_subscriptionId, _resourceGroupName, _dataFactoryName, "VPKFileProcessorPipeline");
                //retrieve the data factory pipeline resource using the data factory pipeline resource id
                var dataFactoryPipelineResource = armClient.GetDataFactoryPipelineResource(dataFactoryPipelineResourceId);

                // Define the parameters to pass to the pipeline                
                var binaryDataPipeParameters = new Dictionary<string, BinaryData>
                {
                    { "inputFileName", BinaryData.FromString(JsonSerializer.Serialize(fileName))}, //inputPath is the parameter name in the pipeline
                    { "outputFileName",BinaryData.FromString(JsonSerializer.Serialize($"{fileName}-OUTPUT")) }, //outputPath is the parameter name in the pipeline

                };

                // Log the parameters before calling CreateRunAsync
                foreach (var param in binaryDataPipeParameters)
                {
                    _logger.LogInformation($"Key is: {param.Key}: Value is: {param.Value}");
                }
                
                var jsonPipeParameters = JsonSerializer.Serialize(binaryDataPipeParameters.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString()));
                _logger.LogInformation("Logging jsonParameters " + jsonPipeParameters); // Log the constructed JSON



                //This Test works finally. Azure Data Factory pipeline is triggered successfully and receives the parameters as expected.
                //This fix error executing Data flow: The pipeline parameters are not being passed correctly to the pipeline. The pipeline is not receiving the parameters as expected.
                //error ex. Error executing Data flow: 'i' is an invalid end of a number. Expected a delimiter. LineNumber: 0 | BytePositionInLine: 14

                //I focused on ensuring that the values are interpreted strictly as strings in the Azure Data Factory pipeline. Here are targeted steps and adjustments that could help:

                //1. Explicitly Confirm Data Types in Azure Data Factory Pipeline: Ensure that the parameters are explicitly defined as strings in the Azure Data Factory pipeline.
                //2. Confirm BinaryData Usage in Azure Functions: Confirm that the BinaryData object is being used correctly in the Azure Functions code.
                //3. 3. Workaround: Wrapping Strings as JSON Literal in Azure Functions: By wrapping each parameter value in JsonSerializer.Serialize, this can ensure each string parameter is treated as a JSON string literal.

                // Dictionary to hold the parameters to pass to the pipeline
                var testDictionaryParameters = new Dictionary<string, string>
                {
                    { "inputFileName", "20241103205428inputinput.csv" },
                    { "outputFileName", "0241103205428inputinputout.csv" }
                };
               
                //Test the BinaryData conversion and wrapping string parameters as JSON string literal
                var binaryDataTestParams = new Dictionary<string, BinaryData>();
                foreach (var kvp in testDictionaryParameters)
                {
                    //here we are converting the string parameters to BinaryData and storing them in a dictionary.
                    //Workaround: Wrapping Strings as JSON Literal.By wrapping each parameter value in JsonSerializer.Serialize, this can ensure each string parameter is treated as a JSON string literal.
                    binaryDataTestParams[kvp.Key] = BinaryData.FromString(JsonSerializer.Serialize(kvp.Value)) ;
                }

                var jsonbinaryDataParams = JsonSerializer.Serialize(binaryDataTestParams.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString()));
                _logger.LogInformation("Logging jsonbinaryDataParamsTest " + jsonbinaryDataParams); // Log the constructed JSON

               
                // Trigger the pipeline run
                var runResponse = await dataFactoryPipelineResource.CreateRunAsync(binaryDataPipeParameters);

                _logger.LogInformation($"Pipeline triggered successfully. RunId: {runResponse.Value.RunId}");
            }
            catch (Exception ex)
            {

                _logger.LogError($"Error executing Data flow: {ex.Message}");
            }
        }
    }
}
