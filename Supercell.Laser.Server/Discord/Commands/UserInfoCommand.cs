namespace Supercell.Laser.Server.Discord.Commands
{
    using NetCord.Services.Commands;
    using Supercell.Laser.Logic.Util;
    using Supercell.Laser.Server.Database;
    using Supercell.Laser.Server.Database.Models;
    using Supercell.Laser.Server.Utils;
    public class UserInfo : CommandModule<CommandContext>
    {
        [Command("userinfo")]
        public static string UserInfoCommand([CommandParameter(Remainder = true)] string playerId)
        {
            if (!playerId.StartsWith("#"))
            {
                return "Невалидный айди юзера. Убедись в том что айди начинается с '#'.";
            }

            long lowID = LogicLongCodeGenerator.ToId(playerId);
            Account account = Accounts.Load(lowID);

            if (account == null)
            {
                return $"Не найден юзер с таким айди {playerId}.";
            }

            string ipAddress = ConvertInfoToData(account.Home.IpAddress);
            string lastLoginTime = account.Home.LastVisitHomeTime.ToString();
            string device = ConvertInfoToData(account.Home.Device);
            string name = ConvertInfoToData(account.Avatar.Name);
            string token = ConvertInfoToData(account.Avatar.PassToken);
            string soloWins = ConvertInfoToData(account.Avatar.SoloWins);
            string duoWins = ConvertInfoToData(account.Avatar.DuoWins);
            string trioWins = ConvertInfoToData(account.Avatar.TrioWins);
            string trophies = ConvertInfoToData(account.Avatar.Trophies);
            string banned = ConvertInfoToData(account.Avatar.Banned);
            string muted = ConvertInfoToData(account.Avatar.IsCommunityBanned);
            string haspremium = ConvertInfoToData(account.Avatar.PremiumLevel == 1);
            string gold = ConvertInfoToData(account.Avatar.Gold);
            string gems = ConvertInfoToData(account.Avatar.Diamonds);
            string starpoints = ConvertInfoToData(account.Avatar.StarPoints);
            string alliancename = ConvertInfoToData(account.Avatar.AllianceName);
            string allianceid = ConvertInfoToData(account.Avatar.AllianceId);
            string alliancerole = ConvertInfoToData(account.Avatar.AllianceRole);
            string username = DatabaseHelper.ExecuteScalar(
                "SELECT username FROM users WHERE id = @id",
                ("@id", lowID)
            );
            string password = DatabaseHelper.ExecuteScalar(
                "SELECT password FROM users WHERE id = @id",
                ("@id", lowID)
            );

            return $"# Информация о {playerId}!\n"
                + $"IP-Адрес: {ipAddress}\n"
                + $"Последний раз заходили: {lastLoginTime} UTC\n"
                + $"Девайс: {device}\n"
                + $"# Статистика аккауна\n"
                + $"Никнейм: {name}\n"
                + $"Токен: {token}\n"
                + $"Монеты: {gold}\n"
                + $"Гемы: {gems}\n"
                + $"Старпоинты: {starpoints}\n"
                + $"Трофеи: {trophies}\n"
                + $"Победы в соло: {soloWins}\n"
                + $"Победы в дуо: {duoWins}\n"
                + $"Победы в трио: {trioWins}\n"
                + $"Премиум: {haspremium}\n"
                + $"Замучен: {muted}\n"
                + $"Заблокирован: {banned}\n"
                + $"# Информация о клубе\n"
                + $"Имя клуба: {alliancename}\n"
                + $"Айди клуба: {allianceid}\n"
                + $"Роль в клубе: {alliancerole}\n"
                + $"# KSD ID\n"
                + $"Юзернейм: {username}\n"
                + $"Пароль: {password}";
        }

        private static string ConvertInfoToData(object data)
        {
            return data?.ToString() ?? "N/A";
        }
    }
}