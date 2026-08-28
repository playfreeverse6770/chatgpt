using System;
using System.Collections.Generic;
using UnityEngine;

namespace GeometricStrategy
{
    [DisallowMultipleComponent]
    public sealed class GeometricAudioService : MonoBehaviour
    {
        [Serializable]
        public sealed class CueClip
        {
            public AudioCue cue;
            public AudioClip clip;
            [Range(0f, 1f)] public float volume = 0.8f;
        }

        public static GeometricAudioService Instance { get; private set; }

        [SerializeField] private List<CueClip> clips = new List<CueClip>();
        [SerializeField, Range(0f, 1f)] private float masterVolume = 0.8f;
        [SerializeField] private bool useProceduralFallback = true;

        private readonly Dictionary<AudioCue, AudioClip> fallbackCache = new Dictionary<AudioCue, AudioClip>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            foreach (AudioClip clip in fallbackCache.Values)
                if (clip != null) Destroy(clip);
        }

        public void Play(AudioCue cue, Vector3 position)
        {
            AudioClip clip = null;
            float volume = masterVolume;

            for (int i = 0; i < clips.Count; i++)
            {
                CueClip item = clips[i];
                if (item != null && item.cue == cue && item.clip != null)
                {
                    clip = item.clip;
                    volume *= item.volume;
                    break;
                }
            }

            if (clip == null && useProceduralFallback)
                clip = GetFallback(cue);

            if (clip != null)
                AudioSource.PlayClipAtPoint(clip, position, volume);
        }

        private AudioClip GetFallback(AudioCue cue)
        {
            if (fallbackCache.TryGetValue(cue, out AudioClip cached) && cached != null)
                return cached;

            AudioClip created = ProceduralSfxFactory.Create(cue);
            fallbackCache[cue] = created;
            return created;
        }
    }

    public static class ProceduralSfxFactory
    {
        private const int SampleRate = 22050;

        public static AudioClip Create(AudioCue cue)
        {
            switch (cue)
            {
                case AudioCue.Hit: return ToneNoise("Hit", 0.12f, 120f, 0.65f, 0.45f);
                case AudioCue.ArrowShot: return Sweep("Arrow", 0.18f, 1100f, 360f, 0.5f, 0.12f);
                case AudioCue.Wolf: return Sweep("Wolf", 0.72f, 260f, 520f, 0.42f, 0.22f);
                case AudioCue.Bear: return Sweep("Bear", 0.62f, 105f, 65f, 0.55f, 0.42f);
                case AudioCue.Eagle: return Sweep("Eagle", 0.45f, 1500f, 650f, 0.32f, 0.08f);
                case AudioCue.Victory: return Arpeggio("Victory", new[] { 523.25f, 659.25f, 783.99f, 1046.5f }, 0.16f, 0.35f);
                case AudioCue.Defeat: return Arpeggio("Defeat", new[] { 392f, 329.63f, 261.63f, 196f }, 0.2f, 0.35f);
                case AudioCue.Upgrade: return Arpeggio("Upgrade", new[] { 440f, 659.25f, 880f }, 0.11f, 0.3f);
                case AudioCue.Build: return ToneNoise("Build", 0.14f, 180f, 0.55f, 0.38f);
                case AudioCue.Harvest: return ToneNoise("Harvest", 0.1f, 260f, 0.4f, 0.3f);
                case AudioCue.Coin: return Arpeggio("Coin", new[] { 1000f, 1450f }, 0.07f, 0.24f);
                default: return ToneNoise("Sfx", 0.1f, 440f, 0.3f, 0.15f);
            }
        }

        private static AudioClip ToneNoise(string name, float duration, float frequency, float toneAmount, float noiseAmount)
        {
            int samples = Mathf.CeilToInt(duration * SampleRate);
            float[] data = new float[samples];
            var random = new System.Random(name.GetHashCode());

            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)SampleRate;
                float envelope = 1f - (i / (float)samples);
                float tone = Mathf.Sin(Mathf.PI * 2f * frequency * t) * toneAmount;
                float noise = ((float)random.NextDouble() * 2f - 1f) * noiseAmount;
                data[i] = Mathf.Clamp((tone + noise) * envelope, -1f, 1f);
            }

            return Clip(name, data);
        }

        private static AudioClip Sweep(string name, float duration, float startFrequency, float endFrequency, float volume, float noiseAmount)
        {
            int samples = Mathf.CeilToInt(duration * SampleRate);
            float[] data = new float[samples];
            var random = new System.Random(name.GetHashCode());
            float phase = 0f;

            for (int i = 0; i < samples; i++)
            {
                float normalized = i / (float)Mathf.Max(1, samples - 1);
                float frequency = Mathf.Lerp(startFrequency, endFrequency, normalized);
                phase += Mathf.PI * 2f * frequency / SampleRate;
                float envelope = Mathf.Sin(Mathf.PI * normalized);
                float noise = ((float)random.NextDouble() * 2f - 1f) * noiseAmount;
                data[i] = Mathf.Clamp((Mathf.Sin(phase) * volume + noise) * envelope, -1f, 1f);
            }

            return Clip(name, data);
        }

        private static AudioClip Arpeggio(string name, float[] notes, float noteDuration, float volume)
        {
            int noteSamples = Mathf.CeilToInt(noteDuration * SampleRate);
            int samples = noteSamples * notes.Length;
            float[] data = new float[samples];

            for (int note = 0; note < notes.Length; note++)
            {
                for (int i = 0; i < noteSamples; i++)
                {
                    float t = i / (float)SampleRate;
                    float n = i / (float)Mathf.Max(1, noteSamples - 1);
                    float envelope = Mathf.Sin(Mathf.PI * n);
                    data[note * noteSamples + i] = Mathf.Sin(Mathf.PI * 2f * notes[note] * t) * volume * envelope;
                }
            }

            return Clip(name, data);
        }

        private static AudioClip Clip(string name, float[] data)
        {
            AudioClip clip = AudioClip.Create("Procedural_" + name, data.Length, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
