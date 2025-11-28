namespace Supercell.Laser.Server.Discord.Commands
{
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json.Linq;
    using NetCord.Services.Commands;

    public class RemoveCreator : CommandModule<CommandContext>
    {
        [Command("removecreator")]
        public static string RemoveCreatorCommand(
            [CommandParameter(Remainder = true)] string parameters
        )
        {
            string creatorCode = parameters.Trim();
            if (string.IsNullOrEmpty(creatorCode))
            {
                return "Использование: /removecreator [CREATOR_CODE]";
            }

            // Пути к конфигурационным файлам
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
                RemoveCreatorCodeFromFile(rootConfigPath, creatorCode);
                RemoveCreatorCodeFromFile(buildConfigPath, creatorCode);
            }
            catch (Exception ex)
            {
                return $"Ошибка удаления кода: {ex.Message}";
            }

            return $"Успешно: код автора \"{creatorCode}\" удалён из конфигурационных файлов.";
        }

        private static void RemoveCreatorCodeFromFile(string filePath, string creatorCode)
        {
            string json = File.ReadAllText(filePath);
            JObject config = JObject.Parse(json);

            if (config["CreatorCodes"] is JValue creatorCodesValue)
            {
                string existingCodes = creatorCodesValue.ToString();

                // Разделение кодов, удаление указанного, и проверка изменений
                var codes = existingCodes.Split(',')
                    .Select(code => code.Trim())
                    .Where(code => !string.Equals(code, creatorCode, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                if (codes.Count == existingCodes.Split(',').Length)
                {
                    throw new Exception($"Код \"{creatorCode}\" не найден в {filePath}.");
                }

                // Обновление строки кодов
                config["CreatorCodes"] = string.Join(",", codes);

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