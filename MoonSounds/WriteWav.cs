using System.Text;

namespace MoonSounds;

/// <summary>
/// Copied from FuncSynth
/// </summary>

public class WriteWav
{
    public static byte[] WriteWavFile(float[] samples, int sampleRate)
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);

        // Write the WAV header
        WriteHeader(writer, samples.Length, sampleRate);

        // Write the audio data
        foreach (var sample in samples)
        {
            // Convert from float [-1, 1] to 32-bit signed int
            var intSample = (int) (sample * int.MaxValue);
            writer.Write(sample);
        }

        writer.Flush();
        return ms.ToArray();
    }

    private static void WriteHeader(BinaryWriter writer, int sampleCount, int sampleRate)
    {
        var byteRate = sampleRate * 4; // 4 bytes per sample (32-bit)
        var dataChunkSize = sampleCount * 4; // Number of samples * 4 bytes
        var fileSize = 36 + dataChunkSize;

        // RIFF header
        writer.Write(Encoding.ASCII.GetBytes("RIFF"));
        writer.Write(fileSize);
        writer.Write(Encoding.ASCII.GetBytes("WAVE"));

        // Format chunk
        writer.Write(Encoding.ASCII.GetBytes("fmt "));

        // Subchunk1 size (16 for PCM)
        writer.Write(16);

        // Audio format (3 for IEEE float)
        writer.Write((short) 3);

        // Number of channels (1 for mono)
        writer.Write((short) 1);

        writer.Write(sampleRate);

        // Byte rate (Sample Rate * Num Channels * Bits per Sample / 8)
        writer.Write(byteRate);

        // Block align (Num Channels * Bits per Sample / 8)
        writer.Write((short) 4);

        // Bits per sample (32 for float)
        writer.Write((short) 32);

        // Data chunk
        writer.Write(Encoding.ASCII.GetBytes("data"));
        writer.Write(dataChunkSize);
    }
}
