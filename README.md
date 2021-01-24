<html>
    <h1 align='center'>The HXE Kernel</h1>
    <p align='center'>
        Loader and wrapper for SPV3 and HCE.
    </p>
</html>

# Introduction

This tree contains the source code & documentation for HXE, a versatile
loader for SPV3 and wrapper for HCE.

# Features

Here are some of the main features the kernel provides:

-   Wrapper around the HCE executable
-   Compatibility with HCE arguments (-window, -vidmode, etc.)
-   Automatic video/audio enhancements
    -   Custom native video resolution
    -   Border-less HCE window mode
-   Support for the SPV3 mod
    -   SPV3.2 loading, updating, installing
    -   SPV3.2/Lumoria campaign resuming
    -   SPV3.2 post-processing tweaking
    -   Backwards compatibility with SPV3.1
        -   Campaign resume
        -   Maps unlocking
-   Automatic profile detection

# Usage

The USAGE document goes into detail on how to use HXE. In a nutshell:

    # automatically loads SPV3 in 720p window
    .\hxe.exe -window -vidmode 1280,720,60

    # installs SPV3 to C:\SPV3
    .\hxe.exe -install "C:\SPV3"

    # configure the kernel
    .\hxe.exe -config
