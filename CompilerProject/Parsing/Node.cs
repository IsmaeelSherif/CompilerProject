using System;
using System.Collections.Generic;
using System.Text;

namespace CompilerProject.Parsing
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
}
