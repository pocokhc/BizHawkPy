$ErrorActionPreference = "Stop"

function Build-Version {
    param (
        [string]$BizHawkPath
    )

    $resolvedBizHawkPath = Resolve-Path $BizHawkPath -ErrorAction Stop
    Write-Host "[INFO] Building $resolvedBizHawkPath"
    dotnet build -c Release /p:BIZHAWK_HOME="$resolvedBizHawkPath/"
    Start-Sleep -Seconds 2

    $externalToolsPath = Join-Path $resolvedBizHawkPath "ExternalTools"

    # __pycache__ を削除
    Get-ChildItem -Path $externalToolsPath -Recurse -Directory -Filter "__pycache__" | Remove-Item -Recurse -Force

    $folderName = Split-Path $resolvedBizHawkPath -Leaf
    $zipPath = Join-Path $resolvedBizHawkPath "BizHawkPy-$folderName.zip"
    Compress-Archive -Path $externalToolsPath -DestinationPath $zipPath -Force
}

Build-Version "BizHawk-2.9.1-win-x64"
Build-Version "BizHawk-2.10-win-x64"
Build-Version "BizHawk-2.11.1-win-x64"

Write-Host "[DONE] All builds completed"
