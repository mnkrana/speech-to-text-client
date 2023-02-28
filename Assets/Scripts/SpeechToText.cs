using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class SpeechToText
{
    // private const string urlFlac = "http://localhost:8000/flac";
    // private const string urlWav = "http://localhost:8000/wav";
    // private const string urlWavPost = "http://localhost:8000/wav_post";
    // private const string urlFlacPost = "http://localhost:8000/flac_post";

    private const string urlFlac = "http://65.0.5.96/flac";
    private const string urlWav = "http://65.0.5.96/wav";
    private const string urlWavPost = "http://65.0.5.96/wav_post";
    private const string urlFlacPost = "http://65.0.5.96/flac_post";

    public static float timeTaken = 0;
    public SpeechToText()
    {
    }

    private class AudioSchema
    {
        public string audio_data;
    }

    public async Task<string> GetTextForWav(byte[] audioData)
    {
        var startTime = Time.time;

        var audioSchema = new AudioSchema
        {
            audio_data = System.Convert.ToBase64String(audioData)
        };
        var json = JsonUtility.ToJson(audioSchema);

        var req = UnityWebRequest.Put(urlWavPost, json);

        req.SetRequestHeader("accept", "application/json; charset=UTF-8");
        req.SetRequestHeader("content-type", "application/json; charset=UTF-8");

        await req.SendWebRequest();

        var result = req.downloadHandler;
        Debug.Log($"Response wav time - {Time.time - startTime}");
        return result.text;
    }

    public async Task<string> GetTextForFlac(byte[] audioData)
    {
        var startTime = Time.time;

        var audioSchema = new AudioSchema
        {
            audio_data = System.Convert.ToBase64String(audioData)
        };
        var json = JsonUtility.ToJson(audioSchema);

        var req = UnityWebRequest.Put(urlFlacPost, json);

        req.SetRequestHeader("accept", "application/json; charset=UTF-8");
        req.SetRequestHeader("content-type", "application/json; charset=UTF-8");

        await req.SendWebRequest();

        var result = req.downloadHandler;
        Debug.Log($"Response flac time - {Time.time - startTime}");
        timeTaken = Time.time - startTime;
        return result.text;
    }

    public async Task Test_GetTextForFlac()
    {
        Debug.Log($"FLAC - {Time.time}");
        UnityWebRequest req = UnityWebRequest.Get(urlFlac);
        await req.SendWebRequest();
        var result = req.downloadHandler;
        Debug.Log(result.text);
    }

    public async Task Test_GetTextForWav()
    {
        Debug.Log($"WAV - {Time.time}");
        UnityWebRequest req = UnityWebRequest.Get(urlWav);
        await req.SendWebRequest();
        var result = req.downloadHandler;
        Debug.Log(result.text);
    }
}