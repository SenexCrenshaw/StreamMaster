$gitVersion = "dotnet-gitversion"
&$gitVersion /output json /showvariable SemVer > Version
$version = Get-Content "Version"
Write-output "Setting version to $version"

$filePath = "StreamMasterDomain\Dto\SettingDto.cs"
$MyFile = Get-Content $filePath
$MyFile = $MyFile -replace 'Version { get; set; } = ".*";', "Version { get; set; } = `"$version`";"
Out-File -InputObject $MyFile -FilePath $filePath

$dockerFilePath = "Dockerfile"
$dockerMyFile = Get-Content $dockerFilePath
$dockerMyFile = $dockerMyFile -replace 'org.opencontainers.image.version=".*"', "org.opencontainers.image.version=`"$version`""
Out-File -InputObject $dockerMyFile -FilePath $dockerFilePath

$dockerComposeFilePath = "docker-compose.yml"
$dockerComposeMyFile = Get-Content $dockerComposeFilePath
$dockerComposeMyFile = $dockerComposeMyFile -replace 'image: senexcrenshaw/streammaster:.*', "image: senexcrenshaw/streammaster:$version"
Out-File -InputObject $dockerComposeMyFile -FilePath $dockerComposeFilePath

&$gitVersion /updateprojectfiles
