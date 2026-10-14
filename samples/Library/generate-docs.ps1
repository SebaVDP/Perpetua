# Builds the solution and runs perpetua-structurizr once per deployment unit.
# Usage: .\samples\Library\generate-docs.ps1
$ErrorActionPreference = 'Stop'

$docs = Join-Path $PSScriptRoot 'docs'
$units = 'Website', 'Reservations', 'Loans', 'Members', 'Catalog', 'Notifications'

Push-Location $PSScriptRoot
try {
    dotnet tool restore | Out-Null
    dotnet build (Join-Path $PSScriptRoot 'Library.slnx') -c Release --nologo -v quiet
    if ($LASTEXITCODE -ne 0) { throw 'Build failed.' }

    foreach ($unit in $units) {
        $assemblies = Join-Path $PSScriptRoot "Library.$unit\bin\Release\net10.0"
        Write-Host "Documenting $unit..."
        dotnet perpetua-structurizr $assemblies $docs
        if ($LASTEXITCODE -ne 0) { throw "perpetua-structurizr failed for $unit." }
    }
}
finally {
    Pop-Location
}
