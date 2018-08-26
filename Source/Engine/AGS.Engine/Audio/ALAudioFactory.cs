using System;
using AGS.API;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using Autofac;

namespace AGS.Engine
{
	public class ALAudioFactory : IAudioFactory
	{
		private readonly IResourceLoader _loader;
		private readonly Dictionary<string, List<ISoundDecoder>> _decoders;
        private readonly Resolver _resolver;
        private readonly IAudioSystem _audioSystem;
        private object _unsafeMemoryLocker = new object();

        public ALAudioFactory(IResourceLoader loader, Resolver resolver, IAudioSystem audioSystem)
		{
            _audioSystem = audioSystem;
			_loader = loader;
            _resolver = resolver;
			_decoders = new Dictionary<string, List<ISoundDecoder>> 
			{
				{ ".WAV", new List<ISoundDecoder> { new WaveDecoder() } }
				,{ ".OGG", new List<ISoundDecoder> { new OggDecoder() } }
				,{ ".FLAC", new List<ISoundDecoder> { new FlacDecoder() } }
			};
            RegisterExternalDecoders?.Invoke(_decoders);
        }

		public static Action<Dictionary<string, List<ISoundDecoder>>> RegisterExternalDecoders { get; set; }

        #region ISoundFactory implementation

        [MethodWizard]
        public IAudioClip LoadAudioClip(string filePath, string id = null)
        {
            Debug.WriteLine("Loading AudioClip: " + filePath);
            ISoundData soundData = loadSoundData(filePath);
            if (soundData == null) return null;
            TypedParameter idParam = new TypedParameter(typeof(string), id ?? filePath);
            TypedParameter soundParam = new TypedParameter(typeof(ISoundData), soundData);
            var clip = _resolver.Container.Resolve<IAudioClip>(idParam, soundParam);
            _audioSystem.AudioClips.Add(clip);
            return clip;
        }

		public async Task<IAudioClip> LoadAudioClipAsync(string filePath, string id = null)
		{
			return await Task.Run(() => LoadAudioClip(filePath, id));
		}

		#endregion

		private ISoundData loadSoundData(string filePath)
		{
			IResource resource = _loader.LoadResource(filePath);
            if (resource == null) return null;
			string fileExtension = Path.GetExtension(filePath).ToUpperInvariant();
            if (fileExtension == "") fileExtension = Path.GetExtension(resource.ID).ToUpperInvariant();
			var stream = resource.Stream;

			try
			{
				if (_decoders.TryGetValue(fileExtension, out var decoders))
				{
					foreach (var decoder in decoders)
					{
                        //Was experiencing crashes when decoding 2 audio streams at the same time (ogg and flac kept crashing randomly),
                        //this was solved by putting a lock on the decoding (assuming unsafe memory access in the decoders is not thread safe)
                        lock (_unsafeMemoryLocker)
                        {
                            var sound = decoder.Decode(stream);
                            if (sound != null) return sound;
                        }
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