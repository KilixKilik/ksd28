namespace Supercell.Laser.Server.Discord.Commands
{
    using NetCord.Services.Commands;
    using Supercell.Laser.Logic.Data;
    using Supercell.Laser.Logic.Home.Structures;
    using Supercell.Laser.Logic.Message.Account.Auth;
    using Supercell.Laser.Logic.Util;
    using Supercell.Laser.Server.Database;
    using Supercell.Laser.Server.Database.Models;
    using Supercell.Laser.Server.Networking.Session;

    public class UnlockBrawler : CommandModule<CommandContext>
    {
        [Command("addbrawler")]
        public static string UnlockBrawlerCommand(string playerTag, int brawlerId)
        {
            if (!playerTag.StartsWith("#"))
            {
                return "Invalid player tag. Make sure it starts with '#'.";
            }

            long playerId = LogicLongCodeGenerator.ToId(playerTag);
            Account account = Accounts.Load(playerId);

            if (account == null)
            {
                return $"User with tag {playerTag} not found.";
            }

            try
            {
                if (account.Avatar.HasHero(16000000 + brawlerId))
                {
                    return $"The player already owns the brawler with ID {brawlerId}.";
                }

                CharacterData character = DataTables
                    .Get(16) // DataType.Character
                    .GetDataWithId<CharacterData>(brawlerId);

                if (character == null)
                {
                    return $"Brawler with ID {brawlerId} not found.";
                }

                CardData unlockCard = DataTables
                    .Get(23) // DataType.Card
                    .GetData<CardData>(character.Name + "_unlock");

                if (unlockCard == null)
                {
                    return $"Unlock card for brawler with ID {brawlerId} not found.";
                }

                account.Avatar.UnlockHero(character.GetGlobalId(), unlockCard.GetGlobalId());


                if (Sessions.IsSessionActive(playerId))
                {
                    Session session = Sessions.GetSession(playerId);
                    session.GameListener.SendTCPMessage(
                        new Supercell.Laser.Logic.Message.Account.Auth.AuthenticationFailedMessage
                        {
                            Message = $"Боец {character.Name} был успешно разблокирован!"
                        }
                    );
                    Sessions.Remove(playerId);
                }

                return $"Боец {character.Name} успешно разблокирован для {playerTag}.";
            }
            catch (Exception ex)
            {
                return $"An error occurred while unlocking the brawler: {ex.Message}";
            }
        }
    }
}