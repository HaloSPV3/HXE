import baseConfig from '@halospv3/hce.shared-config/commitlintConfig';

const scopes = {
  Campaign: 'Affects HXE.Campaign or its descendants.',
  CLI: 'Affects the HXE.CLI namespace.',
  commitlint: 'Affects this repo\'s commitlint config, esp. its commit scopes.',
  Common: 'Affects the HXE.Common namespace and its child classes.',
  contributing: 'Affects CONTRIBUTING.md',
  'conv-pr': 'Affects ".github/workflows/conv-pull-requests.yml".',
  deps: 'Affects dependencies bundled with or depended on by published packages and artifacts. '
    + 'For NuGet package/PackageReferences, this means anything that has "runtime", "native" or '
    + '"contentfiles" included.',
  'deps-dev': 'Affects dependencies used by CI, dev environments, or the build system(s); '
    + 'but are not required at runtime nor bundled with or statically linked into the published '
    + 'binaries or packages. For NuGet packages/PackageReferences, this would be anything with'
    + 'PrivateAssets="All" and no "runtime", "native", or "contentfiles" to be included in output.',
  HCE: 'Affects the HXE.HCE namespace or its descendants.',
  MCC: 'Affects "src/MCC/**" or src/assets/343I_DER.cer',
  README: 'Affects README.md or any other README documents.',
  release: 'Reserved for release commits.',
  SFX: 'Affects the HXE.SFX class or other symbols in its source file.',
  TODO: 'Affects TODO.md or any todo comments.',
  vscode: 'Affects anything in the .vscode directory.',
};

const config: import('@commitlint/types').UserConfig = {
  ...baseConfig,
  rules: {
    ...baseConfig.rules,
    'scope-enum': [
      2,
      'always',
      (Reflect.ownKeys(scopes)) as (keyof typeof scopes)[],
    ],
  },
};

export default config;
