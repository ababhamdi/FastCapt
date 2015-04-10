$tempDirectoryName  = '..\__squirrel_temp__'
$buildScriptFile = '..\BuildDebug.ps1'

if ($DTE -eq $null) {
  echo "Run this from the NuGet Package Console inside VS"
  exit 1
}

Invoke-Expression $buildScriptFile
ls "$tempDirectoryName\Package\*.nupkg" | %{Squirrel --releasify $_ -p .\Packages -r .\Releases}
rm -r -fo "$tempDirectoryName"
