namespace Supercell.Laser.Server.Discord.Commands
{
    using NetCord.Services.Commands;
    using Supercell.Laser.Logic.Message.Account.Auth;
    using Supercell.Laser.Logic.Util;
    using Supercell.Laser.Server.Database;
    using Supercell.Laser.Server.Database.Models;
    using Supercell.Laser.Server.Networking.Session;

    public class TakeGold : CommandModule<CommandContext>
    {
        [Command("takegold")]
        public static string ExecuteTakeGold(
            [CommandParameter(Remainder = true)] string playerIdAndGoldAmount
        )
        {
            string[] parts = playerIdAndGoldAmount.Split(' ');
            if (
                parts.Length != 2
                || !parts[0].StartsWith("#")
                || !int.TryParse(parts[1], out int goldAmount)
                || goldAmount <= 0
            )
            {
                return "Usage: !removegold [TAG] [amount] (amount must be positive)";
            }

            long lowID = LogicLongCodeGenerator.ToId(parts[0]);
            Account account = Accounts.Load(lowID);

            if (account == null)
            {
                return $"Could not find player with ID {parts[0]}.";
            }

            if (account.Avatar.Gold < goldAmount)
            {
                return $"Недостаточно золота! У игрока с ID {parts[0]} только {account.Avatar.Gold} монет.";
            }

            // Уменьшаем количество золота
            account.Avatar.Gold -= goldAmount;

            if (Sessions.IsSessionActive(lowID))
            {
                Session session = Sessions.GetSession(lowID);
                session.GameListener.SendTCPMessage(
                    new AuthenticationFailedMessage()
                    {
                        Message =
                            $"Твой аккаунт был обновлен! У тебя отнято {goldAmount} монет. Текущий баланс: {account.Avatar.Gold} монет."
                    }
                );
                Sessions.Remove(lowID);
            }

            return $"У аккаунта с айди {parts[0]} отнято {goldAmount} монет. Новый баланс: {account.Avatar.Gold}.";
        }
    }
}