import baseConfig from '@halospv3/hce.shared-config/eslintConfig';
import unicorn from 'eslint-unicorn';
import { defineConfig } from 'eslint/config';

export default defineConfig(
  ...baseConfig,
  {
    ...unicorn.configs.recommended,
    rules: {
      ...unicorn.configs.recommended.rules,
      'unicorn/no-array-sort': 'off'
    },
    files: [
      '**/*.js',
      '**/*.mjs',
      '**/*.ts'
    ],
  }
);
