function Get-AssemblyInfo {
    [CmdletBinding()]
    Param (
        [Parameter(Mandatory = $true)]
        [string]$assemblyInfoPath
    )

    if (Test-Path $assemblyInfoPath) {
        $content = Get-Content $assemblyInfoPath -Raw
        $assemblyInformationalVersionPattern = '\[assembly: AssemblyInformationalVersion\("([0-9.]+)-([^.]+)\.[0-9]+\.Sha\.([a-fA-F0-9]+)"\)\]'

        $assemblyInformationalVersionMatch = [regex]::Match($content, $assemblyInformationalVersionPattern)

        if ($assemblyInformationalVersionMatch.Success) {
            $version = $assemblyInformationalVersionMatch.Groups[1].Value
            $branch = $assemblyInformationalVersionMatch.Groups[2].Value
            $sha = $assemblyInformationalVersionMatch.Groups[3].Value

            [PSCustomObject]@{
                Version = $version
                Branch  = $branch
                Sha     = $sha
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
