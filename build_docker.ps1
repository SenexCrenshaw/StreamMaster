param (
    [switch]$buildProd
    # Add $branchName parameter if needed
)

# Define the base image name
$imageName = "docker.io/senexcrenshaw/streammaster"

# Generate version tags using dotnet-gitversion
$semVer = dotnet gitversion /showvariable FullSemVer
$buildMetaDataPadded = dotnet gitversion /showvariable CommitsSinceVersionSourcePadded

if ($buildProd) {
    $tag = "latest"
} else {
    # Define or pass $branchName correctly
    $tag = "$branchName-$semVer-$buildMetaDataPadded"
}

# Build and tag for linux/amd64
docker build --platform linux/amd64 -t "${imageName}:$tag" .

# Build and tag for linux/arm64
docker build --platform linux/arm64 -t "${imageName}:$tag-arm64" .

# Push the images
docker push "${imageName}:latest"
docker push "${imageName}:$semVer"
docker push "${imageName}:$buildMetaDataPadded"
docker push "${imageName}:latest-arm64"
docker push "${imageName}:$semVer-arm64"
docker push "${imageName}:$buildMetaDataPadded-arm64"

# Create and push the manifest
docker manifest create "${imageName}:latest" --amend "${imageName}:latest" --amend "${imageName}:latest-arm64"
docker manifest create "${imageName}:$semVer" --amend "${imageName}:$semVer" --amend "${imageName}:$semVer-arm64"
docker manifest create "${imageName}:$buildMetaDataPadded" --amend "${imageName}:$buildMetaDataPadded" --amend "${imageName}:$buildMetaDataPadded-arm64"

docker manifest push "${imageName}:latest"
docker manifest push "${imageName}:$semVer"
docker manifest push "${imageName}:$buildMetaDataPadded"
