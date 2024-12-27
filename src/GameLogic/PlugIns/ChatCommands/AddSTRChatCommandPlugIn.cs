// <copyright file="AddSTRChatCommandPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions.Character;
using MUnique.OpenMU.PlugIns;
using org.mariuszgromada.math.mxparser;

/// <summary>
/// A chat command plugin which handles the command to add stat points.
/// </summary>
[Guid("246dd788-2366-4626-8b69-dd059c798266")]
[PlugIn("Add Stat Strength chat command", "Handles the chat command '/addstr (amount)'. Adds the specified strength amount to character.")]
[ChatCommandHelp(Command, " Adds the specified strength amount to character.", typeof(Amount), MinimumStatus)]
public class AddSTRChatCommandPlugIn : IChatCommandPlugIn
{
    private const string Command = "/addstr";



    private const CharacterStatus MinimumStatus = CharacterStatus.Normal;

    private readonly IncreaseStatsAction _action = new();

    /// <inheritdoc />
    public string Key => Command;

    /// <inheritdoc />
    public CharacterStatus MinCharacterStatusRequirement => MinimumStatus;

    /// <inheritdoc />
    public async ValueTask HandleCommandAsync(Player player, string command)
    {
        try
        {
            if (player.SelectedCharacter is null)
            {
                return;
            }

            var amount = command.ParseArguments<Amount>();
            var selectedCharacter = player.SelectedCharacter;

            if (!selectedCharacter.CanIncreaseStats(amount.AmountNumber))
            {
                return;
            }

            if (player.CurrentMiniGame is not null)
            {
                await player.ShowMessageAsync("Adding multiple points is not allowed when playing a mini game.").ConfigureAwait(false);
                return;
            }

            await this._action.IncreaseStatsAsync(player, Stats.BaseStrength, amount.AmountNumber).ConfigureAwait(false);
        }
        catch (ArgumentException e)
        {
            await player.ShowMessageAsync(e.Message).ConfigureAwait(false);
        }
    }

    private class Amount : ArgumentsBase
    {
        public ushort AmountNumber { get; set; }
    }
}