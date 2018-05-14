# Splash Screen

MonoAGS comes with a built-in simple splash screen that you can choose to use if needed.
The splash screen can be seen at the beginning of the demo game. It is a simple loading screen, showing a "Loading..." text (which can be changed to any custom text with any custom font/text color/etc) which gets smaller and then bigger in a loop while resources are loading.
When you load the splash screen you get a reference to the room so you can change its background or add any other desired graphics/sound assets, though notes that the text changes its size in a way designed to be independent of the framerate, as loading all of the resources might affect the framerate deeply, so adding your own custom animations to the splash screen might not show smoothly as in the rest of the game.

Here's an example in which we change the loading text, change the text color to blue and add a background:

```cs

var mySplashScreen = new AGSSplashScreen();
mySplashScreen.LoadingText = "Loading, Please Wait...";
mySplashScreen.TextConfig = new AGSTextConfig(brush: game.Factory.Graphics.Brushes.LoadSolidBrush(Colors.Blue));
var room = mySplashScreen.Load(game);

var splashBackground = game.Factory.Object.GetObject("Splash Background");
splashBackground.Image = await game.Factory.Graphics.LoadImageAsync("Rooms/Splash/bg.png");
room.Background = splashBackground;

await game.State.ChangeRoomAsync(room);

//...
//Load stuff here
//...

await game.State.ChangeRoomAsync(firstGameRoom);

```
