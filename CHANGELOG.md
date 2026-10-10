## [3.0.0-alpha.2](https://github.com/HaloSPV3/HXE/compare/v3.0.0-alpha.1...v3.0.0-alpha.2) (2026-10-10)

### ⚠ BREAKING CHANGES

* **Patcher:** `DataSet`'s properties are now `readonly`. As a result, `DataSet` instances must be constructed via the primary constructor. Assignments to properties in a constructor call will error.

### Features

* add CLI argument `--silent` to suppress all console output ([8c1d542](https://github.com/HaloSPV3/HXE/commit/8c1d542ccdce35f466b184699af02a3231647a2c))

### Bug Fixes

* change version variables used in CLI banner's version- and commit-related messages to make them make sense ([c440204](https://github.com/HaloSPV3/HXE/commit/c4402042fc59023dd46c492f36030fc23c246fb4))
* remove unused CLI argument `--registry`; it did nothing; use `reg.exe`, instead ([d86c050](https://github.com/HaloSPV3/HXE/commit/d86c05027ff3386dd64eb9ffd341247166fce052))
* rotate log files so old logs are not immediately overwritten ([7fd6147](https://github.com/HaloSPV3/HXE/commit/7fd61473ac1860adeb67ab1ebd84d0134804c1fa)), closes [#574](https://github.com/HaloSPV3/HXE/issues/574)

### Performance Improvements

* **Patcher:** change `DataSet` from a class to a readonly-struct ([1c07329](https://github.com/HaloSPV3/HXE/commit/1c073290692643e2a3f543b681e1d9f85db53fd1))

## [3.0.0-alpha.1](https://github.com/HaloSPV3/HXE/compare/v2.3.2-alpha.1...v3.0.0-alpha.1) (2026-10-09)

### ⚠ BREAKING CHANGES

* **File:** Attempts to instantiate`HXE.File` will fail without setting `Path`. Set Path when instantiating or use `HXE.File.GetTemporary()`.
* **CLI, HCE, Installer, Manifest, MCC, Net, Options, Process, Update:** Many property types are now explicitly nullable. This should have been done when support for C# 8 and later was first introduced.

### Features

* add Resume values for firefight maps Bridge, Hangar, Lobby, and Swamp Tower ([5deb5ce](https://github.com/HaloSPV3/HXE/commit/5deb5cec149c77783997c87f988c65a04148c858))
* **File:** add simple wrapper for System.IO.File.Exists ([b13fad2](https://github.com/HaloSPV3/HXE/commit/b13fad231e44734c672bb4a1996769a7581ef3b0))

### Bug Fixes

* add `DynamicallyAccessedMembers` attributes to some `OptionSet` methods ([0a52e5c](https://github.com/HaloSPV3/HXE/commit/0a52e5cd3f06fced9c90dce802db6fa0836327a9))
* add check in Test to access embedded 343I cert ([e56b6ce](https://github.com/HaloSPV3/HXE/commit/e56b6ced1045191fb0546888adbc542f43348065))
* add internal "GetTemporary" method to get a `File` instance whose path does not exist; use it for ([c5605fb](https://github.com/HaloSPV3/HXE/commit/c5605fbb48b24d89abded391de0229c1cd1a6838))
* **CLI, HCE, Installer, Manifest, MCC, Net, Options, Process, Update:** add nullable annotations and checks ([4466616](https://github.com/HaloSPV3/HXE/commit/44666168eff38d6da08b159bc8ae93a4f61fa26e))
* **Common:** run `xrandr` via `Start(string, string)` for legacy compatibility; throw if it returns null ([c5f15d0](https://github.com/HaloSPV3/HXE/commit/c5f15d0391b5e838173f180de56d11f1a0a280b5))
* **Compiler:** prevent null assignment to `Path` relative-path string ([b2fc86d](https://github.com/HaloSPV3/HXE/commit/b2fc86d6fe31d2cf04e541a8835b6c87d7a4533c))
* **Compiler:** throw exceptions when package name or package entry name are impossibly null ([712c42a](https://github.com/HaloSPV3/HXE/commit/712c42a468944547b6b2f4da98f4d52f73a8b0eb))
* **deps:** bump `XmlSourceGenerator` to `a97ef26` "fix: explicitly define `PolymorphicMapping`'s properties; add `PublicParameterlessConstructor` to the `Type` property" ([aed3780](https://github.com/HaloSPV3/HXE/commit/aed37806e3451098824dc35d3b80ae77bce064f6))
* **deps:** bump `XmlSourceGenerator` to `e530301` "test: update tests' expectations regarding `PolymorphicMapping`, `PublicParameterlessConstructor` annotations, and null-forgiveness" ([26b9152](https://github.com/HaloSPV3/HXE/commit/26b9152a26aeffcf3f46f82b0539462e46804715))
* **deps:** upgrade `System.Text.Json` in .NET Framework 4 ([dc5f772](https://github.com/HaloSPV3/HXE/commit/dc5f772a183c956873b777e4bbf98edd74e03dab))
* **Difficulty:** annotate with `PublicParameterlessConstructor` ([70226e1](https://github.com/HaloSPV3/HXE/commit/70226e1832901673087dac918426505a1c7d13d2))
* **Difficulty:** set XML root and namespace ([937d93c](https://github.com/HaloSPV3/HXE/commit/937d93c24128fc44f0e3d1f5b463b44f41c1b162))
* **File:** if null, derive `File.Name` from `File.Path` ([b9554bb](https://github.com/HaloSPV3/HXE/commit/b9554bb6c353bc7d6fc0f15a674fcb89672db243))
* **File:** require `Path` be set in before exiting constructor ([4ba25f9](https://github.com/HaloSPV3/HXE/commit/4ba25f9973f901ba2216f52d95a71ddac693c9b9))
* **File:** use `GetFullPath` in `File.Path` setter ([828c116](https://github.com/HaloSPV3/HXE/commit/828c11650186386812fbea71d0234ebe7c09d1b6))
* **HCE:** fall back to `GetPathRoot` when `GetDirectory` returns `null` ([2f0b591](https://github.com/HaloSPV3/HXE/commit/2f0b59157d5569d1b9611c021a97de2cc48372e0))
* **HCE:** prefer the generic overload 'System.Enum.GetValues<TEnum>()' to prevent Reflection use ([7395cd5](https://github.com/HaloSPV3/HXE/commit/7395cd53e2780a86b4e82c8460710f9bf4e26a02))
* **Installer:** throw when properties and variables are impossibly null ([04320ce](https://github.com/HaloSPV3/HXE/commit/04320ce24d092608331287f1b63361029ad99105))
* **Kernel:** change "ignoreing" in a `MAIN.OPEN` message to "ignoring" ([1c88879](https://github.com/HaloSPV3/HXE/commit/1c88879aad19b6df25e6f3715f764ecdf0e56c62))
* **Kernel:** increase max log file size from 1MiB to 8MiB ([43465c6](https://github.com/HaloSPV3/HXE/commit/43465c68553be5f9e6dd84e366eda59bba65f1e9))
* **Kernel:** reduce amount of times "dinput8.dll" and "mods/opensauce.dll" are checked if they exist ([abbb8e9](https://github.com/HaloSPV3/HXE/commit/abbb8e9fc041411b2db67801a235863c80995379))
* **Latest, OpenSauce, Positions, Update:** initialize properties to safe, non-null defaults ([697c4e0](https://github.com/HaloSPV3/HXE/commit/697c4e0a315bdbf75322d9c6288501f2e58dec79))
* **Net:** initialize `DefaultHttpClient` in a way that doesn't cause a crash immediately ([7518948](https://github.com/HaloSPV3/HXE/commit/7518948286f5d3b8cba0211eac4e5bd4e1cdce61))
* **Net:** prevent dereference of maybe-null `fileStream` ([d484946](https://github.com/HaloSPV3/HXE/commit/d484946d2ec14e715f9993e27bde9e04c2728417))
* **Paths:** default `StartDirectory` to current working directory if the current process's executable path cannot be determined ([4b496dc](https://github.com/HaloSPV3/HXE/commit/4b496dcce3d8771043120fdddcf453be8daaa1fc))
* **Paths:** expand (GetGullPath) `steamexepath` when setting Steam paths ([f3edcc8](https://github.com/HaloSPV3/HXE/commit/f3edcc830375cf25a25d2e50857c3e41c2a38dad))
* **Positions, Settings:** use Selawik Light, Inter Light, and Roboto Light as fallbacks when Segoe UI Light is unavailable (e.g. Non-Windows platforms) ([46ef277](https://github.com/HaloSPV3/HXE/commit/46ef277b2c5a19d780c46c9ce287f20e05ba10d5))
* **Process:** initialize `LastResult` to new instance ([979dafe](https://github.com/HaloSPV3/HXE/commit/979dafe7b89ca713a894e73135b036ee1d3abe3b))
* **Registry:** add null checks for opened keys, subkeys ([482ea1d](https://github.com/HaloSPV3/HXE/commit/482ea1d7af6dc26e12ae14d22dc8801bce8fee87))
* **Registry:** deprecate `CreateKeys` ([4b90bd8](https://github.com/HaloSPV3/HXE/commit/4b90bd8e1871f7cc59ef1d49a042e891ce568b8a))
* request compressed HTTP responses ([ea17aef](https://github.com/HaloSPV3/HXE/commit/ea17aefcea6b85c407f7a84a0b7bb881eff339e2)), closes [#185](https://github.com/HaloSPV3/HXE/issues/185)
* return early if a campaign's `Missions` or `Difficulty` lists are null ([8f3d3cd](https://github.com/HaloSPV3/HXE/commit/8f3d3cd4a98ff8c25819d0e621a5724cb6d3050f))
* **Settings:** prefer `Enum.GetValues<T>(..)` to resolve warning IL3050 ([1e6bb4a](https://github.com/HaloSPV3/HXE/commit/1e6bb4adced51a7c72ae0161a72006ff827bfd78))
* **Steam:** remove invalid default value of argument `libraryFoldersVdf` of method `ParseLibrary` ([dd8352b](https://github.com/HaloSPV3/HXE/commit/dd8352b0338d2b430d5f5323f7aa88b737cfdb86))
* **Update:** annotate `Commit` parameter `progress` as nullable ([3d35882](https://github.com/HaloSPV3/HXE/commit/3d35882f214a42c0742be41563620a34d18672be))
* **Update:** handle exceptions from failed http requests; fixes `TaskCanceledException` in Wine Mono ([7c9179e](https://github.com/HaloSPV3/HXE/commit/7c9179efecb56818c6818be5a305e65ad3176d22))
* **Update:** when `totalFileSize` is `null`, use invalid `-1` ([d1c875f](https://github.com/HaloSPV3/HXE/commit/d1c875f030276833eb45d11fced2942c0b581532))
* use PInvoke for NTFS compression; previous implementation did nothing ([0327183](https://github.com/HaloSPV3/HXE/commit/03271839a1b2a5ad1e66924e75201ebafcdc625c))

### Performance Improvements

* **Manifest:** source-generate XML (de)serializers ([e9d6070](https://github.com/HaloSPV3/HXE/commit/e9d6070ec524e4be200f3f873ab97557780e5550))
* **OpenSauce:** source-generate XML (de)serializers ([d8e0738](https://github.com/HaloSPV3/HXE/commit/d8e0738b8e9747ecf52a33e138de694f6ba00edc))
* replace patches.crk read with static definitions ([345f711](https://github.com/HaloSPV3/HXE/commit/345f711cf2426a92d6e973dccd11152669885401))
* **Update:** auto-generate XML serialization implementations for `Update`, `Asset` ([7a61475](https://github.com/HaloSPV3/HXE/commit/7a61475527e7cd8db72d19c1b6440130e2f74bc3))

## [2.3.2-alpha.1](https://github.com/HaloSPV3/HXE/compare/v2.3.1...v2.3.2-alpha.1) (2026-07-27)

### Bug Fixes

* **Net:** resolve HttpClient crash; decrease timeout from insane 24 hours to runtime-default 30 seconds ([a48b4cf](https://github.com/HaloSPV3/HXE/commit/a48b4cf9d01e0607653b4e04ef7b51d768f961fe))

### Reverts

* **deps-dev): "build(deps-dev:** update dependency typescript to v7 ([#537](https://github.com/HaloSPV3/HXE/issues/537))" ([f4d36c2](https://github.com/HaloSPV3/HXE/commit/f4d36c26f2597d3f53f17894514e39ee76621670))

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
