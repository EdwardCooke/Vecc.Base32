namespace Vecc.Base32
{
    public interface IBase32
    {
        byte[] DecodeToBytes(string base32);
        string DecodeToString(string base32);
        string Encode(byte[] value);
        string Encode(string value);
    }
}
