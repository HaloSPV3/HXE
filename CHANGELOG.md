## [2.3.1](https://github.com/HaloSPV3/HXE/compare/v2.3.0...v2.3.1) (2026-07-09)

### Bug Fixes

* **deps:** add `Costura.Fody` to embed dependencies in .NET Framework 4 binaries; change System.Net.Http to Reference; disable (App).exe.config generation ([7d934ed](https://github.com/HaloSPV3/HXE/commit/7d934ed292bb055efcf6f38cc310501830cc8724))
* **deps:** remove explicit dependency on `Microsoft.SourceLink.GitHub` ([2657928](https://github.com/HaloSPV3/HXE/commit/26579288897043a8237753e1d9a2f319cef3d344))
* **MCC:** update 343 Industries code-signing certificate (for halo1.dll recognition) ([9eee2e6](https://github.com/HaloSPV3/HXE/commit/9eee2e6dea5d8eda031c0473929721d2fb11f16c))

### Reverts

* ci: compare Node versions after yarn-install ([6c2f251](https://github.com/HaloSPV3/HXE/commit/6c2f251a12970100a406799647de9430f8904102))

## [2.3.0](https://github.com/HaloSPV3/HXE/compare/v2.2.4...v2.3.0) (2026-07-04)

### Features

* add --cli arg ([c895022](https://github.com/HaloSPV3/HXE/commit/c8950224de39cd8b4af1d9765380d755240ab2bf))
* Add CLI alternative to Positions GUI ([6343390](https://github.com/HaloSPV3/HXE/commit/63433907f582b6a0fa18fc92a59789af2d2ff99d))
* add HXE.Paths.Custom.Configuration(string) ([b4ba0f9](https://github.com/HaloSPV3/HXE/commit/b4ba0f9553b03ed976f2958661eeeaff56f96ea0))
* add InferResult() ([ea755bb](https://github.com/HaloSPV3/HXE/commit/ea755bbc983f0a09dc0307310012758c225cc6c2))

### Bug Fixes

* add using System.Linq ([d85f3c6](https://github.com/HaloSPV3/HXE/commit/d85f3c6ffca203c3f9cae4fd31aa0cba8924d026))
* assign null to nullable variable ([1718990](https://github.com/HaloSPV3/HXE/commit/1718990300661390977fb4bbb8dd6a1ed09eba5f))
* **Campaign:** use XmlSourceGenerator for (de)serialization ([383b151](https://github.com/HaloSPV3/HXE/commit/383b151d270b881f15b1077b534b15e3f0237e2f))
* **deps-dev:** bump `@halospv3/hce.shared-config` to 3.9.3 ([8423ef2](https://github.com/HaloSPV3/HXE/commit/8423ef2f8389fc424dd0c44f86dbe699322ae43a))
* **deps-dev:** recursively upgrade `@semantic-release/github` to 12.0.9 to fix release assets ([cd82554](https://github.com/HaloSPV3/HXE/commit/cd82554422ee02cec55d0c98700b7a135924c77e))
* **deps:** upgrade dotnet dependencies ([ccc1b91](https://github.com/HaloSPV3/HXE/commit/ccc1b91346bd5a5d738c4a770adb70053c0981a5))
* exit when HaloCE.exe cannot be found ([148bbc9](https://github.com/HaloSPV3/HXE/commit/148bbc9ea4b96d762add1b598e568d9b8eaf56aa))
* handle InvalidOperationException sometimes thrown when setting DialogResult ([b1059cb](https://github.com/HaloSPV3/HXE/commit/b1059cbb71fc1e3e5343d726354db06be295a115)), closes [HaloSPV3/HXE#248](https://github.com/HaloSPV3/HXE/issues/248)
* **HCE:** resolve null-to-non-nullable assignments ([beaf2f5](https://github.com/HaloSPV3/HXE/commit/beaf2f54e497d58a56551ca4dc64506c524be10f))
* re-throw exceptions during --test ([cee7a4d](https://github.com/HaloSPV3/HXE/commit/cee7a4d2680a8f5a3488010abfc005b6f35369a3))
* read stream game Progress data stream w/ safe bounds ([5a3cb6e](https://github.com/HaloSPV3/HXE/commit/5a3cb6e085e63a7f8ba33122955b3e934b2788a0))
* remove/replace WinForms references ([fbae24f](https://github.com/HaloSPV3/HXE/commit/fbae24fcfe0b1cbcaaac13d55b4c0f586be7e9f7))
* replace WMI for filesystem compression with DotNet functionality ([c97821b](https://github.com/HaloSPV3/HXE/commit/c97821bc925c338aaa30ccad3b3d986427596f19))
* **SFX:** prevent a DirectoryInfo.Parent NullReferenceException ([1b340e6](https://github.com/HaloSPV3/HXE/commit/1b340e6d337dbd043db70cb56d82fccbe3c9fdf3))
* **SFX:** skip extracting files with empty names ([85708f2](https://github.com/HaloSPV3/HXE/commit/85708f20ff6ef14c845de8dda183909560e3456e))
* **SFX:** use XmlSourceGenerator for (de)serialization ([3d1d8ff](https://github.com/HaloSPV3/HXE/commit/3d1d8ffb7a58e41738609a2316bcd2d55eead1b5))
* support targeting NETFX 4.6.2, 4.8.0 ([6cff163](https://github.com/HaloSPV3/HXE/commit/6cff163f6ff11ece6569559045520241d0af4e3a))

### Reverts

* **deps:** build: raise minimum runtime to net6.0 ([a644c70](https://github.com/HaloSPV3/HXE/commit/a644c70d249fc5bcb62ecfa488ff1f92f8c7cea4))

## [2.3.0-alpha.1](https://github.com/HaloSPV3/HXE/compare/v2.2.4...v2.3.0-alpha.1) (2026-07-04)

### Features

* add --cli arg ([c895022](https://github.com/HaloSPV3/HXE/commit/c8950224de39cd8b4af1d9765380d755240ab2bf))
* Add CLI alternative to Positions GUI ([6343390](https://github.com/HaloSPV3/HXE/commit/63433907f582b6a0fa18fc92a59789af2d2ff99d))
* add HXE.Paths.Custom.Configuration(string) ([b4ba0f9](https://github.com/HaloSPV3/HXE/commit/b4ba0f9553b03ed976f2958661eeeaff56f96ea0))
* add InferResult() ([ea755bb](https://github.com/HaloSPV3/HXE/commit/ea755bbc983f0a09dc0307310012758c225cc6c2))

### Bug Fixes

* add using System.Linq ([d85f3c6](https://github.com/HaloSPV3/HXE/commit/d85f3c6ffca203c3f9cae4fd31aa0cba8924d026))
* assign null to nullable variable ([1718990](https://github.com/HaloSPV3/HXE/commit/1718990300661390977fb4bbb8dd6a1ed09eba5f))
* **Campaign:** use XmlSourceGenerator for (de)serialization ([383b151](https://github.com/HaloSPV3/HXE/commit/383b151d270b881f15b1077b534b15e3f0237e2f))
* **deps-dev:** bump `@halospv3/hce.shared-config` to 3.9.3 ([8423ef2](https://github.com/HaloSPV3/HXE/commit/8423ef2f8389fc424dd0c44f86dbe699322ae43a))
* **deps-dev:** recursively upgrade `@semantic-release/github` to 12.0.9 to fix release assets ([cd82554](https://github.com/HaloSPV3/HXE/commit/cd82554422ee02cec55d0c98700b7a135924c77e))
* exit when HaloCE.exe cannot be found ([148bbc9](https://github.com/HaloSPV3/HXE/commit/148bbc9ea4b96d762add1b598e568d9b8eaf56aa))
* handle InvalidOperationException sometimes thrown when setting DialogResult ([b1059cb](https://github.com/HaloSPV3/HXE/commit/b1059cbb71fc1e3e5343d726354db06be295a115)), closes [HaloSPV3/HXE#248](https://github.com/HaloSPV3/HXE/issues/248)
* **HCE:** resolve null-to-non-nullable assignments ([beaf2f5](https://github.com/HaloSPV3/HXE/commit/beaf2f54e497d58a56551ca4dc64506c524be10f))
* re-throw exceptions during --test ([cee7a4d](https://github.com/HaloSPV3/HXE/commit/cee7a4d2680a8f5a3488010abfc005b6f35369a3))
* read stream game Progress data stream w/ safe bounds ([5a3cb6e](https://github.com/HaloSPV3/HXE/commit/5a3cb6e085e63a7f8ba33122955b3e934b2788a0))
* remove/replace WinForms references ([fbae24f](https://github.com/HaloSPV3/HXE/commit/fbae24fcfe0b1cbcaaac13d55b4c0f586be7e9f7))
* replace WMI for filesystem compression with DotNet functionality ([c97821b](https://github.com/HaloSPV3/HXE/commit/c97821bc925c338aaa30ccad3b3d986427596f19))
* **SFX:** prevent a DirectoryInfo.Parent NullReferenceException ([1b340e6](https://github.com/HaloSPV3/HXE/commit/1b340e6d337dbd043db70cb56d82fccbe3c9fdf3))
* **SFX:** skip extracting files with empty names ([85708f2](https://github.com/HaloSPV3/HXE/commit/85708f20ff6ef14c845de8dda183909560e3456e))
* **SFX:** use XmlSourceGenerator for (de)serialization ([3d1d8ff](https://github.com/HaloSPV3/HXE/commit/3d1d8ffb7a58e41738609a2316bcd2d55eead1b5))
* support targeting NETFX 4.6.2, 4.8.0 ([6cff163](https://github.com/HaloSPV3/HXE/commit/6cff163f6ff11ece6569559045520241d0af4e3a))

### Reverts

* **deps:** build: raise minimum runtime to net6.0 ([a644c70](https://github.com/HaloSPV3/HXE/commit/a644c70d249fc5bcb62ecfa488ff1f92f8c7cea4))

## [2.3.0-develop.2](https://github.com/HaloSPV3/HXE/compare/v2.3.0-develop.1...v2.3.0-develop.2) (2026-07-02)

### Bug Fixes

* **deps-dev:** bump `@halospv3/hce.shared-config` to 3.9.3 ([8423ef2](https://github.com/HaloSPV3/HXE/commit/8423ef2f8389fc424dd0c44f86dbe699322ae43a))

## [2.3.0-develop.1](https://github.com/HaloSPV3/HXE/compare/v2.2.4...v2.3.0-develop.1) (2026-07-02)

### Features

* add --cli arg ([c895022](https://github.com/HaloSPV3/HXE/commit/c8950224de39cd8b4af1d9765380d755240ab2bf))
* Add CLI alternative to Positions GUI ([6343390](https://github.com/HaloSPV3/HXE/commit/63433907f582b6a0fa18fc92a59789af2d2ff99d))
* add HXE.Paths.Custom.Configuration(string) ([b4ba0f9](https://github.com/HaloSPV3/HXE/commit/b4ba0f9553b03ed976f2958661eeeaff56f96ea0))
* add InferResult() ([ea755bb](https://github.com/HaloSPV3/HXE/commit/ea755bbc983f0a09dc0307310012758c225cc6c2))

### Bug Fixes

* add using System.Linq ([d85f3c6](https://github.com/HaloSPV3/HXE/commit/d85f3c6ffca203c3f9cae4fd31aa0cba8924d026))
* assign null to nullable variable ([1718990](https://github.com/HaloSPV3/HXE/commit/1718990300661390977fb4bbb8dd6a1ed09eba5f))
* **Campaign:** use XmlSourceGenerator for (de)serialization ([383b151](https://github.com/HaloSPV3/HXE/commit/383b151d270b881f15b1077b534b15e3f0237e2f))
* exit when HaloCE.exe cannot be found ([148bbc9](https://github.com/HaloSPV3/HXE/commit/148bbc9ea4b96d762add1b598e568d9b8eaf56aa))
* handle InvalidOperationException sometimes thrown when setting DialogResult ([b1059cb](https://github.com/HaloSPV3/HXE/commit/b1059cbb71fc1e3e5343d726354db06be295a115)), closes [HaloSPV3/HXE#248](https://github.com/HaloSPV3/HXE/issues/248)
* **HCE:** resolve null-to-non-nullable assignments ([beaf2f5](https://github.com/HaloSPV3/HXE/commit/beaf2f54e497d58a56551ca4dc64506c524be10f))
* re-throw exceptions during --test ([cee7a4d](https://github.com/HaloSPV3/HXE/commit/cee7a4d2680a8f5a3488010abfc005b6f35369a3))
* read stream game Progress data stream w/ safe bounds ([5a3cb6e](https://github.com/HaloSPV3/HXE/commit/5a3cb6e085e63a7f8ba33122955b3e934b2788a0))
* remove/replace WinForms references ([fbae24f](https://github.com/HaloSPV3/HXE/commit/fbae24fcfe0b1cbcaaac13d55b4c0f586be7e9f7))
* replace WMI for filesystem compression with DotNet functionality ([c97821b](https://github.com/HaloSPV3/HXE/commit/c97821bc925c338aaa30ccad3b3d986427596f19))
* **SFX:** prevent a DirectoryInfo.Parent NullReferenceException ([1b340e6](https://github.com/HaloSPV3/HXE/commit/1b340e6d337dbd043db70cb56d82fccbe3c9fdf3))
* **SFX:** skip extracting files with empty names ([85708f2](https://github.com/HaloSPV3/HXE/commit/85708f20ff6ef14c845de8dda183909560e3456e))
* **SFX:** use XmlSourceGenerator for (de)serialization ([3d1d8ff](https://github.com/HaloSPV3/HXE/commit/3d1d8ffb7a58e41738609a2316bcd2d55eead1b5))
* support targeting NETFX 4.6.2, 4.8.0 ([6cff163](https://github.com/HaloSPV3/HXE/commit/6cff163f6ff11ece6569559045520241d0af4e3a))

### Reverts

* **deps:** build: raise minimum runtime to net6.0 ([a644c70](https://github.com/HaloSPV3/HXE/commit/a644c70d249fc5bcb62ecfa488ff1f92f8c7cea4))
