@description('Call Notification Service.')
param functionAppName string
param storageAccountName string
param cosmosDbDatabaseName string
param location string = resourceGroup().location

resource cosmosDb 'Microsoft.DocumentDB/databaseAccounts@2022-08-15' = {
  name: cosmosDbDatabaseName
  location: location
  kind: 'GlobalDocumentDB'
  properties: {
    enableFreeTier: false
    databaseAccountOfferType: 'Standard'
    capabilities: [
      {
        name: 'EnableServerless'
      }
    ]
    locations: [
      {
        locationName: location
      }
    ]
  }
}

resource hostingPlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: '${functionAppName}-plan'
  location: location
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
    size: 'Y1'
    family: 'Y'
  }
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: storageAccountName
  location: location
  kind: 'StorageV2'
  properties: {
    allowBlobPublicAccess: false
  }
  sku: {
    name: 'Standard_LRS'
  }
}

resource functionApp 'Microsoft.Web/sites@2022-03-01' = {
  name: functionAppName
  location: location
  kind: 'funcationapp'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: hostingPlan.id
    httpsOnly: true
    siteConfig: {
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccountName};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: toLower(functionAppName)
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet'
        }
        {
          name: 'ACS:ConnectionString'
          value: ''
        }
        {
          name: 'CosmosDbConfiguration:ConnectionString'
          value: ''
        }
        {
          name: 'CosmosDbConfiguration:Database'
          value: cosmosDbDatabaseName
        }
        {
          name: 'CosmosDbConfiguration:Database:Tables'
          value: '{ "CallbackRegistrations": -1, "CallNotifications": 35 }'
        }
        {
          name: 'TokenConfiguration:Enabled'
          value: 'false'
        }
        {
          name: 'TokenConfiguration:Issuer'
          value: ''
        }
        {
          name: 'TokenConfiguration:Secret'
          value: ''
        }
        {
          name: 'TokenConfiguration:TimeToLiveInMinutes'
          value: ''
        }
        {
          name: 'NotificationSettings:EnableSendIncomingCallContext'
          value: 'true'
        }
        {
          name: 'NotificationSettings:TimeToLiveInSeconds'
          value: '30'
        }
      ]
    }
  }
}
