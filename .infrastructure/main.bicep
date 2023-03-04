param baseName string
param region string
param buildId string
param environment string

var functionAppName = '${baseName}-fa-${environment}-${region}'
var storageAccountName = '{baseName}sa${environment}${region}'
var appInsightsName = '${baseName}-${environment}-ai-${region}'
var logAnalyticsName = '${baseName}-${environment}-la-${region}'

var functionAppDeploymentName = '${functionAppName}-${buildId}'
var appInsightsDeploymentName = '${appInsightsName}-${buildId}'

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
    storageAccountName: storageAccountName
    appInsightsName: appInsightsName
    location: region
  }
}
