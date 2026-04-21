$script:ArtifactDirectoryNames = @(
    ".git",
    ".vs",
    "bin",
    "obj",
    "TestResults",
    "packages",
    "node_modules",
    "archives"
)

$script:ExcludedFilePatterns = @(
    "*.suo",
    "*.user",
    "*.sln.docstates",
    "*.nupkg",
    "*.snupkg",
    "*.VisualState.xml",
    "TestResult.xml",
    "*.ilk",
    "*.meta",
    "*.obj",
    "*.pch",
    "*.pdb",
    "*.pgc",
    "*.pgd",
    "*.rsp",
    "*.sbr",
    "*.tmp",
    "*.tmp_proj",
    "*.log",
    "*.cachefile",
    "*.psess",
    "*.vsp",
    "*.vspx",
    "*.DotSettings.user",
    "*.dotCover",
    "*.build.csdef",
    "*.Cache",
    "*~",
    "~$*",
    "*.dbmdl",
    "*.pfx",
    "*.publishsettings",
    "*.mdf",
    "*.ldf",
    "*.zip"
)

$script:TextFileExtensions = @(
    ".cs",
    ".xaml",
    ".csproj",
    ".sln",
    ".config",
    ".json",
    ".md",
    ".ps1",
    ".yml",
    ".yaml",
    ".xml",
    ".txt",
    ".resx",
    ".props",
    ".targets",
    ".editorconfig",
    ".gitignore",
    ".cff"
)

$script:ExportProfiles = @{
    source = @{
        IncludedTopLevelDirectories = @(
            ".github",
            "docs",
            "scripts",
            "src"
        )
        IncludedTopLevelFiles = @(
            ".gitignore",
            "CITATION.cff",
            "global.json",
            "LICENSE",
            "README.md"
        )
        ExcludedRelativeDirectories = @(
            "scripts\archives"
        )
    }
    codebase = @{
        IncludedTopLevelDirectories = @(
            ".github",
            "scripts",
            "src"
        )
        IncludedTopLevelFiles = @(
            ".gitignore",
            "global.json",
            "LICENSE",
            "README.md"
        )
        ExcludedRelativeDirectories = @(
            "scripts\archives"
        )
    }
}

function Test-IsArtifactDirectory {
    param(
        [System.IO.DirectoryInfo]$Directory
    )

    return $script:ArtifactDirectoryNames -contains $Directory.Name
}

function Test-IsExcludedFile {
    param(
        [System.IO.FileInfo]$File
    )

    foreach ($pattern in $script:ExcludedFilePatterns) {
        if ($File.Name -like $pattern) {
            return $true
        }
    }

    return $false
}

function Test-IsTextExportFile {
    param(
        [System.IO.FileInfo]$File
    )

    return $script:TextFileExtensions -contains $File.Extension.ToLowerInvariant()
}

function Get-RepoRoot {
    param(
        [string]$ScriptRoot
    )

    return Split-Path -Parent $ScriptRoot
}

function Get-DefaultExportDirectory {
    param(
        [string]$RepoRoot
    )

    return Join-Path (Join-Path $RepoRoot "scripts") "archives"
}

function Get-ResolvedExportDirectory {
    param(
        [string]$RepoRoot,
        [string]$OutputDirectory
    )

    if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
        $OutputDirectory = Get-DefaultExportDirectory -RepoRoot $RepoRoot
    }

    $resolved = [System.IO.Path]::GetFullPath($OutputDirectory)
    [System.IO.Directory]::CreateDirectory($resolved) | Out-Null
    return $resolved
}

