Assets are all the media files which you load in the game to display content.

### Supported asset types:

#### Graphics- 

On Desktop: PNG (recommended), BMP, GIF, EXIF, JPG and TIFF.

On Android: PNG (recommended), BMP, GIF, JPEG and WebP. 

On IOS: PNG (recommended), BMP, GIF, JPG/JPEG, TIFF, ICO, CUR and XBM.

#### Audio- 

OGG Vorbis (recommended), WAV (PCM), FLAC.

#### Fonts-

TTF (recommended), OTF with TrueType (not officially tested yet)

Please let us know on more asset types you want supported.

### Adding assets:

There are 2 ways you can have assets in your game. They can be embedded in the project or loaded from the file system. The advantage of having the assets embedded is that you can rest assured the assets will be distributed with your game and cannot be touched from outside, which is why it's the recommended method. Loading from file system might be useful if you need the assets to be loaded dynamically, for example you might want to download assets from the internet, or if you give the user the option to choose her/his avater.

To embed assets in your game project, first add the asset files to the Assets folder in your shared game project (it doesn't have to be in the root Assets folder, you can have any structure you want in there). Then, in the solution explorer, right click the Assets folder and "Add Existing" (there are options for adding files or complete folders, depending on what you want to do) and add those assets to the project. You should then see those assets in the tree. Lastly, right click those assets and select "Embedded Resource" as your "Build Option".
Note that even the asset is embedded, it's only embedded when compiling the game, so you cannot delete the file before deploying your game, and if you replace the file, it will be automatically replaced in the game on your next run.

### Loading assets in the game:

Currently, all assets should be loaded explicitly from code, and you select when and how they are loaded. There's a low-level `ResourceLoader` class which loads any file into a stream of bytes, and on top of that a bunch of high-level methods to load specific types of resources. Most of those high-level methods sit under the `IGameFactory` interface (which can be accessed under the main `IGame` interface), so for example, you might have a `game.Factory.Sound.LoadAudioClip(pathToClip)`. 
All those methods take a path to a file (though there are some convenient methods that take a folder, like `LoadAnimationFromFolder`) which has to be structred as so: if the asset is to be loaded from a file in the file system, then put the absolute path of the file. If the asset is embedded then put the relative path of the file (when the current folder is the folder where the executable sits, which is 2 folders above the Assets folder). So, for example, if you have an audio file called "trumpet.ogg" sitting under a "Sounds" folder in "Assets", your path would be "../../Assets/Sounds/trumpet.ogg". Note that this would work even if the file is sitting in that folder but not embedded. This is because `ResourceLoader` first search for an embedded asset with that path, but if one is not found, it looks for the file in the file system.


