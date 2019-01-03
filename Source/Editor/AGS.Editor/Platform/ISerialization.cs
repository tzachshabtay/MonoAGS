namespace AGS.Editor
{
    public interface ISerialization
    {
        string Serialize(object obj);
        T Deserialize<T>(string text);
    }
}