// @ts-check
import { getConfig } from '@halospv3/hce.shared-config/semanticReleaseConfigDotnet';
import { isNativeError } from 'node:util/types';
import type { Options } from 'semantic-release';

let config: Options | undefined;

const projectsToPublish = ['./src/HXE.csproj'];
const projectsToPackAndPush = projectsToPublish;
config = await getConfig(
  projectsToPublish,
  projectsToPackAndPush
).catch(error => {
  const _error = isNativeError(error)
    ? error
    : new Error('unknown error', { cause: error });
  console.error(_error);
  return _error;
});

export default config;
