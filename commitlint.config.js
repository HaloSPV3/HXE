//@ts-check
import baseConfig from '@halospv3/hce.shared-config/commitlintConfig';

const scopes = {
  deps: 'Affects dependencies bundled with or depended on by published packages and artifacts. ' +
    'For NuGet package/PackageReferences, this means anything that has "runtime", "native" or ' +
    '"contentfiles" included.',
  'deps-dev': 'Affects dependencies used by CI, dev environments, or the build system(s); ' +
    'but are not required at runtime nor bundled with or statically linked into the published ' +
    'binaries or packages. For NuGet packages/PackageReferences, this would be anything with' +
    'PrivateAssets="All" and no "runtime", "native", or "contentfiles" to be included in output.',
  release: 'Reserved for release commits.',
  vscode: 'Affects anything in the .vscode directory.'
};

/** @type {import('@commitlint/types').UserConfig} */
const config = {
  ...baseConfig,
  rules: {
    ...baseConfig.rules,
    "scope-enum": [
      2,
      'always',
      /** @type {(keyof typeof scopes)[]} */
      (Reflect.ownKeys(scopes))
    ]
  }
};

export default config;
