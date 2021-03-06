Properties `
{
    $Configuration = $null
}

$root = $PSScriptRoot
$src = Resolve-Path "$root\..\src"

Import-Module Saritasa.Build

Task pre-build `
{
    Initialize-MSBuild
    Invoke-NugetRestore -SolutionPath "$src\CrmBot.sln"

    Set-Location $src
    $args = @('restore')
    $result = Start-Process -NoNewWindow -Wait -PassThru dotnet $args
    if ($result.ExitCode)
    {
        throw 'dotnet restore failed.'
    }
}

Task build -depends pre-build -description '* Build solution.' `
    -requiredVariables @('Configuration') `
{
    Invoke-SolutionBuild "$src\CrmBot.sln" -Configuration $Configuration
}

Task update-database -description '* Apply EF migrations to the database.' `
{
    Set-Location "$src\CrmBot.DataAccess"
    $env:ASPNETCORE_ENVIRONMENT = $Configuration
    $args = @('ef',
              'database',
              'update',
              '--startup-project', "$src\CrmBot\CrmBot.csproj",
              '--configuration', $Configuration)
    $result = Start-Process -NoNewWindow -Wait -PassThru dotnet $args
    if ($result.ExitCode)
    {
        throw 'Database update failed.'
    }
}