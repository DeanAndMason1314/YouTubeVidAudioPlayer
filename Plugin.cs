using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using BepInEx;
using BepInEx.Configuration;
using Newtonsoft.Json;
using UnityEngine;

namespace YouTubeVidAudioPlayer
{
    [BepInPlugin("com.dmkdev.ytvid.audioplayer", "YouTube Video Audio Player", "1.0.0")]
    public class YouTubeVidAudioPlayer : BaseUnityPlugin
    {
        private AudioSource audioSource;
        private string youtubeURL;
        private string audioFilePath;

        // Configuration file path and settings
        private static string configFilePath = Path.Combine(Paths.ConfigPath, "YouTubeVidAudioPlayer.cfg");
        private static ConfigFile config;

        void Awake()
        {
            // Log mod loaded
            Logger.LogInfo("YouTube Video Audio Player mod loaded successfully!");

            // Initialize configuration settings
            config = new ConfigFile(configFilePath, true);
            youtubeURL = config.Bind("Settings", "YouTubeURL", "YOUR_YOUTUBE_URL", "Enter the URL of the YouTube video here").Value;

            // Add audio source
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioFilePath = Path.Combine(Application.persistentDataPath, "music.mp3");

            // Start audio download and playback
            Task.Run(DownloadAndPlayMusic);
        }

        // Method to download and play audio from YouTube
        private async Task DownloadAndPlayMusic()
        {
            if (string.IsNullOrEmpty(youtubeURL))
            {
                Logger.LogError("YouTube URL is not provided in the configuration file.");
                return;
            }

            if (File.Exists(audioFilePath)) File.Delete(audioFilePath);

            try
            {
                // Download the audio from YouTube
                byte[] audioData = await DownloadAudioFromYouTube(youtubeURL);
                if (audioData != null && audioData.Length > 0)
                {
                    File.WriteAllBytes(audioFilePath, audioData);
                    PlayDownloadedMusic();
                }
                else
                {
                    Logger.LogError("Failed to download audio from YouTube.");
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error during audio download: {ex.Message}");
            }
        }

        // Method to download audio from YouTube
        private async Task<byte[]> DownloadAudioFromYouTube(string videoUrl)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // Replace this logic with actual YouTube audio download logic
                    // For now, we're simulating a download with a placeholder audio file
                    // You can replace this with actual YouTube API integration for downloading audio

                    var response = await client.GetByteArrayAsync(videoUrl);
                    return response;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error downloading audio: {ex.Message}");
                return null;
            }
        }

        // Method to play the downloaded music
        private void PlayDownloadedMusic()
        {
            if (!File.Exists(audioFilePath))
            {
                Logger.LogError("Music file not found! Download failed.");
                return;
            }

            AudioClip clip = LoadMp3(audioFilePath);
            if (clip != null)
            {
                audioSource.clip = clip;
                audioSource.Play();
                Logger.LogInfo("Playing downloaded music...");
            }
            else
            {
                Logger.LogError("Failed to load MP3 file.");
            }
        }

        // Method to load the MP3 file into an AudioClip
        private AudioClip LoadMp3(string path)
        {
            try
            {
                byte[] fileBytes = File.ReadAllBytes(path);
                if (fileBytes.Length == 0) return null;

                // Convert the byte array to an AudioClip
                float[] audioData = ConvertBytesToAudio(fileBytes);
                AudioClip audioClip = AudioClip.Create("DownloadedMusic", audioData.Length, 1, 44100, false);
                audioClip.SetData(audioData, 0);
                return audioClip;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error loading MP3 file: {ex.Message}");
                return null;
            }
        }

        // Method to convert bytes into audio data (simplified version)
        private float[] ConvertBytesToAudio(byte[] bytes)
        {
            // Simple conversion logic for simulation purposes
            // You would need to implement proper decoding based on your download method
            float[] audioData = new float[bytes.Length];
            for (int i = 0; i < bytes.Length; i++)
            {
                audioData[i] = (bytes[i] / 255f) * 2f - 1f; // Example conversion
            }
            return audioData;
        }
    }
}
