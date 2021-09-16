# Contributing to HXE

Welcome to HXE!<br/>
The console application that began as a HaloCE.exe wrapper

The following is a set of guidelines for contributing to HXE. Code style is recommended, but not required. However, due to the presence of automated tasks affecting this repository, we require [conventional commits](https://www.conventionalcommits.org/) for inferring the release version from commit messages.

## Table of Contents

- [Getting Started](CONTRIBUTING.md#getting-started)
- [How to Contribute](CONTRIBUTING.md#how-to-contribute)
- [Styleguides](CONTRIBUTING.md#styleguides)

## Getting Started

### Requirements

All requirements are multi-platform.

- [Git](https://git-scm.com/)
- [Microsoft .NET SDK](https://dotnet.microsoft.com/) (>=5.0)
- [Node.js](https://nodejs.org/) (LTS or Latest)
- [Powershell](https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell) (>7.1; rec. >=7.2 for command completion)

Normally, one would simply `git clone` this repository and get to work, but we can't make Git automatically set up guardrails like [commitlint](https://github.com/conventional-changelog/commitlint) to ensure automated semantic versioning, releases, and pull requests continue to function as intended.

After cloning, run the following command in the local repository:

```shell
# installs packages listed in package.json
npm install
```

### Recommendations

- **Sign commits with your GPG signature**
  - See [Signing Commits](https://docs.github.com/en/github/authenticating-to-github/managing-commit-signature-verification/signing-commits)
  - Somewhat complex to set up and to ensure its use in all editors.
  - Visual Studio IDE's Git for Windows extension may use a separate PGP keybox from a standalone Git for Windows installation.
- **Visual Studio Code**
  - Compared to its IDE cousin, VSCode is more similar to Atom; Nearly identical, actually
  - Multi-platform: Linux, Mac, Windows
  - Features:
    - Allows recommending workspace extensions via config file in repository
    - Periodically auto-fetch Git refs
    - Stage and commit selections (in Changes editor) or stage *individual changes* in the normal editor to commit later
- **Visual Studio IDE**
  - Windows-exclusive
  - (Visual Studio 2022) Multi-platform: Mac, Windows
  - Features:
    - A great merge/rebase conflict resolver
    - Code decompilation (great for peeking behind the DotNet curtains!)
    - First-party and community extensions (Free, Trial, or Paid)
    - Allows executing post-Publish DotNet/MSBuild tasks (DotNet CLI does not support this)
    - Periodically fetch Git refs (via community extension)
    - ...and many other useful feature which more editors should have.
  - Has three end-user licenses:
    - Community (Free for non-commercial use)
    - Professional
    - Enterprise
  - VS2022 is the first 64-bit release branch in its family and also the first to be able to run on Mac.
    - However, popular extensions that use external code need to be reworked to function on the new 64-bit application.
- **SmartGit**
  - Free for non-commercial use
  - Features:
    - Exposes a lot of Git functionality via GUI
    - Comes with a GUI for Interactive Rebasing (Move/Remove commits from intended history)
- **POSHGIT**
  - A PowerShell extension
- **TortoiseGit**
  - A Windows File Explorer extension
  - Windows-exclusive
  - Features:
    - Displays Git status as file icon overlays
    - Exposes Git functionality via context menu

## How to Contribute

Before submitting a Pull Request, `git fetch` the upstream branch you intend your changes to merge into, then `git rebase` your branch onto the upstream branch. If there are any conflicts, you will be prompted to resolve them one commit at a time. After a successful rebase, you may need to `git push --force-with-lease` to overwrite your remote (GitHub) repository. In Visual Studio, this is toggled by a checkbox in the application's Options.

## Styleguide

We follow the official dotnet-sdk repositories code style whenever possible.
See our EditorConfig file for rules and suggestions.
