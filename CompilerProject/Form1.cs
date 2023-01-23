using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CompilerProject.Parsing;
using CompilerProject.Scanning;

namespace CompilerProject
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = "";

            var scanner = new Scanner();
            scanner.StartScanning(richTextBox1.Text);
            PrintTokens(scanner.Tokens);
            PrintErrors(scanner.ErrorsList);

            List<TokenType> tokenStream = new List<TokenType>();
            var tokens = scanner.Tokens;
            foreach (var lexemeToken in tokens)
            {
                tokenStream.Add(lexemeToken.token_type);
            }
            var parser = new Parser();
            var root = parser.StartParsing(tokenStream);
            PrintErrors(parser.Errors);
            TreeNodeCollection treeNodes = treeView1.Nodes;
            treeNodes.Clear();
            treeView1.Nodes.Add(Parser.PrintParseTree(root));
        }

        void PrintTokens(List<LexemeToken> tokens)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Lexeme");
            dataTable.Columns.Add("Token");

            dataTable.Rows.Clear();
            foreach (var lexemeToken in tokens)
            {
                dataTable.Rows.Add(lexemeToken.lex, lexemeToken.token_type);
            }
            dataGridView1.DataSource = dataTable;
        }

        void PrintErrors(List<string> errorsList)
        {
            foreach (string error in errorsList)
            {
                richTextBox2.Text += error + "\n";
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
