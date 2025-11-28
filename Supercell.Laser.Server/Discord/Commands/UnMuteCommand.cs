namespace Supercell.Laser.Server.Discord.Commands
{
    using NetCord.Services.Commands;
    using Supercell.Laser.Logic.Home.Items;
    using Supercell.Laser.Logic.Message.Account.Auth;
    using Supercell.Laser.Logic.Util;
    using Supercell.Laser.Server.Database;
    using Supercell.Laser.Server.Database.Models;
    using Supercell.Laser.Server.Networking.Session;
    public class UnMute : CommandModule<CommandContext>
    {
        [Command("unmute")]
        public static string UnmuteCommand([CommandParameter(Remainder = true)] string playerId)
        {
            if (!playerId.StartsWith("#"))
            {
                return "Невалидный тэг игрока. Убедись что тэг начинается с '#'.";
            }

            long lowID = LogicLongCodeGenerator.ToId(playerId);
            Account account = Accounts.Load(lowID);

            if (account == null)
            {
                return $"Юзер с айди {playerId} не найден.";
            }

            account.Avatar.IsCommunityBanned = false;
            Notification notification = new() { Id = 81, MessageEntry = "Ты был размучен, можешь общаться в чате." };
            account.Home.NotificationFactory.Add(notification);

            if (Sessions.IsSessionActive(lowID))
            {
                Session session = Sessions.GetSession(lowID);
                session.GameListener.SendTCPMessage(
                    new AuthenticationFailedMessage { Message = "Ты был размучен!" }
                );
                Sessions.Remove(lowID);
            }

            return $"Юзер с айди {playerId} был размучен.";
        }
    }
}