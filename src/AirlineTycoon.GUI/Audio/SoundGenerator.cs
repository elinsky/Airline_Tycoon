using Microsoft.Xna.Framework.Audio;
using System;

namespace AirlineTycoon.GUI.Audio;

/// <summary>
/// Generates simple procedural sound effects programmatically.
/// Creates retro-style beep and tone sounds without needing audio files.
/// </summary>
/// <remarks>
/// This class generates sounds using mathematical waveforms:
/// - Square waves for retro beeps (like old computers/NES)
/// - Sine waves for smoother tones
/// - Combined waveforms for more complex sounds
///
/// Useful for prototyping or when you don't have audio assets yet.
/// For production, real recorded/composed sounds are recommended.
/// </remarks>
public static class SoundGenerator
{
    private const int SampleRate = 44100; // Standard CD quality

    /// <summary>
    /// Generates a simple beep sound (square wave).
    /// </summary>
    /// <param name="frequency">Frequency in Hz (e.g., 440 for middle A).</param>
    /// <param name="durationSeconds">Duration in seconds.</param>
    /// <param name="volume">Volume from 0.0 to 1.0.</param>
    /// <returns>A SoundEffect containing the generated audio.</returns>
    public static SoundEffect GenerateBeep(float frequency, float durationSeconds, float volume = 0.5f)
    {
        int sampleCount = (int)(SampleRate * durationSeconds);
        byte[] audioData = new byte[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            // Generate square wave (classic retro beep sound)
            double time = (double)i / SampleRate;
            double value = Math.Sin(2 * Math.PI * frequency * time) > 0 ? 1.0 : -1.0;

            // Apply volume and envelope (fade out to prevent click)
            double envelope = 1.0;
            if (i > sampleCount - 1000) // Fade out last 1000 samples
            {
                envelope = (double)(sampleCount - i) / 1000.0;
            }

            // Convert to byte (0-255 range)
            audioData[i] = (byte)((value * volume * envelope + 1.0) * 127.5);
        }

        return new SoundEffect(audioData, SampleRate, AudioChannels.Mono);
    }

    /// <summary>
    /// Generates a smoother tone using a sine wave.
    /// </summary>
    /// <param name="frequency">Frequency in Hz.</param>
    /// <param name="durationSeconds">Duration in seconds.</param>
    /// <param name="volume">Volume from 0.0 to 1.0.</param>
    /// <returns>A SoundEffect containing the generated audio.</returns>
    public static SoundEffect GenerateTone(float frequency, float durationSeconds, float volume = 0.5f)
    {
        int sampleCount = (int)(SampleRate * durationSeconds);
        byte[] audioData = new byte[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            // Generate sine wave (smooth tone)
            double time = (double)i / SampleRate;
            double value = Math.Sin(2 * Math.PI * frequency * time);

            // Apply volume and envelope
            double envelope = 1.0;
            if (i < 100) // Fade in first 100 samples
            {
                envelope = (double)i / 100.0;
            }
            else if (i > sampleCount - 500) // Fade out last 500 samples
            {
                envelope = (double)(sampleCount - i) / 500.0;
            }

            // Convert to byte
            audioData[i] = (byte)((value * volume * envelope + 1.0) * 127.5);
        }

        return new SoundEffect(audioData, SampleRate, AudioChannels.Mono);
    }

    /// <summary>
    /// Generates a click sound (very short beep).
    /// Perfect for button clicks.
    /// </summary>
    /// <returns>A SoundEffect for a button click.</returns>
    public static SoundEffect GenerateClick()
    {
        return GenerateBeep(800f, 0.05f, 0.3f); // 800 Hz, 50ms, 30% volume
    }

    /// <summary>
    /// Generates a success/positive sound (ascending tones).
    /// </summary>
    /// <returns>A SoundEffect for success actions.</returns>
    public static SoundEffect GenerateSuccess()
    {
        return GenerateTone(600f, 0.15f, 0.4f); // Pleasant mid-tone
    }

    /// <summary>
    /// Generates an error/negative sound (descending tone).
    /// </summary>
    /// <returns>A SoundEffect for errors or warnings.</returns>
    public static SoundEffect GenerateError()
    {
        return GenerateBeep(200f, 0.2f, 0.4f); // Low buzz
    }

    /// <summary>
    /// Generates an alert/notification sound.
    /// </summary>
    /// <returns>A SoundEffect for alerts.</returns>
    public static SoundEffect GenerateAlert()
    {
        return GenerateBeep(1000f, 0.1f, 0.35f); // High pitched beep
    }

    /// <summary>
    /// Generates a purchase/cash register sound.
    /// </summary>
    /// <returns>A SoundEffect for purchases.</returns>
    public static SoundEffect GeneratePurchase()
    {
        return GenerateTone(750f, 0.12f, 0.4f); // Pleasant higher tone
    }

    /// <summary>
    /// Generates a subtle tick sound for day advancement.
    /// </summary>
    /// <returns>A SoundEffect for day advances.</returns>
    public static SoundEffect GenerateDayAdvance()
    {
        return GenerateBeep(400f, 0.08f, 0.25f); // Subtle low click
    }
}
