using System;
using System.Security.Cryptography;

public class CryptoRandom
{
    static private readonly RandomNumberGenerator RadnomSeed =
        RandomNumberGenerator.Create();


    public int GetRandom(int min, int max) 
    {
        if (min >= max) 
            throw new ArgumentOutOfRangeException();

        byte[] data = new byte[4];
        RadnomSeed.GetBytes(data);

        int value = BitConverter.ToInt32(data, 0) & int.MaxValue;
        return min + (value % (max - min));
    }

    public float GetRandom(float min, float max)
    {
        if (min >= max)
            throw new ArgumentOutOfRangeException();

        byte[] data = new byte[4];
        RadnomSeed.GetBytes(data);

        uint value = BitConverter.ToUInt32(data, 0);
        float t = value / (float)uint.MaxValue;

        return min + (value % (max - min));
    }
}