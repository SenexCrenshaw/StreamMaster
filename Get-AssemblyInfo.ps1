function Get-AssemblyInfo {
    [CmdletBinding()]
    Param (
        [Parameter(Mandatory=$true)]
        [string]$assemblyInfoPath
    )

    if (Test-Path $assemblyInfoPath) {
        $content = Get-Content $assemblyInfoPath -Raw

        # Adjusted regex pattern to capture version, optional branch, and build/revision number
        $assemblyVersionPattern = '\[assembly: AssemblyVersion\("(\d+\.\d+\.\d+)(?:-([^\.\"]+))?(?:\.(\d+))?"\)\]'

        $assemblyVersionMatch = [regex]::Match($content, $assemblyVersionPattern)

        if ($assemblyVersionMatch.Success) {
            $version = $assemblyVersionMatch.Groups[1].Value
            $branch = $assemblyVersionMatch.Groups[2].Success ? $assemblyVersionMatch.Groups[2].Value : "N/A"
            $buildOrRevision = $assemblyVersionMatch.Groups[3].Success ? $assemblyVersionMatch.Groups[3].Value : "N/A"

            [PSCustomObject]@{
                Version = $version
                Branch = $branch
                BuildOrRevision = $buildOrRevision
            }
        }
        else {
            Write-Warning "Version information not found in the file."
        }
    } else {
        Write-Error "File $assemblyInfoPath not found."
    }
}
