$originalPath = $(Get-Location).Path

Set-Location $PSScriptRoot\..\bin
Get-ChildItem -Path "HXE.exe.config" -Recurse | Remove-Item

Set-Location $originalPath
