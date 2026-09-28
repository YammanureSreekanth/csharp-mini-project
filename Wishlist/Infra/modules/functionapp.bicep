param location string
param environmentName string
param storageAccountName string
param appInsightsConnectionString string
param sqlServerFqdn string
param sqlDatabaseName string
param dotnetVersion string = '10.0'

resource storage 'Microsoft.Storage/storageAccounts@2023-05-01' existing = {
  name: storageAccountName
}

resource plan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: 'plan-wishlist-${environmentName}'
  location: location
  kind: 'functionapp'
  sku: { name: 'FC1', tier: 'FlexConsumption' }
  properties: { reserved: true }      // Flex is Linux-only
}

resource app 'Microsoft.Web/sites@2023-12-01' = {
  name: 'func-wishlist-${environmentName}-${uniqueString(resourceGroup().id)}'
  location: location
  kind: 'functionapp,linux'
  identity: { type: 'SystemAssigned' }
  properties: {
    serverFarmId: plan.id
    httpsOnly: true
    functionAppConfig: {
      runtime: { name: 'dotnet-isolated', version: dotnetVersion }
      scaleAndConcurrency: { maximumInstanceCount: 40, instanceMemoryMB: 2048 }
      deployment: {
        storage: {
          type: 'blobContainer'
          value: '${storage.properties.primaryEndpoints.blob}app-package'
          authentication: { type: 'SystemAssignedIdentity' }
        }
      }
    }
    siteConfig: {
      appSettings: [
        { name: 'AzureWebJobsStorage__accountName', value: storage.name }
        { name: 'APPLICATIONINSIGHTS_CONNECTION_STRING', value: appInsightsConnectionString }
        { name: 'SqlConnectionString', value: 'Server=tcp:${sqlServerFqdn},1433;Database=${sqlDatabaseName};Authentication=Active Directory Default;Encrypt=True;' }
      ]
    }
  }
}

// the app's identity must read/write the storage account (no keys anymore)
resource blobOwner 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: storage
  name: guid(storage.id, app.id, 'storage-blob-data-owner')
  properties: {
    principalId: app.identity.principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', 'b7e6dc6d-f1e8-4753-8033-0f276bb0955b')
  }
}

output appName string = app.name
output principalId string = app.identity.principalId
