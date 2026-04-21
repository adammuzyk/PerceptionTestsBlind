param(
    [string]$OutputDirectory = "",
    [string]$FilePrefix = "PerceptionTests_codebase"
)

$ErrorActionPreference = "Stop"

. (Join-Path $PSScriptRoot "source-export-common.ps1")

$repoRoot = Get-RepoRoot -ScriptRoot $PSScriptRoot
$resolvedOutputDirectory = Get-ResolvedExportDirectory -RepoRoot $repoRoot -OutputDirectory $OutputDirectory
$profileName = "codebase"

$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$outputPath = Join-Path $resolvedOutputDirectory ("{0}_{1}.txt" -f $FilePrefix, $timestamp)

$builder = New-Object System.Text.StringBuilder
$textFiles = @(Get-ExportFileInfos -RepoRoot $repoRoot -ProfileName $profileName | Where-Object { Test-IsTextExportFile -File $_ })

[void]$builder.AppendLine("PerceptionTests codebase export")
[void]$builder.AppendLine("GeneratedAt: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss zzz')")
[void]$builder.AppendLine("RepositoryRoot: $repoRoot")
[void]$builder.AppendLine("ExportProfile: $profileName")
[void]$builder.AppendLine("IncludedTextFileCount: $($textFiles.Count)")
[void]$builder.AppendLine()
[void]$builder.AppendLine("Contents")
[void]$builder.AppendLine(("=" * 100))

for ($i = 0; $i -lt $textFiles.Count; $i++) {
    $relativePath = Get-RelativePath -BasePath $repoRoot -TargetPath $textFiles[$i].FullName
    [void]$builder.AppendLine(("{0:D4}. {1}" -f ($i + 1), $relativePath))
}

[void]$builder.AppendLine()

foreach ($file in $textFiles) {
    $relativePath = Get-RelativePath -BasePath $repoRoot -TargetPath $file.FullName
    [void]$builder.AppendLine(("=" * 100))
    [void]$builder.AppendLine("FILE: $relativePath")
    [void]$builder.AppendLine(("=" * 100))
    [void]$builder.AppendLine()
    [void]$builder.AppendLine([System.IO.File]::ReadAllText($file.FullName))
    [void]$builder.AppendLine()
}

[System.IO.File]::WriteAllText($outputPath, $builder.ToString(), [System.Text.Encoding]::UTF8)

Write-Host "Created codebase text export:"
Write-Host $outputPath
