using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEngine;

#pragma warning disable CS1591, IDE1006
namespace AudioLibrary
{
    public class Manager : MonoBehaviour
    {
        class AudioInstance
        {
            public AudioClip audioClip;
            public AudioFileReader reader;
            public float[] dataToSet;
            public int samplesCount;
            public Stream streamToDisposeOnceDone;

            public int channels => reader.WaveFormat.Channels;
            public int sampleRate => reader.WaveFormat.SampleRate;
            public static implicit operator AudioClip(AudioInstance ai) => ai.audioClip;
        }

        static readonly string[] supportedFormats;
        static Dictionary<string, AudioClip> cache;
        static Queue<AudioInstance> deferredLoadQueue;
        static Queue<AudioInstance> deferredSetDataQueue;
        static Queue<AudioInstance> deferredSetFail;
        static Thread deferredLoaderThread;
        static GameObject managerInstance;
        static Dictionary<AudioClip, AudioClipLoadType> audioClipLoadType;
        static Dictionary<AudioClip, AudioDataLoadState> audioLoadState;

        static Manager()
        {
            cache = new Dictionary<string, AudioClip>();
            deferredLoadQueue = new Queue<AudioInstance>();
            deferredSetDataQueue = new Queue<AudioInstance>();
            deferredSetFail = new Queue<AudioInstance>();
            audioClipLoadType = new Dictionary<AudioClip, AudioClipLoadType>();
            audioLoadState = new Dictionary<AudioClip, AudioDataLoadState>();
            supportedFormats = Enum.GetNames(typeof(AudioFormat));
        }

        public static AudioClip Load(string filePath, bool doStream = false, bool loadInBackground = true, bool useCache = true)
        {
            if (!IsSupportedFormat(filePath))
            {
                Console.WriteLine($"Could not load AudioClip at path '{ filePath }' it's extensions marks unsupported format, supported formats are: {string.Join(", ", Enum.GetNames(typeof(AudioFormat)))}");
                return null;
            }
            if (useCache && cache.TryGetValue(filePath, out AudioClip audioClip) && audioClip)
            {
                return audioClip;
            }
            using (StreamReader streamReader = new StreamReader(filePath))
            {
                audioClip = Load(streamReader.BaseStream, GetAudioFormat(filePath), filePath, doStream, loadInBackground, true);
                if (useCache) cache[filePath] = audioClip;
                return audioClip;
            }
        }

        public static AudioClip Load(Stream dataStream, AudioFormat audioFormat, string unityAudioClipName, bool doStream = false, bool loadInBackground = true, bool diposeDataStreamIfNotNeeded = true)
        {
            AudioClip audioClip = null;
            AudioFileReader reader = null;
            try
            {
                reader = new AudioFileReader(dataStream, audioFormat);
                AudioInstance audioInstance = new AudioInstance
                {
                    reader = reader,
                    samplesCount = (int)(reader.Length / (reader.WaveFormat.BitsPerSample / 8))
                };
                if (doStream)
                {
                    audioClip = AudioClip.Create(unityAudioClipName, audioInstance.samplesCount / audioInstance.channels, audioInstance.channels, audioInstance.sampleRate, doStream,
                        (float[] target) => { reader.Read(target, 0, target.Length); },
                        (int target) => { reader.Seek(target * (audioInstance.channels == 1 ? 4 : 8), SeekOrigin.Begin);
                        });

                    audioInstance.audioClip = audioClip;
                    SetAudioClipLoadType(audioInstance, AudioClipLoadType.Streaming);
                    SetAudioClipLoadState(audioInstance, AudioDataLoadState.Loaded);
                }
                else
                {
                    audioClip = AudioClip.Create(unityAudioClipName, audioInstance.samplesCount / audioInstance.channels, audioInstance.channels, audioInstance.sampleRate, doStream);
                    audioInstance.audioClip = audioClip;

                    if (diposeDataStreamIfNotNeeded) audioInstance.streamToDisposeOnceDone = dataStream;
                    SetAudioClipLoadType(audioInstance, AudioClipLoadType.DecompressOnLoad);
                    SetAudioClipLoadState(audioInstance, AudioDataLoadState.Loading);
                    if (loadInBackground)
                    {
                        object obj = deferredLoadQueue;
                        lock (obj) deferredLoadQueue.Enqueue(audioInstance);

                        RunDeferredLoaderThread();
                        EnsureInstanceExists();
                    }
                    else
                    {
                        audioInstance.dataToSet = new float[audioInstance.samplesCount];
                        audioInstance.reader.Read(audioInstance.dataToSet, 0, audioInstance.dataToSet.Length);
                        audioInstance.audioClip.SetData(audioInstance.dataToSet, 0);
                        SetAudioClipLoadState(audioInstance, AudioDataLoadState.Loaded);
                    }
                }
            }
            catch (Exception ex)
            {
                MSCLoader.ModConsole.LogError($"{unityAudioClipName} - Failed: {ex.Message}");
                Console.WriteLine($"Could not load AudioClip named '{unityAudioClipName}'\n{ex}");
            }

            return audioClip;
        }

