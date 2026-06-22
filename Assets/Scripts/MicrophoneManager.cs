using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class MicrophoneManager : MonoBehaviour
{
   public enum Bands
   {
       Eight = 8,
       SixtyFour = 64,
   }


   [SerializeField] private RunWhisperTiny runWhisper;
   [SerializeField] private int microphoneId = 0; // Index in Microphone.devices
   [SerializeField] private int recordingDuration = 10; // seconds
   [SerializeField] private int sampleRate = 16000; // Hz


   public bool IsRecording => m_isRecording;
   private bool m_isRecording = false;


   [Header("Audio Components")]
   [SerializeField] private AudioSource audioSource;
   [SerializeField] private LineRenderer lineRenderer;


   [Header("Spectrum Settings")]
   [SerializeField] private int frequencyBins = 512;
   private float[] samples;
   private float[] sampleBuffer;


   [Header("Visualization Settings")]
   public bool enableVisualization = true;
   public Bands bandsToVisualize = Bands.SixtyFour;
   public float scalar = 100f;
   public float visualizerWidth = 10f;
   public float visualizerYBase = 2f;
   public Color lineColor = Color.white;
   public float lineWidth = 0.05f;


   [Header("Audio Processing")]
   public float smoothDownRate = 10f;


   private float[] freqBands8;
   private float[] freqBands64;


   public IReadOnlyList<float> FreqBands8 => freqBands8;


   void Awake()
   {
       audioSource = GetComponent<AudioSource>();
       if (audioSource == null)
       {
           Debug.LogError("MicrophoneManager requires an AudioSource component!");
       }


       if (runWhisper == null)
       {
           Debug.LogWarning("MicrophoneManager reference is not set.");
       }
   }


   void Start()
   {
       freqBands8 = new float[8];
       freqBands64 = new float[64];
       samples = new float[frequencyBins];
       sampleBuffer = new float[frequencyBins];


       SetupLineRenderer();
   }


   #region Frequency band
   void SetupLineRenderer()
   {
       int bands = bandsToVisualize == Bands.Eight ? freqBands8.Length : freqBands64.Length;
       lineRenderer.positionCount = bands;
       lineRenderer.startWidth = lineWidth;
       lineRenderer.endWidth = lineWidth;
       lineRenderer.startColor = lineColor;
       lineRenderer.endColor = lineColor;
       lineRenderer.useWorldSpace = false;


       enableVisualization = false;
   }


    // Update 함수 수정
    void Update()
    {
        // 3프레임에 한 번만 실행되도록 제한
        if (Time.frameCount % 3 != 0) return;

        if (m_isRecording && audioSource.isPlaying && audioSource.clip != null)
        {
            UpdateFrequencyBand();
        }
    }


    private void UpdateFrequencyBand()
   {
       audioSource.GetSpectrumData(sampleBuffer, 0, FFTWindow.BlackmanHarris);


       for (int i = 0; i < samples.Length; i++)
       {
           samples[i] = sampleBuffer[i] > samples[i]
               ? sampleBuffer[i]
               : Mathf.Lerp(samples[i], sampleBuffer[i], Time.deltaTime * smoothDownRate);
       }


       UpdateFreqBands8();
       UpdateFreqBands64();


       if (enableVisualization)
       {
           lineRenderer.enabled = true;
           DrawVisualizer();
       }
       else
       {
           lineRenderer.enabled = false;
       }
   }


   void DrawVisualizer()
   {
       float[] bands = bandsToVisualize == Bands.Eight ? freqBands8 : freqBands64;
       int bandCount = bands.Length;


       lineRenderer.positionCount = bandCount;
       lineRenderer.startWidth = lineWidth;
       lineRenderer.endWidth = lineWidth;
       lineRenderer.startColor = lineColor;
       lineRenderer.endColor = lineColor;


       Vector3[] positions = new Vector3[bandCount];
       for (int i = 0; i < bandCount; i++)
       {
           float xPos = (float)i / (bandCount - 1) * visualizerWidth;
           float yPos = visualizerYBase + bands[i] * scalar;
           positions[i] = new Vector3(xPos, yPos, 0);
       }
       lineRenderer.SetPositions(positions);
   }


   void UpdateFreqBands8()
   {
       int count = 0;
       for (int i = 0; i < 8; i++)
       {
           float average = 0;
           int sampleCount = (int)Mathf.Pow(2, i) * 2;
           if (i == 7) sampleCount += 2;


           for (int j = 0; j < sampleCount && count < samples.Length; j++, count++)
               average += samples[count] * (count + 1);


           average /= count > 0 ? count : 1;
           freqBands8[i] = average;
       }
   }


   void UpdateFreqBands64()
   {
       int count = 0;
       int sampleCount = 1;
       int power = 0;


       for (int i = 0; i < 64; i++)
       {
           float average = 0;


           if (i == 16 || i == 32 || i == 40 || i == 48 || i == 56)
           {
               power++;
               sampleCount = (int)Mathf.Pow(2, power);
               if (power == 3) sampleCount -= 2;
           }


           for (int j = 0; j < sampleCount && count < samples.Length; j++, count++)
               average += samples[count] * (count + 1);


           average /= count > 0 ? count : 1;
           freqBands64[i] = average;
       }
   }
   #endregion


   /// <summary>
   /// Starts recording from the selected microphone.
   /// </summary>
   public void StartRecording()
   {
#if UNITY_WEBGL && !UNITY_EDITOR
       Debug.LogWarning("Microphone recording is not supported in WebGL builds.");
       m_isRecording = false;
       enableVisualization = false;
       lineRenderer.enabled = false;
       return;
#else
       if (m_isRecording)
       {
           Debug.LogWarning("Already recording.");
           return;
       }


       if (Microphone.devices == null || Microphone.devices.Length == 0)
       {
           Debug.LogError("No microphone devices found.");
           return;
       }


       if (microphoneId < 0 || microphoneId >= Microphone.devices.Length)
       {
           Debug.LogError($"Microphone ID {microphoneId} is out of range. Available: {Microphone.devices.Length} devices.");
           return;
       }


       string micDevice = Microphone.devices[microphoneId];
       if (string.IsNullOrEmpty(micDevice))
       {
           Debug.LogError($"Microphone at index {microphoneId} is not available.");
           return;
       }


       audioSource.Stop();
       audioSource.clip = null;


       audioSource.clip = Microphone.Start(micDevice, true, recordingDuration, sampleRate);


       audioSource.loop = true;
       // audioSource.mute = true; // optional


       StartCoroutine(WaitForMicrophoneStart(micDevice));


       enableVisualization = true;
       lineRenderer.enabled = false;


       m_isRecording = true;
#endif
   }


#if !UNITY_WEBGL || UNITY_EDITOR
   private IEnumerator WaitForMicrophoneStart(string micDevice)
   {
       float initTime = Time.time;
       while (!(Microphone.GetPosition(micDevice) > 0))
       {
           if (Time.time - initTime > 2f)
           {
               Debug.LogError("Microphone failed to start recording in time.");
               m_isRecording = false;
               yield break;
           }
           yield return null;
       }


       audioSource.Play();
       Debug.Log("Microphone recording started and playback begun.");
   }
#endif


   /// <summary>
   /// Stops recording and processes the audio with Whisper.
   /// </summary>
   public void StopRecording()
   {
#if UNITY_WEBGL && !UNITY_EDITOR
       Debug.LogWarning("Microphone recording is not supported in WebGL builds.");
       m_isRecording = false;
       enableVisualization = false;
       lineRenderer.enabled = false;
       return;
#else
       if (!m_isRecording)
       {
           Debug.LogWarning("Not currently recording.");
           return;
       }


       m_isRecording = false;


       if (Microphone.devices == null || Microphone.devices.Length == 0)
       {
           Debug.LogError("No microphone devices found.");
           return;
       }


       if (microphoneId < 0 || microphoneId >= Microphone.devices.Length)
       {
           Debug.LogError($"Microphone ID {microphoneId} is out of range.");
           return;
       }


       string micDevice = Microphone.devices[microphoneId];
       Microphone.End(micDevice);


       if (audioSource.clip == null)
       {
           Debug.LogError("No audio clip recorded.");
           return;
       }


       audioSource.Stop();


       enableVisualization = false;
       lineRenderer.enabled = false;


       if (runWhisper != null)
       {
           runWhisper.RunWhisper(audioSource.clip);
       }
       else
       {
           Debug.LogWarning("RunWhisperTiny reference is missing.");
       }
#endif
   }
}
