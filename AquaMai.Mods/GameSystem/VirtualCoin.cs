using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using AMDaemon;
using AquaMai.Config.Attributes;
using AquaMai.Config.Types;
using AquaMai.Core.Attributes;
using AquaMai.Core.Helpers;
using HarmonyLib;
using Mai2.Mai2Cue;
using Main;
using Manager;
using MelonLoader;
using Credit = AMDaemon.Credit;
using Type = System.Type;


namespace AquaMai.Mods.GameSystem;

[ConfigSection(
    "虚拟投币",
    zh: """
        基于Mod，在不依赖 SegaTools 的前提下实现的投币功能
        支持通过键盘按键、机台上自定义功能键或 HTTP 接口远程增加点数
        注意:该功能增加的点数游戏重启后不会保存
        """,
    en: """
        Implements a custom Credit system without relying on SegaTools
        Supports adding Credits via keyboard input, cabinet custom FN key or remotely through an HTTP service
        Note: Credits added by this feature are not persisted and will be reset after restarting the game
        """)]
public static class VirtualCoin
{
    [ConfigEntry(
        "投币按键",
        """
        Bind a key for add a credit (Keyboard key or cabinet custom FN key)
        """,
        """
        绑定使用的投币按键（键盘按键或自定义功能键均可）
        """)]
    public static readonly KeyCodeOrName CoinKey = KeyCodeOrName.Equals;
    
    [ConfigEntry(
        "长按",
        "Should long press to trigger",
        "是否长按上述按键才能触发"
        )]
    public static readonly bool LongPress = false;

    [ConfigEntry(
        "启用远程投币",
        """
        Remotely add Credits via an HTTP interface.
        Warning: If you enable this option, an HTTP server will listen on 0.0.0.0 on your machine (the port is specified by the option below). Please be mindful of security risks and consider using password authentication.
        """,
        """
        通过 HTTP 接口远程增加点数。
        警告：若开启此项，则会在您机器的0.0.0.0上监听一个HTTP服务器（端口号由下面选项指定）。请注意安全性问题，并考虑配合密码验证。
        """)]
    private static readonly bool IsUseRemote = false;

    [ConfigEntry(
        "监听端口",
        """
        Listening port for the HTTP service.
        """,
        """
        HTTP服务监听的端口
        """)]
    private static readonly int Port = 7654;

    [ConfigEntry(
        "密码验证",
        """
        Enable password verification for the HTTP service, or leave it blank to disable password verification. If enabled, the requests should be with query param '?password='
        """,
        """
        设置访问密码，留空则不使用密码。如果使用密码，请求时URL后应当追加参数'?password='
        """)]
    private static readonly string Password = "";

    [ConfigEntry(
        "启用音效",
        """
        Play Sound Effects.
        """,
        """
        是否播放音效
        """)]
    private static readonly bool IsPlaySound = true;

    private static bool IsUseKeyboard => CoinKey != KeyCodeOrName.None;

    // 出于线程安全，下面变量应该仅在Unity主线程写入（通过MelonCoroutine.Start等方式）。其他线程只能读，不应写入。
    private static volatile int _bufferCredit = 0;

    //缓存以提升性能
    private static PropertyInfo pointerProp;

    private static Dictionary<string, MethodInfo> methods = new();

    public static void OnAfterPatch()
    {
        if (IsUseRemote)
        {
            CreditHttpServerHost.StartOnce();
        }
    }

