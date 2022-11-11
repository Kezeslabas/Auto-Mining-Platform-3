/*
 * R e a d m e
 * -----------
 * 
 * In this file you can include any instructions or other comments you want to have injected onto the 
 * top of your final script. You can safely delete this file if you do not want any such comments.
 */

const bool DEBUG_ENABLED = true;

readonly Debugger debugger;
readonly Config config;
readonly ConfigBlockProvider blockProvider;

public Program()
{
    debugger = new Debugger(Echo, DEBUG_ENABLED);

    config = new Config(debugger);
    blockProvider = new ConfigBlockProvider(debugger, GetBlocksWithName, config, ImmutableList.Create<IBlockConsumer>());
}

public void Save()
{

}

public void Main(string argument, UpdateType updateSource)
{
    blockProvider.LoadBlocks();
}

public void GetBlocksWithName(string name, List<IMyTerminalBlock> blocks)
{
    GridTerminalSystem.SearchBlocksOfName(name, blocks);
}


public interface IBlockConsumer
{
    void ConsumeBlock(IMyTerminalBlock block);
}


public class ConfigBlockProvider : NamedBlockProvider
{
    private readonly IMainTagConfig mainTagConfig;

    public ConfigBlockProvider(Debugger debugger, Action<string, List<IMyTerminalBlock>> loadBlocksAction, IMainTagConfig mainTagConfig, ImmutableList<IBlockConsumer> consumers)
        : base(debugger, loadBlocksAction, consumers)
    {
        this.mainTagConfig = mainTagConfig;
    }

public void LoadBlocks()
    {
        LoadBlocks(mainTagConfig.MainTag);
    }
}

public abstract class Debuggable
{
    protected readonly Debugger debugger;

    protected Debuggable(Debugger debugger)
    {
        this.debugger = debugger;
    }
}

public class Debugger
{
    private readonly bool DEBUG_ENABLED;
    private Action<string> Echo;

    public Debugger(Action<string> echo, bool debugEnabled)
    {
        Echo = echo;
        DEBUG_ENABLED = debugEnabled;
    }

    public void Debug(string text)
    {
        if (DEBUG_ENABLED)
        {
            Echo(text);
        }
    }
}


public class NamedBlockProvider : Debuggable
{
    private readonly Action<string, List<IMyTerminalBlock>> loadBlocksAction;
    private readonly ImmutableList<IBlockConsumer> consumers;

    private readonly List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();

    public NamedBlockProvider(Debugger debugger, Action<string, List<IMyTerminalBlock>> loadBlocksAction, ImmutableList<IBlockConsumer> consumers) : base(debugger)
    {
        this.loadBlocksAction = loadBlocksAction;
        this.consumers = consumers;
    }

public void LoadBlocks(string name)
    {
        blocks.Clear();
        loadBlocksAction.Invoke(name, blocks);

        blocks.ForEach(block => {
            consumers.ForEach(consumer => consumer.ConsumeBlock(block));
        });

        debugger.Debug("Blocks found: " + blocks.Count);
    }
}

public interface IMainTagConfig
{
    string MainTag { get; }
}

public class Config : Debuggable, IMainTagConfig
{
    public Config(Debugger debugger) : base(debugger)
    {
    }

    public string MainTag { get; private set; } = "/Mine 01/";
}