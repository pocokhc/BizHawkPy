$ErrorActionPreference = "Stop"

function Build-Version {
    param (
        [string]$BizHawkPath
    )

    $resolvedBizHawkPath = Resolve-Path $BizHawkPath -ErrorAction Stop
    Write-Host "[INFO] Building $resolvedBizHawkPath"
    dotnet build -c Release /p:BIZHAWK_HOME="$resolvedBizHawkPath/"

    $externalToolsPath = Join-Path $resolvedBizHawkPath "ExternalTools"
    $folderName = Split-Path $resolvedBizHawkPath -Leaf
    $zipPath = Join-Path $resolvedBizHawkPath "BizHawkPy-$folderName.zip"
    Compress-Archive -Path $externalToolsPath -DestinationPath $zipPath -Force
}

Build-Version "BizHawk-2.9.1-win-x64"
Build-Version "BizHawk-2.10-win-x64"

Write-Host "[DONE] All builds completed"
