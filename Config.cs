using System.Diagnostics.CodeAnalysis;
using AquaMai.Attributes;
using AquaMai.CustomKeyMap;

namespace AquaMai
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class Config
    {
        [ConfigComment(
            en: "UX: User Experience Improvements",
            zh: """

                试试使用 MaiChartManager 图形化配置 AquaMai 吧！
                https://github.com/clansty/MaiChartManager

                用户体验改进
                """)]
        public UXConfig UX { get; set; } = new();

        [ConfigComment(
            en: "Cheat: You control the buttons you press",
            zh: "“作弊”功能")]
        public CheatConfig Cheat { get; set; } = new();

        [ConfigComment(
            en: "Fix: Fix some potential issues",
            zh: "修复一些潜在的问题")]
        public FixConfig Fix { get; set; } = new();

        [ConfigComment(
            zh: "实用工具")]
        public UtilsConfig Utils { get; set; } = new();

        [ConfigComment(
            en: "Time Saving: Skip some unnecessary screens",
            zh: "节省一些不知道有用没用的时间，跳过一些不必要的界面")]
        public TimeSavingConfig TimeSaving { get; set; } = new();

        [ConfigComment(
            zh: "窗口相关设置")]
        public WindowStateConfig WindowState { get; set; } = new();

        [ConfigComment(
            en: "Custom camera ID settings",
            zh: "自定义摄像头 ID")]
        public CustomCameraIdConfig CustomCameraId { get; set; } = new();

        [ConfigComment(
            zh: "触摸灵敏度设置")]
        public TouchSensitivityConfig TouchSensitivity { get; set; } = new();

        [ConfigComment(
            zh: "自定义按键映射")]
        public CustomKeyMapConfig CustomKeyMap { get; set; } = new();

        public class CheatConfig
        {
            [ConfigComment(
                en: "Unlock normally event-only tickets",
                zh: "解锁游戏里所有可能的跑图券")]
            public bool TicketUnlock { get; set; }

            [ConfigComment(
                en: "Unlock maps that are not in this version",
                zh: "解锁游戏里所有的区域，包括非当前版本的（并不会帮你跑完）")]
            public bool MapUnlock { get; set; }

            [ConfigComment(
                en: "Unlock Utage without the need of DXRating 10000",
                zh: "不需要万分也可以进宴会场")]
            public bool UnlockUtage { get; set; }
        }

        public class UXConfig
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
                en: """
                    Provide the ability to use custom skins (advanced feature)
                    Load skin textures from LocalAssets\Skins
                    """,
                zh: """
                    提供自定义皮肤的能力（高级功能）
                    从 LocalAssets\Skins 中加载皮肤贴图
                    """)]
            public bool CustomSkins { get; set; }

            [ConfigComment(
                en: """
                    More detailed judgment display
                    Requires CustomSkins to be enabled and the resource file to be downloaded
                    https://github.com/hykilpikonna/AquaDX/releases/download/nightly/JudgeDisplay4B.7z
                    """,
                zh: """
                    更精细的判定表示
                    需开启 CustomSkins 并下载资源文件
                    https://github.com/hykilpikonna/AquaDX/releases/download/nightly/JudgeDisplay4B.7z
                    """)]
            public bool JudgeDisplay4B { get; set; }

            [ConfigComment(
                en: """
                    Custom track start difficulty image (not really custom difficulty)
                    Requires CustomSkins to be enabled
                    Will load four image resources through custom skins: musicBase, musicTab, musicLvBase, musicLvText
                    """,
                zh: """
                    自定义在歌曲开始界面上显示的难度贴图 (并不是真的自定义难度)
                    需要启用自定义皮肤功能
                    会通过自定义皮肤加载四个图片资源: musicBase, musicTab, musicLvBase, musicLvText
                    """)]
            public bool CustomTrackStartDiff { get; set; }

            [ConfigComment(
                en: "Map touch actions to buttons",
                zh: "映射触摸操作至实体按键")]
            public bool TouchToButtonInput { get; set; }

            [ConfigComment(
                en: """
                    Delayed the animation of the song start screen
                    For recording chart confirmation
                    """,
                zh: """
                    推迟了歌曲开始界面的动画
                    录制谱面确认用
                    """)]
            public bool TrackStartProcessTweak { get; set; }

            [ConfigComment(
                en: """
                    Disable the TRACK X text, DX/Standard display box, and the derakkuma at the bottom of the screen in the song start screen
                    For recording chart confirmation
                    """,
                zh: """
                    在歌曲开始界面, 把 TRACK X 字样, DX/标准谱面的显示框, 以及画面下方的滴蜡熊隐藏掉
                    录制谱面确认用
                    """)]
            public bool DisableTrackStartTabs { get; set; }

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

        public class FixConfig
        {
            [ConfigComment(
                en: "Allow login with higher data version",
                zh: """
                    原先如果你的账号版本比当前游戏设定的版本高的话，就会不能登录
                    开了这个选项之后就可以登录了，不过你的账号版本还是会被设定为当前游戏的版本
                    """)]
            public bool SkipVersionCheck { get; set; }

            [ConfigComment(
                zh: """
                    如果你在用未经修改的客户端，会默认加密到服务器的连接，而连接私服的时候不应该加密
                    开了这个选项之后就不会加密连接了，同时也会移除不同版本的客户端可能会对 API 接口加的后缀
                    正常情况下，请保持这个选项开启
                    """)]
            public bool RemoveEncryption { get; set; }

            [ConfigComment(
                zh: "如果要配置店内招募的话，应该要把这个关闭")]
            public bool ForceAsServer { get; set; } = true;

            [ConfigComment(
                en: "Force the game to be in FreePlay mode",
                zh: "强制改为免费游玩（FreePlay）")]
            public bool ForceFreePlay { get; set; } = true;

            [ConfigComment(
                en: "Force the game to be in PaidPlay mode with 24 coins locked, conflicts with ForceFreePlay",
                zh: "强制付费游玩并锁定 24 个币，和 ForceFreePlay 冲突")]
            public bool ForcePaidPlay { get; set; }

            [ConfigComment(
                en: "Add notes sprite to the pool to prevent use up",
                zh: "增加更多待命的音符贴图，防止奇怪的自制谱用完音符贴图池")]
            public int ExtendNotesPool { get; set; }

            [ConfigComment(
                en: "Force the frame rate limit to 60 FPS and disable vSync. Do not use if your game has no issues",
                zh: "强制设置帧率上限为 60 帧并关闭垂直同步。如果你的游戏没有问题，请不要使用")]
            public bool FrameRateLock { get; set; }

            [ConfigComment(
                en: """
                    Use Microsoft YaHei Bold to display characters not in the font library
                    Cannot be used together with CustomFont
                    """,
                zh: """
                    在显示字库里没有的字时使用微软雅黑 Bold 显示
                    不可以和 CustomFont 一起使用
                    """)]
            public bool FontFix { get; set; }

            [ConfigComment(
                en: """
                    Make the judgment display of Wifi Slide different in up and down (originally all Wifi judgment displays are towards the center), just like in majdata
                    The reason for this bug is that SEGA forgot to assign EndButtonId to Wifi
                    """,
                zh: """
                    这个 Patch 让 Wifi Slide 的判定显示有上下的区别 (原本所有 Wifi 的判定显示都是朝向圆心的), 就像 majdata 里那样
                    这个 bug 产生的原因是 SBGA 忘记给 Wifi 的 EndButtonId 赋值了
                    """)]
            public bool FanJudgeFlip { get; set; }

            [ConfigComment(
                en: """
                    This Patch makes the Critical judgment of BreakSlide also flicker like BreakTap
                    Recommended to use with custom skins (otherwise the visual effect may not be good)
                    """,
                zh: """
                    这个 Patch 让 BreakSlide 的 Critical 判定也可以像 BreakTap 一样闪烁
                    推荐与自定义皮肤一起使用 (否则视觉效果可能并不好)
                    """)]
            public bool BreakSlideJudgeBlink { get; set; }

            [ConfigComment(
                en: """
                    Make the AutoPlay random judgment mode really randomize all judgments (down to sub-judgments)
                    // The original random judgment will only produce all 15 judgment results from Miss(TooFast) ~ Critical ~ Miss(TooLate)
                    // Here, it is changed to a triangular distribution to produce all 15 judgment results from Miss(TooFast) ~ Critical ~ Miss(TooLate)
                    // Of course, it will not consider whether the original Note really has a corresponding judgment (such as Slide should not have non-Critical Prefect)
                    """,
                zh: """
                    让 AutoPlay 的随机判定模式真的会随机产生所有的判定 (精确到子判定)
                    // 原本的随机判定只会等概率产生 Critical, LateGreat1st, LateGood, Miss(TooLate)
                    // 这里改成三角分布产生从 Miss(TooFast) ~ Critical ~ Miss(TooLate) 的所有 15 种判定结果
                    // 当然, 此处并不会考虑原本那个 Note 是不是真的有对应的判定 (比如 Slide 实际上不应该有小 p 之类的)
                    """)]
            public bool RealisticRandomJudge { get; set; }

            [ConfigComment(
                en: "Cannot be used together with HideHanabi",
                zh: """
                    修复 1p 模式下的烟花大小
                    不能和 HideHanabi 一起使用
                    """)]
            public bool HanabiFix { get; set; }

            [ConfigComment(
                en: "Prevent gray network caused by mistakenly thinking it's an AimeDB server issue",
                zh: "防止因错误认为 AimeDB 服务器问题引起的灰网，建议开启")]
            public bool IgnoreAimeServerError { get; set; }
        }

        public class UtilsConfig
        {
            [ConfigComment(
                en: "Log user ID on login",
                zh: "登录时将 UserID 输出到日志")]
            public bool LogUserId { get; set; }

            [ConfigComment(
                en: "Globally increase A judgment, unit is the same as in the game",
                zh: "全局增加 A 判，单位和游戏里一样")]
            public float JudgeAdjustA { get; set; }

            [ConfigComment(
                en: "Globally increase B judgment, unit is the same as in the game",
                zh: "全局增加 B 判，单位和游戏里一样")]
            public float JudgeAdjustB { get; set; }

            [ConfigComment(
                en: "Touch screen delay, unit is milliseconds, one second = 1000 milliseconds. Must be an integer",
                zh: "触摸屏延迟，单位为毫秒，一秒 = 1000 毫秒。必须是整数")]
            public int TouchDelay { get; set; }

            [ConfigComment(
                en: """
                    Practice mode, activated by pressing Test in the game
                    Must be used together with TestProof
                    """,
                zh: """
                    练习模式，在游戏中按 Test 打开
                    必须和 TestProof 一起用
                    """)]
            public bool PractiseMode { get; set; }

            [ConfigComment(
                en: "Show detail of selected song in music selection screen",
                zh: "选歌界面显示选择的歌曲的详情")]
            public bool SelectionDetail { get; set; }

            [ConfigComment(
                en: "Show Network error detail in the game",
                zh: "出现灰网时显示原因")]
            public bool ShowNetErrorDetail { get; set; }

            [ConfigComment(
                en: "Show error log in the game",
                zh: "在游戏中显示错误日志窗口而不是关闭游戏进程")]
            public bool ShowErrorLog { get; set; }

            [ConfigComment(
                en: "Display framerate",
                zh: "显示帧率")]
            public bool FrameRateDisplay { get; set; }

            [ConfigComment(
                en: """
                    Adjust the baud rate of the touch screen serial port, default value is 9600
                    Requires hardware support. If you are unsure whether you can use it, you cannot use it
                    Set to 0 to disable
                    """,
                zh: """
                    调整触摸屏串口波特率，默认值 9600
                    需要硬件配合。如果你不清楚你是否可以使用，那你不能使用
                    改为 0 禁用
                    """)]
            public int TouchPanelBaudRate { get; set; }
        }

        public class TimeSavingConfig
        {
            [ConfigComment(
                en: "Skip the warning screen and logo shown after the POST sequence",
                zh: "跳过日服启动时候的 WARNING 界面")]
            public bool SkipWarningScreen { get; set; }

            [ConfigComment(
                en: "Disable some useless delays to speed up the game boot process",
                zh: """
                    在自检界面，每个屏幕结束的时候都会等两秒才进入下一个屏幕，很浪费时间
                    开了这个选项之后就不会等了
                    """)]
            public bool ImproveLoadSpeed { get; set; }

            [ConfigComment(
                en: "Directly enter the song selection screen after login",
                zh: "登录完成后直接进入选歌界面")]
            public bool SkipToMusicSelection { get; set; }

            [ConfigComment(
                en: "Skip possible prompts like \"New area discovered\", \"New songs added\", \"There are events\" during game login/registration",
                zh: "跳过登录 / 注册游戏时候可能的 “发现了新的区域哟” “乐曲增加” “有活动哟” 之类的提示")]
            public bool SkipEventInfo { get; set; }

            [ConfigComment(
                en: "Skip the \"Do not tap or slide vigorously\" screen, immediately proceed to the next screen once data is loaded",
                zh: "跳过“不要大力拍打或滑动哦”这个界面，数据一旦加载完就立马进入下一个界面")]
            public bool IWontTapOrSlideVigorously { get; set; }

            [ConfigComment(
                en: "Skip the \"Goodbye\" screen at the end of the game",
                zh: "跳过游戏结束的“再见”界面")]
            public bool SkipGameOverScreen { get; set; }

            [ConfigComment(
                en: "Skip TrackStart screen",
                zh: "跳过乐曲开始界面")]
            public bool SkipTrackStart { get; set; }

            [ConfigComment(
                en: "Show a \"skip\" button like AstroDX after the notes end",
                zh: "音符结束之后显示像 AstroDX 一样的“跳过”按钮")]
            public bool ShowQuickEndPlay { get; set; }
        }

        public class WindowStateConfig
        {
            [ConfigComment(
                en: "If not enabled, no operations will be performed on the game window",
                zh: "不启用的话，不会对游戏窗口做任何操作")]
            public bool Enable { get; set; }

            [ConfigComment(
                en: "Window the game",
                zh: "窗口化游戏")]
            public bool Windowed { get; set; }

            [ConfigComment(
                en: """
                    Width and height for windowed mode, rendering resolution for fullscreen mode
                    If set to 0, windowed mode will remember the user-set size, fullscreen mode will use the current display resolution
                    """,
                zh: """
                    宽度和高度窗口化时为游戏窗口大小，全屏时为渲染分辨率
                    如果设为 0，窗口化将记住用户设定的大小，全屏时将使用当前显示器分辨率
                    """)]
            public int Width { get; set; }

            [ConfigComment(
                zh: "高度")]
            public int Height { get; set; }
        }

        public class CustomCameraIdConfig
        {
            [ConfigComment(
                en: """
                    Enable custom CameraId
                    If enabled, you can customize the game to use the specified camera
                    """,
                zh: """
                    是否启用自定义摄像头ID
                    启用后可以指定游戏使用的摄像头
                    """)]
            public bool Enable { get; set; }

            [ConfigComment(
                en: "Print the camera list to the log when starting, can be used as a basis for modification",
                zh: "启动时打印摄像头列表到日志中，可以作为修改的依据")]
            public bool PrintCameraList { get; set; } = false;

            [ConfigComment(
                en: "DX Pass 1P",
                zh: "DX Pass 1P")]
            public int LeftQrCamera { get; set; } = 0;

            [ConfigComment(
                en: "DX Pass 2P",
                zh: "DX Pass 2P")]
            public int RightQrCamera { get; set; } = 0;

            [ConfigComment(
                zh: "玩家摄像头")]
            public int PhotoCamera { get; set; } = 0;

            [ConfigComment(
                zh: "二维码扫描摄像头")]
            public int ChimeCamera { get; set; } = 0;
        }

        public class TouchSensitivityConfig
        {
            [ConfigComment(
                en: """
                    Enable custom sensitivity
                    When enabled, the settings in Test mode will not take effect
                    When disabled, the settings in Test mode can still be used
                    """,
                zh: """
                    是否启用自定义灵敏度
                    这里启用之后 Test 里的就不再起作用了
                    这里禁用之后就还是可以用 Test 里的调
                    """)]
            public bool Enable { get; set; }

            [ConfigComment(
                en: """
                    Sensitivity adjustments in Test mode are not linear
                    Default sensitivity in area A: 90, 80, 70, 60, 50, 40, 30, 26, 23, 20, 10
                    Default sensitivity in other areas: 70, 60, 50, 40, 30, 20, 15, 10, 5, 1, 1
                    A setting of 0 in Test mode corresponds to 40, 20 here, -5 corresponds to 90, 70, +5 corresponds to 10, 1
                    The higher the number in Test mode, the lower the number here, resulting in higher sensitivity for official machines
                    For ADX, the sensitivity is reversed, so the higher the number here, the higher the sensitivity
                    """,
                zh: """
                    在 Test 模式下调整的灵敏度不是线性的
                    A 区默认灵敏度 90, 80, 70, 60, 50, 40, 30, 26, 23, 20, 10
                    其他区域默认灵敏度 70, 60, 50, 40, 30, 20, 15, 10, 5, 1, 1
                    Test 里设置的 0 对应的是 40, 20 这一档，-5 是 90, 70，+5 是 10, 1
                    Test 里的数字越大，这里的数字越小，对于官机来说，灵敏度更大
                    而 ADX 的灵敏度是反的，所以对于 ADX，这里的数字越大，灵敏度越大
                    """)]
            public byte A1 { get; set; } = 40;

            public byte A2 { get; set; } = 40;
            public byte A3 { get; set; } = 40;
            public byte A4 { get; set; } = 40;
            public byte A5 { get; set; } = 40;
            public byte A6 { get; set; } = 40;
            public byte A7 { get; set; } = 40;
            public byte A8 { get; set; } = 40;
            public byte B1 { get; set; } = 20;
            public byte B2 { get; set; } = 20;
            public byte B3 { get; set; } = 20;
            public byte B4 { get; set; } = 20;
            public byte B5 { get; set; } = 20;
            public byte B6 { get; set; } = 20;
            public byte B7 { get; set; } = 20;
            public byte B8 { get; set; } = 20;
            public byte C1 { get; set; } = 20;
            public byte C2 { get; set; } = 20;
            public byte D1 { get; set; } = 20;
            public byte D2 { get; set; } = 20;
            public byte D3 { get; set; } = 20;
            public byte D4 { get; set; } = 20;
            public byte D5 { get; set; } = 20;
            public byte D6 { get; set; } = 20;
            public byte D7 { get; set; } = 20;
            public byte D8 { get; set; } = 20;
            public byte E1 { get; set; } = 20;
            public byte E2 { get; set; } = 20;
            public byte E3 { get; set; } = 20;
            public byte E4 { get; set; } = 20;
            public byte E5 { get; set; } = 20;
            public byte E6 { get; set; } = 20;
            public byte E7 { get; set; } = 20;
            public byte E8 { get; set; } = 20;
        }

        public class CustomKeyMapConfig
        {
            [ConfigComment(
                en: "These settings will work regardless of whether you have enabled segatools' io4 emulation",
                zh: "这里的设置无论你是否启用了 segatools 的 io4 模拟都会工作")]
            public bool Enable { get; set; }

            public KeyCodeID Test { get; set; } = (KeyCodeID)115;
            public KeyCodeID Service { get; set; } = (KeyCodeID)5;
            public KeyCodeID Button1_1P { get; set; } = (KeyCodeID)67;
            public KeyCodeID Button2_1P { get; set; } = (KeyCodeID)49;
            public KeyCodeID Button3_1P { get; set; } = (KeyCodeID)48;
            public KeyCodeID Button4_1P { get; set; } = (KeyCodeID)47;
            public KeyCodeID Button5_1P { get; set; } = (KeyCodeID)68;
            public KeyCodeID Button6_1P { get; set; } = (KeyCodeID)70;
            public KeyCodeID Button7_1P { get; set; } = (KeyCodeID)45;
            public KeyCodeID Button8_1P { get; set; } = (KeyCodeID)61;
            public KeyCodeID Select_1P { get; set; } = (KeyCodeID)25;
            public KeyCodeID Button1_2P { get; set; } = (KeyCodeID)80;
            public KeyCodeID Button2_2P { get; set; } = (KeyCodeID)81;
            public KeyCodeID Button3_2P { get; set; } = (KeyCodeID)78;
            public KeyCodeID Button4_2P { get; set; } = (KeyCodeID)75;
            public KeyCodeID Button5_2P { get; set; } = (KeyCodeID)74;
            public KeyCodeID Button6_2P { get; set; } = (KeyCodeID)73;
            public KeyCodeID Button7_2P { get; set; } = (KeyCodeID)76;
            public KeyCodeID Button8_2P { get; set; } = (KeyCodeID)79;
            public KeyCodeID Select_2P { get; set; } = (KeyCodeID)84;
        }
    }
}
