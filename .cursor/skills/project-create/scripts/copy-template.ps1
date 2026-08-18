param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("kotlin", "unity")]
    [string]$Technology,

    [Parameter(Mandatory = $true)]
    [string]$ProjectName
)

$ErrorActionPreference = "Stop"

$templateMap = @{
    kotlin = "project-template\kotlin_app_template"
    unity  = "project-template\unity_app_template"
}

$workspace = (Get-Location).Path
$src = Join-Path $workspace $templateMap[$Technology]

# Normalize folder name: trim, lowercase, spaces/underscores -> hyphens
$safeName = ($ProjectName.Trim() -replace '[\\\/:*?"<>|]', '-' -replace '[\s_]+', '-').ToLower()
if ([string]::IsNullOrWhiteSpace($safeName)) {
    throw "ProjectName is empty after normalization."
}

$dst = Join-Path $workspace $safeName

if (-not (Test-Path -LiteralPath $src)) {
    throw "Template not found: $src"
}

if (Test-Path -LiteralPath $dst) {
    throw "Destination already exists: $dst - choose another ProjectName or remove the folder first."
}

New-Item -ItemType Directory -Path $dst | Out-Null

$excludeDirs = @(
    "node_modules",
    ".dart_tool",
    "build",
    ".gradle",
    ".idea",
    ".cxx",
    "Pods",
    ".git",
    "Library",
    "Temp",
    "Logs",
    "obj",
    "UserSettings",
    "Builds"
)

$robocopyArgs = @(
    $src,
    $dst,
    "/E",
    "/XD"
) + $excludeDirs + @(
    "/XF", "*.iml", "local.properties",
    "/NFL", "/NDL", "/NJH", "/NJS", "/nc", "/ns", "/np"
)

& robocopy @robocopyArgs
$code = $LASTEXITCODE

# robocopy: 0-7 = success / extra files; 8+ = failure
if ($code -ge 8) {
    throw "robocopy failed with exit code $code"
}

Write-Host "Copied $Technology template -> $safeName/ (robocopy code $code)"
exit 0
