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
    $args = @('publish',
              "$src\CrmBot\CrmBot.csproj",
              '--output', "$workspace\pub\CrmBot",
              '--configuration', $Configuration)
    $result = Start-Process -NoNewWindow -Wait -PassThru dotnet $args
    if ($result.ExitCode)
    {
        throw 'Project publish failed.'
    }
}

Task publish -depends pre-publish, package, update-database -description 'Publish CrmBot project to remote server.' `
    -requiredVariables @('Configuration') `
{
    Invoke-WebSiteDeployment "$workspace\pub\CrmBot" $ServerHost $SiteName -Application ''
}
