using System.IO;

namespace Supercell.Laser.Logic.Message.Account.Auth
{
    public class AuthenticationMessage : GameMessage
    {
        public AuthenticationMessage() : base()
        {
            AccountId = 0;
        }

        public long AccountId;
        public string PassToken;
        public string DeviceId;
        public string Device;
        public string ClientVersion;

        // Новые переменные
        public string OsVersion;
        public bool isAndroid;

        public override void Decode()
        {
            AccountId = Stream.ReadLong();
            PassToken = Stream.ReadString();
            Stream.ReadInt();
            Stream.ReadInt();
            Stream.ReadInt();
            Stream.ReadString();
            DeviceId = Stream.ReadString();
            Stream.ReadVInt();
            Stream.ReadVInt();
            Stream.ReadString();

            // Присвоение значений переменным OsVersion и isAndroid
            OsVersion = Stream.ReadString();
            isAndroid = Stream.ReadBoolean();

            Stream.ReadString();
            Stream.ReadString();
            Stream.ReadBoolean();
            Stream.ReadString();
            Stream.ReadInt();
            Stream.ReadVInt();
            ClientVersion = Stream.ReadString();
        }

        public override int GetMessageType()
        {
            return 10101;
        }

        public override int GetServiceNodeType()
        {
            return 1;
        }
    }
}
