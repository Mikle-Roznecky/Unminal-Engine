namespace Unminal.Core.Commands.Struct;
public class Command {
    public string? Name {get; set;}
    public List<Command> Layer {get; set;} = new List<Command>(); 
    public string? ExecuteMetod {get; set;}
    public bool ExecutedLayer {get; set;}
    public Dictionary<string, string> ArgsExecuteMetod {get; set;} = new Dictionary<string, string>();
    public Dictionary<string, Dictionary<string, string>> AdditionalArgs {get; set;} = new Dictionary<string, Dictionary<string, string>>();
    public CastomArgs? castomArgs {get; set;}
    public void error(string reason){} // logic for error metod
    public void range(int s, int e){} // logic for range metod
    public void get(string key){} // logic for get metod
}
public class CastomArgs { 
    // Creates a User
}
// Handmade work (Dov1ntc)
// It should be automated in the future.
public class CommandExecutorConfig {    
    public static void Main23() {
        Command server = new Command {
            Name = "server",
            Layer = new List<Command> {
                new Command {
                    Name = "open",
                    ExecuteMetod = "GameServerOpen",
                    
                    ArgsExecuteMetod = new Dictionary<string, string> {
                        {"port", """USER_VEL_CACOETO_CHISLO?8008"""}, 
                        {"limitUser", """LIMIT_NA_VHODIASHIH?-1"""}
                    },
                    
                    AdditionalArgs = new Dictionary<string, Dictionary<string, string>> {
                        {
                            "port", new Dictionary<string, string> {
                                {"input", """USER_VEL_CACOETO_CHISLO"""},
                                {"configInput", """["type:int", "lim:range(1,99999)"]"""}
                            }},
                            { 
                            "limituser", new Dictionary<string, string>  {
                                {"input", """LIMIT_NA_VHODIASHIH"""},
                                {"configInput", """["type:int", "lim:-1/1+"]"""}
                            }
                        }
                    }
                },
                new Command {
                    Name = "player",
                    Layer = new List<Command> {
                        new Command() {
                            Name = "kick",
                            ExecuteMetod = "KickPlayerFromServer",
                            ArgsExecuteMetod = new Dictionary<string, string>{
                                {"targetId", """PLAYER_UID_TOKEN?error('Cant kick all players')"""},
                                {"reasonCode", """KICK_REASON_CODE?0"""},
                                {"banDuration", """BAN_DURATION_VAL?-1"""}
                            },
                            AdditionalArgs = new Dictionary<string, Dictionary<string, string>> 
                            {
                                { 
                                    "targetId", new Dictionary<string, string> {
                                        {"input", """PLAYER_UID_TOKEN"""},
                                        {"configInput", """["type:int", "lim:-1/1+"]"""}
                                    } 
                                },
                                { 
                                    "reasoncode", new Dictionary<string, string> {
                                        {"input", """KICK_REASON_CODE"""},
                                        {"configInput", """["type:int", "lim:range(0,99)"]"""}
                                    } 
                                },
                                { 
                                    "banduration", new Dictionary<string, string> {
                                        {"input", """BAN_DURATION_VAL"""},
                                        {"configInput", """["type:time", "lim:5m+"]"""}
                                    } 
                                }
                            }
                        },
                        new Command() {
                            Name = "ignore",
                            ExecuteMetod = "PlayerIgnorePlayer",
                            ArgsExecuteMetod = new Dictionary<string, string>{
                                {"targetPlayer", "PLAYER_NICKNAME?PLAYER_UID_TOKEN?error('Who should I block?')"},
                                {"senderPlayer", """get(command.sender.UID)"""}
                            },
                            AdditionalArgs = new Dictionary<string, Dictionary<string, string>>
                            {
                                {
                                    "target", new Dictionary<string, string> {
                                        {"input", """PLAYER_NICKNAME"""},
                                        {"configInput", """["type:str", "lim:range(3,15)"]"""}
                                    }
                                },
                                {
                                    "targetUID", new Dictionary<string, string> {
                                        {"input", """PLAYER_UID_TOKEN"""},
                                        {"configInput", """["type:str", "lim:range(3,15)"]"""}
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
    }
}