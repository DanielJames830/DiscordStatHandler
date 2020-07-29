using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DiscordStatHandler.Commands
{
    public class SheetManager : BaseCommandModule
    {
        [Command("print")]
        public async Task Print(CommandContext ctx, string name)
        {

            StatSheet stat = GetCharacterSheet(name);

            if (stat == null)
                await ctx.Channel.SendMessageAsync(ctx.User.Mention + ": a character by the name of " + name + " does not exist.").ConfigureAwait(false);


            else
            {
                Console.WriteLine("Retreieved");
                await ctx.Channel.SendMessageAsync(ConvertStatToString(stat)).ConfigureAwait(false);
                Console.WriteLine("Test" + stat.debug);
            }
                
        }

        [Command("import")]
        public async Task Import(CommandContext ctx)
        {
            foreach (var _file in ctx.Message.Attachments)
            {
                if (!_file.FileName.Contains(".stat"))
                    await ctx.Channel.SendMessageAsync(ctx.User.Mention + ": this isn't a stat sheet!");
                else
                {
                    DownloadCharacterSheet(_file, ctx);
                    await ctx.Channel.SendMessageAsync(ctx.User.Mention + ": " + _file.FileName + " has been uploaded!");
                }
            }
        }

        [Command("export")]
        public async Task Export(CommandContext ctx, string name)
        {
            StatSheet stat = GetCharacterSheet(name);
            FileInfo fi = new FileInfo(@stat.Path);
            if (fi.Exists)
            {
                await ctx.Channel.SendMessageAsync(ctx.User.Mention + ": Here is your file");
                await ctx.RespondWithFileAsync(@stat.Path).ConfigureAwait(false);
            }

            else
                await ctx.Channel.SendMessageAsync(ctx.User.Mention + ": Hmmm, I don't see " + name + " in my database.");

        }

        [Command("save")]
        public async Task Save(CommandContext ctx, string name)
        {
            var stat = GetCharacterSheet(name);
            SaveCharacterSheet(stat, ctx);
            await ctx.Channel.SendMessageAsync("File saved").ConfigureAwait(false);
        }

        [Command("new")]
        public async Task New(CommandContext ctx)
        {
            var commandInit = ctx.User.Username;
            var stat = new StatSheet();
            stat.madeByDiscord = true;
            var interactivity = ctx.Client.GetInteractivity();

        name:
            await ctx.Channel.SendMessageAsync("Let's get started! Firstly, what is your character's name?");
            var name = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author.Username == commandInit);
            stat.Name = name.Result.Content;

        race:
            await ctx.Channel.SendMessageAsync("What is your character's race?");
            var race = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author.Username == commandInit);
            stat.Race = race.Result.Content;

        gender:
            await ctx.Channel.SendMessageAsync("What is your character's gender?");
            var gender = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author.Username == commandInit);
            stat.Gender = gender.Result.Content;

        level:
            await ctx.Channel.SendMessageAsync("What level are they?");
            var input = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author.Username == commandInit);
            try
            {
                var level = Int32.Parse(input.Result.Content);
                stat.Level = level;

            }

            catch
            {
                await ctx.Channel.SendMessageAsync("Please only type digits!");
                goto level;
            }

            if (stat.Level >= 3)
            {
            architype:
                await ctx.Channel.SendMessageAsync("What is their architype?");
                var architype = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author.Username == commandInit);
                stat.Architype = architype.Result.Content;
            }

        health:
            stat.Health = 4 + (stat.Level - 1);
            await ctx.Channel.SendMessageAsync("How much health do they have?  Suggested amount: " + stat.Health);
            var input1 = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author.Username == commandInit);
            try
            {
                var health = Int32.Parse(input1.Result.Content);
                stat.Health = health;

            }

            catch
            {
                await ctx.Channel.SendMessageAsync("Please only type digits!");
                goto health;
            }

        stats:
            await ctx.Channel.SendMessageAsync("What stats do they have? (please format like this:)" + System.Environment.NewLine + "statName:1" + System.Environment.NewLine + "statName:1" + System.Environment.NewLine + "statName:1");
            var input2 = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author.Username == commandInit);
            var statblock = input2.Result.Content.Split("\n");
            stat.statblock = new System.Collections.Generic.Dictionary<string, int>();
            foreach (var item in statblock)
            {
                var newstat = item.Split(":");

                try
                {

                    stat.statblock.Add(newstat[0], int.Parse(newstat[1]));
                }

                catch
                {
                    await ctx.Channel.SendMessageAsync("Unable to read stats");
                    stat.statblock = new System.Collections.Generic.Dictionary<string, int>();
                    goto stats;
                }
            }

        focuses:

            await ctx.Channel.SendMessageAsync("What focuses do they have? (please format like this:)" + System.Environment.NewLine + "focusName:1" + System.Environment.NewLine + "focusName:1" + System.Environment.NewLine + "focusName:1");
            var input3 = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author.Username == commandInit);
            var focuses = input3.Result.Content.Split("\n");
            List<string> focus = new List<string>();
            List<int> focuslevel = new List<int>();
            foreach (var item in focuses)
            {
                var newfocus = item.Split(":");

                try
                {

                    focus.Add(newfocus[0]);
                    stat.Focuses = focus.ToArray();



                    focuslevel.Add(int.Parse(newfocus[1]));
                    stat.FocusLevels = focuslevel.ToArray();
                }

                catch
                {
                    await ctx.Channel.SendMessageAsync("Unable to read focuses");

                    goto focuses;
                }
            }

        abilities:
            await ctx.Channel.SendMessageAsync("What abilities do they have? (please format like this:)" + System.Environment.NewLine + "abilityName1" + System.Environment.NewLine + "abilityName2" + System.Environment.NewLine + "abilityName3");
            var input4 = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author.Username == commandInit);
            var abilities = input4.Result.Content.Split("\n");
            stat.Abilities = abilities;


        inventory:
            await ctx.Channel.SendMessageAsync("What stats do they have? (please format like this:)" + System.Environment.NewLine + "itemName:1" + System.Environment.NewLine + "itemName:1" + System.Environment.NewLine + "itemName:1");
            var input5 = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author.Username == commandInit);
            var inventory = input5.Result.Content.Split("\n");
            stat.inventory = new System.Collections.Generic.Dictionary<string, int>();
            foreach (var item in inventory)
            {
                var newitem = item.Split(":");

                try
                {

                    stat.inventory.Add(newitem[0], int.Parse(newitem[1]));
                }

                catch
                {
                    await ctx.Channel.SendMessageAsync("Unable to read inventory");
                    stat.inventory = new System.Collections.Generic.Dictionary<string, int>();
                    goto inventory;
                }
            }


            SaveCharacterSheet(stat, ctx);
            await ctx.Channel.SendMessageAsync("Complete!");
        }


        [Command("add")]
        public async Task add(CommandContext ctx, string field, string character)
        {
            var commandInit = ctx.User.Username;
            var interactivity = ctx.Client.GetInteractivity();
            var stat = GetCharacterSheet(character);

            if (field == "focus")
            {
            focuses:
                await ctx.Channel.SendMessageAsync("What focuses would you like to add? (please format like this:)" + System.Environment.NewLine + "focusName:1" + System.Environment.NewLine + "focusName:1" + System.Environment.NewLine + "focusName:1");
                var input3 = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author.Username == commandInit);
                var focuses = input3.Result.Content.Split("\n");
                List<string> focus = stat.Focuses.ToList<string>();
                List<int> focuslevel = stat.FocusLevels.ToList<int>();
                foreach (var item in focuses)
                {
                    var newfocus = item.Split(":");

                    try
                    {

                        focus.Add(newfocus[0]);
                        stat.Focuses = focus.ToArray();



                        focuslevel.Add(int.Parse(newfocus[1]));
                        stat.FocusLevels = focuslevel.ToArray();
                    }

                    catch
                    {
                        await ctx.Channel.SendMessageAsync("Unable to read focuses");

                        goto focuses;
                    }
                }
            }


            SaveCharacterSheet(stat, ctx);
        }
        
        
        
        
        
        
        [Command("additem")]
        public async Task AddToInventory(CommandContext ctx, string name, int count)
        {
            if (RoleplayManager.playerRoles.ContainsKey(ctx.User.Username))
            {
                RoleplayManager.playerRoles[ctx.User.Username].inventory.Add(name, count);
                await ctx.Channel.SendMessageAsync(ctx.User.Mention + ": Added " + "x[" + count + "] " + name + " to " + RoleplayManager.playerRoles[ctx.User.Username].Name + "'s inventory");
                SheetManager.SaveCharacterSheet(RoleplayManager.playerRoles[ctx.User.Username], ctx);
            }
        }

        [Command("removeitem")]
        public async Task RemoveFromInventory(CommandContext ctx, string name, int count)
        {

            if (RoleplayManager.playerRoles.ContainsKey(ctx.User.Username))
            {
                StatSheet stat = RoleplayManager.playerRoles[ctx.User.Username];
                if (stat.inventory.ContainsKey(name))
                {
                    if (stat.inventory[name] >= count)
                    {
                        stat.inventory[name] -= count;
                        await ctx.Channel.SendMessageAsync(ctx.User.Mention + ": Removed " + "x[" + count + "] " + name + " to " + RoleplayManager.playerRoles[ctx.User.Username].Name + "'s inventory").ConfigureAwait(false);
                        if (stat.inventory[name] <= 0)
                        {
                            stat.inventory.Remove(name);
                        }
                    }

                    else
                        await ctx.Channel.SendMessageAsync(ctx.User.Mention + ": You don't have enough of that item in your inventory.").ConfigureAwait(false);

                }

                else
                    await ctx.Channel.SendMessageAsync(ctx.User.Mention + ": You don't have that item in your inventory.").ConfigureAwait(false);



                SheetManager.SaveCharacterSheet(RoleplayManager.playerRoles[ctx.User.Username], ctx);
            }

            else
                await ctx.Channel.SendMessageAsync(ctx.User.Mention + ": You don't seem to be playing a a specific character").ConfigureAwait(false);
        }

        public static void DownloadCharacterSheet(DiscordAttachment _file, CommandContext ctx)
        {
            string directory = @"Data\Stats\";

            string pathString = System.IO.Path.Combine(directory, ctx.User.Username);
            System.IO.Directory.CreateDirectory(pathString);
            pathString = System.IO.Path.Combine(pathString, _file.FileName);

            if (!File.Exists(pathString))
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile(_file.Url, pathString);
                }
            }



        }

        public static void SaveCharacterSheet(StatSheet stat, CommandContext ctx)
        {
            if (File.Exists(@stat.Path))
            {
                stat.debug = "Yeet";
                var _json = JsonConvert.SerializeObject(stat);
                File.WriteAllText(@stat.Path, _json);
            }

            else
            {
                var _json = JsonConvert.SerializeObject(stat);
                string pathString = System.IO.Path.Combine(@"Data\Stats\", ctx.User.Username);
                System.IO.Directory.CreateDirectory(pathString);
                pathString = System.IO.Path.Combine(pathString, stat.Name + ".stat");
                File.WriteAllText(pathString, _json);
            }
        }

        public static void UpdateCharacterSheet(StatSheet sheet)
        {


           var json = JsonConvert.SerializeObject(sheet);



            File.WriteAllText(@"Data\Stats\" + sheet.Name + ".stat", json);
        }

        public static StatSheet GetCharacterSheet(string name)
        {
            string[] files = Directory.GetFiles(@"Data\Stats\", "*.stat", SearchOption.AllDirectories);


            foreach(var f in files)
            {
                if (f.Contains(name))
                {
                    var json = string.Empty;
                    using (var fs = File.OpenRead(@f))
                    using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                        json = sr.ReadToEnd();

                    var stat = JsonConvert.DeserializeObject<StatSheet>(json);
                    stat.Path = f;
                    return stat;
                }

            }
            return null;
        }

        public string ConvertStatToString(StatSheet input)
        {
            var output = string.Empty;
            output += "Name: " + input.Name + System.Environment.NewLine;
            output += "Gender: " + input.Gender + System.Environment.NewLine;
            output += "Race: " + input.Race + System.Environment.NewLine;
            output += System.Environment.NewLine;
            output += "Level: " + input.Level + System.Environment.NewLine;
            output += "Architype: " + input.Architype + System.Environment.NewLine;
            output += "Health: " + input.Health + System.Environment.NewLine;
            output += System.Environment.NewLine;
            output += "Stats:" + System.Environment.NewLine;


            if (!input.madeByDiscord)
            {
                output += "-Command: " + input.stats[0] + System.Environment.NewLine;
                output += "-Perception: " + input.stats[1] + System.Environment.NewLine;
                output += "-Intelligence: " + input.stats[2] + System.Environment.NewLine;
                output += "-Accuracy: " + input.stats[3] + System.Environment.NewLine;
                output += "-Reflex: " + input.stats[4] + System.Environment.NewLine;
                output += System.Environment.NewLine;

            }

            else
            {
                foreach (var stat in input.statblock)
                    output += "-" + stat.Key + ": " + stat.Value + System.Environment.NewLine;
                output += System.Environment.NewLine;
            }

            output += "Focuses:" + System.Environment.NewLine;

            foreach (var focus in input.Focuses)
            {
                var focusList = input.Focuses.ToList();
                output += "-" + focus + ": " + input.FocusLevels[focusList.IndexOf(focus)] + System.Environment.NewLine;
            }

            output += System.Environment.NewLine;
            output += "Abilities:" + System.Environment.NewLine;

            foreach (var ability in input.Abilities)
            {
                output += "-" + ability + System.Environment.NewLine;
            }

            output += System.Environment.NewLine;
            output += "Inventory:" + System.Environment.NewLine;

            foreach(var item in input.inventory)
            {
                output +=  "-" + item.Key + ":" + item.Value + System.Environment.NewLine;
            }

            return output;


        }

        

    }
}
