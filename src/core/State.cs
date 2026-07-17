namespace Unminal.Core.State;

public static class Engine {
    public static Vector2i WindowSize {get; set;}
    public static float DeltaTime {get; set;}
    public static double TotalTime {get; set;}
    public static KeyboardState? CurrentKeyboard {get; set;}
    public static MouseState? CurrentMouse {get; set;}
    public static bool IsPaused {get; set;}
    public static bool IsConsoleOpen {get; set;}
    public static bool IsDebugOpen {get; set;}
    public static bool CanF3 {get; set;}
    public static bool IsDebug {get; set;}

    // idk why im added this but perhaps in the 
    // future the meaning and uniqueness engine will be 
    // that it is easy to create a network division (maybe)
    public static class Player {
        public static string? userName {get; set;}
        public static string? id {get; set;}
        public static string? language {get; set;}
    }

    public static class ExtensionData {
        // here may be a engine "extensions" data (maybe)
    };

    public static class Paths {
        public static readonly string[] BaseSkyBoxAssets = {
            "./Assets/SkyBox/right.png",
            "./Assets/SkyBox/left.png",
            "./Assets/SkyBox/top.png",
            "./Assets/SkyBox/bottom.png",
            "./Assets/SkyBox/front.png",
            "./Assets/SkyBox/back.png"
        };
        public class Config {
            private static readonly Dictionary<string, string> _ = new() {
                ["CommandConfigJ"] = "./Assets/data/CommandExecutorConfig.json", 
                ["ConsoleHistoryL"] = "./Assets/data/ConsoleHistory.log",
                ["ConsoleHistoryJ"] = "./Assets/data/ConsoleHistory.json"
            };
            public static string CommandConfigJ => _["CommandConfigJ"];
            public static string ConsoleHistoryL => _["ConsoleHistoryL"]; 
            public static string ConsoleHistoryJ => _["ConsoleHistoryJ"];
        }
        public class Shaders {
            private static readonly Dictionary<string, string> _ = new() {
                ["mainV"] = "./Assets/shaders/main/shader.vert",
                ["mainF"] = "./Assets/shaders/main/shader.frag",

                ["skyboxV"] = "./Assets/shaders/SkyBox/shader.vert",
                ["skyboxF"] = "./Assets/shaders/SkyBox/shader.frag",

                ["textV"] = "./Assets/shaders/text/shader.vert",
                ["textF"] = "./Assets/shaders/text/shader.frag",

                ["baseV"] = "./Assets/shaders/base.vert",
                ["baseF"] = "./Assets/shaders/base.frag"
            };
            public static string mainV => _["mainV"];
            public static string mainF => _["mainF"];

            public static string skyboxV => _["skyboxV"];
            public static string skyboxF => _["skyboxF"];

            public static string textV => _["textV"];
            public static string textF => _["textF"];

            public static string baseV => _["baseV"];
            public static string baseF => _["baseF"];
        }

        public class Fonts {
            private static readonly Dictionary<string, string> _ = new(){
                ["Metroplex_Shadow"] = "./Assets/fonts/Metroplex-Shadow.ttf",
                ["PFAgoraSlabPro_Bold"] = "./Assets/fonts/PFAgoraSlabPro-Bold.ttf",
                ["VCR_OSD_MONO"] = "./Assets/fonts/VCR-OSD-MONO.ttf"
            };
            public static string Metroplex_Shadow => _["Metroplex_Shadow"];
            public static string PFAgoraSlabPro_Bold => _["PFAgoraSlabPro_Bold"];
            public static string VCR_OSD_MONO => _["VCR_OSD_MONO"];
        }
    }
}