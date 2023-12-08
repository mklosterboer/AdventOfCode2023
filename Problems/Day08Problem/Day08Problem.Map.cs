using System.Text.RegularExpressions;

namespace AdventOfCode2023.Problems
{
    internal partial class Day08Problem
    {
        private partial class Map
        {
            public Dictionary<string, Node> Nodes { get; init; }
            private Direction[] Instructions { get; init; }

            public Map(string[] input)
            {
                Instructions = input.ElementAt(0).Select(x => x == 'L' ? Direction.Left : Direction.Right).ToArray();
                Nodes = ParseNodes(input);
            }

            public long GetTicksToEnd(Node startingNode, Func<string, bool> finishCondition)
            {
                var ticks = (long)0;
                var currentNodeValue = startingNode.Value;
                var instructionIndex = 0;

                while (!finishCondition(currentNodeValue))
                {
                    var instruction = Instructions[instructionIndex];
                    var curr = Nodes[currentNodeValue];

                    var next = curr.GetNextNode(instruction);

                    ticks++;
                    currentNodeValue = next.Value;

                    // This cycles an index back to the start after it reaches the end
                    // Thanks carousels...
                    instructionIndex = (instructionIndex + 1) % Instructions.Length;
                }

                return ticks;
            }

            private static Dictionary<string, Node> ParseNodes(string[] input)
            {
                var nodes = new Dictionary<string, Node>();

                for (var i = 2; i < input.Length; i++)
                {
                    var matchGroups = NodeRegex.Match(input.ElementAt(i)).Groups;

                    var leftNodeKey = matchGroups["left"].Value;
                    nodes.TryGetValue(leftNodeKey, out var leftNode);
                    if (leftNode == null)
                    {
                        leftNode = new Node(leftNodeKey);
                        nodes.Add(leftNodeKey, leftNode);
                    }

                    var rightNodeKey = matchGroups["right"].Value;
                    nodes.TryGetValue(rightNodeKey, out var rightNode);
                    if (rightNode == null)
                    {
                        rightNode = new Node(rightNodeKey);
                        nodes.Add(rightNodeKey, rightNode);
                    }

                    var thisNodeKey = matchGroups["node"].Value;
                    if (nodes.TryGetValue(thisNodeKey, out var thisNode))
                    {
                        thisNode.Left = leftNode;
                        thisNode.Right = rightNode;
                    }
                    else
                    {
                        nodes.Add(thisNodeKey, new Node(thisNodeKey, leftNode, rightNode));
                    }
                }

                return nodes;
            }

            private static readonly Regex NodeRegex = GetNodeRegex();

            [GeneratedRegex(@"(?'node'\S{3}) = \((?'left'\S{3}), (?'right'\S{3})\)")]
            private static partial Regex GetNodeRegex();
        }
    }
}
