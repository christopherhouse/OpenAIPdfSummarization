param cogSvcsAccountName string
param region string

resource cogSvcs 'Microsoft.CognitiveServices/accounts@2022-12-01' = {
  name: cogSvcsAccountName
  location: region
  sku: {
    name: 'S0'
  }
  kind: 'CognitiveServices'
  properties: {
    networkAcls: {
      defaultAction: 'Allow'
      ipRules: []
    }
    publicNetworkAccess: 'Enabled'
  }
}
