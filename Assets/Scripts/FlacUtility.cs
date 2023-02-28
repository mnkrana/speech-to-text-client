using System.IO;
using System.Linq;
using FFmpegUnityBind2;
using UnityEngine;
using static FFmpegUnityBind2.Instructions;

public class FlacUtility : IFFmpegCallbacksHandler
{
    private System.Action<string> callback;
    private string outputPath;

    private class FlacCommand : BaseCommand
    {
        public FlacCommand(string inputPath, string outputPath) : base(inputPath, outputPath)
        {
        }

        //ffmpeg -i wav.wav temp.flac
        public override string ToString()
        {
            return
                $"{INPUT_INSTRUCTION} {InputPaths.First()} {OutputPath}";
        }
    }

    public void ConvertToFlac(string inputPath, System.Action<string> callback)
    {
        this.callback = callback;
        var dir = Path.GetDirectoryName(inputPath);
        var fileName = Path.GetFileNameWithoutExtension(inputPath);
        var time = Time.time;
        var extension = ".flac";
        outputPath = Path.Combine(dir, fileName + time + extension);
        var command = new FlacCommand(inputPath, outputPath).ToString();
        FFmpeg.Execute(command, this);
    }

    public void OnStart(long executionId)
    {
        Debug.Log($"OnStart");
    }

    public void OnLog(long executionId, string message)
    {
        Debug.Log($"OnLog");
    }

    public void OnWarning(long executionId, string message)
    {
        Debug.Log($"OnWarning");
    }

    public void OnError(long executionId, string message)
    {
        Debug.Log($"OnError");
    }

    public void OnSuccess(long executionId)
    {
        Debug.Log($"OnSuccess");
        callback(outputPath);
    }

    public void OnCanceled(long executionId)
    {
        Debug.Log($"OnCanceled");
    }

    public void OnFail(long executionId)
    {
        Debug.Log($"OnFail");
    }
}