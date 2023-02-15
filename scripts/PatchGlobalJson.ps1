if ($true) {
    return
}

Write-Host 'Disabling rollForward to pin to version in global.json.'
$globalJsonPath = Join-Path $PSScriptRoot '../global.json'
$globalJson = Get-Content $globalJsonPath -Raw | ConvertFrom-Json
$globalJson.sdk.rollForward = 'disable'
$globalJson | ConvertTo-Json | Set-Content -Path $globalJsonPath
