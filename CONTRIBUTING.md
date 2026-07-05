# Contributing to HXE

Welcome to HXE!
> The console application that began as a HaloCE.exe wrapper

The following is a set of guidelines for contributing to HXE. Code style is recommended, but not required. However, due to the presence of automated tasks affecting this repository, we require [conventional commits](https://www.conventionalcommits.org)\* for inferring the release version from commit messages.

\* This website details the _convention_ rather than what's used in practice. See [commitlint](https://github.com/conventional-changelog/commitlint#get-started--website) for actual usage.

## Table of Contents

- [Getting Started](CONTRIBUTING.md#getting-started)
- [How to Contribute](CONTRIBUTING.md#how-to-contribute)
- [Style Guides](CONTRIBUTING.md#styleguides)

## Getting Started

### Requirements

All requirements are multi-platform...except NVM. Windows users have to use NVM-Windows or NVM via WSL.

> We recommend using package managers to install these requirements.
>
> - Windows:
>   - [`scoop`](https://scoop.sh) -or-
>   - [`choco`](https://chocolatey.org/install#install-step2) -or-
>   - [`winget`](https://github.com/microsoft/winget-cli) (included with Win10/11)
>   - Prefer a GUI? Consider [UnitGetUI](https://github.com/Devolutions/UniGetUI) to manager any/all of the above and more.
> - MacOS:
>   - [`brew`](https://brew.sh)
>   - [`macports`](https://www.macports.org)
> - Linux:
>   - Alpine? `apk`
>   - Arch? `pacman`, [`paru`](https://github.com/Morganamilo/paru) (pre-installed on some Arch-base distros), or [`yay`](https://github.com/Jguer/yay)
>   - Debian/Ubuntud? `apt`
>   - RHEL/Fedora? ~~`yum`~~ `dnf`
>   - SUSE? `zypper`

- [Git](https://git-scm.com)
- [Microsoft .NET SDK](https://dotnet.microsoft.com) (LTS or Latest)
- Version manager for Node.js e.g. [NVM (Linux, Mac)](https://github.com/nvm-sh/nvm/blob/master/README.md), [NVM-Windows 1.x (2.x is broken)](https://github.com/coreybutler/nvm-windows); [`nodenv`](https://github.com/nodenv/nodenv) may also work, but has not been tested.
  - `nvm use`: Swaps out [Node.js](https://nodejs.org) to use the version defined in file://./.nvmrc. You may be prompted to install the version if it's missing.
  - `corepack enable`: Yarn -  in the repo. Make sure
- [Powershell](https://docs.microsoft.com/powershell/scripting/install/installing-powershell) (>7.1; rec. >=7.2 for command completion)

Normally, one would simply `git clone` this repository and get to work, but we can't make Git automatically set up guardrails like [commitlint](https://github.com/conventional-changelog/commitlint) to ensure automated semantic versioning, releases, and pull requests continue to function as intended.

After cloning, run the following command in the local repository:

```shell
# installs packages listed in package.json and sets up git hooks via Husky
yarn
```

### Recommendations

- **Git customizations**
  - alias `git frappt`:
    - About: Run `git fetch  --all --prune --prune-tags --tags` periodically to fetch changes all remotes, forget deleted remote branches, forget deleted tags, and fetch all tags.
    - Setup: `git config alias.frappt 'fetch --recurse-submodules --all --prune --prune-tags --tags'`.
  - alias `git adog`:
    - About: `git log --all --decorate --online --graph` starts a user-friendly commit graph. On platforms with pagers (e.g. `less`, `most`), you can use arrow keys to navigate.
    - Setup: `git config alias.adog 'log --all --decorate --online --graph'`
- **Sign commits with your GPG signature**
  - See [Signing Commits](https://docs.github.com/en/github/authenticating-to-github/managing-commit-signature-verification/signing-commits)
  - Somewhat complex to set up and to ensure its use in all editors.
  - Note: Visual Studio IDE's Git for Windows extension may its own PGP keybox separate from a standalone Git for Windows installation.
- **Your choice of Editor/IDE**
  - **Visual Studio Code**
    - Not to be confused with its Windows-only cousin, VSCode is a flexible editor for (almost) any desktop!
    - Multi-platform: Linux, Mac, Windows
    - Features:
      - Allows recommending workspace extensions via config file in repository. We recommend quite a few. Some are required for the editor to work as intended with our repo.
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
  - A PowerShell extension for showing Git status in the terminal.
  - Also consider oh-my-posh (which is available for other shells, too!)

- **TortoiseGit**
  - A Windows File Explorer extension
  - Features:
    - Displays Git status as file icon overlays
    - Exposes Git functionality via context menu

## How to Contribute

Forked this repo via GitHub and git-cloned your fork? Add the `upstream` remote to work with our changes locally.
`git remote add upstream https://github.com/HaloSPV3/HXE.git && git fetch --all --tags --recurse-submodules`.

Before submitting a Pull Request, `git fetch` the upstream branch you intend your changes to merge into (e.g. `git fetch upstream develop`), then `git rebase upstream develop` your branch onto the upstream branch. If there are any conflicts, you will be prompted to resolve them one commit at a time. After a successful rebase, you may need to `git push --force-with-lease` to overwrite your remote (GitHub) repository. In Visual Studio, this is toggled by a checkbox in the application's Options.

## Styleguide

We follow the official dotnet-sdk repositories code style whenever possible.
See our EditorConfig file for rules and suggestions.
