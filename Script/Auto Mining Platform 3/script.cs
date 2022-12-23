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

readonly PlatformConfig config;
readonly IniStateManager configManager;

bool indicator = false;

public Program()
{
    Debugger.Init(DEBUG_ENABLED, Echo);

    runManager = new RunManager(Runtime);

    config = new PlatformConfig();
    configManager = new PartialCustomDataIniStateManager(SaveConfig, LoadConfig, config.StatesToImmutableDictionary());

    router = new Router(Echo, new Dictionary<string, Action<MyCommandLine>>(){
        { "test", p => Echo("Test") },
        { "pause", p => { runManager.Paused = true; }},
        { "start", p => { runManager.Paused = false; }},
        { "load", p => { configManager.LoadStates(); }}
    });

    configManager.SaveStates();
    configManager.LoadStates();
}

public void Save()
{

}

public void Main(string argument, UpdateType updateSource)
{

    Echo(indicator ? "[/-/-/-]" : "[-/-/-/]");
    indicator = !indicator;

    runManager.AnalyzeUpdateType(updateSource);

    if (runManager.IsAutoRun())
    {
    }
    else
    {
        router.ParseAndRoute(argument);
    }

    Debugger.Debug(config.MainTag.Value);
    runManager.ApplySchedule();
}

public void SaveConfig(string customData) => Me.CustomData = customData;

public string LoadConfig() => Me.CustomData;

public void SaveToStorage(string storageString) => Storage = storageString;

public string LoadFromStorage() => Storage;

public class Debugger
{
    private static Action<string> DoLog = t => { return; };

public static void Init(bool enabled, Action<string> echo)
    {
        DoLog = enabled ? echo : t => { return; };
    }

public static void Debug(string text)
    {
        DoLog("[Debug]: " + text);
    }

public static void Warn(string text)
    {
        DoLog("[Warn]: " + text);
    }
}

public abstract class IniState
{
    public static readonly string DEFAULT_SECTION = "X";

    protected readonly MyIniKey iniKey;

    public MyIniKey GetIniKey()
    {
        return iniKey;
    }

public abstract void ParseAndSet(MyIni ini);

public abstract override string ToString();

    public IniState(string label)
    {
        iniKey = new MyIniKey(DEFAULT_SECTION, label);
    }

    public IniState(string section, string label)
    {
        iniKey = new MyIniKey(section, label);
    }
}

public class TypedIniState<T> : IniState
{
public delegate bool StateValueExtractor(MyIniValue value, out T tempValue);

    public T Value { get; set; }
    protected readonly T DefaultValue;
    private readonly StateValueExtractor valueExtractor;


    public TypedIniState(string section, string label, T defaultValue, StateValueExtractor valueExtractor) : base(section, label)
    {
        DefaultValue = defaultValue;
        Value = defaultValue;
        this.valueExtractor = valueExtractor;
    }

public override void ParseAndSet(MyIni ini)
    {
        MyIniValue myIniValue = ini.Get(iniKey);

        if (myIniValue.IsEmpty)
        {
            Value = DefaultValue;
            return;
        }

        T temp;
        if (!valueExtractor(myIniValue, out temp))
        {
            Debugger.Warn(iniKey.Name + " can't be parsed: " + myIniValue.ToString());
            return;
        }

        Value = temp == null ? DefaultValue : temp;
    }

public override string ToString()
    {
        return Value.ToString();
    }

    public KeyValuePair<MyIniKey, IniState> ToKeyValuePair()
    {
        return new KeyValuePair<MyIniKey, IniState>(GetIniKey(), this);
    }

    private static bool BoolExtractor(MyIniValue p, out bool k) => p.TryGetBoolean(out k);

public static TypedIniState<bool> OfBool(string section, string label, bool defaultValue, Func<bool, bool> validator = null, string validationFailedMessage = null)
    {

        return validator == null ?
            new TypedIniState<bool>(section, label, defaultValue, BoolExtractor) :
            new ValidatedTypedIniState<bool>(section, label, defaultValue, BoolExtractor, validator, validationFailedMessage);
    }

