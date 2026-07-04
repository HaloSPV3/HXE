# Thanks to:
# torek, StackOverflow community | https://stackoverflow.com/questions/37531605/how-to-test-if-git-repository-is-shallow
# Fabian | https://invoke-thebrain.com/2018/12/comparing-version-numbers-powershell/

## WARNING! This script may fail if Origin is a private repository!
# If credentials are required to fetch from the remote repository,
# git processes spawned by this script may be unable to fetch successfully.

function prebuild
{
    $isShallow = $true;

    # Announce
    Write-Host "GitVersion requires unshallow repositories.`n",
               "We will use Git to determine if the current repository needs to be un-shallowed.";

    # Check if the repository is shallow
    Write-Host "Checking if repository is shallow..."
    $isShallow = git rev-parse --is-shallow-repository

    # If the repository is shallow, then unshallow
    if ($isShallow -eq $true)
    {
        Write-Warning "Repository is shallow. Fetching full history..."
        git fetch --unshallow
        Write-Host "Fetch Completed. Proceeding to Build...`n"
    }
    else
    {
        Write-Host "Repository is complete. Proceeding to Build..."
    }
}

$(prebuild)
