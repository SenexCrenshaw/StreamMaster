.\UpdateVersions.ps1
$version = Get-Content "Version"
$dockerCommand = ".\build_docker.bat"
&$dockerCommand $version
