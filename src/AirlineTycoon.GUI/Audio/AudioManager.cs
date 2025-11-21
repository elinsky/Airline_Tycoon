using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;

namespace AirlineTycoon.GUI.Audio;

/// <summary>
/// Manages audio playback for the game, including sound effects and background music.
/// Provides a simple interface for playing retro-style chiptune sounds.
/// </summary>
/// <remarks>
/// The AudioManager handles:
/// - Loading and caching sound effects
/// - Volume control for SFX and music separately
/// - Sound effect playback with cooldown to prevent spam
/// - Mute functionality
///
/// Inspired by RollerCoaster Tycoon's simple but effective sound design:
/// - Button clicks give immediate feedback
/// - Success sounds reward player actions
/// - Alert sounds draw attention to important events
/// </remarks>
public class AudioManager
{
    private readonly Dictionary<string, SoundEffect> soundEffects;
    private readonly Dictionary<string, DateTime> lastPlayedTimes;
    private readonly TimeSpan minimumPlayInterval = TimeSpan.FromMilliseconds(50);

    private float sfxVolume = 0.7f;
    private float musicVolume = 0.5f;
    private bool isSfxMuted;
    private bool isMusicMuted;

    private SoundEffect? currentMusic;
    private SoundEffectInstance? musicInstance;

    /// <summary>
    /// Gets or sets the sound effects volume (0.0 to 1.0).
    /// </summary>
    public float SfxVolume
    {
        get => this.sfxVolume;
        set => this.sfxVolume = Math.Clamp(value, 0f, 1f);
    }

    /// <summary>
    /// Gets or sets the music volume (0.0 to 1.0).
    /// </summary>
    public float MusicVolume
    {
        get => this.musicVolume;
        set => this.musicVolume = Math.Clamp(value, 0f, 1f);
    }

    /// <summary>
    /// Gets or sets whether sound effects are muted.
    /// </summary>
    public bool IsSfxMuted
    {
        get => this.isSfxMuted;
        set => this.isSfxMuted = value;
    }

