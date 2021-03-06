# USAGE INSTRUCTIONS

At the most basic level, use it *exactly* how you would use the HCE
executable. Double click it, or pass any HCE parameters into it.

------------------------------------------------------------------------

HXE supports arguments for both its own purposes, and for the HCE
executable. In fact, HXE can be used as a wrapper around the HCE
executable, without the need for any additional/different arguments!

## EXAMPLES

      # automatically loads SPV3 in 720p window
      .\hxe.exe -window -vidmode 1280,720,60

      # installs SPV3 to C:\SPV3
      .\hxe.exe /install "C:\SPV3"

## ARGUMENTS

Arguments are categorised into two types:

- parameters: passed onto the Halo CE executable (prefixed with -)
- commands: instructs the loader to do something (prefixed with /)

Providing no command will result in HCE being automatically loaded. If
you provide only parameters, then the loader will automatically load HCE
with these parameters.

### PARAMETERS

| Parameter              | Description
| ---------------------- | ------------------------------------
| -console               | Loads HCE with console mode
| -devmode               | Loads HCE with developer mode
| -screenshot            | Loads HCE with screenshot ability
| -window                | Loads HCE in window mode
| -nogamma               | Loads HCE without gamma overriding
| -adapter X             | Loads HCE on monitor X
| -vidmode W,H,R         | Loads HCE with video mode
| -path "C:\path"        | Loads HCE with custom profile path
| -exec "C:\initc.txt"   | Loads HCE with custom init file

### COMMANDS

| Argument                       | Description
| ------------------------------ | ----------------------------------
| /load                          | Initiates HCE/SPV3
| /install "C:\path\to\target"   | Installs SPV3 to destination
| /compile "C:\path\to\target"   | Compiles SPV3 to destination
| /update "http://path/to/xml"   | Updates directory using manifest
| /config                        | Opens the configuration UI
| /positions                     | Opens the positions UI
