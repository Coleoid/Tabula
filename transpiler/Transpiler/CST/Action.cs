using System.Collections.Generic;

namespace Tabula.CST
{
    public class Action : ITaggable
    {
        public List<string> Tags { get; set; }
        public int StartLine { get; set; }
        public int EndLine { get; set; }
    }

    public class Block : Action
    {
        public List<Action> Actions { get; set; }  //TODO: promote to List<Section>
        public Block(List<Action> actions)
        {
            Actions = actions;
        }
    }

    public class CommandAlias : Action
    {
        public string Name { get; set; }
        public Action Action { get; set; }

        public CommandAlias(string name, Action action)
        {
            Name = name;
            Action = action;
        }
    }

    public class CommandSet : Action
    {
        public string Name { get; set; }
        public Symbol Term { get; set; }

        public CommandSet(string name, Symbol term)
        {
            Name = name;
            Term = term;
        }
    }

    public class CommandUse : Action
    {
        public List<string> Workflows { get; set; }
        public CommandUse(List<string> workflows)
        {
            Workflows = workflows;
        }
    }

}
