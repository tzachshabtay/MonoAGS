using System;
using AGS.API;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

namespace AGS.Engine
{
	public class ALAudioFactory : IAudioFactory
	{
		private readonly IResourceLoader _loader;
		private readonly Dictionary<string, List<ISoundDecoder>> _decoders;
		private readonly IAudioSystem _system;

		public ALAudioFactory(IResourceLoader loader, IAudioSystem system)
		{
			_system = system;
			_loader = loader;
			_decoders = new Dictionary<string, List<ISoundDecoder>> 
			{
				{ ".WAV", new List<ISoundDecoder> { new WaveDecoder() } }
				,{ ".OGG", new List<ISoundDecoder> { new OggDecoder() } }
			};
			var registerExternlDecoders = RegisterExternalDecoders;
			if (registerExternlDecoders != null) registerExternlDecoders(_decoders);
		}

		public static Action<Dictionary<string, List<ISoundDecoder>>> RegisterExternalDecoders { get; set; }

		#region ISoundFactory implementation

		public IAudioClip LoadAudioClip(string filePath, string id = null)
		{
			ISoundData soundData = loadSoundData(filePath);
			if (soundData == null) return null;
			return new ALAudioClip (id ?? filePath, soundData, _system);
		}

		public async Task<IAudioClip> LoadAudioClipAsync(string filePath, string id = null)
		{
			return await Task.Run(() => LoadAudioClip(filePath, id));
		}

		#endregion

		private ISoundData loadSoundData(string filePath)
		{
			IResource resource = _loader.LoadResource(filePath);
			string fileExtension = Path.GetExtension(filePath).ToUpperInvariant();
			var stream = resource.Stream;

			try
			{
				List<ISoundDecoder> decoders;
				if (_decoders.TryGetValue(fileExtension, out decoders))
				{
					foreach (var decoder in decoders)
					{
						var sound = decoder.Decode(stream);
						if (sound != null) return sound;

						stream = rewindStream(stream);
					}
				}
				Debug.WriteLine("ALAudioFactory: File extension is not supported- " + fileExtension);
				return null;
			}
			finally
			{
				stream.Dispose();
			}
		}

		private Stream rewindStream(Stream stream)
		{
			if (!stream.CanSeek)
			{
				MemoryStream memoryStream = new MemoryStream ((int)stream.Length);
				stream.CopyTo(memoryStream);
				stream.Dispose();
				stream = memoryStream;
			}
			stream.Position = 0;
			return stream;
		}
	}
}

