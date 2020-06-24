# Borders

You can assign a border to an object, to have a border drawn around the object.
A border can be drawn both behind and in front of the object it's assigned to, which opens up a lot of possibilities for things you can do with the border.

## Built in borders

`MonoAGS` comes with several built in "border styles" which you can use.

### Solid Color

A solid color border will be drawn around the object.
You can choose the color, the border width, and whether or not to make the corners rounded.
You can create a solid border by using the border factory: `factory.Graphics.Border.SolidColor(...)`.
By default the border will be drawn in front of the object, but you can change it to be drawn behind the object by setting the `DrawBorderBehind` property of the border to `true`.

### Gradient Color

A gradient color border will be drawn around the object.
This gives you the same choices as the solid color border, only instead of selecting one color, you can select 4 colors, one for each corner of the border. The color along the edges will be interpolated, thus creating the gradient effect.
You can create a gradient border by using the border factory: `factory.Graphics.Border.Gradient(...)`.

### 9-slice Image

This allows you to use an image, slice it to 9 pieces, and spread it in various ways to create a border. This is similar to the [customized text window](http://www.adventuregamestudio.co.uk/wiki/Text_Window,_Customised) feature that "Classic" AGS currently has, only more powerful and available for all objects, not just for text windows.
The 9-slice image border is heavily inspired by the border image used by CSS, so you can look at a [CSS example](https://css-tricks.com/almanac/properties/b/border-image/), to see what can be done with a 9-slice image.

## Writing your own

You can also implement your custom border if you want. For that you need to implement the `IBorderStyle` interface which let you write rendering for the back and for the front of the border.
Take a look at the existing implementation in the engine code to get an idea of how to render your own border. 