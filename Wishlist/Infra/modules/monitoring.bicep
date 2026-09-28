param location string
param environmentName string

resource logs 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: 'log-wishlist-${environmentName}'
  location: location
  properties: {
    sku: { name: 'PerGB2018' }
    retentionInDays: 30
    workspaceCapping: { dailyQuotaGb: 1 }   // cost guard
  }
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: 'appi-wishlist-${environmentName}'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logs.id
  }
}

output appInsightsConnectionString string = appInsights.properties.ConnectionString
