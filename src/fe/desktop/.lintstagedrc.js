export default {
  '*.md': ['npx markdownlint-cli2 --fix', 'prettier --write'],
  '*.{js,ts,jsx,tsx,vue}': ['eslint --fix', 'prettier --write'],
  '*.{css,less,scss}': [
    'stylelint --fix --allow-empty-input',
    'prettier --write',
  ],
}
