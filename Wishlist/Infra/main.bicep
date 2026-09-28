targetScope = 'subscription'

param location string = 'northeurope'
param sqlLocation string = 'swedencentral'
param environmentName string = 'dev'
param sqlAdminLogin string
param sqlAdminObjectId string

resource rg 'Microsoft.Resources/resourceGroups@2024-03-01' = {
  name: 'rg-wishlist-${environmentName}'
  location: location
}

module monitoring 'modules/monitoring.bicep' = {
  name: 'monitoring'
  scope: rg
  params: { location: location, environmentName: environmentName }
}

module storage 'modules/storage.bicep' = {
  name: 'storage'
  scope: rg
  params: { location: location, environmentName: environmentName }
}

module sql 'modules/sql.bicep' = {
  name: 'sql'
  scope: rg
  params: {
    location: sqlLocation
    environmentName: environmentName
    adminLogin: sqlAdminLogin
    adminObjectId: sqlAdminObjectId
  }
}

module functionApp 'modules/functionapp.bicep' = {
  name: 'functionapp'
  scope: rg
  params: {
    location: location
    environmentName: environmentName
    storageAccountName: storage.outputs.name
    appInsightsConnectionString: monitoring.outputs.appInsightsConnectionString
    sqlServerFqdn: sql.outputs.serverFqdn
    sqlDatabaseName: sql.outputs.databaseName
  }
}

output functionAppName string = functionApp.outputs.appName
output functionPrincipalId string = functionApp.outputs.principalId
