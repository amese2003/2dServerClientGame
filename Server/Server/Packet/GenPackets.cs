
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using ServerCore;

public enum PacketID
{
    S_BroadcastEnterGame = 1,
	C_LeaveGame = 2,
	S_BroadcastLeaveGame = 3,
	S_PlayerList = 4,
	C_Move = 5,
	S_BroadcastMove = 6,
	
}

public interface IPacket
{
	ushort Protocol { get; }
	void Read(ArraySegment<byte> segment);
	ArraySegment<byte> Write();
}


public class S_BroadcastEnterGame : IPacket
{
	public int playerId;
	public float posX;
	public float posY;
	public float posZ;	


	public ushort Protocol { get { return (ushort)PacketID.S_BroadcastEnterGame; } }

    public void Read(ArraySegment<byte> segment)
    {
		ushort pos = 0;

		pos += sizeof(ushort);
		pos += sizeof(ushort);
		this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + pos);
		pos += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + pos);
		pos += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + pos);
		pos += sizeof(float);
		
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + pos);
		pos += sizeof(float);
		
	}

	public ArraySegment<byte> Write()
    {
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort pos = 0;


		pos += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastEnterGame), 0, segment.Array, segment.Offset + pos, sizeof(ushort));
		pos += sizeof(ushort);
		
		Array.Copy(BitConverter.GetBytes(playerId), 0, segment.Array, segment.Offset + pos, sizeof(int));
		pos += sizeof(int);
		
		
		Array.Copy(BitConverter.GetBytes(posX), 0, segment.Array, segment.Offset + pos, sizeof(float));
		pos += sizeof(float);
		
		
		Array.Copy(BitConverter.GetBytes(posY), 0, segment.Array, segment.Offset + pos, sizeof(float));
		pos += sizeof(float);
		
		
		Array.Copy(BitConverter.GetBytes(posZ), 0, segment.Array, segment.Offset + pos, sizeof(float));
		pos += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(pos), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(pos);
	}
}

public class C_LeaveGame : IPacket
{
		


	public ushort Protocol { get { return (ushort)PacketID.C_LeaveGame; } }

    public void Read(ArraySegment<byte> segment)
    {
		ushort pos = 0;

		pos += sizeof(ushort);
		pos += sizeof(ushort);
		
	}

	public ArraySegment<byte> Write()
    {
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort pos = 0;


		pos += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_LeaveGame), 0, segment.Array, segment.Offset + pos, sizeof(ushort));
		pos += sizeof(ushort);
		
		Array.Copy(BitConverter.GetBytes(pos), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(pos);
	}
}

public class S_BroadcastLeaveGame : IPacket
{
	public int playerId;	


	public ushort Protocol { get { return (ushort)PacketID.S_BroadcastLeaveGame; } }

    public void Read(ArraySegment<byte> segment)
    {
		ushort pos = 0;

		pos += sizeof(ushort);
		pos += sizeof(ushort);
		this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + pos);
		pos += sizeof(int);
		
	}

	public ArraySegment<byte> Write()
    {
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort pos = 0;


		pos += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastLeaveGame), 0, segment.Array, segment.Offset + pos, sizeof(ushort));
		pos += sizeof(ushort);
		
		Array.Copy(BitConverter.GetBytes(playerId), 0, segment.Array, segment.Offset + pos, sizeof(int));
		pos += sizeof(int);
		
		Array.Copy(BitConverter.GetBytes(pos), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(pos);
	}
}

public class S_PlayerList : IPacket
{
	
	public class Player
	{
		public bool isSelf;
		public int playerId;
		public float posX;
		public float posY;
		public float posZ;
	
		public void Read(ArraySegment<byte> segment, ref ushort pos)
	    {
			this.isSelf = BitConverter.ToBoolean(segment.Array, segment.Offset + pos);
			pos += sizeof(bool);
			
			this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + pos);
			pos += sizeof(int);
			
			this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + pos);
			pos += sizeof(float);
			
