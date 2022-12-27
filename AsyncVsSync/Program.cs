#region using
using System.Diagnostics;
#endregion

#region Init
var length = 100;

var list = new List<AsyncService>(length);

for (int i = 0; i < length; i++) list.Add(new AsyncService());
#endregion

var watchS = await SyncTasks();
var watchA = await AsyncTasks();

Console.WriteLine($"Sync:  \t{watchS}");
Console.WriteLine($"Async: \t{watchA}");

async Task<TimeSpan> SyncTasks()
{
    var watch = new Stopwatch();
    watch.Start();
    Console.WriteLine("Sync");
    foreach (var item in list)
    {
        await item.Run();
    }
    watch.Stop();
    return watch.Elapsed;
}

async Task<TimeSpan> AsyncTasks()
{
    var watch = new Stopwatch();
    watch.Start();
    Console.WriteLine("Async");
    var asyncList = new List<Task>(length);
    //foreach (var item in list) asyncList.Add(item.Run());
    list.ForEach(item => asyncList.Add(item.Run()));

    while (asyncList.Count > 0)
    {
        var item = await Task.WhenAny(asyncList);
        asyncList.Remove(item);
    }
    watch.Stop();
    return watch.Elapsed;
}

class AsyncService
{
    private readonly int _waitTime;
    private readonly int _number;
    private static int _nextNumber = 0;
    private static readonly Random _random = new();
    public AsyncService()
    {
        _number = _nextNumber++;
        _waitTime = _random.Next(1000);
    }

    public async Task Run()
    {
        await Task.Delay(_waitTime);
        Console.WriteLine(this);
    }

    public override string ToString() => $"Task: {_number} \t Time:{_waitTime}";

}