    private static bool IntExtractor(MyIniValue p, out int k) => p.TryGetInt32(out k);

public static TypedIniState<int> OfInt(string section, string label, int defaultValue, Func<int, bool> validator = null, string validationFailedMessage = null)
    {
        return validator == null ?
            new TypedIniState<int>(section, label, defaultValue, IntExtractor) :
            new ValidatedTypedIniState<int>(section, label, defaultValue, IntExtractor, validator, validationFailedMessage);
    }

    private static bool StringExtractor(MyIniValue p, out string k) => p.TryGetString(out k);

public static TypedIniState<string> OfString(string section, string label, string defaultValue, Func<string, bool> validator = null, string validationFailedMessage = null)
    {
        return validator == null ?
            new TypedIniState<string>(section, label, defaultValue, StringExtractor) :
            new ValidatedTypedIniState<string>(section, label, defaultValue, StringExtractor, validator, validationFailedMessage);
    }
}

public class ValidatedTypedIniState<T> : TypedIniState<T>
{
    private readonly Func<T, bool> validator;
    private readonly string validationFailedMessage;

    public ValidatedTypedIniState(string section, string label, T defaultValue, StateValueExtractor valueExtractor, Func<T, bool> validator, string validationFailedMessage) : base(section, label, defaultValue, valueExtractor)
    {
        this.validator = validator;
        this.validationFailedMessage = validationFailedMessage;
    }

    public override void ParseAndSet(MyIni ini)
    {
        base.ParseAndSet(ini);

        if (validator != null && !validator.Invoke(Value))
        {
            Value = DefaultValue;
            Debugger.Warn(GetIniKey().Name + " => " + (validationFailedMessage ?? "Validation Failed!"));
            Debugger.Warn(GetIniKey().Name + " falling back to default!");
        }
    }
}

public abstract class IniStateManager
{
    protected readonly MyIni _ini = new MyIni();

    protected readonly Action<string> setStateString;
    protected readonly Func<string> getStateString;

protected IniStateManager(Action<string> setStateString, Func<string> getStateString)
    {
        this.setStateString = setStateString;
        this.getStateString = getStateString;
    }

public abstract void SaveStates();

protected abstract void SetStates(MyIni ini);

public MyIniParseResult LoadStates()
    {
        MyIniParseResult result;
        if (!_ini.TryParse(getStateString(), out result))
        {
            return result;
        }

        SetStates(_ini);

        return result;
    }
}

public class PartialCustomDataIniStateManager : IniStateManager
{
    public const string DEFAULT_SECTION = "Config-Start";
    private const string CONFIG_END_SECTION = "Config-End";
    private const string CONFIG_START = "[" + DEFAULT_SECTION + "]";
    private const string CONFIG_END = "[" + CONFIG_END_SECTION + "]";

    private const string INI_END = "---";

    private const string DESCRIPTION =
        "You may add config VALUES to the script\n" +
        "between " + CONFIG_START + " and " + CONFIG_END + " in any order.\n" +
        "You can check the possible config VALUES\n" +
        "in the script's description.\n" +
        "\n" +
        "Please DO NOT change the content here,\n" +
        "except between " + CONFIG_START + " and " + CONFIG_END + ",\n" +
        "otherwise your config values may be lost,\n" +
        "or not applied correctly!\n";

    private const string FINISHER = "///";

    private const string DEFAULT_CONTENT =
        CONFIG_START + "\n" +
        "\n" +
        "\n" +
        "\n" +
        CONFIG_END + "\n" +
        INI_END + "\n" +
        DESCRIPTION +
        FINISHER;

    private readonly List<MyIniKey> _tempIniKeys = new List<MyIniKey>();

    private readonly ImmutableDictionary<MyIniKey, IniState> statesByKeys;

public PartialCustomDataIniStateManager(Action<string> setStateString, Func<string> getStateString, ImmutableDictionary<MyIniKey, IniState> statesByKeys) : base(setStateString, getStateString)
    {
        this.statesByKeys = statesByKeys;
    }

public override void SaveStates()
    {
        string stateString = getStateString();

        if (!stateString.StartsWith(CONFIG_START) || !stateString.EndsWith(FINISHER))
        {
            setStateString(DEFAULT_CONTENT);
        }
    }

protected override void SetStates(MyIni ini)
    {
        ini.GetKeys(DEFAULT_SECTION, _tempIniKeys);

        _tempIniKeys.ForEach(k => {
            if (!statesByKeys.ContainsKey(k))
            {
                Debugger.Warn(k.Name + " is not a valid config property!");
                return;
            }

            statesByKeys[k].ParseAndSet(ini);
        });

    }

}

