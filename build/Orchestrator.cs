using System;
using Colors.Net;
using Colors.Net.StringColorExtensions;

namespace Build
{
    public class Orchestrator
    {
        public static Orchestrator CreateForTarget(string targetToRun) => new Orchestrator(targetToRun);

        private Node head = null;
        private Node tail = null;
        private string targetToRun;

        private Orchestrator(string targetToRun)
        {
            this.targetToRun = targetToRun;
        }

        public Orchestrator Then(Action target, string name = null, bool skip = false)
        {
            var node = new Node
            {
                Target = target,
                Name = name ?? target.Method.Name,
                Skip = skip
            };

            if (head == null)
            {
                head = node;
                tail = node;
            }
            else
            {
                tail.Next = node;
                node.Previous = tail;
                tail = node;
            }

            return this;
        }

        public void Run()
        {
            var start = head;
            var end = tail;
            while (end != null &&
                   !string.IsNullOrWhiteSpace(targetToRun) &&
                   !end.Name.Equals(targetToRun))
            {
                end = end.Previous;
            }

            if (end == null)
            {
                ColoredConsole.Error.WriteLine("No target was found to start".Red());
                return;
            }

            PrintTargetsToRun(start, end);

            while (start != null)
            {
                if (!start.Skip)
                {
                    ColoredConsole.WriteLine($"Running Target {start.Name}".Cyan());
                    start.Target();
                }

                if (start == end)
                {
                    break;
                }

                start = start.Next;
            }
        }

        private void PrintTargetsToRun(Node start, Node end)
        {
            while (start != null)
            {
                ColoredConsole.Write($"{start.Name} ==> ".Magenta());
                if (start == end)
                {
                    break;
                }

                start = start.Next;
            }
            ColoredConsole.WriteLine("DONE".Magenta());
        }

        class Node
        {
            public Action Target;
            public string Name;
            public Node Next;
            public Node Previous;
            public bool Skip;
        }
    }
}