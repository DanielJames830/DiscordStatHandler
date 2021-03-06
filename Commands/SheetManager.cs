﻿using DSharpPlus.CommandsNext;
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

            StatSheet stat = BotFunctionality.GetCharacterSheet(name);

            if (stat == null)
                await ctx.Channel.SendMessageAsync(ctx.User.Mention + ": a character by the name of " + name + " does not exist.").ConfigureAwait(false);


            else
            {
                Console.WriteLine("Retreieved");
                await ctx.Channel.SendMessageAsync(BotFunctionality.ConvertStatToString(stat)).ConfigureAwait(false);
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
                    BotFunctionality.DownloadCharacterSheet(_file, ctx);
                    await ctx.Channel.SendMessageAsync(ctx.User.Mention + ": " + _file.FileName + " has been uploaded!");
                }
            }
        }

        [Command("export")]
        public async Task Export(CommandContext ctx, string name)
        {
            StatSheet stat = BotFunctionality.GetCharacterSheet(name);
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
            var stat = BotFunctionality.GetCharacterSheet(name);
            BotFunctionality.SaveCharacterSheet(stat, ctx);
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
            BotFunctionality.ChangeName(name.Result.Content, stat);

        race:
            await ctx.Channel.SendMessageAsync("What is your character's race?");
            var race = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author.Username == commandInit);
            BotFunctionality.ChangeRace(race.Result.Content, stat);

        gender:
            await ctx.Channel.SendMessageAsync("What is your character's gender?");
            var gender = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author.Username == commandInit);
            BotFunctionality.ChangeGender(gender.Result.Content, stat);

        level:
            await ctx.Channel.SendMessageAsync("What level are they?");
            var input = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author.Username == commandInit);
            try
            {
                var level = Int32.Parse(input.Result.Content);
                BotFunctionality.ChangeLevel(level, stat);

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
                BotFunctionality.ChangeArchitype(architype.Result.Content, stat);
            }

        health:
            stat.Health = 4 + (stat.Level - 1);
            await ctx.Channel.SendMessageAsync("How much health do they have?  Suggested amount: " + stat.Health);
            var input1 = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author.Username == commandInit);
            try
            {
                var health = Int32.Parse(input1.Result.Content);
                BotFunctionality.ChangeHealth(health, stat);
            }

            catch
            {
                await ctx.Channel.SendMessageAsync("Please only type digits!");
                goto health;
            }

        stats:
            await ctx.Channel.SendMessageAsync("What stats do they have? (please format like this)" + System.Environment.NewLine + "statName:1" + System.Environment.NewLine + "statName:1" + System.Environment.NewLine + "statName:1");
            var input2 = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author.Username == commandInit);
            var statblock = input2.Result.Content.Split("\n");
            stat.statblock = new Dictionary<string, int>();
            try
            {
                BotFunctionality.AddStat(statblock, stat);
            }

            catch
            {
                await ctx.Channel.SendMessageAsync("Unable to read stats");
                goto stats;
            }

        focuses:

            await ctx.Channel.SendMessageAsync("What focuses do they have? (please format like this)" + System.Environment.NewLine + "focusName:1" + System.Environment.NewLine + "focusName:1" + System.Environment.NewLine + "focusName:1");
            var input3 = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author.Username == commandInit);
            var focuses = input3.Result.Content.Split("\n");
            try
            {
                BotFunctionality.AddFocus(focuses, stat);
            }

            catch
            {
                await ctx.Channel.SendMessageAsync("Unable to read focuses");
                goto focuses;
            }
            

        abilities:
            await ctx.Channel.SendMessageAsync("What abilities do they have? (please format like this)" + System.Environment.NewLine + "abilityName1" + System.Environment.NewLine + "abilityName2" + System.Environment.NewLine + "abilityName3");
            var input4 = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author.Username == commandInit);
            var abilities = input4.Result.Content.Split("\n");
            try
            {
                BotFunctionality.AddAbility(abilities, stat);
            }

            catch
            {
                await ctx.Channel.SendMessageAsync("Unable to read abilities");
                goto abilities;
            }


        inventory:
            await ctx.Channel.SendMessageAsync("What items do they have? (please format like this)" + System.Environment.NewLine + "itemName:1" + System.Environment.NewLine + "itemName:1" + System.Environment.NewLine + "itemName:1");
            var input5 = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author.Username == commandInit);
            var inventory = input5.Result.Content.Split("\n");
            try
            {
                BotFunctionality.AddItem(inventory, stat);
            }

            catch
            {
                await ctx.Channel.SendMessageAsync("Unable to read items");
                goto inventory;
            }


            BotFunctionality.SaveCharacterSheet(stat, ctx);
            await ctx.Channel.SendMessageAsync("Complete!");
        }


        [Command("add")]
        public async Task Add(CommandContext ctx, string field, string character)
        {
            var commandInit = ctx.User.Username;
            var interactivity = ctx.Client.GetInteractivity();
            var stat = BotFunctionality.GetCharacterSheet(character);

            if (field == "focus")
            {
            focuses:
                await ctx.Channel.SendMessageAsync("What focuses would you like to add? (please format like this)" + System.Environment.NewLine + "focusName:1" + System.Environment.NewLine + "focusName:1" + System.Environment.NewLine + "focusName:1");
                var input3 = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author.Username == commandInit);
                var focuses = input3.Result.Content.Split("\n");

                try
                {
                    BotFunctionality.AddFocus(focuses, stat);
                }

                catch
                {
                    await ctx.Channel.SendMessageAsync("Unable to read focuses");
                    goto focuses;
                }

            }

            if (field == "ability")
            {
            abilities:
                await ctx.Channel.SendMessageAsync("What abilities would you like to add? (please format like this)" + System.Environment.NewLine + "abilityName1" + System.Environment.NewLine + "abilityName2" + System.Environment.NewLine + "abilityName3");
                var input4 = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author.Username == commandInit);
                var abilities = input4.Result.Content.Split("\n");
                try
                {
                    BotFunctionality.AddAbility(abilities, stat);
                }

                catch
                {
                    await ctx.Channel.SendMessageAsync("Unable to read abilities");
                    goto abilities;
                }
            }

            if (field == "item")
            {
            items:
                await ctx.Channel.SendMessageAsync("What items would you like to add? (please format like this)" + System.Environment.NewLine + "itemName:1" + System.Environment.NewLine + "itemName:1" + System.Environment.NewLine + "itemName:1");
                var input3 = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author.Username == commandInit);
                var items = input3.Result.Content.Split("\n");

                try
                {
                    BotFunctionality.AddItem(items, stat);
                }

                catch
                {
                    await ctx.Channel.SendMessageAsync("Unable to read items");
                    goto items;
                }

            }


            BotFunctionality.SaveCharacterSheet(stat, ctx);
        }
        
        [Command("edit")]
        public async Task Edit(CommandContext ctx, string name)
        {

        }
        
        
        
        
        [Command("additem")]
        public async Task AddToInventory(CommandContext ctx, string name, int count)
        {
            if (RoleplayManager.playerRoles.ContainsKey(ctx.User.Username))
            {
                RoleplayManager.playerRoles[ctx.User.Username].inventory.Add(name, count);
                await ctx.Channel.SendMessageAsync(ctx.User.Mention + ": Added " + "x[" + count + "] " + name + " to " + RoleplayManager.playerRoles[ctx.User.Username].Name + "'s inventory");
                BotFunctionality.SaveCharacterSheet(RoleplayManager.playerRoles[ctx.User.Username], ctx);
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



                BotFunctionality.SaveCharacterSheet(RoleplayManager.playerRoles[ctx.User.Username], ctx);
            }

            else
                await ctx.Channel.SendMessageAsync(ctx.User.Mention + ": You don't seem to be playing a a specific character").ConfigureAwait(false);
        }

       
       
    }
}
