namespace Supercell.Laser.Server.Discord.Commands
{
    using System.IO;
    using Newtonsoft.Json.Linq;
    using NetCord.Services.Commands;
    using Supercell.Laser.Logic.Command.Home;
    using Supercell.Laser.Logic.Home.Items;
    using Supercell.Laser.Logic.Message.Home;
    using Supercell.Laser.Logic.Util;
    using Supercell.Laser.Server.Database;
    using Supercell.Laser.Server.Database.Models;
    using Supercell.Laser.Server.Networking.Session;

    public class AddCreator : CommandModule<CommandContext>
    {
        [Command("addcreator")]
        public static string AddCreatorCommand(
            [CommandParameter(Remainder = true)] string parameters
        )
        {
            string[] parts = parameters.Split(' ');
            if (parts.Length != 2)
            {
                return "Использование: /addcreator [TAG] [CODE]";
            }

            string creatorCode = parts[1].Trim();
            if (string.IsNullOrEmpty(creatorCode))
            {
                return "Невалидный код автора.";
            }

            // Абсолютные пути к файлам конфигурации
            string rootConfigPath = "/root/ksd28/Supercell.Laser.Server/config.json";
            string buildConfigPath = "/root/ksd28/Supercell.Laser.Server/bin/Release/net8.0/config.json";

            // Проверка наличия файлов
            if (!File.Exists(rootConfigPath))
            {
                return $"Файл конфигурации {rootConfigPath} не найден.";
            }
            if (!File.Exists(buildConfigPath))
            {
                return $"Файл конфигурации {buildConfigPath} не найден.";
            }

            try
            {
                // Добавление кода в оба файла
                AddCreatorCodeToFile(rootConfigPath, creatorCode);
                AddCreatorCodeToFile(buildConfigPath, creatorCode);
            }
            catch (Exception ex)
            {
                return $"Ошибка добавления кода: {ex.Message}";
            }
            
            long lowID = LogicLongCodeGenerator.ToId(parts[0]);
            Account account = Accounts.Load(lowID);

            if (account == null)
            {
                return $"Could not find player with ID {parts[0]}.";
            }

            // Создание уведомления для игрока
            Notification nGems = new()
            {
                Id = 89,
                DonationCount = 170,
                MessageEntry = $"<c6>Поздравляем со вступлением на креатора!</c>\nТвой код: {creatorCode}"
            };
            account.Home.NotificationFactory.Add(nGems);
            LogicAddNotificationCommand acmGems = new() { Notification = nGems };
            AvailableServerCommandMessage asmGems = new();
            asmGems.Command = acmGems;

            if (Sessions.IsSessionActive(lowID))
            {
                Session sessionGems = Sessions.GetSession(lowID);
                sessionGems.GameListener.SendTCPMessage(asmGems);
            }

            return $"Успешно: код автора \"{creatorCode}\" добавлен в конфигурационные файлы.";
        }

        private static void AddCreatorCodeToFile(string filePath, string creatorCode)
        {
            string json = File.ReadAllText(filePath);
            JObject config = JObject.Parse(json);

            if (config["CreatorCodes"] is JValue creatorCodesValue)
            {
                string existingCodes = creatorCodesValue.ToString();
                string updatedCodes = string.IsNullOrEmpty(existingCodes)
                    ? creatorCode
                    : $"{existingCodes},{creatorCode}";

                config["CreatorCodes"] = updatedCodes;

                // Сохранение изменений
                File.WriteAllText(filePath, config.ToString());
            }
            else
            {
                throw new Exception($"CreatorCodes отсутствует в {filePath}.");
            }
        }
    }
}