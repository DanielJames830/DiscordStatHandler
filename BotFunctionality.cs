using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace DiscordStatHandler
{
    public class BotFunctionality
    {
        public static string ConvertStatToString(StatSheet input)
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

            foreach (var item in input.inventory)
            {
                output += "-" + item.Key + ":" + item.Value + System.Environment.NewLine;
            }

            return output;


        }

        public static StatSheet GetCharacterSheet(string name)
        {
            string[] files = Directory.GetFiles(@"Data\Stats\", "*.stat", SearchOption.AllDirectories);


            foreach (var f in files)
            {
                if (f.ToLower().Contains(name.ToLower()))
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


       
        
        
        
        
        
        
        
        
        
        
        public static void AddFocus(string[] focuses, StatSheet stat)
        {
            var existingFocuses = new List<string>();
            var existingFocusLevels = new List<int>();
            if (stat.Focuses != null)
            {
                existingFocuses = stat.Focuses.ToList();
                existingFocusLevels = stat.FocusLevels.ToList();
            }

            foreach (var item in focuses)
            {
                var newfocus = item.Split(":");


                existingFocuses.Add(newfocus[0]);
                stat.Focuses = existingFocuses.ToArray();



                existingFocusLevels.Add(int.Parse(newfocus[1]));
                stat.FocusLevels = existingFocusLevels.ToArray();
            }
        }

        public static void AddAbility(string[] abilities, StatSheet stat)
        {
            if(stat.Abilities == null)
            {
                stat.Abilities = abilities;
            }
            
            else
            {
                List<string> list = stat.Abilities.ToList();
                foreach (var item in abilities)
                {
                    list.Add(item);
                }

                stat.Abilities = list.ToArray();
            }
            
        }

        public static void AddItem(string[] items, StatSheet stat)
        {
            foreach (var item in items)
            {

                var newitem = item.Split(":");
                if (!stat.inventory.ContainsKey(newitem[0]))
                {
                    stat.inventory.Add(newitem[0], int.Parse(newitem[1]));
                }

                else
                    stat.inventory[newitem[0]] += int.Parse(newitem[1]);
                    
            }
        }

        public static void AddStat(string[] stats, StatSheet stat)
        {
            foreach (var item in stats)
            {

                var newstat = item.Split(":");
                stat.statblock.Add(newstat[0], int.Parse(newstat[1]));
            }
        }

        public static void ChangeHealth(int i, StatSheet stat)
        {
            stat.Health = i;
        }

        public static void ChangeLevel(int i, StatSheet stat)
        {
            stat.Level = i;
        }

        public static void ChangeName(string name, StatSheet stat)
        {
            stat.Name = name;
        }

        public static void ChangeGender(string gender, StatSheet stat)
        {
            stat.Gender = gender;
        }

        public static void ChangeRace(string race, StatSheet stat)
        {
            stat.Race = race;
        }

        public static void ChangeArchitype(string architype, StatSheet stat)
        {
            stat.Architype = architype;
        }

    }
}
