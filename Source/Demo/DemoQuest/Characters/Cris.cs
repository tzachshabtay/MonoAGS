using System.Threading.Tasks;
using AGS.API;
using AGS.Engine;

namespace DemoGame
{
    public class Cris
	{
		private ICharacter _character;
		private const string _baseFolder = "../../Assets/Characters/Cris/";

		public async Task<ICharacter> LoadAsync(IGame game)
		{
            AGSLoadImageConfig loadConfig = new AGSLoadImageConfig(new AGS.API.Point(0, 0));

			var footstep = await game.Factory.Sound.LoadAudioClipAsync("../../Assets/Sounds/151238__owlstorm__hard-female-footstep-2.wav");
			ISoundEmitter emitter = new AGSSoundEmitter (game);
			emitter.AudioClip = footstep;

			IOutfit outfit = await game.Factory.Outfit.LoadOutfitFromFoldersAsync(_baseFolder, walkLeftFolder: "Walk/left",
				walkDownFolder: "Walk/front", idleLeftFolder: "Idle/left", idleDownFolder: "Idle/front", 
				speakLeftFolder: "Talk", loadConfig: loadConfig);

			_character = game.Factory.Object.GetCharacter("Cris", outfit).Remember(game, c => _character = c);
            _character.SpeechConfig.TextConfig = AGSTextConfig.ChangeColor(_character.SpeechConfig.TextConfig, Colors.OrangeRed, Colors.Black, 1f);
            var approach = _character.AddComponent<IApproachComponent>();
            approach.ApproachStyle.ApproachWhenVerb["Talk"] = ApproachHotspots.WalkIfHaveWalkPoint;
			emitter.Object = _character;

            _character.AddComponent<ISaturationEffectComponent>();
            //Uncomment for portrait
            /*
            var portrait = game.Factory.Object.GetObject("CrisPortrait");
            portrait.StartAnimation(game.Factory.Graphics.LoadAnimationFromFolder(_baseFolder + "Talk"));
            portrait.Border = AGSBorders.SolidColor(Colors.AliceBlue, 3f, true);
            portrait.Visible = false;
            portrait.RenderLayer = AGSLayers.Speech;
            portrait.IgnoreViewport = true;
            portrait.IgnoreScalingArea = true;
            game.State.UI.Add(portrait);
            _character.SpeechConfig.PortraitConfig = new AGSPortraitConfig { Portrait = portrait, Positioning = PortraitPositioning.Alternating };
            */

            var speakAnimation = _character.Outfit[AGSOutfit.Speak];
			Characters.RandomAnimationDelay(speakAnimation.Left);
			Characters.RandomAnimationDelay(speakAnimation.Right);
            emitter.Assign(_character.Outfit[AGSOutfit.Walk], 1, 5);

            _character.StartAnimation (_character.Outfit[AGSOutfit.Idle].Down);
			_character.DisplayName = "Cris";
            _character.IsPixelPerfect = true;

			Characters.Cris = _character;
			return _character;
		}
	}
}

