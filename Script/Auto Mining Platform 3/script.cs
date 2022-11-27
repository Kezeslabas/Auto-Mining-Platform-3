/*
 * R e a d m e
 * -----------
 * 
 * In this file you can include any instructions or other comments you want to have injected onto the 
 * top of your final script. You can safely delete this file if you do not want any such comments.
 */

readonly bool DEBUG_ENABLED = true;

readonly Debugger debugger = new Debugger();

public Program()
{
    Debugger.Log("Before Init");

    Debugger.Init(DEBUG_ENABLED, Echo);

    Debugger.Log("After Init");
}

public void Save()
{
}

public void Main(string argument, UpdateType updateSource)
{
    MyClass asd = new MyClass();
    asd.DoStaff();
}

class MyClass
{
    public void DoStaff()
    {
        Debugger.Log("Inside Class");
    }
}

public void GetBlocksWithName(string name, List<IMyTerminalBlock> blocks)
{
    GridTerminalSystem.GetBlocksOfType(blocks, block => block.CustomName.Contains(name));
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