namespace Supercell.Laser.Server.Discord.Commands
{
    using NetCord.Services.Commands;
    using Supercell.Laser.Logic.Message.Account.Auth;
    using Supercell.Laser.Logic.Util;
    using Supercell.Laser.Server.Database;
    using Supercell.Laser.Server.Database.Models;
    using Supercell.Laser.Server.Networking.Session;

    public class TakeDiamonds : CommandModule<CommandContext>
    {
        [Command("takegems")]
        public static string ExecuteTakeDiamonds(
            [CommandParameter(Remainder = true)] string playerIdAndDiamondAmount
        )
        {
            string[] parts = playerIdAndDiamondAmount.Split(' ');
            if (
                parts.Length != 2
                || !parts[0].StartsWith("#")
                || !int.TryParse(parts[1], out int diamondAmount)
                || diamondAmount <= 0
            )
            {
                return "Usage: !takegems [TAG] [amount] (amount must be positive)";
            }

            long lowID = LogicLongCodeGenerator.ToId(parts[0]);
            Account account = Accounts.Load(lowID);

            if (account == null)
            {
                return $"Could not find player with ID {parts[0]}.";
            }

            if (account.Avatar.Diamonds < diamondAmount)
            {
                return $"Недостаточно гемов! У игрока с ID {parts[0]} только {account.Avatar.Diamonds} гемов.";
            }

            account.Avatar.Diamonds -= diamondAmount;

            if (Sessions.IsSessionActive(lowID))
            {
                Session session = Sessions.GetSession(lowID);
                session.GameListener.SendTCPMessage(
                    new AuthenticationFailedMessage()
                    {
                        Message =
                            $"Твой аккаунт был обновлен! У тебя отнято {diamondAmount} гемов. Текущий баланс: {account.Avatar.Diamonds} гемов."
                    }
                );
                Sessions.Remove(lowID);
            }

            return $"У аккаунта с айди {parts[0]} отнято {diamondAmount} гемов. Новый баланс: {account.Avatar.Diamonds}.";
        }
    }
}