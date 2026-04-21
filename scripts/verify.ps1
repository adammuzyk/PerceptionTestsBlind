param(
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

function Invoke-Step {
    param(
        [Parameter(Mandatory = $true)]
        [scriptblock]$Command
    )

    & $Command
    if ($LASTEXITCODE -ne 0) {
        exit $LASTEXITCODE
    }
}

Write-Host "Restoring solution..."
Invoke-Step { dotnet restore .\src\PerceptionTests.sln }

Write-Host "Building solution ($Configuration)..."
Invoke-Step { dotnet build .\src\PerceptionTests.sln -c $Configuration --no-restore }

Write-Host "Running tests..."
Invoke-Step { dotnet test .\src\PerceptionTests.Tests\PerceptionTests.Tests.csproj -c $Configuration --no-build }

Write-Host "Verification finished successfully."