public class PlatformConfig
{
    private const string DEFAULT_SECTION = PartialCustomDataIniStateManager.DEFAULT_SECTION;

public readonly TypedIniState<string> MainTag = TypedIniState<string>.OfString(DEFAULT_SECTION, "MAIN_TAG", "/Mine 01/", p => !string.IsNullOrEmpty(p), "Can't be empty!");

public ImmutableDictionary<MyIniKey, IniState> StatesToImmutableDictionary()
    {
        return new IniState[]
        {
            MainTag

        }.ToImmutableDictionary(k => k.GetIniKey(), v => v);
    }
}

public class Router
{
    private readonly Action<string> notification;
    private readonly Dictionary<string, Action<MyCommandLine>> routes;

    private readonly MyCommandLine _cl = new MyCommandLine();

public Router(Action<string> notification, Dictionary<string, Action<MyCommandLine>> routes)
    {
        this.notification = notification;
        this.routes = routes;
    }

public bool ParseAndRoute(string argument)
    {
        Debugger.Debug("Arg: " + argument);
        if (string.IsNullOrEmpty(argument))
        {
            notification("No Argument Provided!");
            return false;
        }
        else if (_cl.TryParse(argument))
        {
            routes.GetValueOrDefault(_cl.Argument(0), p => notification("Argument not found: " + argument))
                .Invoke(_cl);
            return true;
        }
        else
        {
            notification("Argument can't be parse: " + argument);
            return false;
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

private byte weightOf1 = 0;

private byte weightOf10 = 0;

public bool Paused { get; set; } = false;

public UpdateFrequency ScheduledFrequency { get; private set; } = UpdateFrequency.None;

public UpdateFrequency CurrentFrequency { get; private set; } = UpdateFrequency.None;

public RunManager(IMyGridProgramRuntimeInfo runtime)
    {
        this.runtime = runtime;
    }

public UpdateFrequency AnalyzeUpdateType(UpdateType updateType)
    {
        CurrentFrequency = UpdateFrequency.None;

        if (Paused)
        {
            return CurrentFrequency;
        }

        if (updateType.HasFlag(UpdateType.Once))
        {
            CurrentFrequency |= UpdateFrequency.Once;
            weightOf1++;
        }

        if (updateType.HasFlag(UpdateType.Update1))
        {
            CurrentFrequency |= UpdateFrequency.Update1;
            weightOf1++;
        }

        if (weightOf1 >= 10 || updateType.HasFlag(UpdateType.Update10))
        {
            CurrentFrequency |= UpdateFrequency.Update10;
            weightOf1 = 0;
            weightOf10++;
        }

        if (weightOf10 >= 10 || updateType.HasFlag(UpdateType.Update100))
        {
            CurrentFrequency |= UpdateFrequency.Update100;
            weightOf10 = 0;
        }

        return CurrentFrequency;
    }

public bool CheckForFrequency(UpdateFrequency frequency)
    {
        return CurrentFrequency.HasFlag(frequency);
    }

public bool CheckForFrequency(params UpdateFrequency[] frequencies)
    {
        return frequencies.Any(frequency => CheckForFrequency(frequency));
    }

public void ScheduleRunFrequency(UpdateFrequency frequency)
    {
        if (!ScheduledFrequency.HasFlag(frequency))
        {
            ScheduledFrequency |= frequency;
        }
    }

public bool IsAutoRun()
    {
        return CurrentFrequency != UpdateFrequency.None;
    }

public void ApplySchedule()
    {
        if (Paused)
        {
            runtime.UpdateFrequency = UpdateFrequency.None;
        }
        else
        {
            runtime.UpdateFrequency = ScheduledFrequency;
            ScheduledFrequency = UpdateFrequency.None;
        }
    }
}