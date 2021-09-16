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

## How to Contribute

Before submitting a Pull Request, `git fetch` the upstream branch you intend your changes to merge into, then `git rebase` your branch onto the upstream branch. If there are any conflicts, you will be prompted to resolve them one commit at a time. After a successful rebase, you may need to `git push --force-with-lease` to overwrite your remote (GitHub) repository. In Visual Studio, this is toggled by a checkbox in the application's Options.

## Styleguide

We follow the official dotnet-sdk repositories code style whenever possible.
See our EditorConfig file for rules and suggestions.
