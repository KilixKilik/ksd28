namespace Supercell.Laser.Server.Discord.Commands
{
    using System.Diagnostics;
    using System.Net;
    using NetCord.Services.Commands;

    public class IPBan : CommandModule<CommandContext>
    {
        [Command("banip")]
        public static string BanIpCommand([CommandParameter(Remainder = true)] string ipAddress)
        {
            if (!IPAddress.TryParse(ipAddress, out _))
            {
                return "Невалидный формат IP-Адреса.";
            }

            if (IsIpBanned(ipAddress))
            {
                return $"IP-Адрес {ipAddress} уже был заблокирован.";
            }

            try
            {
                // Блокируем IP через iptables
                BanIp(ipAddress);

                // Добавляем IP в локальный файл для учёта
                File.AppendAllText("ipblacklist.txt", ipAddress + Environment.NewLine);

                return $"IP-Адрес {ipAddress} был заблокирован.";
            }
            catch (Exception ex)
            {
                return $"Произошла ошибка при блокировке IP: {ex.Message}";
            }
        }

        private static bool IsIpBanned(string ipAddress)
        {
            if (!File.Exists("ipblacklist.txt"))
            {
                return false;
            }

            string[] bannedIps = File.ReadAllLines("ipblacklist.txt");
            return bannedIps.Contains(ipAddress);
        }

        private static void BanIp(string ip)
        {
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = $"-c \"iptables -A INPUT -s {ip} -j DROP\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            Process.Start(psi);
        }
    }
}