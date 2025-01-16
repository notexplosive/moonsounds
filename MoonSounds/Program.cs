using ExplogineCore;
using ExplogineCore.Lua;
using MoonSounds;

var commandLineParameters = new CommandLineParameters(args);
commandLineParameters.RegisterParameter<bool>("help", "Prints help message");
commandLineParameters.RegisterParameter<int>("sampleRate", "Number of samples per second (default 48000)");

var commandLineArguments = commandLineParameters.Args;

if (commandLineArguments.GetValue<bool>("help"))
{
    Console.WriteLine(commandLineArguments.HelpOutput());
}

var sampleRate = commandLineArguments.GetValue<int>("sampleRate");
if (sampleRate <= 0)
{
    sampleRate = 48000;
}

var orderedArgs = commandLineParameters.Args.OrderedArgs().ToList();
var fileName = orderedArgs.FirstOrDefault();

if (string.IsNullOrEmpty(fileName))
{
    Console.WriteLine("No file provided");
    return;
}

var fileSystem = new RealFileSystem(".");
var luaRuntime = new LuaRuntime(fileSystem);
var resultFunction = luaRuntime.DoFile(fileName);
Console.WriteLine($"Attempting to run: {fileName}");

void EmitError()
{
    Console.WriteLine(luaRuntime!.CurrentError);
    Console.WriteLine(luaRuntime.Callstack());
}

if (luaRuntime.CurrentError != null)
{
    EmitError();
    return;
}

var sampleIncrement = 1f / sampleRate;
var foundFunction = resultFunction.Function;
if (foundFunction != null)
{
    var frames = new List<float>();
    for (var t = 0f; t < 5f; t += sampleIncrement)
    {
        var sample = luaRuntime.SafeCallFunction(foundFunction, t);
        if (luaRuntime.CurrentError != null)
        {
            Console.WriteLine($"Failed with input {t}");
            EmitError();
            return;
        }

        var outputAsNumber = sample.CastToNumber();
        if (!outputAsNumber.HasValue)
        {
            Console.WriteLine($"Input {t} did not give a numerical output, got {sample.CastToString()}");
            return;
        }

        // frames.Add((float) Math.Clamp(outputAsNumber.Value, -1, 1));
        frames.Add((float) outputAsNumber);
    }

    var wavFileName = fileName + ".wav";
    fileSystem.WriteToFileBytes(wavFileName, WriteWav.WriteWavFile(frames.ToArray(), sampleRate));
    Console.WriteLine($"Finished writing file: {wavFileName}");
}
