param baseName string
param region string
param buildId string
param environment string
param modelsToDeploy array
param containerName string

var functionAppName = '${baseName}-fa-${environment}-${region}'
var storageAccountName = '${baseName}sa${environment}${region}'
var appInsightsName = '${baseName}-${environment}-ai-${region}'
var logAnalyticsName = '${baseName}-${environment}-la-${region}'
var cogSvcsAccountName = '${baseName}-cog-${environment}-${region}'
var openAIAccountName = '${baseName}-aoai-${environment}-${region}'

var functionAppDeploymentName = '${functionAppName}-${buildId}'
var appInsightsDeploymentName = '${appInsightsName}-${buildId}'
var cogSvcsDeploymentName = '${cogSvcsAccountName}-${buildId}'
var openAIDeploymentName = '${openAIAccountName}-${buildId}'
var storageAccountDeploymentName = '${storageAccountName}-${buildId}'

module storage 'modules/storage.bicep' = {
  name: storageAccountDeploymentName
  params: {
    storageAccountName: storageAccountName
    region: region
    containerName: containerName
  }
}

module cogSvcs 'modules/cognitiveServices.bicep' = {
  name: cogSvcsDeploymentName
  params: {
    cogSvcsAccountName: cogSvcsAccountName
    region: region
  }
}

module openAI 'modules/azureOpenAI.bicep' = {
  name: openAIDeploymentName
  params: {
    azureOpenAIAccountName: openAIAccountName
    region: region
    modelsToDeploy: modelsToDeploy
  }
}

module appInsights 'modules/appInsights.bicep' = {
  name: appInsightsDeploymentName
  params: {
    appInsightsName: appInsightsName
    logAnalyticsWorkspaceName: logAnalyticsName
    location: region
  }
}

module functionApp 'modules/functionApp.bicep' = {
  name: functionAppDeploymentName
  params: {
    functionAppName: functionAppName
    storageAccountName: storage.outputs.name
    appInsightsName: appInsights.outputs.name
    location: region
  }
}
