properties { 
    $baseDir  = resolve-path ..
    $buildDir = "$baseDir\build"
    $workDir = "$buildDir\work"
    $packageDir = "$workDir\package"
    $nugetDir = "$workDir\nuget"
    $nuspecFileName = "$nugetDir\Enku.nuspec"
    $releaseDir = "$baseDir\Enku\bin\Release"
    $assemblyFileName = "$baseDir\Enku\AssemblyInfo.fs"
    $assemblyVersionNumber = "0.0.0.0"
}

task default -depends ShowProperties

task ShowProperties {
    "`$baseDir = $baseDir"
    "`$buildDir = $buildDir"
    "`$assemblyFileName = $assemblyFileName"
    "`$workDir = $workDir"
    "`$releaseDir = $releaseDir"
    "`$assemblyVersionNumber = $assemblyVersionNumber"
}

task Clean -depends ShowProperties {
   Set-Location $baseDir
   if (Test-Path -path $workDir)
   {
        Write-Output -ForegroundColor Green "Deleting $workDir"    
        del $workDir -Recurse -Force
   }
   New-Item -Path $workDir -ItemType Directory
   New-Item -Path $nugetDir -ItemType Directory
   New-Item -Path $nugetDir\lib -ItemType Directory
   Copy-Item -Path $buildDir\Enku.nuspec -Destination $nuspecFileName
}

task UpdateVersion -depends Clean {
    $assemblyVersionPattern = 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
    $assemblyVersion = 'AssemblyVersion("' + $assemblyVersionNumber + '")';
    (Get-Content $assemblyFileName) -replace $assemblyVersionPattern, $assemblyVersion | Set-Content $assemblyFileName

    $nuspecVersionPattern = '<version>[0-9]+(\.([0-9]+|\*)){1,3}</version>'
    $nuspecVersion = "<version>$assemblyVersionNumber</version>";
    (Get-Content $nuspecFileName) -replace $nuspecVersionPattern, $nuspecVersion | Set-Content $nuspecFileName
}

task Build -depends UpdateVersion {
    Write-Host -ForegroundColor Green "Building"
    Write-Host
    exec { msbuild "/t:Clean;Rebuild" /p:Configuration=Release /p:OutputPath=$workDir\Enku "$baseDir\Enku\Enku.fsproj" } "Error Build"
}

task Test -depends Build {
    Write-Host -ForegroundColor Green "Testing"
    Write-Host
    exec { msbuild "/t:Clean;Rebuild" /p:Configuration=Release /p:OutputPath=$workDir\Enku.Test "$baseDir\Enku.Test\Enku.Test.fsproj" } "Error Build"
    exec { .\tools\NUnit-2.6.2\bin\nunit-console.exe "$workDir\Enku.Test\Enku.Test.dll" /framework=$framework /xml:$workDir\Enku.Test\testResult.xml } "Error running $name tests" 
}

task Package -depends Test {
    Write-Host -ForegroundColor Green "Package"
    Write-Host
    robocopy $workDir\Enku $nugetDir\lib\net45 Enku.* /MIR /NP
    exec { .\tools\NuGet\NuGet.exe pack $nuspecFileName }
    move -Path .\*.nupkg -Destination $workDir
}