using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace DiscordStatHandler.Commands
{
    public class RoleplayManager : BaseCommandModule
    {
        public static Dictionary<string, StatSheet> playerRoles = new Dictionary<string, StatSheet>();

        [Command("playas")]
        public async Task PlayAs(CommandContext ctx, string name)
        {
            var character = BotFunctionality.GetCharacterSheet(name);

            if (character == null)
               await CommandFailed(ctx);

            else
            {


                if (!playerRoles.ContainsKey(ctx.Member.Username))
                    playerRoles.Add(ctx.Member.Username, character);

                else
                {
                    await ctx.Channel.SendMessageAsync(ctx.User.Mention + ": You are no longer playing as " + playerRoles[ctx.Member.Username].Name).ConfigureAwait(false);
                    playerRoles[ctx.Member.Username] = character;
                }


                await ctx.Channel.SendMessageAsync(ctx.User.Mention + ": You are now playing as " + character.Name).ConfigureAwait(false);
            }


        }

        [Command("r")]
        public async Task Roll(CommandContext ctx)
        {
            List<string> command = ctx.Message.Content.Remove(0, 2).Split(" ").ToList<string>();

            var die = InitializeRoll(command);
            if (die == null)
            {
                await CommandFailed(ctx);
                return;
            }

            string output = string.Empty;
            int tally = 0;
            for (int i = 0; i < die.numberOfRolls; i++)
            {
                Random rand = new Random();
                int next = rand.Next(1, die.numberOfSides + 1);
                tally += next;

                if (i != 0 && i != die.numberOfRolls)
                    output += " + ";

                if (i == 0)
                    output += ctx.Member.Mention + ": (";

                output += next;

                if (i == die.numberOfRolls - 1)
                    output += ") ";
            }


            foreach (string arg in command)
            {
                if (command.IndexOf(arg) == 1)
                    continue;

                else
                {
                    if (arg.Contains("+") || arg.Contains("-") || arg.Contains("*") || arg.Contains("/"))
                    {
                        tally = DoMath(arg, command, tally);
                        if (tally == 99999)
                        {
                            await CommandFailed(ctx);
                            return;
                        }

                        output += arg + " " + command[command.IndexOf(arg) + 1] + " ";
                    }
                }


            }






            output += "= " + tally;
            await ctx.Channel.SendMessageAsync(output).ConfigureAwait(false);
        }

        


        public static Die InitializeRoll(List<string> input)
        {

            try
            {
                Die die = new Die()
                {
                    numberOfRolls = Int32.Parse(input[1].Split("d")[0]),
                    numberOfSides = Int32.Parse(input[1].Split("d")[1])
                };
                return die;
            }

            catch
            {
                Console.WriteLine("Parsing failed");
                return null;
            }
        }

        public static int DoMath(string arg, List<string> command, int tally)
        {
            if (arg.Contains("+"))
            {
                if (!arg.Any(c => char.IsDigit(c)))
                {
                    try
                    {
                        tally += int.Parse(command[command.IndexOf(arg) + 1]);
                    }


                    catch
                    {
                        Console.WriteLine("Failed to parse arg");
                    }
                }
                else
                    return 99999;
            }

            else if (arg.Contains("-"))
            {
                if (!arg.Any(c => char.IsDigit(c)))
                {
                    try
                    {
                        tally -= int.Parse(command[command.IndexOf(arg) + 1]);
                    }


                    catch
                    {
                        Console.WriteLine("Failed to parse arg");
                    }
                }
                else
                    return 99999;
            }

            else if (arg.Contains("*"))
            {
                if (!arg.Any(c => char.IsDigit(c)))
                {
                    try
                    {
                        tally *= int.Parse(command[command.IndexOf(arg) + 1]);
                    }


                    catch
                    {
                        Console.WriteLine("Failed to parse arg");
                    }
                }
                else
                    return 99999;
            }

            else if (arg.Contains("/"))
            {
                if (!arg.Any(c => char.IsDigit(c)))
                {
                    try
                    {
                        tally /= int.Parse(command[command.IndexOf(arg) + 1]);
                    }


                    catch
                    {
                        Console.WriteLine("Failed to parse arg");
                    }
                }
                else
                    return 99999;
            }

            else
                return 99999;



            return tally;

        }

        public async Task CommandFailed(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Command Failed");
        }
    }
}
