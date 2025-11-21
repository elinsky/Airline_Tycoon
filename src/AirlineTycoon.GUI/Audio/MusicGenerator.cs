using System;
using Microsoft.Xna.Framework.Audio;

namespace AirlineTycoon.GUI.Audio;

/// <summary>
/// Generates procedural background music in chiptune/retro style.
/// Creates simple melodies using waveforms for a nostalgic gaming atmosphere.
/// </summary>
/// <remarks>
/// This class generates loopable background music tracks:
/// - Uses simple melodies with major/minor scales
/// - Arpeggio patterns for harmonic interest
/// - Bass line for rhythm
/// - Retro 8-bit/16-bit game music aesthetic
///
/// Perfect for creating atmosphere without needing audio files.
/// </remarks>
public static class MusicGenerator
{
    private const int SampleRate = 44100; // Standard CD quality

    // Musical note frequencies (middle octave)
    private static readonly float[] NoteFrequencies = new float[]
    {
        261.63f,  // C4
        293.66f,  // D4
        329.63f,  // E4
        349.23f,  // F4
        392.00f,  // G4
        440.00f,  // A4
        493.88f,  // B4
        523.25f   // C5
    };

    /// <summary>
    /// Generates an upbeat airline/travel theme music loop.
    /// Creates a catchy, optimistic melody suitable for a tycoon game.
    /// </summary>
    /// <param name="durationSeconds">Length of the music loop in seconds.</param>
    /// <param name="volume">Volume from 0.0 to 1.0.</param>
    /// <returns>A SoundEffect containing the generated music loop.</returns>
    public static SoundEffect GenerateAirlineTheme(float durationSeconds = 30f, float volume = 0.3f)
    {
        int sampleCount = (int)(SampleRate * durationSeconds);
        byte[] audioData = new byte[sampleCount];

        // Define a simple melody pattern (using scale degrees)
        // Pattern: C-E-G-E-C-E-G-E (I-III-V-III pattern, upbeat and positive)
        int[] melodyPattern = new int[] { 0, 2, 4, 2, 0, 2, 4, 2, 4, 6, 7, 6, 4, 2, 0, 0 };
        int[] bassPattern = new int[] { 0, 0, 4, 4 }; // Simple I-V bass line

        float noteLength = durationSeconds / melodyPattern.Length;
        float bassNoteLength = durationSeconds / bassPattern.Length;

        for (int i = 0; i < sampleCount; i++)
        {
            double time = (double)i / SampleRate;

            // Melody voice (lead)
            int melodyIndex = (int)(time / noteLength) % melodyPattern.Length;
            float melodyFreq = NoteFrequencies[melodyPattern[melodyIndex]];
            double melodyValue = Math.Sin(2 * Math.PI * melodyFreq * time);

            // Add harmony (one octave higher, quieter)
            float harmonyFreq = melodyFreq * 2f;
            double harmonyValue = Math.Sin(2 * Math.PI * harmonyFreq * time) * 0.3;

            // Bass voice (lower octave)
            int bassIndex = (int)(time / bassNoteLength) % bassPattern.Length;
            float bassFreq = NoteFrequencies[bassPattern[bassIndex]] / 2f;
            double bassValue = GenerateSquareWave(bassFreq, time) * 0.4;

            // Mix voices together
            double mixedValue = (melodyValue * 0.5) + (harmonyValue * 0.2) + (bassValue * 0.3);

            // Apply envelope for smooth note transitions
            double noteProgress = (time % noteLength) / noteLength;
            double envelope = 1.0;

            // Quick attack (first 5% of note)
            if (noteProgress < 0.05)
            {
                envelope = noteProgress / 0.05;
            }
            // Decay to sustain (next 10%)
            else if (noteProgress < 0.15)
            {
                envelope = 1.0 - ((noteProgress - 0.05) / 0.10) * 0.2;
            }
            // Sustain (rest of note at 80%)
            else
            {
                envelope = 0.8;
            }

            // Apply fade out at end of loop for smooth looping
            if (i > sampleCount - 4410) // Last 0.1 seconds
            {
                double fadeOut = (double)(sampleCount - i) / 4410.0;
                envelope *= fadeOut;
            }

            // Convert to byte and clamp
            double finalValue = mixedValue * volume * envelope;
            finalValue = Math.Clamp(finalValue, -1.0, 1.0);
            audioData[i] = (byte)((finalValue + 1.0) * 127.5);
        }

        return new SoundEffect(audioData, SampleRate, AudioChannels.Mono);
    }

    /// <summary>
    /// Generates a square wave for retro bass tones.
    /// </summary>
    /// <param name="frequency">Frequency in Hz.</param>
    /// <param name="time">Current time in seconds.</param>
    /// <returns>Square wave value (-1.0 to 1.0).</returns>
    private static double GenerateSquareWave(float frequency, double time)
    {
        return Math.Sin(2 * Math.PI * frequency * time) > 0 ? 1.0 : -1.0;
    }

    /// <summary>
    /// Generates a relaxed ambient loop for menu/idle screens.
    /// </summary>
    /// <param name="durationSeconds">Length of the music loop in seconds.</param>
    /// <param name="volume">Volume from 0.0 to 1.0.</param>
    /// <returns>A SoundEffect containing the ambient music.</returns>
    public static SoundEffect GenerateAmbientLoop(float durationSeconds = 20f, float volume = 0.25f)
    {
        int sampleCount = (int)(SampleRate * durationSeconds);
        byte[] audioData = new byte[sampleCount];

        // Slow arpeggio pattern for ambient feel
        int[] pattern = new int[] { 0, 4, 7, 4, 0, 2, 4, 2 }; // C-G-C(high)-G-C-E-G-E
        float noteLength = durationSeconds / pattern.Length;

        for (int i = 0; i < sampleCount; i++)
        {
            double time = (double)i / SampleRate;

            int noteIndex = (int)(time / noteLength) % pattern.Length;
            float freq = NoteFrequencies[pattern[noteIndex]];

            // Soft sine wave for ambient feel
            double value = Math.Sin(2 * Math.PI * freq * time);

            // Very gentle envelope
            double noteProgress = (time % noteLength) / noteLength;
            double envelope = Math.Sin(noteProgress * Math.PI); // Bell curve

            // Apply fade for looping
            if (i > sampleCount - 8820) // Last 0.2 seconds
            {
                envelope *= (double)(sampleCount - i) / 8820.0;
            }

            double finalValue = value * volume * envelope;
            finalValue = Math.Clamp(finalValue, -1.0, 1.0);
            audioData[i] = (byte)((finalValue + 1.0) * 127.5);
        }

        return new SoundEffect(audioData, SampleRate, AudioChannels.Mono);
    }
}
