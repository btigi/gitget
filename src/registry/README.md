# gitget package registry

This folder is intended to be the root of a GitHub repository (e.g. `btigi/gitget-pkgs`).

## Adding a package

1. Add a JSON file under `manifests/` (one package per file), for example `manifests/myApp.json`:

```json
{
  "name": "myApp",
  "description": "Short description for search results",
  "repo": "owner/repo",
  "asset": null
}
```

- `asset` is optional. When omitted, gitget installs the first `.zip` release asset (excluding GitHub’s auto-generated source archives). Use a pattern like `MyApp-*.zip` when a release has multiple zip files.

2. Push to `main`. The GitHub Action merges all manifests into `packages.json`.

## gitget configuration

Point gitget at this repo by editing `%LOCALAPPDATA%\gitget\config.json`:

```json
{
  "registryOwner": "btigi",
  "registryRepo": "gitget-pkgs",
  "registryBranch": "main",
  "installRoot": "C:\\temp"
}
```
