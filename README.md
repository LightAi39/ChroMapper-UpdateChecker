# ChroMapper-UpdateChecker
This plugin checks when plugins need to be updated and lists them in the song select menu if they are outdated.

Any plugins that need to be updated will be listed in the top right corner with the format `name - [local version] > [latest version]`

## How to install
Drop the files inside the `ChroMapper-UpdateChecker(version).zip` into the `Plugins` folder in your ChroMapper directory. This plugin should work on any ChroMapper version.

## For plugin developers
Any plugin can use this!  Simply add a `manifest.json` as an embedded resource to your project and it will automatically be read. To create the `manifest.json`, follow the format below:

```json
{
  "id": "ChroMapper-UpdateChecker",
  "name": "Update Checker",
  "author": "Pink & Light Ai",
  "version": "v1.0.0",
  "description": "Checks when plugins need updating",
  "git": "https://github.com/LightAi39/ChroMapper-UpdateChecker"
}
```
The `manifest.json` must include every field above. Make sure that `version` is the exact same as your GitHub tag linked to your release, and that the `git` link is a link to the main page of your repository.

The plugin will compare the `version` in the manifest with the tag of the release which is marked as latest on GitHub. If there is a mismatch, the plugin is considered out of date which will be shown to the users.

Also, note that your plugin does NOT need to reference the `.dll` of this plugin! Any `manifest.json` files are automatically found and checked.
