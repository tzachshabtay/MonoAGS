using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using FFmpeg.AutoGen;

namespace AGS.Engine
{
    public static class FFTester
    {
        public static void Test()
        {
            const int out_channels = 2, out_samples = 512, sample_rate = 44100;
            const string file = "/home/tshabtay/Personal/git/MonoAGS/Source/Demo/DemoQuest/Assets/Sounds/AMemoryAway.ogg";

            unsafe
            {
                //https://github.com/gavv/snippets/blob/master/decode_play/ffmpeg_decode.cpp
                ulong max_buffer_size = (ulong)ffmpeg.av_samples_get_buffer_size(null, out_channels, out_samples, AVSampleFormat.AV_SAMPLE_FMT_FLT, 1);
                ffmpeg.av_register_all();
                AVFormatContext* fmt_ctx = ffmpeg.avformat_alloc_context();
                if (fmt_ctx == null)
                {
                    throw new Exception("fmt_ctx is null");
                }

                int err = ffmpeg.avformat_open_input(&fmt_ctx, file, null, null);
                if (err != 0) 
                {
                    throw new Exception($"error: avformat_open_input: {err}");
                }

                err = ffmpeg.avformat_find_stream_info(fmt_ctx, null);
                if (err < 0)
                {
                    throw new Exception($"error: avformat_find_stream_info: {err}");
                }

                //ffmpeg.av_dump_format(fmt_ctx, 0, file, 0);

                uint stream = 0;
                for (; stream < fmt_ctx->nb_streams; stream++)
                {
                    if (fmt_ctx->streams[stream]->codec->codec_type == AVMediaType.AVMEDIA_TYPE_AUDIO)
                    {
                        break;
                    }
                }
                if (stream == fmt_ctx->nb_streams)
                {
                    throw new Exception("error: no audio stream found");
                }

                AVCodecContext* codec_ctx = fmt_ctx->streams[stream]->codec;
                if (codec_ctx == null)
                {
                    throw new Exception("codec_ctx is null");
                }

                if (codec_ctx->channel_layout == 0)
                {
                    codec_ctx->channel_layout = ffmpeg.AV_CH_FRONT_LEFT | ffmpeg.AV_CH_FRONT_RIGHT;
                }

                // find decoder for audio stream
                AVCodec* codec = ffmpeg.avcodec_find_decoder(codec_ctx->codec_id);
                if (codec == null)
                {
                    throw new Exception("error: avcodec_find_decoder()");
                }

                // initialize codec context with decoder we've found
                if (ffmpeg.avcodec_open2(codec_ctx, codec, null) < 0)
                {
                    throw new Exception("error: avcodec_open2()");
                }

                // initialize converter from input audio stream to output stream
                // provides methods for converting decoded packets to output stream
                SwrContext* swr_ctx =
                    ffmpeg.swr_alloc_set_opts(null,
                                       ffmpeg.AV_CH_FRONT_LEFT | ffmpeg.AV_CH_FRONT_RIGHT, // output
                                       AVSampleFormat.AV_SAMPLE_FMT_FLT,                    // output
                                       sample_rate,                          // output
                                       (long)codec_ctx->channel_layout,  // input
                                       codec_ctx->sample_fmt,      // input
                                       codec_ctx->sample_rate,     // input
                                       0,
                                       null);
                if (swr_ctx == null)
                {
                    throw new Exception("error: swr_alloc_set_opts()");
                }
                ffmpeg.swr_init(swr_ctx);

                // create empty packet for input stream
                AVPacket packet;
                ffmpeg.av_init_packet(&packet);
                packet.data = null;
                packet.size = 0;

                // allocate empty frame for decoding
                AVFrame* frame = ffmpeg.av_frame_alloc();
                if (frame == null)
                {
                    throw new Exception("frame is null");
                }

                // allocate buffer for output stream
                byte* buffer = (byte*)ffmpeg.av_malloc(max_buffer_size);
                if (buffer == null)
                {
                    throw new Exception("buffer is null");
                }

                // read packet from input audio file
                while (ffmpeg.av_read_frame(fmt_ctx, &packet) >= 0)
                {
                    // skip non-audio packets
                    if (packet.stream_index != stream)
                    {
                        continue;
                    }

                    // decode packet to frame
                    int got_frame = 0;
                    if (ffmpeg.avcodec_decode_audio4(codec_ctx, frame, &got_frame, &packet) < 0)
                    {
                        throw new Exception("error: avcodec_decode_audio4()");
                    }

                    if (got_frame == 0)
                    {
                        continue;
                    }

                    int got_samples = 0;
                    fixed (byte** frame_data = (byte*[])frame->data)
                    {
                        // convert input frame to output buffer
                        got_samples = ffmpeg.swr_convert(
                            swr_ctx,
                            &buffer, out_samples,
                            frame_data, frame->nb_samples);
                    }

                    if (got_samples < 0)
                    {
                        throw new Exception("error: swr_convert()");
                    }

                    while (got_samples > 0)
                    {
                        ulong buffer_size = (ulong)
                            ffmpeg.av_samples_get_buffer_size(
                                null, out_channels, got_samples, AVSampleFormat.AV_SAMPLE_FMT_FLT, 1);

                        if (buffer_size > max_buffer_size)
                        {
                            throw new Exception($"buffer size {buffer_size} bigger than max {max_buffer_size}");
                        }

                        // write output buffer to stdout
                        Debug.WriteLine("Buffer Size: " + buffer_size);
                        /*if (write(STDOUT_FILENO, buffer, buffer_size) != buffer_size)
                        {
                            fprintf(stderr, "error: write(stdout)\n");
                            exit(1);
                        }*/

                        // process samples buffered inside swr context
                        got_samples = ffmpeg.swr_convert(swr_ctx, &buffer, out_samples, null, 0);
                        if (got_samples < 0)
                        {
                            throw new Exception("error: swr_convert()");
                        }
                    }

                    // free packet created by decoder
                    ffmpeg.av_free_packet(&packet);
                }

                ffmpeg.av_free(buffer);
                ffmpeg.av_frame_free(&frame);

                ffmpeg.swr_free(&swr_ctx);

                ffmpeg.avcodec_close(codec_ctx);
                ffmpeg.avformat_close_input(&fmt_ctx);

                /*
                AVCodec* audioCodec = ffmpeg.avcodec_find_encoder(AVCodecID.AV_CODEC_ID_AAC);
                AVCodecContext* audioCodecContext = ffmpeg.avcodec_alloc_context3(audioCodec);
                audioCodecContext->bit_rate = 1280000;
                audioCodecContext->sample_rate = 48000;
                audioCodecContext->channels = 2;
                audioCodecContext->channel_layout = ffmpeg.AV_CH_LAYOUT_STEREO;
                audioCodecContext->frame_size = 1024;
                audioCodecContext->sample_fmt = audioCodec->sample_fmts[0];
                audioCodecContext->profile = ffmpeg.FF_PROFILE_AAC_LOW;
                audioCodecContext->codec_id = audioCodec->id;
                audioCodecContext->codec_type = audioCodec->type;*/
            }
        }
    }
}