function Get-RelativePath {
    param(
        [string]$BasePath,
        [string]$TargetPath
    )

    $baseUri = [System.Uri]([System.IO.Path]::GetFullPath($BasePath).TrimEnd('\') + '\')
    $targetUri = [System.Uri]([System.IO.Path]::GetFullPath($TargetPath))
    return [System.Uri]::UnescapeDataString($baseUri.MakeRelativeUri($targetUri).ToString()).Replace('/', '\')
}

function Get-ExportProfile {
    param(
        [string]$ProfileName
    )

    if (-not $script:ExportProfiles.ContainsKey($ProfileName)) {
        throw "Unknown export profile: $ProfileName"
    }

    return $script:ExportProfiles[$ProfileName]
}

function Test-IsUnderRelativeDirectory {
    param(
        [string]$RelativePath,
        [string]$DirectoryRelativePath
    )

    return $RelativePath -eq $DirectoryRelativePath -or $RelativePath.StartsWith($DirectoryRelativePath + "\", [System.StringComparison]::OrdinalIgnoreCase)
}

function Get-ExportProfileSummary {
    param(
        [string]$ProfileName
    )

    $profile = Get-ExportProfile -ProfileName $ProfileName
    return [PSCustomObject]@{
        ProfileName = $ProfileName
        IncludedTopLevelDirectories = @($profile.IncludedTopLevelDirectories)
        IncludedTopLevelFiles = @($profile.IncludedTopLevelFiles)
        ExcludedRelativeDirectories = @($profile.ExcludedRelativeDirectories)
    }
}

function Get-FilesFromRoot {
    param(
        [string]$RootPath
    )

    $files = New-Object System.Collections.Generic.List[System.IO.FileInfo]

    function Add-FilesRecursively {
        param(
            [string]$SourcePath
        )

        foreach ($entry in [System.IO.Directory]::EnumerateFileSystemEntries($SourcePath)) {
            $item = Get-Item -LiteralPath $entry -Force

            if ($item -is [System.IO.DirectoryInfo]) {
                if (Test-IsArtifactDirectory -Directory $item) {
                    continue
                }

                Add-FilesRecursively -SourcePath $item.FullName
                continue
            }

            if (Test-IsExcludedFile -File $item) {
                continue
            }

            $files.Add($item)
        }
    }

    Add-FilesRecursively -SourcePath $RootPath
    return $files
}

function Get-ExportFileInfos {
    param(
        [string]$RepoRoot,
        [string]$ProfileName = "source"
    )

    $profile = Get-ExportProfile -ProfileName $ProfileName
    $files = New-Object System.Collections.Generic.List[System.IO.FileInfo]

    foreach ($name in @($profile.IncludedTopLevelDirectories)) {
        $path = Join-Path $RepoRoot $name
        if (-not (Test-Path -LiteralPath $path -PathType Container)) {
            continue
        }

        foreach ($file in Get-FilesFromRoot -RootPath $path) {
            $files.Add($file)
        }
    }

    foreach ($name in @($profile.IncludedTopLevelFiles)) {
        $path = Join-Path $RepoRoot $name
        if (Test-Path -LiteralPath $path -PathType Leaf) {
            $file = Get-Item -LiteralPath $path -Force
            if (-not (Test-IsExcludedFile -File $file)) {
                $files.Add($file)
            }
        }
    }

    $filtered = foreach ($file in $files) {
        $relativePath = Get-RelativePath -BasePath $RepoRoot -TargetPath $file.FullName
        $shouldExclude = $false

        foreach ($excludedDirectory in @($profile.ExcludedRelativeDirectories)) {
            if (Test-IsUnderRelativeDirectory -RelativePath $relativePath -DirectoryRelativePath $excludedDirectory) {
                $shouldExclude = $true
                break
            }
        }

        if (-not $shouldExclude) {
            $file
        }
    }

    return $filtered | Sort-Object { Get-RelativePath -BasePath $RepoRoot -TargetPath $_.FullName } -Unique
}

function Copy-ExportContent {
    param(
        [string]$RepoRoot,
        [string]$DestinationRoot,
        [string]$ProfileName = "source"
    )

    $repoFolderName = Split-Path $RepoRoot -Leaf
    $destinationRepo = Join-Path $DestinationRoot $repoFolderName
    [System.IO.Directory]::CreateDirectory($destinationRepo) | Out-Null

    foreach ($file in Get-ExportFileInfos -RepoRoot $RepoRoot -ProfileName $ProfileName) {
        $relativePath = Get-RelativePath -BasePath $RepoRoot -TargetPath $file.FullName
        $destinationPath = Join-Path $destinationRepo $relativePath
        $destinationDirectory = Split-Path -Parent $destinationPath
        [System.IO.Directory]::CreateDirectory($destinationDirectory) | Out-Null
        Copy-Item -LiteralPath $file.FullName -Destination $destinationPath
    }
}
