function Get-AssemblyInfo {
    [CmdletBinding()]
    Param (
        [Parameter(Mandatory = $true)]
        [string]$assemblyInfoPath
    )

    if (Test-Path $assemblyInfoPath) {
        $content = Get-Content $assemblyInfoPath -Raw

        # Adjusted regex pattern to capture version, optional branch, optional build/revision number, and SHA
        $assemblyInformationalVersionPattern = '\[assembly: AssemblyInformationalVersion\("(\d+\.\d+\.\d+)(?:-([^.]+))?\.?(?:([0-9]+)\.)?Sha\.([a-fA-F0-9]+)"\)\]'

        $assemblyInformationalVersionMatch = [regex]::Match($content, $assemblyInformationalVersionPattern)

        if ($assemblyInformationalVersionMatch.Success) {
            $version = $assemblyInformationalVersionMatch.Groups[1].Value
            $branch = if ($assemblyInformationalVersionMatch.Groups[2].Success) { $assemblyInformationalVersionMatch.Groups[2].Value } else { "N/A" }
            $buildOrRevision = if ($assemblyInformationalVersionMatch.Groups[3].Success) { $assemblyInformationalVersionMatch.Groups[3].Value } else { "N/A" }
            $sha = $assemblyInformationalVersionMatch.Groups[4].Value

            [PSCustomObject]@{
                Version         = $version
                Branch          = $branch
                BuildOrRevision = $buildOrRevision
                Sha             = $sha
            }
        }
        else {
            Write-Warning "Version information not found in the file."
        }
    }
    else {
        Write-Error "File $assemblyInfoPath not found."
    }
}
