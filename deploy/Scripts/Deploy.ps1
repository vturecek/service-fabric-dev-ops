
#!
# use your repo absolute path here and run this command before continuing. PowerShell hates relative paths.
$repoDir = "C:\path\to\service-fabric-dev-ops"
$resourceGroupName = ""
$keyvualtName = ""
$subscriptionName = ""
$region = "westus2"

# Login and subsription stuff
Login-AzureRmAccount
Set-AzureRmContext -Subscription $subscriptionName
$subscription = Get-AzureRmSubscription -SubscriptionName $subscriptionName

# Resource group
New-AzureRmResourceGroup -Name $resourceGroupName -Location $region

# create a keyvault and upload a certificate to it
Import-Module "$repoDir\deploy\Scripts\ServiceFabricRPHelpers\ServiceFabricRPHelpers.psm1"

# New cert
$keyvault = Invoke-AddCertToKeyVault -SubscriptionId $subscription.Id -ResourceGroupName $resourceGroupName -Location $region -VaultName $keyvualtName -CertificateName 'clustercert' -CreateSelfSignedCertificate -OutputPath "$repoDir\deploy\Certs" -DnsName 'mycluster.io' -Password ""

# or existing cert
$keyvault = Invoke-AddCertToKeyVault -SubscriptionId $subscription.Id -ResourceGroupName $resourceGroupName -Location $region -VaultName $keyvualtName -CertificateName 'clustercert' -UseExistingCertificate -ExistingPfxFilePath "$repoDir\deploy\Certs\clustercert.pfx" -Password ""

# template test and deploy
Test-AzureRmResourceGroupDeployment -ResourceGroupName $resourceGroupName -TemplateFile "$repoDir\deploy\Templates\cluster-deploy.json" -TemplateParameterFile "$repoDir\deploy\Templates\cluster-deploy.parameters.json" -Verbose -clusterCertificateThumbprint $keyvault.CertificateThumbprint -sourceVaultValue $keyvault.SourceVault -clusterCertificateUrlValue $keyvault.CertificateURL -location $region -Debug
New-AzureRmResourceGroupDeployment  -ResourceGroupName $resourceGroupName -TemplateFile "$repoDir\deploy\Templates\cluster-deploy.json" -TemplateParameterFile "$repoDir\deploy\Templates\cluster-deploy.parameters.json" -Verbose -clusterCertificateThumbprint $keyvault.CertificateThumbprint -sourceVaultValue $keyvault.SourceVault -clusterCertificateUrlValue $keyvault.CertificateURL -location $region

# get base-64 cluster certificate for DevOps
[System.Convert]::ToBase64String([System.IO.File]::ReadAllBytes("$repoDir\deploy\Certs\clustercert.pfx"))