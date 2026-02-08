<html>
    <h1 align='center'>The HXE Kernel</h1>
    <p align='center'>
        Loader and wrapper for SPV3 and HCE.
    </p>
</html>

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

## Note: Upgrading To Windows 10

Using the [Windows Installation Media Creation Tool](https://www.microsoft.com/en-us/software-download/windows10?36261b60-2f68-4336-abe2-4b00f210b6aa=True), you can still upgrade to Windows 10 with your Windows 7/8/8.1 license.
HOWEVER...

- If your hardware distributor does not make Windows 10 drivers for your hardware, you may have a worse Windows 10 experience than expected.
- Some drivers made for earlier Windows releases may work on Windows 10; some won't. You won't know until you try.
