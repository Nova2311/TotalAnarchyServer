using System;
using System.Collections.Generic;
using System.Text;


public class ByteBuffer : IDisposable
{
    private List<byte> Buff;
    private int readpos;
    private bool buffUpdated = false;
    private byte[] readbuff;
    #region ByteBuffer Functions
    public ByteBuffer()
    {
        Buff = new List<byte>();
        readpos = 0;
    }
    public long GetReadPos()
    {
        return readpos;
    }
    public byte[] ToArray()
    {
        return Buff.ToArray();
    }
    public int Count()
    {
        return Buff.Count;
    }
    public int Length()
    {
        return Count() - readpos;
    }
    public void Clear()
    {
        Buff.Clear();
        readpos = 0;
    }
    #endregion

    public void WriteInteger(int Input)
    {
        Buff.AddRange(BitConverter.GetBytes(Input));
        buffUpdated = true;
    }
    public void WriteBytes(byte[] Input)
    {
        Buff.AddRange(Input);
        buffUpdated = true;
    }
    public void WriteFloat(float Input)
    {
        Buff.AddRange(BitConverter.GetBytes(Input));
        buffUpdated = true;
    }
    public void WriteLong(long Input)
    {
        Buff.AddRange(BitConverter.GetBytes(Input));
        buffUpdated = true;
    }
    public void WriteShort(short Input)
    {
        Buff.AddRange(BitConverter.GetBytes(Input));
        buffUpdated = true;
    }
    public void WriteString(string Input)
    {
        Buff.AddRange(BitConverter.GetBytes(Input.Length));
        Buff.AddRange(Encoding.ASCII.GetBytes(Input));
        buffUpdated = true;
    }

    // Short  = 16bit variable  = 2 as a byte
    // Integer = 32Bit variable  = 4 as a byte
    // Long  = 64bit variable  = 8 as a byte

    public int ReadInteger(bool Peek = true)
    {
        if (Buff.Count > readpos)
        {
            if (buffUpdated)
            {
                readbuff = Buff.ToArray();
                buffUpdated = false;
            }
            int value = BitConverter.ToInt32(readbuff, readpos);
            if (Peek & Buff.Count > readpos)
            {
                readpos += 4;
            }
            return value;
        }
        else
        {
            throw new Exception("Byte Buffer is Past Limit!Make sure you are reading out the correct value!");
        }
    }
    public byte[] ReadBytes(int Length, bool Peek = true)
    {
        if (buffUpdated)
        {
            readbuff = Buff.ToArray();
            buffUpdated = false;
        }
        byte[] value = Buff.GetRange(readpos, Length).ToArray();
        if (Peek)
        {
            readpos += Length;
        }
        return value;
    }
    public float ReadFloat(bool Peek = true)
    {
        if (Buff.Count > readpos)
        {
            if (buffUpdated)
            {
                readbuff = Buff.ToArray();
                buffUpdated = false;
            }
            float value = BitConverter.ToSingle(readbuff, readpos);
            if (Peek & Buff.Count > readpos)
            {
                readpos += 4;
            }
            return value;
        }
        else
        {
            throw new Exception("Byte Buffer is Past Limit!Make sure you are reading out the correct value!");
        }
    }
    public long ReadLong(bool Peek = true)
    {
        if (Buff.Count > readpos)
        {
            if (buffUpdated)
            {
                readbuff = Buff.ToArray();
                buffUpdated = false;
            }
            long value = BitConverter.ToInt64(readbuff, readpos);
            if (Peek & Buff.Count > readpos)
            {
                readpos += 8;
            }
            return value;
        }
        else
        {
            throw new Exception("Byte Buffer is Past Limit!Make sure you are reading out the correct value!");
        }
    }
    public short ReadShort(bool Peek = true)
    {
        if (Buff.Count > readpos)
        {
            if (buffUpdated)
            {
                readbuff = Buff.ToArray();
                buffUpdated = false;
            }
            short value = BitConverter.ToInt16(readbuff, readpos);
            if (Peek & Buff.Count > readpos)
            {
                readpos += 2;
            }
            return value;
        }
        else
        {
            throw new Exception("Byte Buffer is Past Limit!Make sure you are reading out the correct value!");
        }
    }
    public string ReadString(bool Peek = true)
    {
        int stringLength = ReadInteger(true);
        if (buffUpdated)
        {
            readbuff = Buff.ToArray();
            buffUpdated = false;
        }
        string value = Encoding.ASCII.GetString(readbuff, readpos, stringLength);
        if (Peek & Buff.Count > readpos)
        {
            readpos += stringLength;
        }
        return value;
    }

    #region IDisposable Interface
    private bool disposedValue = false; // detect rendudant calls
    protected virtual void Dispose(bool dispoing)
    {
        if (!disposedValue)
        {
            if (dispoing)
            {
                Buff.Clear();
                readpos = 0;
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
