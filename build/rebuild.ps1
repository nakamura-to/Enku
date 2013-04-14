param($assemblyVersionNumber)

cls

Import-Module '..\tools\psake-4.1.0\psake.psm1'
Invoke-psake '.\build.ps1' -framework 4.0 -properties @{"assemblyVersionNumber" = $assemblyVersionNumber} Package
Remove-Module psake