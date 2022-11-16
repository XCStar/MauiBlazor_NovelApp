namespace MauiApp3.Data;


public class ReceiveData
{
    public int status { get; set; }
    public string message { get; set; }
    public Data data { get; set; }
}

public class Data
{
    public DateTime time { get; set; }
    public List<News> list { get; set; }
}

public class News
{
    public int id { get; set; }
    public string title { get; set; }
    public string extra { get; set; }
    public string link { get; set; }
}
