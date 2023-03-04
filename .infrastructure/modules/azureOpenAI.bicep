param azureOpenAIAccountName string
param region string
param modelsToDeploy array

resource openAI 'Microsoft.CognitiveServices/accounts@2022-12-01' = {
  name: azureOpenAIAccountName
  location: region
  sku: {
    name: 'S0'
  }
  kind: 'OpenAI'
  properties: {
    networkAcls: {
      defaultAction: 'Allow'
      ipRules: []
      virtualNetworkRules: []
    }
    publicNetworkAccess: 'Enabled'
  }
}

resource deployments 'Microsoft.CognitiveServices/accounts/deployments@2022-12-01' = [for modelToDeploy in modelsToDeploy: {
  name: '${modelToDeploy}-deployment'
  parent: openAI
  properties: {
    model: {
      format: 'OpenAI'
      name: modelToDeploy
      version: '1'
    }
    scaleSettings: {
      scaleType: 'Standard'
    }
  }
}]
