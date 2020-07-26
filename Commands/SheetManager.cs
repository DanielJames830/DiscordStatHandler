using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
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
                await ctx.Channel.SendMessageAsync(ConvertStatToString(stat)).ConfigureAwait(false);
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

        public static void SaveCharacterSheet(StatSheet stat)
        {

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
            output += "-Command: " + input.stats[0] + System.Environment.NewLine;
            output += "-Perception: " + input.stats[1] + System.Environment.NewLine;
            output += "-Intelligence: " + input.stats[2] + System.Environment.NewLine;
            output += "-Accuracy: " + input.stats[3] + System.Environment.NewLine;
            output += "-Reflex: " + input.stats[4] + System.Environment.NewLine;
            output += System.Environment.NewLine;
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

            return output;


        }

        

    }
}
