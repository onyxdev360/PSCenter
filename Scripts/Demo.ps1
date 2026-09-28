$ErrorActionPreference = 'Stop'

$os = [System.Runtime.InteropServices.RuntimeInformation]::OSDescription
$architecture = [System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture
$powerShellEdition = $PSVersionTable.PSEdition
$powerShellVersion = $PSVersionTable.PSVersion.ToString()

[PSCustomObject]@{
    ComputerName      = [Environment]::MachineName
    CurrentUser       = [Environment]::UserName
    OperatingSystem   = $os
    Architecture      = $architecture
    PowerShellEdition = $powerShellEdition
    PowerShellVersion = $powerShellVersion
    Timestamp         = (Get-Date).ToString('yyyy-MM-dd HH:mm:ss zzz')
} | Format-List | Out-String | Write-Output
