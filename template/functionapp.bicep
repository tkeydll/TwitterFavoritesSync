param appName string
param location string = resourceGroup().location

param ScheduleAppSetting string
param FOLLOW_LIST_PATH string
param SOURCE_API_KEY string
@secure()
param SOURCE_API_SECRET string
param SOURCE_ACCESS_TOKEN string
@secure()
param SOURCE_ACCESS_SECRET string
param TARGET_API_KEY string
@secure()
param TARGET_API_SECRET string
param TARGET_ACCESS_TOKEN string
@secure()
param TARGET_ACCESS_SECRET string

var functionAppName = appName
var hostingPlanName = appName
var applicationInsightsName = appName
var storageAccountName = '${uniqueString(resourceGroup().id)}azfunctions'
var functionWorkerRuntime = 'dotnet'

resource storageAccount 'Microsoft.Storage/storageAccounts@2021-08-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'Storage'
}

resource hostingPlan 'Microsoft.Web/serverfarms@2021-03-01' = {
  name: hostingPlanName
  location: location
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
  properties: {}
}

resource functionApp 'Microsoft.Web/sites@2021-03-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: hostingPlan.id
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
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: applicationInsights.properties.InstrumentationKey
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: functionWorkerRuntime
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'ScheduleAppSetting'
          value: ScheduleAppSetting
        }
        {
          name: 'FOLLOW_LIST_PATH'
          value: FOLLOW_LIST_PATH
        }
        {
          name: 'SOURCE_API_KEY'
          value: SOURCE_API_KEY
        }
        {
          name: 'SOURCE_API_SECRET'
          value: SOURCE_API_SECRET
        }
        {
          name: 'SOURCE_ACCESS_TOKEN'
          value: SOURCE_ACCESS_TOKEN
        }
        {
          name: 'SOURCE_ACCESS_SECRET'
          value: SOURCE_ACCESS_SECRET
        }
        {
          name: 'TARGET_API_KEY'
          value: TARGET_API_KEY
        }
        {
          name: 'TARGET_API_SECRET'
          value: TARGET_API_SECRET
        }
        {
          name: 'TARGET_ACCESS_TOKEN'
          value: TARGET_ACCESS_TOKEN
        }
        {
          name: 'TARGET_ACCESS_SECRET'
          value: TARGET_ACCESS_SECRET
        }
        {
          name: 'WEBSITE_TIME_ZONE'
          value: 'Tokyo Standard Time'
        }
    
      ]      
      ftpsState: 'FtpsOnly'
      minTlsVersion: '1.2'
      use32BitWorkerProcess: false
      netFrameworkVersion: 'v6.0'
    }
    httpsOnly: true
  }
}

resource applicationInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: applicationInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    Request_Source: 'rest'
  }
}
