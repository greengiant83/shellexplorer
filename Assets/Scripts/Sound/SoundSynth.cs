using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum WaveFormType
{
    Sine,
    Sawtooth,
    Square,
    Noise
}

public class SoundSynth
{
    int sampleRate;
    Dictionary<int, AudioClip> notes = new Dictionary<int, AudioClip>();

    public float NoteDuration = 1.0f;
    public float AttackPercentage = 0.2f;
    public float DecayPercentage = 0.2f;
    public float Volume = 1.0f;

    float AttackDuration { get { return NoteDuration * AttackPercentage; } }
    float DecayDuration { get { return NoteDuration * DecayPercentage; } }

    private WaveFormType _waveForm = WaveFormType.Sawtooth;
    public WaveFormType WaveForm
    {
        get { return _waveForm;  }
        set
        {
            _waveForm = value;
            notes.Clear();
        }
    }
    

    public SoundSynth()
    {
        WaveForm = WaveFormType.Sine;
        sampleRate = AudioSettings.outputSampleRate;
    }

    public void PlayNote(AudioSource Speaker, int Note)
    {
        AudioClip clip;
        if (true || !notes.ContainsKey(Note))
        {
            clip = CreateAudioClip(GetNoteFrequency(Note));
            //notes.Add(Note, clip);
        }
        else clip = notes[Note];
        
        Speaker.PlayOneShot(clip);
        //PlayClipAt(clip, Speaker.transform.position);
    }

    AudioSource PlayClipAt(AudioClip clip, Vector3 pos)
    {
        var tempGO = new GameObject("TempAudio"); // create the temp object
        tempGO.transform.position = pos; // set its position
        var aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = clip; // define the clip
        aSource.dopplerLevel = 0;
        aSource.volume = Volume;
        aSource.Play(); // start the sound
        GameObject.Destroy(tempGO, clip.length); // destroy object after clip duration
        return aSource; // return the AudioSource reference
    }

    public float GetNoteFrequency(int note)
    {
        //Middle C = 60 to jive with midi
        int cOffset = note - 60;
        float baseFrequency = 256;
        float frequency = baseFrequency * Mathf.Pow(Mathf.Pow(2, 1.0f / 12.0f), cOffset);

        return frequency;
    }

    public AudioClip CreateAudioClip(float frequency)
    {
        Debug.Log("Frequency: " + frequency);
        int sampleCount = (int)(sampleRate * NoteDuration);
        var clip = AudioClip.Create("Test Sine Wave", sampleCount, 1, sampleRate, true, false);

        float increment = frequency * 2 * Mathf.PI / sampleRate;
        float[] data = new float[sampleCount];
        float phase = 0;
        float time = 0;
        float timeIncrement = NoteDuration / sampleRate;

        for (var i = 0; i < sampleCount; i++)
        {
            time += timeIncrement;
            float volume = getVolume(time);
            float waveValue = getWavePosition(i, frequency);

            phase = phase + increment;
            data[i] = volume * waveValue;
            if (phase > 2 * Mathf.PI) phase = 0;
        }

        clip.SetData(data, 0);

        return clip;
    }

    private float getWavePosition(float position, float frequency)
    {
        //switch (_waveForm)
        //{
        //    case WaveFormType.Sine:
        //        return Mathf.Sin(position);
        //    case WaveFormType.Sawtooth:
        //        return Mathf.PingPong(position, 1.0f);
        //    default:
        //        throw new Exception("Unrecognized wave form type");
        //}

        switch (_waveForm)
        {
            case WaveFormType.Sine:
                return Mathf.Sin(2 * Mathf.PI * frequency * position / sampleRate); 
            case WaveFormType.Sawtooth:
                return (Mathf.PingPong(frequency * position / sampleRate, 0.5f)).Remap(0, 0.5f, -1, 1);
            case WaveFormType.Square:
                return Mathf.Sign(Mathf.Sin(2 * Mathf.PI * frequency * position / sampleRate)) * 1f;
            case WaveFormType.Noise:
                return (Mathf.PerlinNoise(2 * Mathf.PI * frequency * position / sampleRate, 0) * 2 - 1) * 2; 
            default:
                throw new Exception("Unrecognized wave form type");
        }
        
    }

    private float getVolume(float time)
    {
        if (time < AttackDuration)
            return Mathf.Lerp(0, 1, time / AttackDuration);
        else if (time > NoteDuration - DecayDuration)
            return Mathf.Lerp(0, 1, (NoteDuration - time) / DecayDuration);
        else
            return 1.0f;
    }
}
