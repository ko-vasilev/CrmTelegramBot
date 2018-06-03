Framework 4.6
$InformationPreference = 'Continue'
$env:PSModulePath += ";$PSScriptRoot\scripts\Modules"

. .\scripts\BuildTasks.ps1
. .\scripts\PublishTasks.ps1

Properties `
{
    $Configuration = 'Debug'
    $DeployUsername = $env:DeployUsername
    $DeployPassword = $env:DeployPassword
    $ServerHost = $null
    $SiteName = $null
}

TaskSetup `
{
}
