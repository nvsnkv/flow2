dotnet build -c Release

$outputPath = "../../../../build/plugins/tkf"
if (Test-Path -PathType Container $outputPath)
{
   	Remove-Item $outputPath -Recurse
}

New-Item -ItemType Directory $outputPath

Copy-Item -Path "bin/Release/net6.0/*" -Destination $outputPath
