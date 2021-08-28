# Git - Ensure we can execute Git
try {
  Write-Host "Git was found. It is $(git --version) at $(where.exe git)"
}
catch {
  Write-Error "Git is not installed or it isn't in the environment variable PATH!";
}

# GitVersion - Ensure unshallow checkout
if ('true' -eq $(git rev-parse --is-shallow-repository)) {
  git fetch --unshallow;
}
