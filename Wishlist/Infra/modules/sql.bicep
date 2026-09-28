param location string
param environmentName string
param adminLogin string        // your user principal name, e.g. you@domain.com
param adminObjectId string     // your Entra object id
param tenantId string = tenant().tenantId

resource server 'Microsoft.Sql/servers@2023-08-01-preview' = {
  name: 'sql-wishlist-${environmentName}-${uniqueString(resourceGroup().id)}'
  location: location
  properties: {
    minimalTlsVersion: '1.2'
    administrators: {
      administratorType: 'ActiveDirectory'
      login: adminLogin
      sid: adminObjectId
      tenantId: tenantId
      azureADOnlyAuthentication: true
    }
  }
}

resource db 'Microsoft.Sql/servers/databases@2023-08-01-preview' = {
  parent: server
  name: 'Wishlist'
  location: location
  sku: { name: 'GP_S_Gen5', tier: 'GeneralPurpose', family: 'Gen5', capacity: 1 }
  properties: {
    autoPauseDelay: 60          // minutes idle before pausing
    minCapacity: json('0.5')
    // useFreeLimit: true       // the free offer; verify the property name and availability
  }
}

// lets Azure services (your Function App) reach the server
resource allowAzure 'Microsoft.Sql/servers/firewallRules@2023-08-01-preview' = {
  parent: server
  name: 'AllowAzureServices'
  properties: { startIpAddress: '0.0.0.0', endIpAddress: '0.0.0.0' }
}

output serverFqdn string = server.properties.fullyQualifiedDomainName
output databaseName string = db.name
