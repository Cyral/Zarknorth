using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace ZarknorthClient
{
    public class Command
    {
        public static List<Command> Commands = new List<Command>();
        private static Color ErrorColor = Color.Red;

        public string Name { get; set; }
        public string Description { get; set; }
        public string Usage { get; set; }

        public event ExecuteCommandEventHandler Execute;
        public virtual ConsoleMessage OnExecute(ExecuteCommandEventArgs e)
        {
            if (Execute != null)
            {
                if (CorrectArgs(e.Arguments))
                return Execute(this, e);
                else return IncorrectArgs();
            }
            else return new ConsoleMessage("No action defined for this command", 2, ErrorColor);
        }

        private ConsoleMessage IncorrectArgs()
        {
            return new ConsoleMessage("Incorrect arguments! Usage: /" + Name + " " + Usage, 2, ErrorColor);
        }

        public static ConsoleMessage ProcessCommand(string buffer)
        {
            string commandName = GetCommandName(buffer);
            Command command = Commands.Where(c => c.Name == commandName).FirstOrDefault();
            var arguments = GetArguments(buffer);
            if (command == null)
                return new ConsoleMessage("Error: Command not found!", 2, ErrorColor);
            ConsoleMessage commandOutput;
            try
            {
                commandOutput = command.OnExecute(new ExecuteCommandEventArgs(arguments));
            }
            catch (Exception ex)
            {
                commandOutput = new ConsoleMessage("Error: " + ex.Message, 2, ErrorColor);
            }
            return commandOutput;
        }
        static string GetCommandName(string buffer)
        {
            var firstSpace = buffer.IndexOf(' ');
            return buffer.Substring(0, firstSpace < 0 ? buffer.Length : firstSpace);
        }
        static string[] GetArguments(string buffer)
        {
            var firstSpace = buffer.IndexOf(' ');
            if (firstSpace < 0)
            {
                return new string[0];
            }

            var args = buffer.Substring(firstSpace, buffer.Length - firstSpace).Split(' ');
            return args.Where(a => a != "").ToArray();
        }
        private bool CorrectArgs(string[] args)
        {
            if (Usage == string.Empty)
                return args == null || args.Count() == 0;

            string[] realArgs = Usage.Split(' ');

            bool[] correct = new bool[realArgs.Length];
            for (int i = 0; i < realArgs.Length; i++)
            {
                if ((i >= args.Count() && ((realArgs[i][0] == '[' && realArgs[i][realArgs[i].Length - 1] == ']'))))
                {
                    correct[i] = true;
                        continue;
                }
                if ((i >= args.Count() || string.IsNullOrWhiteSpace(args[i])))
                {
                    correct[i] = false;
                        continue;
            }

                string realArg = realArgs[i];
                string arg = args[i];

                bool required = realArg[0] == '<' && realArg[realArg.Length - 1] == '>';
                bool optional = realArg[0] == '[' && realArg[realArg.Length - 1] == ']';

                if (required)
                    correct[i] = i <= args.Length;
                else if (optional)
                    correct[i] = true;
            }
            return correct.All(o => o);
        }

        public Command(string name, string description, string usage)
        {
            Name = name;
            Description = description;
            Usage = usage;
            Commands.Add(this);
        }

        public static Command AutoSave,Weather, ClearInventory, GiveItem, FullBright, Help, ReloadContent, ItemList, Suicide, Heal, Spawn, Time, SettleLiquid;
        static Command()
        {
            AutoSave = new Command(
    "autosave",
    "",
    "");
            AutoSave.Execute += new ExecuteCommandEventHandler(delegate(object o, ExecuteCommandEventArgs e)
            {
                Game.level.AutoSave(null,null);
                return new ConsoleMessage("Autosaving", 2);
            });
            SettleLiquid = new Command(
                "settleliquid",
                "Settles all liquids using 1000 iterations by default. May take a while.",
                "[iterations]");
            SettleLiquid.Execute += new ExecuteCommandEventHandler(delegate(object o, ExecuteCommandEventArgs e)
            {
                int iterations = 1000;
                if (e.Arguments.Length > 0 && e.Arguments[0] != null)
                    int.TryParse(e.Arguments[0], out iterations);
                new Thread((ThreadStart)delegate {
                    Game.level.LiquidManager.SettleLiquids(iterations);
                }).Start();
                return new ConsoleMessage("Settling Liquids...", 2);
            });
            Time = new Command(
                "time",
                "Sets the world time in milliseconds, or to a predefined time.",
                "[milliseconds|day/night/morning/evening|hour(am/pm)]");
            Time.Execute += new ExecuteCommandEventHandler(delegate(object o, ExecuteCommandEventArgs e)
            {
                int time = 0;
                if (int.TryParse(e.Arguments[0], out time))
                {
                    Game.level.elapsedTime = time;
                }
                else if (e.Arguments[0].ToLower().Contains("am"))
                {
                    e.Arguments[0] = e.Arguments[0].Remove(e.Arguments[0].Length - 2, 2);
                    if (int.TryParse(e.Arguments[0], out time))
                        time = 18 + (time) >= 24 ? (18 + (time)) - 24 : 18 + (time);
                    Game.level.elapsedTime = time * 60 * 60;
                }
                else if (e.Arguments[0].ToLower().Contains("pm"))
                {
                    e.Arguments[0] = e.Arguments[0].Remove(e.Arguments[0].Length - 2, 2);
                    if (int.TryParse(e.Arguments[0], out time))
                        Game.level.elapsedTime = (6 + (time)) * 60 * 60;
                }
                else
                {
                    if (e.Arguments[0].Equals("day", StringComparison.CurrentCultureIgnoreCase))
                        Game.level.elapsedTime = 6 * 60 * 60;
                    else if (e.Arguments[0].Equals("night", StringComparison.CurrentCultureIgnoreCase))
                        Game.level.elapsedTime = 18 * 60 * 60;
                    else if (e.Arguments[0].Equals("morning", StringComparison.CurrentCultureIgnoreCase))
                        Game.level.elapsedTime = 1 * 60 * 60;
                    else if (e.Arguments[0].Equals("evening", StringComparison.CurrentCultureIgnoreCase))
                        Game.level.elapsedTime = 12 * 60 * 60;
                }
                Game.level.isDay = Game.level.elapsedTime < Level.MaxTime / 2;
                Game.level.ComputeLighting = true;

                return new ConsoleMessage(string.Empty, 2);
            });
            Weather = new Command(
            "weather",
            "Changes the current weather.",
            "[Normal/Rain/Storm/Snow]");
            Weather.Execute += new ExecuteCommandEventHandler(delegate(object o, ExecuteCommandEventArgs e)
            {
                if (e.Arguments.Length == 0 || string.IsNullOrEmpty(e.Arguments[0]))
                    return new ConsoleMessage("Please specify a weather type. (Currently: " + Game.level.CurrentWeather.ToString() + ")", 2);
                WeatherType type = (WeatherType)Enum.Parse(typeof(WeatherType), e.Arguments[0], true);
                Game.level.CurrentWeather = type;
                return new ConsoleMessage(string.Empty, 2);
            });
            Suicide = new Command(
                "suicide",
                "Kills the player and respawns.",
                "");
            Suicide.Execute += new ExecuteCommandEventHandler(delegate(object o, ExecuteCommandEventArgs e)
            {
                Game.level.Player.Kill(Entities.DamageType.Self);
                return new ConsoleMessage(string.Empty, 2);
            });
            Heal = new Command(
                "heal",
                "Heals the player.",
                "");
            Heal.Execute += new ExecuteCommandEventHandler(delegate(object o, ExecuteCommandEventArgs e)
            {
                Game.level.Player.Heal();
                return new ConsoleMessage(Game.UserName + " has been [color:Green]healed[/color].", 2, Color.White);
            });
            Spawn = new Command(
                "spawn",
                "Teleports the player to spawn.",
                "");
            Spawn.Execute += new ExecuteCommandEventHandler(delegate(object o, ExecuteCommandEventArgs e)
            {
                Game.level.Player.Respawn();
                return new ConsoleMessage("Teleported to spawn.", 2);
            });
            ClearInventory = new Command(
                "clearinventory",
                "Clears the player's inventory.",
                "");
            ClearInventory.Execute += new ExecuteCommandEventHandler(delegate(object o, ExecuteCommandEventArgs e)
            {
                foreach (Slot s in Game.level.Player.Inventory)
                {
                    s.Stack = 0;
                    s.Item = Item.Blank;
                }
                return new ConsoleMessage("Inventory Cleared.", 2);
            });

            FullBright = new Command(
                "fullbright",
                "Toggles FullBright mode, in which there is no lighting.",
                "");
            FullBright.Execute += new ExecuteCommandEventHandler(delegate(object o, ExecuteCommandEventArgs e)
            {
                Level.FullBright = !Level.FullBright;
                string mode = Level.FullBright ? "enabled." : "disabled.";
                return new ConsoleMessage("FullBright mode " + mode, 2);
            });

            Help = new Command(
                "help",
                "Lists the availible commands, or information on a specific command.",
                "[command]");
            Help.Execute += new ExecuteCommandEventHandler(delegate(object o, ExecuteCommandEventArgs e)
            {
                if (e.Arguments != null && e.Arguments.Length >= 1)
                {
                    var command = Commands.Where(c => c.Name != null && c.Name == e.Arguments[0]).FirstOrDefault();
                    if (command != null)
                    {
                        return new ConsoleMessage(String.Format("[color:white]-{0}: {1}{2}[/color]", command.Name, command.Description, String.Format("&n    /{0} [color:gray]{1}[/color]", command.Name, command.Usage)), 2);

                    }
                    return new ConsoleMessage("Error: Command " + e.Arguments[0] + " not found.", 2, ErrorColor);
                }
                var help = new StringBuilder();
                help.Append("List of commands:");
                Commands.Sort((x, y) => string.Compare(x.Name, y.Name));
                foreach (Command command in Commands)
                {
                    help.Append(String.Format("&n-[color:white]{0}: {1}[/color]&n    /{0} [color:gray]{2}[/color]", command.Name, command.Description, command.Usage));
                }
                return new ConsoleMessage(help.ToString(), 2);
            });
            ItemList = new Command(
                "itemdb",
                "Displays all of the items, or the specified item, and information about it.",
                "[item]");
            ItemList.Execute += new ExecuteCommandEventHandler(delegate(object o, ExecuteCommandEventArgs e)
            {
                if (e.Arguments != null && e.Arguments.Length >= 1)
                {
                    var item = Item.ItemList.Where(c => c.Name != null && c.Name.Equals(e.Arguments[0], StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    if (item != null)
                    {
                        string prt = "";
                        foreach (string str in item.OnPrintStats(new PrintItemDataEventArgs()))
                            prt += "&n    [color:lightgray]" + str + "[/color]";
                        return new ConsoleMessage(String.Format("[color:white]Item: {0} ID: {1}{2}[/color]", item.Name, item.ID, String.Format("&n    [color:lightgray]{0}[/color]{1}", item.Description, prt)), 2);
                    }
                    return new ConsoleMessage("Error: Command " + e.Arguments[0] + " not found.", 2, ErrorColor);
                }
                var items = new StringBuilder();
                items.Append("List of Items:");
                foreach (Item item in Item.ItemList)
                {
                    items.Append(String.Format("&n-[color:white]{0} - {1}[/color]&n    [color:gray]{2}[/color]", item.Name, item.ID, item.Description));
                }
                return new ConsoleMessage(items.ToString(), 2);
            });
            #if DEBUG
            ReloadContent = new Command(
               "reloadcontent",
               "Reloads the tile textures - For debugging.",
               "");
            ReloadContent.Execute += new ExecuteCommandEventHandler(delegate(object o, ExecuteCommandEventArgs e)
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                ContentPack.Textures.Clear();
                IO.LoadContentPacks(Game.level.game);
                Game.level.SetItems(Game.level.game.GraphicsDevice);
                stopwatch.Stop();
                return new ConsoleMessage("Tile Content Reloaded. [color:SkyBlue](" + stopwatch.Elapsed.TotalSeconds + "s)[/color]", 2);
            });
            #endif
            GiveItem = new Command(
                "giveitem",
                "Gives the player the specified item.",
                "<name|id> [amount]");
            GiveItem.Execute += new ExecuteCommandEventHandler(delegate(object o, ExecuteCommandEventArgs e)
            {
                int id = 0;
                if (int.TryParse(e.Arguments[0], out id))
                {
                    Slot s = Game.level.Players[0].Inventory.First(i => i.Item == Item.Blank);
                    Item item = Item.Blank;
                    if (Item.ItemList.Exists(it => it.ID == id))
                        item = Item.FindItem(id);
                    else
                        return new ConsoleMessage("Item with id " + id + " not found", 2, ErrorColor);
                    s.Item = item;
                    int amount = item.MaxStack;
                    if (e.Arguments.Count() == 2)
                    {
                        if (!int.TryParse(e.Arguments[1], out amount))
                            return new ConsoleMessage("Error: Amount must be a number!", 2, ErrorColor);
                    }
                    s.Stack = amount;
                    string plural = s.Stack > 1 ? s.Item.Name + "(s)" : s.Item.Name;
                    return new ConsoleMessage(s.Stack.ToString() + " " + plural + " given to " + Game.UserName, 2);
                }
                else
                {
                    string name = e.Arguments[0];
                    Slot s = Game.level.Players[0].Inventory.First(i => i.Item == Item.Blank);
                    Item item = Item.Blank;
                    if (Item.ItemList.Exists(it => it.Name.ToLower() == name.ToLower().Replace('_', ' ')))
                        item = Item.ItemList.Find(it => it.Name.ToLower() == name.ToLower().Replace('_', ' '));
                    else
                        return new ConsoleMessage("Item \"" + name + "\" not found", 2, ErrorColor);
                    s.Item = item;
                    int amount = s.Item.MaxStack;
                    if (e.Arguments.Count() == 2)
                    {
                        if (!int.TryParse(e.Arguments[1], out amount))
                            return new ConsoleMessage("Error: Amount must be a number!", 2, ErrorColor);
                    }
                    s.Stack = amount;
                    string plural = s.Stack > 1 ? s.Item.Name + "(s)" : s.Item.Name;
                    return new ConsoleMessage(s.Stack.ToString() + " " + plural + " given to " + Game.UserName, 2);
                }
            });
        }

        public delegate ConsoleMessage ExecuteCommandEventHandler(object o, ExecuteCommandEventArgs e);
        public class ExecuteCommandEventArgs : System.EventArgs
        {
            public readonly string[] Arguments;
            public ExecuteCommandEventArgs(string[] arguments)
            {
                Arguments = arguments;
            }
        }
    }
}
