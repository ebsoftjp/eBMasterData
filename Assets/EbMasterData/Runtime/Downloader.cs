using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace EbMasterData
{
    public abstract class Downloader<T>
    {
        public const string streamingAssetsPath = "Assets/StreamingAssets/";

        protected string url;
        protected T res;
        protected string error;
        protected System.Action<UnityWebRequest> responseAction;

        private readonly string logName;
        private CancellationTokenSource cancellationTokenSource;

        public Downloader(string url, bool isLocal)
        {
            this.url = isLocal ? ToStreamingPathWithFile(url) : url;
            logName = $"{GetType().Name}";
        }

        ~Downloader()
        {
            Cancel();
            Debug.Log($"Destroy {logName}");
        }

        // to URL
        public static string ToStreamingPath(string self)
        {
#if UNITY_IOS
            var path = $"{Application.dataPath}/Raw";
#elif UNITY_ANDROID
            var path = $"jar:file://{Application.dataPath}!/assets";
#else
            var path = Application.streamingAssetsPath;
#endif
            return $"{path}/{self}";
        }

        // to URL
        public static string ToStreamingPathWithFile(string self) => $"file://{ToStreamingPath(self)}";

        public void Cancel()
        {
            if (cancellationTokenSource != null)
            {
                Debug.Log($"Cancel {logName}");
                cancellationTokenSource.Cancel();
                cancellationTokenSource = null;
            }
        }

        public void Dispose()
        {
            if (cancellationTokenSource != null)
            {
                Debug.Log($"Dispose {logName}");
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }
        }

        public virtual async Task Request() => await Task.CompletedTask;

        public async Task<T> Get()
        {
            Cancel();
            cancellationTokenSource = new();
            var token = cancellationTokenSource.Token;

            try
            {
                await Request();
                token.ThrowIfCancellationRequested();
                Dispose();
                return res;
            }
            catch (System.Exception e)
            {
                if (e is System.OperationCanceledException)
                {
                    Debug.Log(e.Message);
                }
                else
                {
                    Debug.LogError(e.Message);
                }
                Dispose();
                error = e.Message;
                return res;
            }
        }

        public System.Action<UnityWebRequest> ResponseAction { set { responseAction = value; } }
    }

    public class DownloaderText : Downloader<string>
    {
        public DownloaderText(string id, bool isLocal) : base(id, isLocal) { }

        public override async Task Request()
        {
            Debug.Log(url);
            var request = UnityWebRequest.Get(url);
            await request.SendWebRequest();
            res = request.downloadHandler.text;
            responseAction?.Invoke(request);
            error = request.error;
        }
    }

    public class DownloaderTexture : Downloader<Texture2D>
    {
        public DownloaderTexture(string id, bool isLocal) : base(id, isLocal) { }

        public override async Task Request()
        {
            var request = UnityWebRequestTexture.GetTexture(url);
            await request.SendWebRequest();
            res = DownloadHandlerTexture.GetContent(request);
            responseAction?.Invoke(request);
            error = request.error;
        }
    }

    public class DownloaderAudioClip : Downloader<AudioClip>
    {
        public DownloaderAudioClip(string id, bool isLocal) : base(id, isLocal) { }

        public override async Task Request()
        {
            var request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG);
            //((DownloadHandlerAudioClip)request.downloadHandler).streamAudio = streaming;
            await request.SendWebRequest();
            res = DownloadHandlerAudioClip.GetContent(request);
            error = request.error;
        }
    }
}