    /// <summary>
    /// Gets or sets whether music is muted.
    /// </summary>
    public bool IsMusicMuted
    {
        get => this.isMusicMuted;
        set => this.isMusicMuted = value;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioManager"/> class.
    /// </summary>
    public AudioManager()
    {
        this.soundEffects = new Dictionary<string, SoundEffect>();
        this.lastPlayedTimes = new Dictionary<string, DateTime>();
    }

    /// <summary>
    /// Loads a sound effect from the content pipeline.
    /// </summary>
    /// <param name="contentManager">The MonoGame ContentManager.</param>
    /// <param name="soundName">The name of the sound (without extension).</param>
    /// <param name="assetPath">The path to the sound asset in Content folder.</param>
    public void LoadSound(ContentManager contentManager, string soundName, string assetPath)
    {
        try
        {
            if (!this.soundEffects.ContainsKey(soundName))
            {
                var sound = contentManager.Load<SoundEffect>(assetPath);
                this.soundEffects[soundName] = sound;
                System.Diagnostics.Debug.WriteLine($"Loaded sound: {soundName} from {assetPath}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load sound {soundName}: {ex.Message}");
        }
    }

    /// <summary>
    /// Loads a procedurally generated sound effect.
    /// </summary>
    /// <param name="soundName">The name to register the sound under.</param>
    /// <param name="soundEffect">The SoundEffect instance.</param>
    public void LoadProceduralSound(string soundName, SoundEffect soundEffect)
    {
        if (!this.soundEffects.ContainsKey(soundName))
        {
            this.soundEffects[soundName] = soundEffect;
            System.Diagnostics.Debug.WriteLine($"Loaded procedural sound: {soundName}");
        }
    }

    /// <summary>
    /// Plays a sound effect by name.
    /// </summary>
    /// <param name="soundName">The name of the sound to play.</param>
    /// <param name="pitch">Pitch adjustment (-1.0 to 1.0, default 0).</param>
    /// <param name="pan">Stereo pan (-1.0 left to 1.0 right, default 0).</param>
    public void PlaySound(string soundName, float pitch = 0f, float pan = 0f)
    {
        if (this.isSfxMuted)
        {
            return;
        }

        if (!this.soundEffects.TryGetValue(soundName, out var soundEffect))
        {
            System.Diagnostics.Debug.WriteLine($"Sound not found: {soundName}");
            return;
        }

        // Prevent sound spam by checking last played time
        if (this.lastPlayedTimes.TryGetValue(soundName, out var lastPlayed))
        {
            var timeSinceLastPlayed = DateTime.Now - lastPlayed;
            if (timeSinceLastPlayed < this.minimumPlayInterval)
            {
                return; // Too soon since last play, skip
            }
        }

        try
        {
            soundEffect.Play(this.sfxVolume, pitch, pan);
            this.lastPlayedTimes[soundName] = DateTime.Now;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error playing sound {soundName}: {ex.Message}");
        }
    }

    /// <summary>
    /// Plays a button click sound.
    /// Short, crisp click for immediate feedback.
    /// </summary>
    public void PlayButtonClick()
    {
        this.PlaySound("button_click");
    }

    /// <summary>
    /// Plays a success sound for positive actions.
    /// Used when player makes money, opens routes, etc.
    /// </summary>
    public void PlaySuccess()
    {
        this.PlaySound("success");
    }

    /// <summary>
    /// Plays an error/warning sound for negative events.
    /// Used for insufficient funds, errors, warnings.
    /// </summary>
    public void PlayError()
    {
        this.PlaySound("error");
    }

    /// <summary>
    /// Plays an alert sound for important notifications.
    /// Used for events, competitor actions, etc.
    /// </summary>
    public void PlayAlert()
    {
        this.PlaySound("alert");
    }

    /// <summary>
    /// Plays a purchase sound when buying/leasing aircraft.
    /// Classic cash register "cha-ching" sound.
    /// </summary>
    public void PlayPurchase()
    {
        this.PlaySound("purchase");
    }

    /// <summary>
    /// Plays a day advance sound.
    /// Subtle tick or whoosh to mark passage of time.
    /// </summary>
    public void PlayDayAdvance()
    {
        this.PlaySound("day_advance");
    }

    /// <summary>
    /// Loads a music track for background playback.
    /// </summary>
    /// <param name="music">The SoundEffect to use as music.</param>
    public void LoadMusic(SoundEffect music)
    {
        this.StopMusic();
        this.currentMusic = music;
        System.Diagnostics.Debug.WriteLine("Background music loaded");
    }

    /// <summary>
    /// Starts playing the loaded background music in a loop.
    /// </summary>
    public void PlayMusic()
    {
        if (this.currentMusic == null || this.isMusicMuted)
        {
            return;
        }

        this.StopMusic();

        try
        {
            this.musicInstance = this.currentMusic.CreateInstance();
            this.musicInstance.IsLooped = true;
            this.musicInstance.Volume = this.musicVolume;
            this.musicInstance.Play();
            System.Diagnostics.Debug.WriteLine("Background music started");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error starting music: {ex.Message}");
        }
    }

    /// <summary>
    /// Stops the currently playing background music.
    /// </summary>
    public void StopMusic()
    {
        if (this.musicInstance != null)
        {
            this.musicInstance.Stop();
            this.musicInstance.Dispose();
            this.musicInstance = null;
        }
    }

    /// <summary>
    /// Updates the music volume if music is currently playing.
    /// </summary>
    public void UpdateMusicVolume()
    {
        if (this.musicInstance != null)
        {
            this.musicInstance.Volume = this.isMusicMuted ? 0f : this.musicVolume;
        }
    }

    /// <summary>
    /// Unloads all sound resources.
    /// Call this when disposing the AudioManager.
    /// </summary>
    public void Unload()
    {
        this.StopMusic();

        foreach (var sound in this.soundEffects.Values)
        {
            sound.Dispose();
        }
        this.soundEffects.Clear();
        this.lastPlayedTimes.Clear();

        if (this.currentMusic != null)
        {
            this.currentMusic.Dispose();
            this.currentMusic = null;
        }
    }
}
