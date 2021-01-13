
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using ServerCore;

public enum PacketID
{
    PlayerInfoReq = 1,
	Test = 2,
	
}


class PlayerInfoReq
{
	public byte testByte;
	public long playerId;
	public string name;
	
	public struct Skill
	{
		public int id;
		public short level;
		public float duration;
		
		public struct Attribute
		{
			public int att;
		
			public void Read(ReadOnlySpan<byte> s, ref ushort pos)
		    {
				this.att = BitConverter.ToInt32(s.Slice(pos, s.Length - pos));
				pos += sizeof(int);
				
			}
		
			public bool Wrtie(Span<byte> s, ref ushort pos)
		    {
				bool success = true;
				success &= BitConverter.TryWriteBytes(s.Slice(pos, s.Length - pos), this.att);
				pos += sizeof(int);
				
		
				return success;
		    }
		}
		
		public List<Attribute> attributes = new List<Attribute>();
		
	
		public void Read(ReadOnlySpan<byte> s, ref ushort pos)
	    {
			this.id = BitConverter.ToInt32(s.Slice(pos, s.Length - pos));
			pos += sizeof(int);
			
			this.level = BitConverter.ToInt16(s.Slice(pos, s.Length - pos));
			pos += sizeof(short);
			
			this.duration = BitConverter.ToSingle(s.Slice(pos, s.Length - pos));
			pos += sizeof(float);
			
			this.attributes.Clear();
			ushort attributeLeng = BitConverter.ToUInt16(s.Slice(pos, s.Length - pos));
			pos += sizeof(ushort);			
			for (int i = 0; i < attributeLeng; i++)
			{
				Attribute attribute = new Attribute();
				attribute.Read(s, ref pos);
				attributes.Add(attribute);
			}
			
		}
	
		public bool Wrtie(Span<byte> s, ref ushort pos)
	    {
			bool success = true;
			success &= BitConverter.TryWriteBytes(s.Slice(pos, s.Length - pos), this.id);
			pos += sizeof(int);
			
			success &= BitConverter.TryWriteBytes(s.Slice(pos, s.Length - pos), this.level);
			pos += sizeof(short);
			
			success &= BitConverter.TryWriteBytes(s.Slice(pos, s.Length - pos), this.duration);
			pos += sizeof(float);
			
			success &= BitConverter.TryWriteBytes(s.Slice(pos, s.Length - pos), (ushort)this.attributes.Count);
			pos += sizeof(ushort);
			
			foreach(Attribute attribute in this.attributes)
				success &= attribute.Wrtie(s, ref pos);
			
	
			return success;
	    }
	}
	
	public List<Skill> skills = new List<Skill>();
		


    public void Read(ArraySegment<byte> segment)
    {
		ushort pos = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		pos += sizeof(ushort);
		pos += sizeof(ushort);
		this.testByte = (byte)segment.Array[segment.Offset + pos];
		pos += sizeof(byte);
		
		this.playerId = BitConverter.ToInt64(s.Slice(pos, s.Length - pos));
		pos += sizeof(long);
		
		ushort nameLeng = BitConverter.ToUInt16(s.Slice(pos, s.Length - pos));
		pos += sizeof(ushort);
		this.name = Encoding.Unicode.GetString(s.Slice(pos, nameLeng));
		pos += nameLeng;
		
		this.skills.Clear();
		ushort skillLeng = BitConverter.ToUInt16(s.Slice(pos, s.Length - pos));
		pos += sizeof(ushort);			
		for (int i = 0; i < skillLeng; i++)
		{
			Skill skill = new Skill();
			skill.Read(s, ref pos);
			skills.Add(skill);
		}
		
	}

	public ArraySegment<byte> Write()
    {
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort pos = 0;
		bool success = true;

		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

		pos += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(pos, s.Length - pos), (ushort)PacketID.PlayerInfoReq);
		pos += sizeof(ushort);
		segment.Array[segment.Offset + pos] = (byte)this.testByte;
		pos += sizeof(byte);
		
		success &= BitConverter.TryWriteBytes(s.Slice(pos, s.Length - pos), this.playerId);
		pos += sizeof(long);
		
		ushort nameLeng = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + pos + sizeof(ushort));
		success &= BitConverter.TryWriteBytes(s.Slice(pos, s.Length - pos), nameLeng);
		pos += sizeof(ushort);
		pos += nameLeng;
		
		success &= BitConverter.TryWriteBytes(s.Slice(pos, s.Length - pos), (ushort)this.skills.Count);
		pos += sizeof(ushort);
		
		foreach(Skill skill in this.skills)
			success &= skill.Wrtie(s, ref pos);
		
		success &= BitConverter.TryWriteBytes(s, pos);

		if (success == false)
			return null;

		return SendBufferHelper.Close(pos);
	}
}

class Test
{
	public int testint;	


    public void Read(ArraySegment<byte> segment)
    {
		ushort pos = 0;

		ReadOnlySpan<byte> s = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);
		pos += sizeof(ushort);
		pos += sizeof(ushort);
		this.testint = BitConverter.ToInt32(s.Slice(pos, s.Length - pos));
		pos += sizeof(int);
		
	}

	public ArraySegment<byte> Write()
    {
		ArraySegment<byte> segment = SendBufferHelper.Open(4096);
		ushort pos = 0;
		bool success = true;

		Span<byte> s = new Span<byte>(segment.Array, segment.Offset, segment.Count);

		pos += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(pos, s.Length - pos), (ushort)PacketID.Test);
		pos += sizeof(ushort);
		success &= BitConverter.TryWriteBytes(s.Slice(pos, s.Length - pos), this.testint);
		pos += sizeof(int);
		
		success &= BitConverter.TryWriteBytes(s, pos);

		if (success == false)
			return null;

		return SendBufferHelper.Close(pos);
	}
}

