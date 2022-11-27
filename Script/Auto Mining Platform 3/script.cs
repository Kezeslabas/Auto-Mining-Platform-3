/*
 * R e a d m e
 * -----------
 * 
 * In this file you can include any instructions or other comments you want to have injected onto the 
 * top of your final script. You can safely delete this file if you do not want any such comments.
 */

readonly bool DEBUG_ENABLED = true;

readonly MyCommandLine CL = new MyCommandLine();
readonly Router router;

public Program()
{
    Debugger.Init(DEBUG_ENABLED, Echo);
    router = new Router(Echo, new Dictionary<string, Action<MyCommandLine>>{
        { "test", p => Echo("Test") }
    });
}

public void Save()
{
}

public void Main(string argument, UpdateType updateSource)
{
    router.ParseAndRoute(argument);
}

public class Debugger
{
    private static Action<string> Echo = t => { return; };

    public static void Init(bool enabled, Action<string> echo)
    {
        if (enabled)
        {
            Echo = echo;
        }
        else
        {
            Echo = t => { return; };
        }
    }

    public static void Log(string text)
    {
        Echo(text);
    }
}

public class Router
{
    private readonly Action<string> notification;
    private readonly Dictionary<string, Action<MyCommandLine>> routes;

    private readonly MyCommandLine cl = new MyCommandLine();

    public Router(Action<string> notification, Dictionary<string, Action<MyCommandLine>> routes)
    {
        this.notification = notification;
        this.routes = routes;
    }

    public void ParseAndRoute(string argument)
    {
        if (string.IsNullOrEmpty(argument))
        {
            notification("No Argument Provided!");
        }
        else if (cl.TryParse(argument))
        {
            routes.GetValueOrDefault(cl.Argument(0), p => notification("Argument not found: " + argument))
                .Invoke(cl);
        }
        else
        {
            notification("Argument can't be parse: " + argument);
        }
    }
}