using System;
using System.IO;
using System.Linq;
using AquaMai.Config.Attributes;
using HarmonyLib;
using MAI2.Util;
using Manager;
using MelonLoader;
using Monitor.Game;
using UnityEngine;
using UnityEngine.Video;

namespace AquaMai.Mods.GameSystem.Assets;

[ConfigSection(
    en: "Use mp4 files or the png jacket above as PV if no .dat found in the movie folder.",
    zh: "使用封面作为 PV 以及直读 MP4 格式 PV 的选项")]
public class MovieLoader
{
    [ConfigEntry(
        en: """
            Use jacket as movie
            Use together with `LoadLocalImages`.
            """,
        zh: """
            用封面作为背景 PV
            请和 `LoadLocalImages` 一起用
            """)]
    private static bool jacketAsMovie = true;

    [ConfigEntry(
        en: "Load MP4 files from LocalAssets",
        zh: "从 LocalAssets 中加载 MP4 文件作为 PV")]
    private static bool loadMp4Movie = true;

    private static VideoPlayer _videoPlayer;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameCtrl), "IsReady")]
    public static void LoadLocalBgaAwake(GameObject ____movieMaskObj)
    {
        var music = Singleton<DataManager>.Instance.GetMusic(GameManager.SelectMusicID[0]);
        if (music is null) return;

        var moviePath = Singleton<OptionDataManager>.Instance.GetMovieDataPath($"{music.movieName.id:000000}") + ".dat";
        if (!moviePath.Contains("dummy")) return;

        var mp4Path = Path.Combine(Environment.CurrentDirectory, "LocalAssets", $"{music.movieName.id:000000}.mp4");
        var mp4Exists = File.Exists(mp4Path);
        var jacket = LoadLocalImages.GetJacketTexture2D(music.movieName.id);
        if (!mp4Exists && jacket is null)
        {
            MelonLogger.Msg("No jacket found for music " + music);
            return;
        }

        if (mp4Exists && _videoPlayer is null)
        {
# if DEBUG
            MelonLogger.Msg("Init _videoPlayer");
# endif
            _videoPlayer = ____movieMaskObj.AddComponent<VideoPlayer>();
            _videoPlayer.url = mp4Path;
            _videoPlayer.playOnAwake = false;
            _videoPlayer.renderMode = VideoRenderMode.MaterialOverride;
            _videoPlayer.audioOutputMode = VideoAudioOutputMode.None;
        }
# if DEBUG
        else if (mp4Exists)
        {
            MelonLogger.Msg("_videoPlayer already exists");
        }
# endif

        var components = ____movieMaskObj.GetComponentsInChildren<Component>(false);
        var movies = components.Where(it => it.name == "Movie");

        foreach (var movie in movies)
        {
# if DEBUG
            MelonLogger.Msg("Found movie component: " + movie);
# endif
            // If I create a new RawImage component, the jacket will be not be displayed
            // I think it will be difficult to make it work with RawImage
            // So I change the material that plays video to default sprite material
            // The original player is actually a sprite renderer and plays video with a custom material
            var sprite = movie.GetComponent<SpriteRenderer>();
            if (mp4Exists)
            {
                if (_videoPlayer.targetMaterialRenderer == null)
                {
                    sprite.material = new Material(Shader.Find("Sprites/Default"));
                    _videoPlayer.targetMaterialRenderer = sprite;
                }
                else
                {
                    sprite.material = _videoPlayer.targetMaterialRenderer.material;
                }
            }
            else
            {
                sprite.sprite = Sprite.Create(jacket, new Rect(0, 0, jacket.width, jacket.height), new Vector2(0.5f, 0.5f));
                sprite.material = new Material(Shader.Find("Sprites/Default"));
            }
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MovieController), "Play")]
    public static void Play(int frame)
    {
        if (_videoPlayer == null) return;
        _videoPlayer.frame = frame;
        _videoPlayer.Play();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(MovieController), "Pause")]
    public static void Pause(bool pauseFlag)
    {
        if (_videoPlayer == null) return;
        if (pauseFlag)
        {
            _videoPlayer.Pause();
        }
        else
        {
            _videoPlayer.Play();
        }
    }
}