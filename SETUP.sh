
npm install --save-dev @commitlint/cli @commitlint/config-angular
echo "module.exports = {extends: ['@commitlint/config-angular']};" > commitlint.config.js

npm install --g @commitlint/prompt-cli @commitlint/config-angular
echo "module.exports = {extends: ['@commitlint/config-angular']};" > commitlint.config.js

npm install --save-dev @commitlint/prompt @commitlint/config-conventional commitizen
echo "module.exports = {extends: ['@commitlint/config-conventional']};" > commitlint.config.js

pip install pre-commit

# https://github.com/commitizen/cz-cli
# Install Commitizen CLI
npx cz