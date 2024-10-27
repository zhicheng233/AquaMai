using AquaMai.Attributes;

namespace AquaMai.UX;

public class Config
{
    [ConfigComment(
        en: "Language for mod UI, supports en and zh",
        zh: "Mod 界面的语言，支持 en 和 zh")]
    public string Locale { get; set; } = "";

    [ConfigComment(
        en: "Single player: Show 1P only, at the center of the screen",
        zh: "单人模式，不显示 2P")]
    public bool SinglePlayer { get; set; }

    [ConfigComment(
        en: "Remove the circle mask in the game",
        zh: "移除遮罩")]
    public bool HideMask { get; set; }

    [ConfigComment(
        en: "Load Jacket image from folder \"LocalAssets\" and filename \"{MusicID}.png\" for self-made charts",
        zh: "通过游戏目录下 `LocalAssets\\000000（歌曲 ID）.png` 加载封面，自制谱用")]
    public bool LoadAssetsPng { get; set; }

    [ConfigComment(
        en: "Add \".ab\" image resources without the need of rebuilding a manifest",
        zh: """
            优化图片资源的加载，就算没有 AssetBundleImages.manifest 也可以正常加载 ab 格式的图片资源
            导入了删除曲包之类的话，应该需要开启这个
            """)]
    public bool LoadAssetBundleWithoutManifest { get; set; }

    [ConfigComment(
        en: """
            Press key "7" for 1 second to skip to next step or restart current song
            Hold the bottom four buttons (3456) for official quick retry (non-utage only)
            """,
        zh: """
            长按 Service 键或者键盘上的 “7” 键（ADX 默认固件下箭头键中间的圆形按键）可以：
            - 跳过登录过程中的界面直接进入选歌界面
            - 在选歌界面直接结束游戏
            在游玩界面，按一下 “7” 或者 Service 键重开当前的歌，按 1P 的“选择”键立即结束当前乐曲
            打完最后一个音符之后也可以
            按住下方四个按钮（3456）使用官方快速重开（仅对非宴谱有效）
            """)]
    public bool QuickSkip { get; set; }

    [ConfigComment(
        en: """
            Random BGM, put Mai2Cue.{acb,awb} of old version of the game in `LocalAssets\Mai2Cue` and rename them
            Do not enable when SinglePlayer is off
            """,
        zh: """
            在 `LocalAssets\Mai2Cue` 这个目录下放置了旧版游戏的 Mai2Cue.{acb,awb} 并重命名的话，可以在播放游戏 BGM 的时候随机播放这里面的旧版游戏 BGM
            和 2P 模式有冲突，如果你没有开启 'SinglePlayer' 的话，请关闭这个
            """)]
    public bool RandomBgm { get; set; }

    [ConfigComment(
        en: "Play \"Master\" difficulty on Demo screen",
        zh: "在闲置时的演示画面上播放紫谱而不是绿谱")]
    public bool DemoMaster { get; set; }

    [ConfigComment(
        en: """
            Disable timers
            Not recommand to enable when SinglePlayer is off
            """,
        zh: """
            关掉那些游戏中的倒计时
            如果你没有开启 'SinglePlayer' 的话，不建议开这个，不过要开的话也不是不可以
            """)]
    public bool ExtendTimer { get; set; }

    [ConfigComment(
        en: "Save immediate after playing a song",
        zh: "打完一首歌的时候立即向服务器保存成绩")]
    public bool ImmediateSave { get; set; }

    [ConfigComment(
        en: """
            Use the png jacket above as BGA if BGA is not found for self-made charts
            Use together with `LoadJacketPng`
            """,
        zh: """
            如果没有 dat 格式的 BGA 的话，就用歌曲的封面做背景，而不是显示迪拉熊的笑脸
            请和 `LoadJacketPng` 一起用
            """)]
    public bool LoadLocalBga { get; set; }

    [ConfigComment(
        en: "Prevent accidental touch of the Test button, requires 1 second long press to take effect",
        zh: "防止你不小心按到 Test 键，Test 键需要长按 1 秒才能生效")]
    public bool TestProof { get; set; }

    [ConfigComment(
        en: """
            In the song selection screen, press the Service button or the "7" key (the round button in the middle of the arrow keys in the default ADX firmware) to toggle the display of self-made charts.
            A directory is considered to contain self-made charts if it does not have DataConfig.xml or OfficialChartsMark.txt in the Axxx directory.
            """,
        zh: """
            选歌界面按下 Service 键或者键盘上的 “7” 键（ADX 默认固件下箭头键中间的圆形按键）切换自制谱的显示和隐藏
            是否是自制谱的判断方式是 Axxx 目录里没有 DataConfig.xml 或 OfficialChartsMark.txt 就认为这个目录里是自制谱
            """)]
    public bool HideSelfMadeCharts { get; set; }

    [ConfigComment(
        en: """
            Place font.ttf in the LocalAssets directory to replace the game's global font
            Cannot be used together with FontFix
            """,
        zh: """
            在 LocalAssets 目录下放置 font.ttf 可以替换游戏的全局字体
            不可以和 FontFix 一起使用
            """)]
    public bool CustomFont { get; set; }

    [ConfigComment(
        en: "Map touch actions to buttons",
        zh: "映射触摸操作至实体按键")]
    public bool TouchToButtonInput { get; set; }

    [ConfigComment(
        en: "Cannot be used together with HanabiFix",
        zh: """
            完全隐藏烟花
            不能和 HanabiFix 一起使用
            """)]
    public bool HideHanabi { get; set; }

    [ConfigComment(
        en: "Set the version string displayed at the top-right corner of the screen",
        zh: "把右上角的版本更改为自定义文本")]
    public string CustomVersionString { get; set; } = "";

    [ConfigComment(
        en: """
            Custom shop name in photo
            Also enable shop name display in SDGA
            """,
        zh: """
            自定义拍照的店铺名称
            同时在 SDGA 中会启用店铺名称的显示（但是不会在游戏里有设置）
            """)]
    public string CustomPlaceName { get; set; } = "";

    [ConfigComment(
        en: "Execute some command on game idle",
        zh: """
            在游戏闲置的时候执行指定的命令脚本
            比如说可以在游戏闲置是降低显示器的亮度
            """)]
    public string ExecOnIdle { get; set; } = "";

    [ConfigComment(
        en: "Execute some command on game start",
        zh: "在玩家登录的时候执行指定的命令脚本")]
    public string ExecOnEntry { get; set; } = "";
}
