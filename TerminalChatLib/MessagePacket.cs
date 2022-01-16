using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace TerminalChatLib
{
    public class MessagePacket
    {
        public int Length { get; set; }
        public string Username { get; set; }
        public MessageType MessageType { get; set; }
        public string Message { get; set; }

        public MessagePacket(NetworkStream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException($"{nameof(stream)} cannot be null");
            }

            var reader = new BinaryReader(stream);
            Length = reader.ReadInt32();
            Username = reader.ReadString();
            MessageType = (MessageType)reader.ReadInt32();
            Message = reader.ReadString();
        }

        public MessagePacket(string username, MessageType mType, string message)
        {
            var userBytes = Encoding.UTF8.GetBytes(username);
            var messageBytes = Encoding.UTF8.GetBytes(message);

            Length = sizeof(Int32) + userBytes.Length + sizeof(Int32) + messageBytes.Length;
            Username = username;
            MessageType = mType;
            Message = message;
        }
    }

    public static class MessagePacketExtensions
    {
        // public static MessagePacket ReadPacket(this NetworkStream stream)
        // {
        //     if (stream is null)
        //     {
        //         throw new ArgumentNullException($"{nameof(stream)} cannot be null");
        //     }

        //     var reader = new BinaryReader(stream);
        //     return new MessagePacket(reader.ReadInt32(), reader.ReadString(), (MessageType)reader.ReadInt32(), reader.ReadString());
        // }   

        public static BinaryWriter WriteToStream(this MessagePacket messagePacket, NetworkStream stream) 
        {
            if (messagePacket is null)
            {
                throw new ArgumentNullException($"{nameof(messagePacket)} cannot be null");
            }

            if (stream is null)
            {
                throw new ArgumentNullException($"NetworkStream {nameof(stream)} cannot be null");
            }

            var writer = new BinaryWriter(stream);
            writer.Write(messagePacket.Length);
            writer.Write(messagePacket.Username);
            writer.Write((Int32)messagePacket.MessageType);
            writer.Write(messagePacket.Message);

            return writer;
        }
    }
}