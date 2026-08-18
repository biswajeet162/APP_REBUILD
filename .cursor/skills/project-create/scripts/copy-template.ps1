param(
    [Parameter(Mandatory = $true)]
    [ValidateSet("flutter", "react_native", "kotlin")]
    [string]$Technology
)

$ErrorActionPreference = "Stop"

$templateMap = @{
    flutter      = "project-template\flutter_app_template"
    react_native = "project-template\react_native_app_template"
    kotlin       = "project-template\kotlin_app_template"
}

$workspace = (Get-Location).Path
$src = Join-Path $workspace $templateMap[$Technology]
$dst = Join-Path $workspace "new-project"

if (-not (Test-Path -LiteralPath $src)) {
    throw "Template not found: $src"
}

if (Test-Path -LiteralPath $dst) {
    Remove-Item -LiteralPath $dst -Recurse -Force
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
    ".git"
)

$robocopyArgs = @(
    $src,
    $dst,
    "/E",
    "/XD"
) + $excludeDirs + @(
    "/XF", "*.iml",
    "/NFL", "/NDL", "/NJH", "/NJS", "/nc", "/ns", "/np"
)

& robocopy @robocopyArgs
$code = $LASTEXITCODE

# robocopy: 0-7 = success / extra files; 8+ = failure
if ($code -ge 8) {
    throw "robocopy failed with exit code $code"
}

Write-Host "Copied $Technology template -> new-project/ (robocopy code $code)"
exit 0
