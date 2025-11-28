namespace Supercell.Laser.Server.Discord.Commands
{
    using NetCord.Services.Commands;
    using Supercell.Laser.Logic.Message.Account.Auth;
    using Supercell.Laser.Logic.Util;
    using Supercell.Laser.Server.Database;
    using Supercell.Laser.Server.Database.Models;
    using Supercell.Laser.Server.Networking.Session;

    public class AddGold : CommandModule<CommandContext>
    {
        [Command("addgold")]
        public static string ExecuteAddGold(
            [CommandParameter(Remainder = true)] string playerIdAndGoldAmount
        )
        {
            string[] parts = playerIdAndGoldAmount.Split(' ');
            if (
                parts.Length != 2
                || !parts[0].StartsWith("#")
                || !int.TryParse(parts[1], out int goldAmount)
            )
            {
                return "Usage: !addgold [TAG] [amount]";
            }

            long lowID = LogicLongCodeGenerator.ToId(parts[0]);
            Account account = Accounts.Load(lowID);

            if (account == null)
            {
                return $"Could not find player with ID {parts[0]}.";
            }

            // Добавляем золото к текущему балансу
            account.Avatar.Gold += goldAmount;

            if (Sessions.IsSessionActive(lowID))
            {
                Session session = Sessions.GetSession(lowID);
                session.GameListener.SendTCPMessage(
                    new AuthenticationFailedMessage()
                    {
                        Message =
                            $"Твой аккаунт был обновлен! Теперь у тебя добавлено {goldAmount} монет!"
                    }
                );
                Sessions.Remove(lowID);
            }

            return $"Добавлено {goldAmount} золота для аккаунта с айди {parts[0]}.";
        }
    }
}