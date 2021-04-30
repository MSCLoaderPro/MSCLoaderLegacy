﻿using AudioLibrary;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

namespace MSCLoader
{
    /// <summary>
    /// Audio library (play local *.mp3, *.ogg, *.wav, *.aiff)
    /// </summary>
    public class ModAudio : MonoBehaviour
    {
        /// <summary>
        /// Your AudioSource goes here
        /// </summary>
        public AudioSource audioSource;

        /// <summary>
        /// Load audio from file
        /// </summary>
        /// <param name="path">Full path to audio file</param>
        /// <param name="doStream">Stream from HDD instead of loading to memory (recommended)</param>
        /// <param name="background">Load file in background</param>
        public void LoadAudioFromFile(string path, bool doStream, bool background)
        {
            Stream stream = new MemoryStream(File.ReadAllBytes(path));
            AudioFormat format = Manager.GetAudioFormat(path);
            string filename = Path.GetFileName(path);

            if (format == AudioFormat.unknown)
            {
                ModConsole.LogError($"Unknown audio format of file {filename}");
            }

            try
            {
                if (audioSource == null)
                {
                    audioSource = gameObject.GetComponent<AudioSource>();
                }

                audioSource.clip = Manager.Load(stream, format, filename, doStream, background, true);
            }
            catch (Exception e)
            {
                ModConsole.LogError(e.ToString());
                System.Console.WriteLine(e);
                audioSource.clip = null;
            }

        }

        /// <summary>
        /// Get current time position of audio file
        /// </summary>
        /// <returns>Time in TimeSpan format</returns>
        public TimeSpan Time()
        {
            if (audioSource.clip != null)
                return TimeSpan.FromSeconds(audioSource.time);
            else
                return TimeSpan.FromSeconds(0);
        }

        /// <summary>
        /// Get total time of audio file
        /// </summary>
        /// <returns>Time in TimeSpan format</returns>
        public TimeSpan TotalTime()
        {
            if (audioSource.clip != null)
                return TimeSpan.FromSeconds(audioSource.clip.length);
            else
                return TimeSpan.FromSeconds(0);
        }

        /// <summary>
        /// Play loaded audio file from specifed time.
        /// </summary>
        /// <param name="time">time to start</param>
        /// <param name="delay">optional delay</param>
        public void Play(float time, float delay = 1f)
        {
            audioSource.mute = true;
            audioSource.PlayDelayed(delay);
            audioSource.time = time;
            StartCoroutine(TimeDelay());
        }

        /// <summary>
        /// Play loaded audio file
        /// </summary>
        public void Play()
        {
            audioSource.mute = false;
            audioSource.Play();
        }

        /// <summary>
        /// Stop playing audio file
        /// </summary>
        public void Stop()
        {
            audioSource.Stop();
        }

        IEnumerator TimeDelay()
        {
            yield return new WaitForSeconds(1f);
            if (audioSource.isPlaying)
            {
                Play();
            }
        }
    }
}