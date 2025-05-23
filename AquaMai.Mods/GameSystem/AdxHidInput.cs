using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using AMDaemon;
using AquaMai.Config.Attributes;
using HarmonyLib;
using HidLibrary;
using MelonLoader;
using UnityEngine;

namespace AquaMai.Mods.GameSystem;

[ConfigSection(
    en: "Input using ADX HID firmware (do not enable if you are not using ADX's HID firmware, be sure to delete the existing HID related DLL when enabled)",
    zh: "使用 ADX HID 固件的自定义输入（如果你没有使用 ADX 的 HID 固件，请不要启用。启用时请务必删除现有 HID 相关 DLL）")]
public class AdxHidInput
{
    private static HidDevice adxController1P = null;
    private static HidDevice adxController2P = null;
    private static byte[] inputBuf1P = new byte[32];
    private static byte[] inputBuf2P = new byte[32];

    private static void HidInputThread()
    {
        while (true)
        {
            if (adxController1P != null)
            {
                var report1P = adxController1P.Read();
                if (report1P.Status == HidDeviceData.ReadStatus.Success && report1P.Data.Length > 13)
                    inputBuf1P = report1P.Data;
            }

            if (adxController2P != null)
            {
                var report2P = adxController2P.Read();
                if (report2P.Status == HidDeviceData.ReadStatus.Success && report2P.Data.Length > 13)
                    inputBuf2P = report2P.Data;
            }
        }
    }

    public static void OnBeforePatch()
    {
        adxController1P = HidDevices.Enumerate(0x2E3C, 0x5750).FirstOrDefault();
        adxController2P = HidDevices.Enumerate(0x2E4C, 0x5750).FirstOrDefault();

        if (adxController1P == null)
        {
            MelonLogger.Msg("[HidInput] Open HID 1P Failed");
        }
        else
        {
            MelonLogger.Msg("[HidInput] Open HID 1P OK");
        }

        if (adxController2P == null)
        {
            MelonLogger.Msg("[HidInput] Open HID 2P Failed");
        }
        else
        {
            MelonLogger.Msg("[HidInput] Open HID 2P OK");
        }

        if (adxController1P != null || adxController2P != null)
        {
            Thread hidThread = new Thread(HidInputThread);
            hidThread.Start();
        }
    }

    public enum AdxKeyMap
    {
        None = 0,
        Select1P,
        Select2P,
        Service,
        Test,
    }

    [ConfigEntry(zh: "按钮 1（向上的三角键）")]
    private static readonly AdxKeyMap button1 = AdxKeyMap.Select1P;

    [ConfigEntry(zh: "按钮 2（三角键中间的圆形按键）")]
    private static readonly AdxKeyMap button2 = AdxKeyMap.Service;

    [ConfigEntry(zh: "按钮 3（向下的三角键）")]
    private static readonly AdxKeyMap button3 = AdxKeyMap.Select2P;

    [ConfigEntry(zh: "按钮 4（最下方的圆形按键）")]
    private static readonly AdxKeyMap button4 = AdxKeyMap.Test;

    private static bool GetPushedByButton(int playerNo, InputId inputId)
    {
        var current = inputId.Value switch
        {
            "test" => AdxKeyMap.Test,
            "service" => AdxKeyMap.Service,
            "select" when playerNo == 0 => AdxKeyMap.Select1P,
            "select" when playerNo == 1 => AdxKeyMap.Select2P,
            _ => AdxKeyMap.None,
        };

        AdxKeyMap[] arr = [button1, button2, button3, button4];
        if (current != AdxKeyMap.None)
        {
            for (int i = 0; i < 4; i++)
            {
                if (arr[i] != current) continue;
                var keyIndex = 10 + i;
                if (inputBuf1P[keyIndex] == 1 || inputBuf2P[keyIndex] == 1)
                {
                    return true;
                }
            }
            return false;
        }

        var buf = playerNo == 0 ? inputBuf1P : inputBuf2P;
        return inputId.Value switch
        {
            "button_01" => buf[5] == 1,
            "button_02" => buf[4] == 1,
            "button_03" => buf[3] == 1,
            "button_04" => buf[2] == 1,
            "button_05" => buf[9] == 1,
            "button_06" => buf[8] == 1,
            "button_07" => buf[7] == 1,
            "button_08" => buf[6] == 1,
            _ => false,
        };
    }

    [HarmonyPatch]
    public static class Hook
    {
        public static IEnumerable<MethodBase> TargetMethods()
        {
            var jvsSwitch = typeof(IO.Jvs).GetNestedType("JvsSwitch", BindingFlags.NonPublic | BindingFlags.Public);
            return [jvsSwitch.GetMethod("Execute")];
        }

        public static bool Prefix(
            int ____playerNo,
            InputId ____inputId,
            ref bool ____isStateOnOld2,
            ref bool ____isStateOnOld,
            ref bool ____isStateOn,
            ref bool ____isTriggerOn,
            ref bool ____isTriggerOff,
            KeyCode ____subKey)
        {
            var flag = GetPushedByButton(____playerNo, ____inputId);
            // 不影响键盘
            if (!flag) return true;

            var isStateOnOld2 = ____isStateOnOld;
            var isStateOnOld = ____isStateOn;

            if (isStateOnOld2 && !isStateOnOld)
            {
                return true;
            }

            ____isStateOn = true;
            ____isTriggerOn = !isStateOnOld;
            ____isTriggerOff = false;
            ____isStateOnOld2 = isStateOnOld2;
            ____isStateOnOld = isStateOnOld;
            return false;
        }
    }
}