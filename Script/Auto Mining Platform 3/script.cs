/*
 * R e a d m e
 * -----------
 * 
 * In this file you can include any instructions or other comments you want to have injected onto the 
 * top of your final script. You can safely delete this file if you do not want any such comments.
 */

readonly bool DEBUG_ENABLED = true;

readonly Router router;
readonly RunManager runManager;

bool indicator = false;

int run1 = 0;
int run1Target = 0;
int run10 = 0;
int run10Target = 0;
int run100 = 0;
int run100Target = 0;

public Program()
{
    Debugger.Init(DEBUG_ENABLED, Echo);

    runManager = new RunManager(Runtime);

    router = new Router(Echo, new Dictionary<string, Action<MyCommandLine>>(){
        { "test", p => Echo("Test") },
        { "run1", p => {
            int.TryParse(p.Argument(1), out run1Target);
            runManager.ScheduleRunFrequency(UpdateFrequency.Update1);
        }},
        { "run10", p => {
            int.TryParse(p.Argument(1), out run10Target);
            runManager.ScheduleRunFrequency(UpdateFrequency.Update10);
        }},
        { "run100", p => {
            int.TryParse(p.Argument(1), out run100Target);
            runManager.ScheduleRunFrequency(UpdateFrequency.Update100);
        }},
        { "pause", p => { runManager.Paused = true; }},
        { "start", p => { runManager.Paused = false; }},
        { "reset1", p => { run1 = 0; run1Target = 0; }},
        { "reset10", p => { run10 = 0; run10Target = 0; }},
        { "reset100", p => { run100 = 0; run100Target = 0; }}
    });
}

public void Save()
{
}

public void Main(string argument, UpdateType updateSource)
{
    Echo(indicator ? "[/-/-/-]" : "[-/-/-/]");
    indicator = !indicator;

    Echo("Main Source: " + updateSource);

    runManager.AnalyzeUpdateType(updateSource);
    if (runManager.IsAutoRun())
    {
        run1 = CheckTarget(run1, run1Target, UpdateFrequency.Update1);
        run10 = CheckTarget(run10, run10Target, UpdateFrequency.Update10);
        run100 = CheckTarget(run100, run100Target, UpdateFrequency.Update100);
    }
    else
    {
        router.ParseAndRoute(argument);
    }

    Echo("Run1: " + run1);
    Echo("Run1T: " + run1Target);

    runManager.ApplySchedule();
}

public int CheckTarget(int num, int numTarget, UpdateFrequency frequency)
{
    if (runManager.CheckForFrequency(frequency))
    {
        if (num < numTarget)
        {
            num++;
            runManager.ScheduleRunFrequency(frequency);
        }
        else
        {
            runManager.ScheduleRunFrequency(UpdateFrequency.None);
        }
    }

    return num;
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
        Debugger.Log("Arg: " + argument);
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

public interface IRunScheduler
{
    bool CheckForFrequency(UpdateFrequency frequency);
    bool CheckForFrequency(params UpdateFrequency[] frequencies);
    void ScheduleRunFrequency(UpdateFrequency frequency);
}

public class RunManager : IRunScheduler
{
    private readonly IMyGridProgramRuntimeInfo runtime;
    private byte weigth1to10 = 0;
    private byte weigth10to100 = 0;


    bool was1 = false;
    bool was10 = false;
    bool was100 = false;

    public bool Paused { get; set; }
    private UpdateFrequency scheduledFrequency = UpdateFrequency.None;
    private UpdateFrequency currentFrequency = UpdateFrequency.None;

    public RunManager(IMyGridProgramRuntimeInfo runtime)
    {
        this.runtime = runtime;
    }

    public void AnalyzeUpdateType(UpdateType updateType)
    {
        if (Paused)
        {
            return;
        }

        currentFrequency = UpdateFrequency.None;

        if (updateType.HasFlag(UpdateType.Update1))
        {
            was1 = true;
            weigth1to10++;
            currentFrequency |= UpdateFrequency.Update1;
        }

        if (updateType.HasFlag(UpdateType.Update10) || weigth1to10 >= 10)
        {
            was10 = true;
            weigth1to10 = 0;
            weigth10to100++;
            currentFrequency |= UpdateFrequency.Update10;
        }

        if (updateType.HasFlag(UpdateType.Update100) || weigth10to100 >= 10)
        {
            weigth10to100 = 0;
            was100 = true;
            currentFrequency |= UpdateFrequency.Update100;
        }

        if (updateType.HasFlag(UpdateType.Once))
        {
            weigth1to10++;
            currentFrequency |= UpdateFrequency.Once;
        }

        Debugger.Log("|1: " + was1 + " |10: " + was10 + " |100: " + was100);
        Debugger.Log("Analyzed As: " + currentFrequency);
    }

    public bool CheckForFrequency(UpdateFrequency frequency)
    {
        return currentFrequency.HasFlag(frequency);
    }

    public bool CheckForFrequency(params UpdateFrequency[] frequencies)
    {
        return frequencies.Any(frequency => currentFrequency.HasFlag(frequency));
    }

    public void ScheduleRunFrequency(UpdateFrequency frequency)
    {
        if (!scheduledFrequency.HasFlag(frequency))
        {
            scheduledFrequency |= frequency;
        }
    }

    public bool IsAutoRun()
    {
        return currentFrequency != UpdateFrequency.None;
    }

    public void ApplySchedule()
    {
        if (Paused)
        {
            runtime.UpdateFrequency = UpdateFrequency.None;
        }
        else
        {
            runtime.UpdateFrequency = scheduledFrequency;
            scheduledFrequency = UpdateFrequency.None;
        }

    }
}