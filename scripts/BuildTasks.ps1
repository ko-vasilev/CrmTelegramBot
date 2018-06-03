Properties `
{
    $Configuration = $null
}

$root = $PSScriptRoot
$src = Resolve-Path "$root\..\src"
$lib = Resolve-Path "$root\..\lib"

Import-Module Saritasa.Build

Task pre-build `
{
    Initialize-MSBuild
    Invoke-NugetRestore -SolutionPath "$src\CrmBot.sln"
}

Task build -depends pre-build -description '* Build solution.' `
    -requiredVariables @('Configuration') `
{
    Invoke-SolutionBuild "$src\CrmBot.sln" -Configuration $Configuration
}

Task update-database -description '* Apply EF migrations to the database.' `
{
    $args = @('ef',
              'database',
              'update',
              '--startup-project', "'$src\CrmBot\CrmBot.csproj'",
              '--project', "'$src\CrmBot.DataAccess\CrmBot.DataAccess.csproj'",
              '--configuration', $Configuration)
    $result = Start-Process -NoNewWindow -Wait -PassThru dotnet $args
    if ($result.ExitCode)
    {
        throw 'Database update failed.'
    }
}