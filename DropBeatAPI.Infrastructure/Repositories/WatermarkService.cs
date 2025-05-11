using Microsoft.AspNetCore.Http;
using NAudio.Lame;
using NAudio.Wave;
using NLayer.NAudioSupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DropBeatAPI.Infrastructure.Repositories
{



    using NAudio.Wave;
    using NAudio.Lame;
    using System.IO;
    using Microsoft.AspNetCore.Http;
    using NAudio.Wave.SampleProviders;

    public class WatermarkService
    {
        private readonly string _watermarkPath;
        private readonly float _watermarkVolume;

        public WatermarkService(float watermarkVolume = 1.0f)
        {
            _watermarkPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "audios", "watermark.mp3");
            if (!File.Exists(_watermarkPath))
            {
                throw new FileNotFoundException($"Watermark file not found at {_watermarkPath}");
            }
            _watermarkVolume = Math.Clamp(watermarkVolume, 0.1f, 1.0f);
        }

        public async Task<Stream> AddWatermarkAsync(IFormFile originalAudio)
        {
            if (!originalAudio.ContentType.Equals("audio/mpeg", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("Файл должен быть в формате MP3");
            }

            var tempInputPath = Path.GetTempFileName();
            var tempOutputPath = Path.GetTempFileName();

            try
            {
                await using (var fileStream = File.Create(tempInputPath))
                {
                    await originalAudio.CopyToAsync(fileStream);
                }

                await ProcessAudioWithWatermark(tempInputPath, tempOutputPath);

                var resultStream = new MemoryStream();
                await using (var fileStream = File.OpenRead(tempOutputPath))
                {
                    await fileStream.CopyToAsync(resultStream);
                }

                resultStream.Position = 0;
                return resultStream;
            }
            finally
            {
                if (File.Exists(tempInputPath)) File.Delete(tempInputPath);
                if (File.Exists(tempOutputPath)) File.Delete(tempOutputPath);
            }
        }

        private async Task ProcessAudioWithWatermark(string inputPath, string outputPath)
        {
            await Task.Run(() =>
            {
                // Читаем основной файл через MediaFoundationReader
                using var mainReader = new MediaFoundationReader(inputPath);
                var mainProvider = mainReader.ToSampleProvider();

                // Читаем водяной знак через MediaFoundationReader
                using var watermarkReader = new MediaFoundationReader(_watermarkPath);
                var watermarkProvider = watermarkReader.ToSampleProvider();

                // Проверяем и выравниваем форматы
                if (mainProvider.WaveFormat.SampleRate != watermarkProvider.WaveFormat.SampleRate ||
                    mainProvider.WaveFormat.Channels != watermarkProvider.WaveFormat.Channels)
                {
                    // Приводим водяной знак к формату основного трека
                    watermarkProvider = new WdlResamplingSampleProvider(
                        watermarkProvider,
                        mainProvider.WaveFormat.SampleRate)
                    .ToMono()
                    .ToStereo();
                }

                var watermarkDuration = watermarkReader.TotalTime.TotalSeconds;
                if (watermarkDuration > 5)
                {
                    throw new InvalidOperationException("Водяной знак должен быть не длиннее 5 секунд");
                }

                // Читаем весь водяной знак в память
                var watermarkBuffer = new float[(int)(watermarkDuration * mainProvider.WaveFormat.SampleRate * mainProvider.WaveFormat.Channels)];
                var watermarkSamples = watermarkProvider.Read(watermarkBuffer, 0, watermarkBuffer.Length);

                // Создаем временный WAV файл для результата
                var tempWavPath = Path.GetTempFileName();
                try
                {
                    using (var waveWriter = new WaveFileWriter(tempWavPath, mainProvider.WaveFormat))
                    {
                        var mainBuffer = new float[mainProvider.WaveFormat.SampleRate * mainProvider.WaveFormat.Channels];
                        int samplesRead;
                        double totalSeconds = 0;
                        const double interval = 20.0;
                        const double startTime = 5.0;

                        while ((samplesRead = mainProvider.Read(mainBuffer, 0, mainBuffer.Length)) > 0)
                        {
                            totalSeconds += (double)samplesRead / (mainProvider.WaveFormat.SampleRate * mainProvider.WaveFormat.Channels);

                            // Добавляем водяной знак в нужные моменты
                            if (totalSeconds >= startTime &&
                                (totalSeconds - startTime) % interval < watermarkDuration)
                            {
                                var watermarkPos = (int)(((totalSeconds - startTime) % interval) *
                                                      mainProvider.WaveFormat.SampleRate *
                                                      mainProvider.WaveFormat.Channels);

                                for (int i = 0; i < samplesRead && watermarkPos + i < watermarkSamples; i++)
                                {
                                    mainBuffer[i] = Math.Clamp(
                                        mainBuffer[i] + (watermarkBuffer[watermarkPos + i] * _watermarkVolume),
                                        -1.0f, 1.0f);
                                }
                            }

                            waveWriter.WriteSamples(mainBuffer, 0, samplesRead);
                        }
                    }

                    // Конвертируем WAV в MP3
                    using (var wavReader = new WaveFileReader(tempWavPath))
                    using (var mp3Writer = new LameMP3FileWriter(outputPath, wavReader.WaveFormat, LAMEPreset.STANDARD))
                    {
                        wavReader.CopyTo(mp3Writer);
                    }
                }
                finally
                {
                    if (File.Exists(tempWavPath)) File.Delete(tempWavPath);
                }
            });
        }
    }

    //public class WatermarkService
    //{
    //    private readonly string _watermarkPath;

    //    public WatermarkService()
    //    {
    //        _watermarkPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "audios", "watermark.mp3");
    //        if (!File.Exists(_watermarkPath))
    //        {
    //            throw new FileNotFoundException($"Watermark file not found at {_watermarkPath}");
    //        }
    //    }

    //    public async Task<Stream> AddWatermarkAsync(IFormFile originalAudio)
    //    {
    //        // Проверка формата файла
    //        if (!originalAudio.ContentType.Equals("audio/mpeg", StringComparison.OrdinalIgnoreCase))
    //        {
    //            throw new ArgumentException("Файл должен быть в формате MP3");
    //        }

    //        using var inputStream = originalAudio.OpenReadStream();
    //        var outputStream = new MemoryStream();

    //        try
    //        {
    //            // Чтение оригинального аудио
    //            using var reader = new Mp3FileReaderBase(inputStream, waveFormat => new Mp3FrameDecompressor(waveFormat));

    //            // Чтение водяного знака
    //            using var watermarkReader = new Mp3FileReaderBase(new FileStream(_watermarkPath, FileMode.Open),
    //                waveFormat => new Mp3FrameDecompressor(waveFormat));

    //            // Создание выходного MP3 потока
    //            using var writer = new LameMP3FileWriter(outputStream, reader.WaveFormat, LAMEPreset.STANDARD);

    //            var buffer = new byte[4096];
    //            int bytesRead;
    //            var position = TimeSpan.Zero;
    //            var interval = TimeSpan.FromSeconds(30);
    //            var startTime = TimeSpan.FromSeconds(5);

    //            // Обработка аудио
    //            while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
    //            {
    //                writer.Write(buffer, 0, bytesRead);
    //                position += TimeSpan.FromSeconds((double)bytesRead / reader.WaveFormat.AverageBytesPerSecond);

    //                // Добавление водяного знака через заданные интервалы
    //                if (position >= startTime && (position - startTime).TotalSeconds % interval.TotalSeconds < 1)
    //                {
    //                    watermarkReader.Position = 0;
    //                    var wmBuffer = new byte[watermarkReader.Length];
    //                    watermarkReader.Read(wmBuffer, 0, wmBuffer.Length);
    //                    writer.Write(wmBuffer, 0, wmBuffer.Length);
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            throw new Exception("Ошибка при обработке аудио с водяным знаком", ex);
    //        }

    //        outputStream.Position = 0;
    //        return outputStream;
    //    }
    //}




}
