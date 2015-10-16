using UnityEngine;
using System.Collections;

public class DerivativeTracker
{
    float currentValue = 0;
    float lastValue = 0;
    public float DerivativeValue;

    public void Update(float value)
    {
        currentValue = (value + currentValue) / 2f;
        
        DerivativeValue = currentValue - lastValue;

        lastValue = currentValue;
    }
}

public class MusicThing : MonoBehaviour 
{
    public Hand LeftHand;
    public Hand RightHand;
    public Transform Speaker;

    AudioSource speakerAudio;

    DerivativeTracker handDistanceVelocity = new DerivativeTracker();
    float baseFrequency;

	void Start () 
    {
        speakerAudio = Speaker.GetComponent<AudioSource>();

        baseFrequency = getNoteFrequency(60);
        speakerAudio.clip = createAudioClip(baseFrequency);
        speakerAudio.loop = true;
        speakerAudio.Play();
	}

	void FixedUpdate () 
    {
        //Update volume
        var handDelta = LeftHand.transform.position - RightHand.transform.position;
        //print("hand distance: " + handDelta.magnitude);
        handDistanceVelocity.Update(handDelta.magnitude);

        float handSpeed = Mathf.Abs(handDistanceVelocity.DerivativeValue);
        float minSpeed = .0f;
        float maxSpeed = 0.05f;
        float volume = handSpeed.Remap(minSpeed, maxSpeed, 0f, 1f, true);

        //speakerAudio.volume = volume;
        speakerAudio.volume = handDelta.magnitude.Remap(0.1f, 1f, 0, 1, true);

        //Update speaker position;
        var middlePoint = (LeftHand.transform.position + RightHand.transform.position) / 2f;
        Speaker.position = new Vector3(0, middlePoint.y, 0);
        Speaker.localScale = Vector3.one * speakerAudio.volume.Remap(0, 1, 0.01f, 0.2f, true);
    }

    public void SetNote(int Note)
    {
        float pitch = getNoteFrequency(Note);
        float pitchScalar = pitch / baseFrequency;
        speakerAudio.pitch = pitchScalar;
    }

    #region -- Helper Methods --
    AudioClip createAudioClip(float frequency)
    {
        int sampleRate = AudioSettings.outputSampleRate;
        float NoteDuration = 1f / frequency;
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
            float volume = 1;
            float waveValue = getWavePosition(i, frequency, sampleRate);

            

            phase = phase + increment;
            data[i] = volume * waveValue;

            if (phase > 2 * Mathf.PI) phase = 0;
        }

        clip.SetData(data, 0);
        return clip;
    }

    float getNoteFrequency(int note)
    {
        //Middle C = 60 to jive with midi
        int cOffset = note - 60;
        float baseFrequency = 256;
        float frequency = baseFrequency * Mathf.Pow(Mathf.Pow(2, 1.0f / 12.0f), cOffset);

        return frequency;
    }

    float getWavePosition(float position, float frequency, float sampleRate)
    {
        return Mathf.Sin(2 * Mathf.PI * frequency * position / sampleRate);
    }
    #endregion
}
