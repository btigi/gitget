# gitget

A simple command-line app acting as a GitHub zip file release based equivalant to `winget`. The latest release of a GitHub repo can be downloaded and unzipped to a specified directory; no installer project types are supported.

The app checks for a packages.json file in the GitHUb btigi/gitget-pkgs repo (note this repo does not currently exist). As an alternative the app will read a local `gitget.packages` from the app directory, and prefer this package source if it exists.

A sample `gitget.packages` file is included in the repo.

## Requirements

- Windows
- [.NET 10 SDK](https://dotnet.microsoft.com/download)

## Compiling

To clone and run this application, you'll need [Git](https://git-scm.com) and [.NET](https://dotnet.microsoft.com/) installed on your computer. From your command line:

```powershell
# Clone this repository
$ git clone https://github.com/btigi/gitget

# Go into the repository
$ cd src

# Build  the app
$ dotnet build
```
The project supports single file, installation independent, trimmed, AoT distribution:

```powershell
dotnet publish gitget -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=true -p:PublishAot=true


## Usage

| Command | Description |
|---------|-------------|
| `gitget search <term>` | List registry packages matching the term |
| `gitget install <name>` | Download latest release zip and extract to `C:\temp\<name>` |
| `gitget uninstall <name>` | Delete the install folder and local state |

## Local package list

If `gitget.packages` exists in the same directory as the executable, that file is used instead of fetching `packages.json` from GitHub. Format matches the generated registry index:

```json
{
  "packages": [
    {
      "name": "dgmlViewer",
      "description": "A DGML file viewer",
      "repo": "btigi/dgmlViewer",
      "asset": null
    }
  ]
}
```

## Configuration

On first run settings are created at `%LOCALAPPDATA%\gitget\config.json` (only used when no local `gitget.packages` is present):

```json
{
  "registryOwner": "btigi",
  "registryRepo": "gitget-pkgs",
  "registryBranch": "main",
  "installRoot": "C:\\temp"
}
```

Installed packages are tracked in `%LOCALAPPDATA%\gitget\installed.json`.

## Licencing

gitget is licenced under the MIT license. Full license details are available in license.md