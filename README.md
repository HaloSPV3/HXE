<html>
    <h1 align='center'>The HXE Kernel</h1>
    <p align='center'>
        Loader and wrapper for SPV3 and HCE.
    </p>
</html>

[![Commitizen friendly](https://img.shields.io/badge/commitizen-friendly-brightgreen.svg?style=flat-square)](http://commitizen.github.io/cz-cli/)
[![Conventional Commits](https://img.shields.io/badge/Conventional%20Commits-1.0.0-yellow.svg?style=flat-square)](https://conventionalcommits.org)
[![Codecov](https://img.shields.io/codecov/c/github/HaloSPV3/HXE.svg?style=flat-square)](https://codecov.io/gh/HaloSPV3/HXE)

Main: [![ci - main](https://github.com/HaloSPV3/HXE/actions/workflows/ci.yml/badge.svg)](https://github.com/HaloSPV3/HXE/actions/workflows/ci.yml)
Develop: [![ci - develop](https://github.com/HaloSPV3/HXE/actions/workflows/ci.yml/badge.svg?branch=develop)](https://github.com/HaloSPV3/HXE/actions/workflows/ci.yml)

# Introduction

This tree contains the source code & documentation for HXE, a versatile
loader for SPV3 and wrapper for HCE.

# Features

Here are some of the main features the kernel provides:

- Wrapper around the HCE executable
- Compatibility with HCE arguments (-window, -vidmode, etc.)
- Automatic video/audio enhancements
  - Custom native video resolution
  - Border-less HCE window mode
- Support for the SPV3 mod
  - SPV3 loading, updating, installing
  - SPV3/Lumoria campaign resuming
  - SPV3 post-processing tweaking
  - Backwards compatibility with SPV3.1
    - Campaign resume
    - Maps unlocking
  - Backwards compatibility with SPV3.2
- Automatic profile detection

# Usage

The USAGE document goes into detail on how to use HXE. In a nutshell:

    # automatically loads SPV3 in 720p window
    .\hxe.exe -window -vidmode 1280,720,60

    # installs SPV3 to C:\SPV3
    .\hxe.exe -install "C:\SPV3"

    # configure the kernel
    .\hxe.exe -config

# Requirements

## Operating System

| Minimum                                  | Recommended
| ---------------------------------------- | -----------
| Windows 7 SP1 32-bit (w/ addl. software) | Windows 10 64-bit

## .NET 6.0

Because HXE is built on the relatively new .NET 6, you may need to download the [.NET 6.0 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) for this app to work. Hopefully, this will be distributed via Windows Updates to Windows 10 and Windows 11 sooner rather than later.
For 64-bit PCs: [Dotnet Runtime (Desktop) 6.0.1 Windows x64 Installer](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-6.0.1-windows-x64-installer)
For 32-bit PCs: [Dotnet Runtime (Desktop) 6.0.1 Windows x86 Installer](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/runtime-desktop-6.0.1-windows-x86-installer)

### Windows 7/8.1

Additional software dependencies to be installed for this .NET-based app to work on Windows 8.1 and Windows 7
Read the [Microsoft docs](https://docs.microsoft.com/en-us/dotnet/core/install/windows?tabs=net60#additional-deps) to learn what you need and how to get it.

## Note: Upgrading To Windows 10

Using the [Windows Installation Media Creation Tool](https://www.microsoft.com/en-us/software-download/windows10?36261b60-2f68-4336-abe2-4b00f210b6aa=True), you can still upgrade to Windows 10 with your Windows 7/8/8.1 license.
HOWEVER...

- If your hardware distributor does not make Windows 10 drivers for your hardware, you may have a worse Windows 10 experience than expected.
- Some drivers made for earlier Windows releases may work on Windows 10; some won't. You won't know until you try.
