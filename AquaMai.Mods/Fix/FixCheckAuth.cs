using System;
using System.Text.RegularExpressions;
using AMDaemon.Allnet;
using AquaMai.Config.Attributes;
using HarmonyLib;
using Manager;
using Manager.Operation;

namespace AquaMai.Mods.Fix;

[ConfigSection(exampleHidden: true, defaultOn: true)]
public class FixCheckAuth
{
    [ConfigEntry]
    private static readonly bool allowHttpsUpgrade = true;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(OperationManager), "CheckAuth_Proc")]
    private static void PostCheckAuthProc(ref OperationData ____operationData)
    {
        if (Auth.GameServerUri.StartsWith("http://") || Auth.GameServerUri.StartsWith("https://"))
        {
            ____operationData.ServerUri = Auth.GameServerUri;

            // Host is used only for "CheckServerHash" and it is disabled now
            // it originally contains server's tls cert hash
            // So this can be used to transfer ambiguous data
            // we use this to notify that we can upgrade the link to https
            // as if we originally pass a https link to game, games without CheckServerHash will reject the link because ssl pinning
            if (Auth.GameServerHost == "_AquaMai_Upgrade_" && allowHttpsUpgrade)
            {
                ____operationData.ServerUri = Auth.GameServerUri.Replace("http://", "https://");
            }
            else if (upgradePort.IsMatch(Auth.GameServerHost) && allowHttpsUpgrade)
            {
                var match = upgradePort.Match(Auth.GameServerHost);
                var builder = new UriBuilder(Auth.GameServerUri)
                {
                    Port = int.Parse(match.Groups[1].Value),
                    Scheme = Uri.UriSchemeHttps,
                };
                ____operationData.ServerUri = builder.ToString();
            }
        }
    }

    private static readonly Regex upgradePort = new Regex(@"_AquaMai_UpgradPort_(\d+)_", RegexOptions.Compiled);
}