        static void RunDeferredLoaderThread()
        {
            if (deferredLoaderThread == null || !deferredLoaderThread.IsAlive)
            {
                deferredLoaderThread = new Thread(new ThreadStart(DeferredLoaderMain)) { IsBackground = true };
                deferredLoaderThread.Start();
            }
        }

        static void DeferredLoaderMain()
        {
            AudioInstance audioInstance = null;
            bool flag = true;
            long num = 100000L;

            while (flag || num > 0L)
            {
                num -= 1L;
                object obj = deferredLoadQueue;
                lock (obj)
                {
                    flag = (deferredLoadQueue.Count > 0);
                    if (!flag) continue;
                    audioInstance = deferredLoadQueue.Dequeue();
                }

                num = 100000L;

                try
                {
                    audioInstance.dataToSet = new float[audioInstance.samplesCount];
                    audioInstance.reader.Read(audioInstance.dataToSet, 0, audioInstance.dataToSet.Length);
                    audioInstance.reader.Close();
                    audioInstance.reader.Dispose();

                    if (audioInstance.streamToDisposeOnceDone != null)
                    {
                        audioInstance.streamToDisposeOnceDone.Close();
                        audioInstance.streamToDisposeOnceDone.Dispose();
                        audioInstance.streamToDisposeOnceDone = null;
                    }

                    object obj2 = deferredSetDataQueue;
                    lock (obj2) deferredSetDataQueue.Enqueue(audioInstance);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                    object obj3 = deferredSetFail;
                    lock (obj3) deferredSetFail.Enqueue(audioInstance);
                }
            }
        }

        void Update()
        {
            AudioInstance audioInstance = null;
            bool flag = true;

            while (flag)
            {
                object obj = deferredSetDataQueue;
                lock (obj)
                {
                    flag = (deferredSetDataQueue.Count > 0);
                    if (!flag) break;
                    audioInstance = deferredSetDataQueue.Dequeue();
                }

                audioInstance.audioClip.SetData(audioInstance.dataToSet, 0);
                SetAudioClipLoadState(audioInstance, AudioDataLoadState.Loaded);
                audioInstance.audioClip = null;
                audioInstance.dataToSet = null;
            }

            object obj2 = deferredSetFail;

            lock (obj2)
            {
                while (deferredSetFail.Count > 0)
                {
                    audioInstance = deferredSetFail.Dequeue();
                    SetAudioClipLoadState(audioInstance, AudioDataLoadState.Failed);
                }
            }
        }

        static void EnsureInstanceExists()
        {
            if (!managerInstance)
            {
                managerInstance = new GameObject("Runtime AudioClip Loader Manger singleton instance") { hideFlags = HideFlags.HideAndDontSave };
                managerInstance.AddComponent<Manager>();
            }
        }

        public static void SetAudioClipLoadState(AudioClip audioClip, AudioDataLoadState newLoadState)
        {
            audioLoadState[audioClip] = newLoadState;
        }

        public static AudioDataLoadState GetAudioClipLoadState(AudioClip audioClip)
        {
            AudioDataLoadState result = AudioDataLoadState.Failed;
            if (audioClip != null)
            {
                //result = audioClip.loadState;
                audioLoadState.TryGetValue(audioClip, out result);
            }

            return result;
        }

        public static void SetAudioClipLoadType(AudioClip audioClip, AudioClipLoadType newLoadType)
        {
            audioClipLoadType[audioClip] = newLoadType;
        }

        public static AudioClipLoadType GetAudioClipLoadType(AudioClip audioClip)
        {
            AudioClipLoadType result = (AudioClipLoadType)(-1);
            if (audioClip != null)
            {
                //result = audioClip.loadType;
                audioClipLoadType.TryGetValue(audioClip, out result);
            }
            return result;
        }

        static string GetExtension(string filePath) => Path.GetExtension(filePath).Substring(1).ToLower();

        public static bool IsSupportedFormat(string filePath) => supportedFormats.Contains(GetExtension(filePath));

        public static AudioFormat GetAudioFormat(string filePath)
        {
            AudioFormat result = AudioFormat.unknown;
            try
            {
                result = (AudioFormat)Enum.Parse(typeof(AudioFormat), GetExtension(filePath), true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return result;
        }

        public static void ClearCache() => cache.Clear();
    }
}