    //该功能实现实际上很简单，就是整一个缓存点数，扣Credit的时候优先扣缓存中的Credit，然后不够再交给原逻辑
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CreditUnit), "Credit", MethodType.Getter)]
    public static void CreditPatch(ref uint __result)
    {
        __result += (uint)_bufferCredit;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CreditUnit), "IsZero", MethodType.Getter)]
    public static void IsZeroPatch(ref bool __result)
    {
        __result = __result && _bufferCredit == 0;
    }

    private static int GetNeedCost(CreditUnit __instance, int gameCostIndex, int count)
    {
        var gameCosts = __instance.GameCosts;
        if (gameCosts == null)
        {
            throw new Exception("Failed to get GameCosts value");
        }

        var needCost = count * (int)gameCosts[gameCostIndex];
        return needCost;
    }

    private static IntPtr GetPointer(CreditUnit __instance)
    {
        if (pointerProp == null) pointerProp = AccessTools.Property(typeof(CreditUnit), "Pointer");
        var pointerValue = pointerProp?.GetValue(__instance);
        if (pointerValue == null)
        {
            throw new Exception("Failed to get Pointer value");
        }

        return (IntPtr)pointerValue;
    }

    private static MethodInfo GetAPIMethod(string apiName, params Type[] parameterTypes)
    {
        var apiType = AccessTools.TypeByName("AMDaemon.Api");
        if (apiType == null)
            throw new Exception("Failed to access AMDaemon.Api type");

        if (apiName == "Call")
        {
            var methods = apiType.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.Name == "Call" && m.GetParameters().Length == 1)
                .ToArray();

            // 1.优先匹配非泛型: bool Call(Func<bool>)
            var nonGenericBool = methods.FirstOrDefault(m =>
                !m.IsGenericMethodDefinition &&
                m.ReturnType == typeof(bool) &&
                m.GetParameters()[0].ParameterType == typeof(Func<bool>));

            if (nonGenericBool != null)
                return nonGenericBool;

            // 2.兼容泛型: T Call<T>(Func<T>) 闭包成 bool
            var genericDef = methods.FirstOrDefault(m =>
                m.IsGenericMethodDefinition &&
                m.GetGenericArguments().Length == 1 &&
                m.GetParameters()[0].ParameterType.IsGenericType &&
                m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(Func<>));

            if (genericDef != null)
                return genericDef.MakeGenericMethod(typeof(bool));

            var overloads = string.Join(" | ", methods.Select(m => m.ToString()));
            throw new Exception("Failed to access Api.Call(Func<bool>) method. Overloads: " + overloads);
        }

        MethodInfo calledMethod;
        if (parameterTypes != null && parameterTypes.Length > 0)
            calledMethod = AccessTools.Method(apiType, apiName, parameterTypes);
        else
            calledMethod = AccessTools.Method(apiType, apiName);

        if (calledMethod == null)
            throw new Exception($"Failed to access Api.{apiName} method");

        return calledMethod;
    }

    private static bool CallAPIMethod(string methodName, object[] arguments)
    {
        if (!methods.ContainsKey("Call")) methods["Call"] = GetAPIMethod("Call");
        if (!methods.ContainsKey(methodName)) methods[methodName] = GetAPIMethod(methodName);
        
        Func<bool> lambda = () => (bool)methods[methodName].Invoke(null, arguments);
        return (bool)methods["Call"].Invoke(null, [lambda]);
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(CreditUnit), "IsGameCostEnough", new Type[] { typeof(int), typeof(int) })]
    public static bool IsGameCostEnough(CreditUnit __instance, ref bool __result, int gameCostIndex, int count)
    {
        try
        {
            var needCost = GetNeedCost(__instance, gameCostIndex, count);
            if (_bufferCredit >= needCost)
            {
                __result = true;
                return false;
            }
            else if (_bufferCredit != 0 && _bufferCredit < needCost)
            {
                needCost -= _bufferCredit;
                __result = CallAPIMethod("CreditUnit_isGameCostEnough", [GetPointer(__instance), 0, needCost]);
                return false;
            }

            return true;
        }
        catch (Exception e)
        {
            MelonLogger.Warning("[VirtualCoin]Patch IsGameCostEnough Failed:" + e.Message);
            return true;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(CreditUnit), "PayGameCost", new Type[] { typeof(int), typeof(int) })]
    public static bool PayGameCostPatch(CreditUnit __instance, ref bool __result, int gameCostIndex, int count)
    {
        try
        {
            var needCost = GetNeedCost(__instance, gameCostIndex, count);
            if (_bufferCredit - needCost >= 0)
            {
                _bufferCredit -= (int)needCost;

# if DEBUG
                MelonLogger.Msg($"#1:_buffer:{_bufferCredit}");
# endif

                __result = true;
                return false;
            }

            if (_bufferCredit > 0 && _bufferCredit - needCost < 0)
            {
                needCost -= _bufferCredit;
                _bufferCredit = 0;

# if DEBUG
                MelonLogger.Msg($"#2:_buffer:{_bufferCredit};needCost:{needCost}");
# endif
                __result = CallAPIMethod("CreditUnit_payGameCost", [GetPointer(__instance), 0, (int)needCost]);
                return false;
            }

            return true;
        }
        catch (Exception e)
        {
            MelonLogger.Warning("[VirtualCoin]Patch PayGameCost Failed" + e.Message);
            return true;
        }
    }

    [EnableIf(nameof(IsUseKeyboard))]
    [HarmonyPatch(typeof(GameMainObject), "Update")]
    [HarmonyPostfix]
    public static void OnUpdatePatch()
    {
        if (KeyListener.GetKeyDownOrLongPress(CoinKey, LongPress))
        {
            _bufferCredit += 1;
            if (IsPlaySound)
            {
                SoundManager.PlaySystemSE(Cue.SE_SYS_CREDIT);
            }
        }
    }

    private static class CreditHttpServerHost
    {
        private static readonly object StartSync = new object();

        private static HttpListener _listener;

        private static Thread _listenThread;

        private static volatile bool _running;

        public static void StartOnce()
        {
            if (_running)
            {
                return;
            }

            lock (StartSync)
            {
                if (_running)
                {
                    return;
                }

                try
                {
                    var port = GetPort();
                    _listener = new HttpListener();
                    _listener.Prefixes.Add($"http://*:{port}/");
                    _listener.Start();

                    _running = true;
                    _listenThread = new Thread(ListenLoop)
                    {
                        IsBackground = true,
                        Name = "AquaMai.VirtualCoin.CreditHttpServer"
                    };
                    _listenThread.Start();
                    MelonLogger.Msg($"[VirtualCoin] HTTP Server started: Success on {port}");
                }
                catch (Exception e)
                {
                    _running = false;
                    MelonLogger.Error($"[VirtualCoin] HTTP Server started Failed: {e}");
                    try
                    {
                        _listener?.Close();
                    }
                    catch
                    {
                    }

                    _listener = null;
                }
            }
        }

        private static int GetPort()
        {
            if (Port > 0 && Port <= 65535)
            {
                return Port;
            }

            return 7654;
        }

        private static void ListenLoop()
        {
            while (_running && _listener != null)
            {
                HttpListenerContext context;
                try
                {
                    context = _listener.GetContext();
                }
                catch
                {
                    break;
                }

                try
                {
                    HandleRequest(context);
                }
                catch
                {
                    WriteJson(context.Response, 500, "{\"ok\":false,\"error\":\"internal_error\"}");
                }
            }
        }

        private static void HandleRequest(HttpListenerContext context)
        {
            var path = context.Request.Url?.AbsolutePath ?? "/";

            if (string.Equals(path, "/", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(path, "/health", StringComparison.OrdinalIgnoreCase))
            {
                WriteJson(context.Response, 200, "{\"ok\":true,\"service\":[\"credit\",\"add\"]}");
                return;
            }

            if (string.Equals(path, "/credit", StringComparison.OrdinalIgnoreCase))
            {
                if (!CheckPassword(context.Request))
                {
                    WriteJson(context.Response, 401, "{\"ok\":false,\"error\":\"unauthorized\"}");
                    return;
                }

                uint credit = 0;
                uint amDcredit = 0;
                uint remain = 0;
                uint bufferCredit = 0;
                var freePlay = false;

                // 对下列的信息，可以不做多线程保护，因为是只读的、而且变量之间相互没有依赖，
                // 而且这只是一个信息性质的GET接口、对实时性没有要求，所以直接只读访问变量问题不大。
                var player = Credit.Players[0];
                bufferCredit = (uint)_bufferCredit;
                credit = player.Credit;
                amDcredit = credit - (uint)bufferCredit;
                remain = player.Remain;
                freePlay = player.IsFreePlay;
                WriteJson(context.Response, 200,
                    "{\"ok\":true,\"bufferCredit\":" + bufferCredit + ",\"Credit\":" + credit + ",\"AMDCredit\":" +
                    amDcredit + ",\"remain\":" + remain + ",\"isFreePlay\":" + (freePlay ? "true" : "false") + "}");
                return;
            }

            if (string.Equals(path, "/add", StringComparison.OrdinalIgnoreCase))
            {
                if (!CheckPassword(context.Request))
                {
                    WriteJson(context.Response, 401, "{\"ok\":false,\"error\":\"unauthorized\"}");
                    return;
                }

                MelonCoroutines.Start(AddCredit());
                WriteJson(context.Response, 200, "{\"ok\":true}");
                return;
            }

            WriteJson(context.Response, 404, "{\"ok\":false,\"error\":\"not_found\"}");
        }
        
        private static IEnumerator AddCredit()
        {
            _bufferCredit += 1;
            if (IsPlaySound)
            {
                SoundManager.PlaySystemSE(Cue.SE_SYS_CREDIT);
            }
            yield return true;
        }

        private static void WriteJson(HttpListenerResponse response, int statusCode, string payload)
        {
            response.StatusCode = statusCode;
            response.ContentType = "application/json; charset=utf-8";
            var bytes = Encoding.UTF8.GetBytes(payload);
            response.ContentLength64 = bytes.LongLength;
            using var stream = response.OutputStream;
            stream.Write(bytes, 0, bytes.Length);
        }

        private static bool CheckPassword(HttpListenerRequest request)
        {
            if (string.IsNullOrEmpty(Password))
            {
                return true; // skip
            }

            var input = request.QueryString["password"];
            return FixedTimeEquals(input, Password);
        }
        
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static bool FixedTimeEquals(string left, string right)
        {
            if (left == null || right == null) return false;
            if (left.Length != right.Length) return false;

            int result = 0;
            for (int i = 0; i < left.Length; i++)
            {
                // 使用按位或，只要有一处不等，result 就会变成非零值
                result |= left[i] ^ right[i];
            }

            return result == 0;
        }
    }
}
