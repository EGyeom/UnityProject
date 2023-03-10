using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ServerCore;
using System.Net;
using Google.Protobuf.Protocol;
using static Google.Protobuf.Protocol.Person.Types;
using Google.Protobuf;

namespace Server
{
	class ClientSession : PacketSession
	{
		public int SessionId { get; set; }
		//public GameRoom Room { get; set; }

		public void Send(IMessage packet)
        {
			string msgName = packet.Descriptor.Name.Replace("_", String.Empty);
			MsgId msgid = (MsgId)Enum.Parse(typeof(MsgId), msgName);

			ushort size = (ushort)packet.CalculateSize();
			byte[] sendBuffer = new byte[size + 4];
			Array.Copy(BitConverter.GetBytes(size + 4), 0, sendBuffer, 0, sizeof(short));
			Array.Copy(BitConverter.GetBytes((ushort)msgid), 0, sendBuffer, 2, sizeof(short));
			Array.Copy(packet.ToByteArray(), 0, sendBuffer, 4, size);

			Send(new ArraySegment<byte>(sendBuffer));
		}

		public override void OnConnected(EndPoint endPoint)
		{
			Console.WriteLine($"OnConnected : {endPoint}");

			//Program.Room.Push(() => Program.Room.Enter(this));
			S_Chat chat = new S_Chat()
			{
				Context = "안녕하세요"	
			};
			Send(chat);
		}

		public override void OnRecvPacket(ArraySegment<byte> buffer)
		{
			//PacketManager.Instance.OnRecvPacket(this, buffer);
		}

		public override void OnDisconnected(EndPoint endPoint)
		{
			SessionManager.Instance.Remove(this);
			//if (Room != null)
			//{
			//	GameRoom room = Room;
			//	room.Push(() => room.Leave(this));
			//	Room = null;
			//}

			Console.WriteLine($"OnDisconnected : {endPoint}");
		}

		public override void OnSend(int numOfBytes)
		{
			//Console.WriteLine($"Transferred bytes: {numOfBytes}");
		}
	}
}
