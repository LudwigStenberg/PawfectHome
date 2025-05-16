
using System.Reflection.Metadata.Ecma335;

public class ApiResponse<T>
{
    public T Data { get; set; }
    public MetaData Meta { get; set; }

    public ApiResponse(T data)
    {
        Data = data;
        Meta = new MetaData();
    }
}

public class MetaData
{
    public bool AuthenticationChanged { get; set; }
    public DateTime ResponseTime { get; set; } = DateTime.UtcNow;
}