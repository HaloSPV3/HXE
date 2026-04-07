import baseConfig from '@halospv3/hce.shared-config/eslintConfig';
import unicorn from 'eslint-plugin-unicorn';
import { defineConfig } from 'eslint/config';

export default defineConfig(
  ...baseConfig,
  {
    ...unicorn.configs.recommended,
    rules: {
      ...unicorn.configs.recommended.rules,
      'unicorn/no-array-sort': 'off',
      'unicorn/expiring-todo-comments': 'off',
    },
    files: [
      '**/*.js',
      '**/*.mjs',
      '**/*.ts',
    ],
  },
);
