@description('Azure region for all resources. Defaults to the resource group location.')
param location string = resourceGroup().location

@description('Short prefix used for globally unique resource names.')
@minLength(3)
@maxLength(16)
param namePrefix string = 'flowjudge-vcs'

@description('GitHub webhook secret stored in Key Vault and exposed to the Function App as a Key Vault reference.')
@secure()
param githubWebhookSecret string

@description('Linux runtime for the Function App.')
@allowed([
  'DOTNET-ISOLATED|10'
  'DOTNET-ISOLATED|9'
  'DOTNET-ISOLATED|8'
])
param functionLinuxFxVersion string = 'DOTNET-ISOLATED|10'

var suffix = take(uniqueString(resourceGroup().id, namePrefix), 6)
var normalizedPrefix = toLower(replace(namePrefix, '-', ''))
var storageAccountName = take('${normalizedPrefix}${suffix}', 24)
var logAnalyticsName = '${namePrefix}-log-${suffix}'
var appInsightsName = '${namePrefix}-appi-${suffix}'
var functionPlanName = '${namePrefix}-plan-${suffix}'
var functionAppName = '${namePrefix}-func-${suffix}'
var keyVaultName = take('${namePrefix}-kv-${suffix}', 24)
var serviceBusNamespaceName = '${namePrefix}-sb-${suffix}'
var serviceBusConnectionSecretName = 'servicebus-connection-string'
var githubWebhookSecretName = 'github-webhook-secret'
var packageContainerName = 'function-packages'
var storageConnectionString = 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${storageAccount.listKeys().keys[0].value}'

var topics = [
  'flowjudge.vcs.event.integration'
  'flowjudge.vcs.event.repository'
  'flowjudge.vcs.event.pull-request'
  'flowjudge.vcs.event.code-review'
]

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: logAnalyticsName
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
  }
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalytics.id
  }
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    allowBlobPublicAccess: false
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
  }
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2023-05-01' = {
  parent: storageAccount
  name: 'default'
}

resource packageContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-05-01' = {
  parent: blobService
  name: packageContainerName
  properties: {
    publicAccess: 'None'
  }
}

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2022-10-01-preview' = {
  name: serviceBusNamespaceName
  location: location
  sku: {
    name: 'Standard'
    tier: 'Standard'
  }
  properties: {
    minimumTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
  }
}

resource serviceBusRule 'Microsoft.ServiceBus/namespaces/authorizationRules@2022-10-01-preview' = {
  parent: serviceBusNamespace
  name: 'flowjudge-vcs-worker'
  properties: {
    rights: [
      'Listen'
      'Send'
    ]
  }
}

resource serviceBusTopics 'Microsoft.ServiceBus/namespaces/topics@2022-10-01-preview' = [for topicName in topics: {
  parent: serviceBusNamespace
  name: topicName
  properties: {
    defaultMessageTimeToLive: 'P14D'
    duplicateDetectionHistoryTimeWindow: 'PT10M'
    enableBatchedOperations: true
    requiresDuplicateDetection: false
    supportOrdering: false
  }
}]

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: keyVaultName
  location: location
  properties: {
    tenantId: tenant().tenantId
    enableRbacAuthorization: false
    enabledForTemplateDeployment: true
    enableSoftDelete: true
    softDeleteRetentionInDays: 7
    accessPolicies: []
    sku: {
      family: 'A'
      name: 'standard'
    }
  }
}

resource serviceBusConnectionSecret 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: serviceBusConnectionSecretName
  properties: {
    value: listKeys(serviceBusRule.id, serviceBusRule.apiVersion).primaryConnectionString
  }
}

resource githubWebhookSecretResource 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: githubWebhookSecretName
  properties: {
    value: githubWebhookSecret
  }
}

resource functionPlan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: functionPlanName
  location: location
  kind: 'functionapp'
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
  properties: {
    reserved: true
  }
}

resource functionApp 'Microsoft.Web/sites@2023-12-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: functionPlan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: functionLinuxFxVersion
      ftpsState: 'FtpsOnly'
      minTlsVersion: '1.2'
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: storageConnectionString
        }
        {
          name: 'AzureWebJobsServiceBus'
          value: '@Microsoft.KeyVault(SecretUri=${serviceBusConnectionSecret.properties.secretUriWithVersion})'
        }
        {
          name: 'GitHub__WebhooksSecret'
          value: '@Microsoft.KeyVault(SecretUri=${githubWebhookSecretResource.properties.secretUriWithVersion})'
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: appInsights.properties.ConnectionString
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet-isolated'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: storageConnectionString
        }
        {
          name: 'WEBSITE_CONTENTSHARE'
          value: toLower(replace(functionAppName, '-', ''))
        }
      ]
    }
  }
}

resource functionKeyVaultAccessPolicy 'Microsoft.KeyVault/vaults/accessPolicies@2023-07-01' = {
  parent: keyVault
  name: 'add'
  properties: {
    accessPolicies: [
      {
        tenantId: tenant().tenantId
        objectId: functionApp.identity.principalId
        permissions: {
          secrets: [
            'get'
          ]
        }
      }
    ]
  }
}

output functionAppName string = functionApp.name
output functionHostname string = functionApp.properties.defaultHostName
output githubWebhookUrl string = 'https://${functionApp.properties.defaultHostName}/api/github/event'
output keyVaultName string = keyVault.name
output packageContainerName string = packageContainer.name
output storageAccountName string = storageAccount.name
output serviceBusNamespaceName string = serviceBusNamespace.name
output serviceBusTopics array = topics
