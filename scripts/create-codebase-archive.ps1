param(
    [string]$OutputDirectory = "",
    [string]$ArchivePrefix = "PerceptionTests_codebase"
)

$ErrorActionPreference = "Stop"

. (Join-Path $PSScriptRoot "source-export-common.ps1")

$repoRoot = Get-RepoRoot -ScriptRoot $PSScriptRoot
$resolvedOutputDirectory = Get-ResolvedExportDirectory -RepoRoot $repoRoot -OutputDirectory $OutputDirectory
$profileName = "codebase"

$stagingRoot = Join-Path ([System.IO.Path]::GetTempPath()) ("PerceptionTests-codebase-" + [System.Guid]::NewGuid().ToString("N"))
[System.IO.Directory]::CreateDirectory($stagingRoot) | Out-Null

try {
    Copy-ExportContent -RepoRoot $repoRoot -DestinationRoot $stagingRoot -ProfileName $profileName

    $timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
    $archiveName = "{0}_{1}.zip" -f $ArchivePrefix, $timestamp
    $archivePath = Join-Path $resolvedOutputDirectory $archiveName

    if (Test-Path -LiteralPath $archivePath) {
        Remove-Item -LiteralPath $archivePath -Force
    }

    Add-Type -AssemblyName System.IO.Compression.FileSystem
    [System.IO.Compression.ZipFile]::CreateFromDirectory($stagingRoot, $archivePath)

    Write-Host "Created codebase archive:"
    Write-Host $archivePath
    Write-Host ""
    Write-Host "Export profile:"
    Get-ExportProfileSummary -ProfileName $profileName | Format-List
}
finally {
    if (Test-Path -LiteralPath $stagingRoot) {
        Remove-Item -LiteralPath $stagingRoot -Recurse -Force
    }
}
