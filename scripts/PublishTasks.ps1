Properties `
{
    $Configuration = $null
    $ServerHost = $null
    $SiteName = $null
}

$root = $PSScriptRoot
$workspace = Resolve-Path "$root\.."

Import-Module Saritasa.Build
Import-Module Saritasa.WebDeploy

Task pre-publish -depends build -description 'Set common publish settings.' `
    -requiredVariables @('DeployUsername', 'DeployPassword') `
{
    $credential = New-Object System.Management.Automation.PSCredential($DeployUsername, (ConvertTo-SecureString $DeployPassword -AsPlainText -Force))
    Initialize-WebDeploy -Credential $credential
}

Task package -description 'Make publish package.' `
    -requiredVariables @('Configuration') `
{
    Invoke-PackageBuild "$src\CrmBot\CrmBot.csproj" "$workspace\CrmBot.zip" $Configuration -Precompile $false
}

Task publish -depends pre-publish, package, update-database -description 'Publish CrmBot project to remote server.' `
    -requiredVariables @('Configuration') `
{
    $msdeployParams = @(
    )
    Invoke-WebDeployment "$workspace\CrmBot.zip" $ServerHost $SiteName -Application '' -MSDeployParams $msdeployParams
}
