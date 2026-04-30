using AquaMai.Config.Interfaces;

namespace AquaMai.Config.Migration;

public class ConfigMigration_V2_5_V2_6 : IConfigMigration
{
    public string FromVersion => "2.5";
    public string ToVersion => "2.6";

    public ConfigView Migrate(ConfigView src)
    {
        var dst = (ConfigView)src.Clone();
        dst.SetValue("Version", ToVersion);

        // AdxHidInput: button1~4/disableButtons → p1Button1~4/p1DisableButtons + p2Button1~4/p2DisableButtons
        MigrateButtonKeys(dst, "GameSystem.AdxHidInput", withDisable: true);

        // MaimollerIO: button1~4 → p1Button1~4 + p2Button1~4
        MigrateButtonKeys(dst, "GameSystem.MaimollerIO", withDisable: false);

        return dst;
    }

    private static void MigrateButtonKeys(ConfigView dst, string section, bool withDisable)
    {
        for (int i = 1; i <= 4; i++)
        {
            var oldKey = $"{section}.button{i}";
            if (dst.TryGetValue<string>(oldKey, out var val))
            {
                dst.SetValue($"{section}.p1Button{i}", val);
                dst.SetValue($"{section}.p2Button{i}", val);
                dst.Remove(oldKey);
            }
        }

        if (withDisable && dst.TryGetValue<bool>($"{section}.disableButtons", out var disable))
        {
            dst.SetValue($"{section}.p1DisableButtons", disable);
            dst.SetValue($"{section}.p2DisableButtons", disable);
            dst.Remove($"{section}.disableButtons");
        }
    }
}
