using System;
using System.Collections;
using AquaMai.Config.Attributes;
using AquaMai.Core.Helpers;
using AquaMai.Core.Resources;
using DB;
using HarmonyLib;
using JetBrains.Annotations;
using Manager;
using Manager.Operation;
using MelonLoader;
using Net.Packet;
using Net.Packet.Helper;
using Net.VO;
using Process;
using UnityEngine;
using UnityEngine.Networking;

namespace AquaMai.Mods.UX;

[ConfigSection(
    defaultOn: true,
    en: "Show announcement from server.",
    zh: "获取和显示来自服务端的公告")]
public static class ServerAnnouncement
{
    [Serializable]
    private class ServerAnnouncementRequestVO : VOSerializer
    {
        public string clientId;
        public string aquaMaiVersion;
    }

    [Serializable]
    private class ServerAnnouncementResponseVO : VOSerializer
    {
        [CanBeNull] public string title;
        [CanBeNull] public string announcement;
        [CanBeNull] public string imageUrl;
        public bool showOnIdle;
        public bool showOnUserLogin;
    }

    private class PacketGetServerAnnouncement : Packet
    {
        private readonly Action<ServerAnnouncementResponseVO> _onDone;

        private readonly Action<PacketStatus> _onError;

        public PacketGetServerAnnouncement(Action<ServerAnnouncementResponseVO> onDone, Action<PacketStatus> onError = null)
        {
            _onDone = onDone;
            _onError = onError;
            var netQuery = new NetQuery<ServerAnnouncementRequestVO, ServerAnnouncementResponseVO>("GetServerAnnouncementApi");
            netQuery.Request.clientId = AMDaemon.System.KeychipId.ShortValue;
            Create(netQuery);
        }

        public override PacketState Proc()
        {
            switch (ProcImpl())
            {
                case PacketState.Done:
                {
                    var netQuery = Query as NetQuery<ServerAnnouncementRequestVO, ServerAnnouncementResponseVO>;
                    _onDone(netQuery.Response);
                    break;
                }
                case PacketState.Error:
                    _onError?.Invoke(Status);
                    break;
            }

            return State;
        }
    }

    private static ServerAnnouncementResponseVO _announcement;
    private static Sprite _sprite;

    private class DownloadTexture : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(GetTexture());
        }

        private IEnumerator GetTexture()
        {
            var www = UnityWebRequestTexture.GetTexture(_announcement.imageUrl);
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                MelonLogger.Error($"[ServerAnnouncement] Failed to download image: {www.error}");
            }
            else
            {
                var texture = DownloadHandlerTexture.GetContent(www);
                _sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), texture.width / 4f);
#if DEBUG
                MelonLogger.Msg($"[ServerAnnouncement] Downloaded image success");
#endif
            }

            Destroy(gameObject);
        }
    }


    [HarmonyPatch(typeof(DataDownloaderMai2), "InitPacketList")]
    [HarmonyPostfix]
    private static void GetServerAnnouncement()
    {
        if (_announcement is not null) return;
        PacketHelper.StartPacket(new PacketGetServerAnnouncement(delegate(ServerAnnouncementResponseVO data)
        {
            _announcement = data;
            MelonLogger.Msg($"[ServerAnnouncement] {data.title}: {data.announcement}");
            if (!string.IsNullOrWhiteSpace(data.imageUrl))
            {
                new GameObject("[AquaMai] ServerAnnouncement - DownloadTexture").AddComponent<DownloadTexture>();
            }
        }, delegate(PacketStatus status)
        {
            // 可能是第三方服务器不支持这个功能，忽略即可
#if DEBUG
            MelonLogger.Error($"[ServerAnnouncement] Failed to get server announcement. {status}");
#endif
        }));
    }

    [HarmonyPatch(typeof(AdvertiseProcess), "OnStart")]
    [HarmonyPostfix]
    private static void AdvertiseProcessOnStart()
    {
        if (!_announcement.showOnIdle) return;
        MessageHelper.ShowMessage(_announcement.announcement, title: _announcement.title, sprite: _sprite, size: WindowSizeID.LargeVerticalPostImage);
    }

    [HarmonyPatch(typeof(EntryProcess), "OnStart")]
    [HarmonyPostfix]
    private static void EntryProcessOnStart()
    {
        if (!_announcement.showOnUserLogin) return;
        MessageHelper.ShowMessage(_announcement.announcement, title: _announcement.title, sprite: _sprite, size: WindowSizeID.LargeVerticalPostImage);
    }
}