			this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + pos);
			pos += sizeof(float);
			
			this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + pos);
			pos += sizeof(float);
			
		}
	
		public bool Wrtie(ArraySegment<byte> segment, ref ushort pos)
	    {
			bool success = true;
			
			Array.Copy(BitConverter.GetBytes(isSelf), 0, segment.Array, segment.Offset + pos, sizeof(bool));
			pos += sizeof(bool);
			
			
			Array.Copy(BitConverter.GetBytes(playerId), 0, segment.Array, segment.Offset + pos, sizeof(int));
			pos += sizeof(int);
			
			
			Array.Copy(BitConverter.GetBytes(posX), 0, segment.Array, segment.Offset + pos, sizeof(float));
			pos += sizeof(float);
			
			
			Array.Copy(BitConverter.GetBytes(posY), 0, segment.Array, segment.Offset + pos, sizeof(float));
			pos += sizeof(float);
			
			
			Array.Copy(BitConverter.GetBytes(posZ), 0, segment.Array, segment.Offset + pos, sizeof(float));
			pos += sizeof(float);
			
	
			return success;
	    }
	}
	
	public List<Player> players = new List<Player>();
		


	public ushort Protocol { get { return (ushort)PacketID.S_PlayerList; } }

    public void Read(ArraySegment<byte> segment)
    {
		ushort pos = 0;

		pos += sizeof(ushort);
		pos += sizeof(ushort);
		this.players.Clear();
		ushort playerLeng = BitConverter.ToUInt16(segment.Array, segment.Offset + pos);
		pos += sizeof(ushort);			
		for (int i = 0; i < playerLeng; i++)
		{
			Player player = new Player();
			player.Read(segment, ref pos);
			players.Add(player);
		}
		
	}

	public ArraySegment<byte> Write()
    {
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort pos = 0;


		pos += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_PlayerList), 0, segment.Array, segment.Offset + pos, sizeof(ushort));
		pos += sizeof(ushort);
		
		Array.Copy(BitConverter.GetBytes((ushort)this.players.Count), 0, segment.Array, segment.Offset + pos, sizeof(ushort));
		pos += sizeof(ushort);
		
		foreach(Player player in this.players)
			player.Wrtie(segment, ref pos);
		
		Array.Copy(BitConverter.GetBytes(pos), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(pos);
	}
}

public class C_Move : IPacket
{
	public float posX;
	public float posY;
	public float posZ;	


	public ushort Protocol { get { return (ushort)PacketID.C_Move; } }

    public void Read(ArraySegment<byte> segment)
    {
		ushort pos = 0;

		pos += sizeof(ushort);
		pos += sizeof(ushort);
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + pos);
		pos += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + pos);
		pos += sizeof(float);
		
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + pos);
		pos += sizeof(float);
		
	}

	public ArraySegment<byte> Write()
    {
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort pos = 0;


		pos += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.C_Move), 0, segment.Array, segment.Offset + pos, sizeof(ushort));
		pos += sizeof(ushort);
		
		Array.Copy(BitConverter.GetBytes(posX), 0, segment.Array, segment.Offset + pos, sizeof(float));
		pos += sizeof(float);
		
		
		Array.Copy(BitConverter.GetBytes(posY), 0, segment.Array, segment.Offset + pos, sizeof(float));
		pos += sizeof(float);
		
		
		Array.Copy(BitConverter.GetBytes(posZ), 0, segment.Array, segment.Offset + pos, sizeof(float));
		pos += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(pos), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(pos);
	}
}

public class S_BroadcastMove : IPacket
{
	public int playerId;
	public float posX;
	public float posY;
	public float posZ;	


	public ushort Protocol { get { return (ushort)PacketID.S_BroadcastMove; } }

    public void Read(ArraySegment<byte> segment)
    {
		ushort pos = 0;

		pos += sizeof(ushort);
		pos += sizeof(ushort);
		this.playerId = BitConverter.ToInt32(segment.Array, segment.Offset + pos);
		pos += sizeof(int);
		
		this.posX = BitConverter.ToSingle(segment.Array, segment.Offset + pos);
		pos += sizeof(float);
		
		this.posY = BitConverter.ToSingle(segment.Array, segment.Offset + pos);
		pos += sizeof(float);
		
		this.posZ = BitConverter.ToSingle(segment.Array, segment.Offset + pos);
		pos += sizeof(float);
		
	}

	public ArraySegment<byte> Write()
    {
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort pos = 0;


		pos += sizeof(ushort);
		Array.Copy(BitConverter.GetBytes((ushort)PacketID.S_BroadcastMove), 0, segment.Array, segment.Offset + pos, sizeof(ushort));
		pos += sizeof(ushort);
		
		Array.Copy(BitConverter.GetBytes(playerId), 0, segment.Array, segment.Offset + pos, sizeof(int));
		pos += sizeof(int);
		
		
		Array.Copy(BitConverter.GetBytes(posX), 0, segment.Array, segment.Offset + pos, sizeof(float));
		pos += sizeof(float);
		
		
		Array.Copy(BitConverter.GetBytes(posY), 0, segment.Array, segment.Offset + pos, sizeof(float));
		pos += sizeof(float);
		
		
		Array.Copy(BitConverter.GetBytes(posZ), 0, segment.Array, segment.Offset + pos, sizeof(float));
		pos += sizeof(float);
		
		Array.Copy(BitConverter.GetBytes(pos), 0, segment.Array, segment.Offset, sizeof(ushort));

		return SendBufferHelper.Close(pos);
	}
}

