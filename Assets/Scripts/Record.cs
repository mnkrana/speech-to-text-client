using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

[RequireComponent(typeof(AudioSource))]
public class Record : MonoBehaviour
{
    [Serializable]
    public enum AudioFormat
    {
        Wav,
        Flac
    }

    [SerializeField] private AudioFormat audioFormat;
    [SerializeField] private Text debugText;

    private AudioSource audioSource;

    private float startRecordingTime;

    private SpeechToText speechToText;
    private FlacUtility flacUtility;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        speechToText = new SpeechToText();
        flacUtility = new FlacUtility();
    }

    private IEnumerator Start()
    {
        yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
        Debug.Log(Application.HasUserAuthorization(UserAuthorization.Microphone)
            ? "Mic Permission granted!"
            : "Mic Permission needed!");
    }

    public void StartMic()
    {
        debugText.text = "Start Mic";
        audioSource.clip = Microphone.Start(Microphone.devices[0], true, 60, 44100);
        startRecordingTime = Time.time;
    }

    public void StopMic()
    {
        debugText.text = "Stop Mic";
        Microphone.End(Microphone.devices[0]);
        switch (audioFormat)
        {
            case AudioFormat.Wav:
                ProcessRecordingWav();
                break;
            case AudioFormat.Flac:
                ProcessRecordingFlac();
                break;
        }
    }

    private async void ProcessRecordingWav()
    {
        var trimmedClip = TrimRecording(audioSource.clip);
        var audioData = SaveWavFile(trimmedClip);
        var respose = await speechToText.GetTextForWav(audioData);
        Debug.Log(respose);
    }

    private async void ProcessRecordingFlac()
    {
        var trimmedClip = TrimRecording(audioSource.clip);
        var fileName = SaveWav(trimmedClip);
        var flacFilePath = await SaveFlacFile(fileName);
        var audioData = System.IO.File.ReadAllBytes(flacFilePath);
        var respose = await speechToText.GetTextForFlac(audioData);
        debugText.text = respose + "\n" + SpeechToText.timeTaken;
    }

    private AudioClip TrimRecording(AudioClip clip)
    {
        var length = Time.time - startRecordingTime;
        var trimmedRecording = AudioClip.Create(clip.name, ((int) (length) * clip.frequency),
            clip.channels, clip.frequency, false);
        var data = new float[(int) ((Time.time - startRecordingTime) * clip.frequency)];
        clip.GetData(data, 0);
        trimmedRecording.SetData(data, 0);
        return trimmedRecording;
    }

    private byte[] SaveWavFile(AudioClip ac)
    {
        var data = WavUtility.FromAudioClip(ac, out var filepath);
        debugText.text = "File : " + filepath;
        return data;
    }

    private string SaveWav(AudioClip ac)
    {
        WavUtility.FromAudioClip(ac, out var filepath);
        return filepath;
    }

    private async Task<string> SaveFlacFile(string fileName)
    {
        var flacPath = string.Empty;
        flacUtility.ConvertToFlac(fileName, (path) => flacPath = path);
        while (flacPath == String.Empty)
        {
            await Task.Delay(100);
        }

        return flacPath;
    }
}