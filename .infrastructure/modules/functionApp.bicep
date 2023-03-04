param functionAppName string
param storageAccountName string
param appInsightsName string
param cognitiveServicesAccountName string
param cognitiveServicesResourceId string
param cognitiveServicesAPIVersion string
param openAIAccountName string
param openAIResourceId string
param openAIAPIVersion string
param blobContainerName string
param openAIDeploymentName string
param location string

var cognitiveSvcsKey = listKeys(cognitiveServicesResourceId, cognitiveServicesAPIVersion).key1
var openAIKey = listKeys(openAIResourceId, openAIAPIVersion).key1

resource cogSvcs 'Microsoft.CognitiveServices/accounts@2022-12-01' existing = {
  name: cognitiveServicesAccountName
}

resource openAI 'Microsoft.CognitiveServices/accounts@2022-12-01' existing = {
  name: openAIAccountName
}

resource funcHhostingPlan 'Microsoft.Web/serverfarms@2021-03-01' ={
  name: '${functionAppName}-asp'
  location: location
  kind: 'functionapp'
  properties: {
  }
  sku: {
    tier: 'Dynamic'
    name: 'Y1'
  }
}

resource funcApp 'Microsoft.Web/sites@2022-03-01' = {
  name: '${functionAppName}-fa'
  kind: 'functionapp'
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    siteConfig: {
      appSettings: [
        {
          name: 'cognitiiveServicesEndpoint'
          value: cogSvcs.properties.endpoint
        }
        {
          name: 'cognitiiveServicesKey'
          value: cognitiveSvcsKey
        }
        {
          name: 'openAIEndpoint'
          value: openAI.properties.endpoint
        }
        {
          name: 'openAIKey'
          value: openAIKey
        }
        {
          name: 'openAIDeployment'
          value: openAIDeploymentName
        }
        {
          name: 'blobContainer'
          value: blobContainerName
        }
        {
          name: 'storageAccountName'
          value: storageAccountName
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: reference('microsoft.insights/components/${appInsightsName}', '2015-05-01').InstrumentationKey
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: reference('microsoft.insights/components/${appInsightsName}', '2015-05-01').ConnectionString
        }
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};AccountKey=${listKeys(resourceId('Microsoft.Storage/storageAccounts', storageAccountName), '2019-06-01').keys[0].value};EndpointSuffix=core.windows.net'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};AccountKey=${listKeys(resourceId('Microsoft.Storage/storageAccounts', storageAccountName), '2019-06-01').keys[0].value};EndpointSuffix=core.windows.net'
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: '${functionAppName}ba91'
        }
      ]
    }
    httpsOnly: true
  }
}

output id string = funcApp.id
