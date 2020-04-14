using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace NMaier.GetOptNet
{
  [PublicAPI]
  public abstract class GetOptCommandBase : GetOpt
  {
    public abstract void Execute();

    public abstract string Name { get; }
  }

  [PublicAPI]
  public abstract class GetOptCommand<TOwner> : GetOptCommandBase where TOwner : GetOpt
  {
    protected GetOptCommand(TOwner owner)
    {
      Owner = owner;
    }

    public TOwner Owner { get; }

    protected override string GetUsageIntro(string image, string command)
    {
      return base.GetUsageIntro(image, Name);
    }

    public override string AssembleUsage(int width, HelpCategory category = HelpCategory.Basic, bool fixedWidthFont = true,
      bool introAndEpilogue = true, bool includeCommands = true)
    {
      var usage = base.AssembleUsage(width, category, fixedWidthFont, introAndEpilogue, false);
      var global = Owner.AssembleUsage(width, category, fixedWidthFont, false, false);
      if (!string.IsNullOrWhiteSpace(global)) {
        usage += Environment.NewLine + "Global Options:" + Environment.NewLine + global;
      }

      return usage;
    }
  }

  public abstract partial class GetOpt
  {
    [GetOptOptions(UsageIntro = "Print help about a command")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
    private sealed class HelpCommand : GetOptCommand<GetOpt>
    {
      [Parameters(Min = 0, Max = 1)]
      [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
      public string[] Command = { "help" };

      public HelpCommand(GetOpt owner) : base(owner)
      {
      }

      public override string Name => "help";

      public override void Execute()
      {
        if (Command.Length <= 0 || !Owner.commands.TryGetValue(Command[0], out var cmd)) {
          throw new GetOptException($"Invalid command: {Command[0]}");
        }

        if (cmd == this) {
          Owner.PrintUsage();
        }
        else {
          cmd.PrintUsage();
        }

        Environment.Exit(0);
      }
    }

    private Dictionary<string, GetOptCommandBase> commands = new Dictionary<string, GetOptCommandBase>(StringComparer.OrdinalIgnoreCase);

    private string defaultCommand;

    public GetOptCommandBase SelectedCommand { get; private set; }

    public void AddHelpCommand(bool isDefault = false)
    {
      AddCommand(new HelpCommand(this), isDefault);
    }

    public void AddCommand<TOwner>(GetOptCommand<TOwner> processor, bool isDefault = false)  where TOwner : GetOpt
    {
      commands.Add(processor.Name, processor);
      if (isDefault) {
        defaultCommand = processor.Name;
      }
    }
  }
}
