using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using HarmonyLib;
using Manager;
using MelonLoader;
using UnityEngine;
using AquaMai.Config.Attributes;
using AquaMai.Core.Helpers;
using AquaMai.Core.Resources;

namespace AquaMai.Mods.GameSystem;

[ConfigSection(
    en: """
        Use custom CameraId rather than the default ones.
        If enabled, you can customize the game to use the specified camera.
        """,
    zh: """
        使用自定义的摄像头 ID 而不是默认的
        启用后可以指定游戏使用的摄像头
        """)]
public class CustomCameraId
{
    [ConfigEntry(
        en: "Print the camera list to the log when starting, can be used as a basis for modification.",
        zh: "启动时打印摄像头列表到日志中，可以作为修改的依据")]
    public static bool printCameraList;

    [ConfigEntry(
        en: "DX Pass 1P.",
        zh: "DX Pass 1P")]
    public static int leftQrCamera;

    [ConfigEntry(
        en: "DX Pass 2P.",
        zh: "DX Pass 2P")]
    public static int rightQrCamera;

    [ConfigEntry(
        en: "Player Camera.",
        zh: "玩家摄像头")]
    public static int photoCamera;

    [ConfigEntry(
        en: "WeChat QRCode Camera.",
        zh: "二维码扫描摄像头")]
    public static int chimeCamera;
    [ConfigEntry(
        en: "Custom WeChat QRCode camera width.",
        zh: "自定义二维码扫描摄像头宽度")]
    public static int customChimeCameraWidth;

    [ConfigEntry(
        en: "Custom WeChat QRCode camera height.",
        zh: "自定义二维码扫描摄像头高度")]
    public static int customChimeCameraHeight;
    private static readonly Dictionary<string, string> cameraTypeMap = new()
    {
        ["LeftQrCamera"] = "QRLeft",
        ["RightQrCamera"] = "QRRight",
        ["PhotoCamera"] = "Photo",
        ["ChimeCamera"] = "Chime",
    };

    [HarmonyPrefix]
    [HarmonyPatch(typeof(CameraManager), "CameraInitialize")]
    public static bool CameraInitialize(CameraManager __instance, ref IEnumerator __result)
    {
        __result = CameraInitialize(__instance);
        return false;
    }

    private static IEnumerator CameraInitialize(CameraManager __instance)
    {
        if (WebCamTexture.devices.Length == 0)
        {
            MelonLogger.Warning("[CustomCameraId] No camera detected, camera initialization failed.");
            MessageHelper.ShowMessage(Locale.NoCamera, title: "CustomCameraId");
            CameraManager.IsReady = true;
            yield break;
        }

        var textureCache = new WebCamTexture[WebCamTexture.devices.Length];
        SortedDictionary<CameraManager.CameraTypeEnum, WebCamTexture> webCamTextures = [];
        foreach (var (configEntry, cameraTypeName) in cameraTypeMap)
        {
            int deviceId = Traverse.Create(typeof(CustomCameraId)).Field(configEntry).GetValue<int>();
            if (deviceId < 0 || deviceId >= WebCamTexture.devices.Length)
            {
                MelonLogger.Warning($"[CustomCameraId] Ignoring custom camera {configEntry}: camera ID {deviceId} out of range");
                continue;
            }

            if (!Enum.TryParse<CameraManager.CameraTypeEnum>(cameraTypeName, out var cameraType))
            {
                MelonLogger.Warning($"[CustomCameraId] Ignoring custom camera {configEntry}: camera type {cameraTypeName} not present");
                continue;
            }

            if (textureCache[deviceId] != null)
            {
                webCamTextures[cameraType] = textureCache[deviceId];
            }
            else
            {
                if (cameraTypeName == CameraManager.CameraTypeEnum.Chime &&
                    (customChimeCameraWidth != null || customChimeCameraHeight != null))
                {
                    var webCamTexture = new WebCamTexture(
                        WebCamTexture.devices[deviceId].name,
                        customChimeCameraWidth,
                        customChimeCameraHeight
                    );
                }
                else
                {
                    var webCamTexture = new WebCamTexture(WebCamTexture.devices[deviceId].name);
                    webCamTextures[cameraType] = webCamTexture;
                    textureCache[deviceId] = webCamTexture;
                }
            }
        }

        int textureCount = webCamTextures.Count;
        __instance.isAvailableCamera = new bool[textureCount];
        __instance.cameraProcMode = new CameraManager.CameraProcEnum[textureCount];

        int textureIndex = 0;
        foreach (var (cameraType, webCamTexture) in webCamTextures)
        {
            __instance.isAvailableCamera[textureIndex] = true;
            __instance.cameraProcMode[textureIndex] = CameraManager.CameraProcEnum.Good;
            CameraManager.DeviceId[(int)cameraType] = textureIndex;
            textureIndex++;
        }

        Traverse.Create(__instance).Field("_webcamtex").SetValue(webCamTextures.Values.ToArray());

        CameraManager.IsReady = true;
        yield break;
    }

    public static void OnBeforePatch()
    {
        if (!printCameraList)
        {
            return;
        }

        WebCamDevice[] devices = WebCamTexture.devices;
        string cameraList = "Connected Web Cameras:\n";
        for (int i = 0; i < devices.Length; i++)
        {
            WebCamDevice webCamDevice = devices[i];
            WebCamTexture webCamTexture = new WebCamTexture(webCamDevice.name);
            webCamTexture.Play();
            cameraList += "==================================================\n";
            cameraList += "Name: " + webCamDevice.name + "\n";
            cameraList += $"ID: {i}\n";
            cameraList += $"Resolution: {webCamTexture.width} * {webCamTexture.height}\n";
            cameraList += $"FPS: {webCamTexture.requestedFPS}\n";
            webCamTexture.Stop();
        }

        cameraList += "==================================================";

        foreach (var line in cameraList.Split('\n'))
        {
            MelonLogger.Msg($"[CustomCameraId] {line}");
        }
    }
}
