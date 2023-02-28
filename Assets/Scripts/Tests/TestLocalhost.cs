using UnityEngine;

namespace Tests
{
    public class TestLocalhost : MonoBehaviour
    {
        async void Start()
        {
            var api = new SpeechToText();
            await api.Test_GetTextForFlac();
            await api.Test_GetTextForWav();
            Debug.Log($"END - {Time.time}");
        }
    }
}