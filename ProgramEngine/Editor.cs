using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Speech.Recognition;
using System.Speech.Recognition.SrgsGrammar;
using System.Xml;
using System.Globalization;
using System.Windows.Documents;

using System.CodeDom.Compiler;
using System.Diagnostics;
using Microsoft.CSharp;

namespace ProgramEngine
{
    public partial class Editor : Form
    {
        public enum CodeLang { Text, HTML, PHP, CS, JavaScript, CSS, XML, JSON, NONE };

        public CodeLang VoiceLang = CodeLang.PHP;


        public int cursorlinenum = 0;//current cursor line number
        int cursorpos;//overall position
        int colpos;//position on cur line
        string[] alllines;//every line of text split into an array
        int curlinelength;//length of cur line
        int prevlinelength;//length of prev line
        int nextlinelength;//length of next line
        int startofcurline;//position for the start of cur line
        int startofprevline;//position for the start of prev line
        int startofnextline;//position for the start of next line


        string openfilepath = "";


        WebView WebViewTab= new WebView();

        public Editor()
        {
            InitializeComponent();
        }

        private void pasteToolStripButton_Click(object sender, EventArgs e)
        {
            pasteToolStripMenuItem.PerformClick();
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            newToolStripMenuItem.PerformClick();
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            openToolStripMenuItem.PerformClick();
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            saveToolStripMenuItem.PerformClick();
        }

        private void cutToolStripButton_Click(object sender, EventArgs e)
        {
            cutToolStripMenuItem.PerformClick();
        }

        private void copyToolStripButton_Click(object sender, EventArgs e)
        {
            copyToolStripMenuItem.PerformClick();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Paste();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Copy();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Cut();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.CanUndo)
            {
                richTextBox1.Undo();
                richTextBox1.ClearUndo();
            }
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (richTextBox1.CanRedo)
            {
               if (richTextBox1.RedoActionName != "Delete") {
                    richTextBox1.Redo();
                }
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openfilepath = "";
            richTextBox1.Clear();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openfile = new OpenFileDialog();
            openfile.Title = "Open File";
            if (openfile.ShowDialog() == DialogResult.OK)
            {
                openfilepath = openfile.FileName;
                richTextBox1.Clear();
                using (StreamReader sr = new StreamReader(openfile.FileName))
                {
                    richTextBox1.Text = sr.ReadToEnd();
                    sr.Close();
                }

            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (openfilepath == "") {
                saveAsToolStripMenuItem.PerformClick();
            } else {
                StreamWriter sw = new StreamWriter(openfilepath);
                sw.Write(richTextBox1.Text);
                sw.Close();
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.Title = "Save File as";
            if (savefile.ShowDialog() == DialogResult.OK)
            {
                openfilepath = savefile.FileName;
                StreamWriter sw = new StreamWriter(savefile.FileName);
                sw.Write(richTextBox1.Text);
                sw.Close();
            }

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

            // richTextBox1.ProcessAllLines();

            //  int StartCursorPosition = richTextBox1.SelectionStart;
            //  int StartSelLen = richTextBox1.SelectionLength;

            // richText_syntax_tokens();

            //richTextBox1.SelectionStart = StartCursorPosition;
            //richTextBox1.SelectionLength = StartSelLen;
            //richTextBox1.SelectionColor = Color.Black;

        }


        private void richText_syntax_tokens()
        {

            string tokens = "(echo|auto|double|int|struct|break|else|long|switch|case|enum|register|typedef|char|extern|return|union|const|float|short|unsigned|continue|for|signed|void|default|goto|sizeof|volatile|do|if|static|while)";
            Regex rex = new Regex(tokens);
            MatchCollection mc = rex.Matches(richTextBox1.Text);

            foreach (Match m in mc)
            {
                int startIndex = m.Index;
                int StopIndex = m.Length;

                richTextBox1.SelectionStart = startIndex;
                richTextBox1.SelectionLength = StopIndex;
                //richTextBox1.Select(startIndex, StopIndex);
                richTextBox1.SelectionColor = Color.Blue;

                richTextBox1.SelectionStart = startIndex + StopIndex;
                richTextBox1.SelectionLength = 0;
                richTextBox1.SelectionColor = Color.Black;

            }

        }

        private void Editor_Load(object sender, EventArgs e)
        {
            
            richTextBox1.Font= new Font("Microsoft Sans Serif", 16, FontStyle.Bold);//set font size and style
                                                                                    // Add the keywords to the list.
            richTextBox1.Settings.EnableVarPrefix = true;
            //richTextBox1.Settings.VarPrefix = "";//match any word
            richTextBox1.Settings.VarPrefix = "\\$";// match php vars // targets from $ to end of a word


            richTextBox1.Settings.Keywords.Add("function");
            richTextBox1.Settings.Keywords.Add("if");
            richTextBox1.Settings.Keywords.Add("then");
            richTextBox1.Settings.Keywords.Add("else");
            richTextBox1.Settings.Keywords.Add("elseif");
            richTextBox1.Settings.Keywords.Add("else if");
            richTextBox1.Settings.Keywords.Add("end");


            richTextBox1.Settings.Keywords.Add("public");
            richTextBox1.Settings.Keywords.Add("protected");


            richTextBox1.Settings.Keywords.Add("class");

            richTextBox1.Settings.Keywords.Add("void");
            richTextBox1.Settings.Keywords.Add("string");
            richTextBox1.Settings.Keywords.Add("int");
            richTextBox1.Settings.Keywords.Add("float");
            richTextBox1.Settings.Keywords.Add("bool");
            richTextBox1.Settings.Keywords.Add("list");
            richTextBox1.Settings.Keywords.Add("array");
            richTextBox1.Settings.Keywords.Add("object");
            richTextBox1.Settings.Keywords.Add("true");
            richTextBox1.Settings.Keywords.Add("false");
            richTextBox1.Settings.Keywords.Add("and");
            richTextBox1.Settings.Keywords.Add("&&");
            richTextBox1.Settings.Keywords.Add("new");

            // Set the comment identifier. 
            // For Lua this is two minus-signs after each other (--).
            // For C++ code we would set this property to "//".
            richTextBox1.Settings.Comment = "//";

            // Set the colors that will be used.
            richTextBox1.Settings.KeywordColor = Color.Blue;
            richTextBox1.Settings.CommentColor = Color.Gray;
            richTextBox1.Settings.StringColor = Color.Purple;
            richTextBox1.Settings.IntegerColor = Color.Red;

            // Let's not process strings and integers.
            richTextBox1.Settings.EnableStrings = true;
            richTextBox1.Settings.EnableIntegers = true;

            // Let's make the settings we just set valid by compiling
            // the keywords to a regular expression.
            richTextBox1.CompileKeywords();

            // Load a file and update the syntax highlighting.
            //  richTextBox1.LoadFile("script.txt", RichTextBoxStreamType.PlainText);

            richTextBox1.SelectionStart = 0;
            richTextBox1.SelectionLength = 0;

            richTextBox1.Text = " ";
            richTextBox1.Text = "";
            setupGrammerREC();

        }


        public void setupGrammerREC() {

            // Create an in-process speech recognizer for the en-US locale.

            SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-US"));


            // Attach event handlers.
            recognizer.LoadGrammarCompleted +=
                  new EventHandler<LoadGrammarCompletedEventArgs>(recognizer_LoadGrammarCompleted);
            recognizer.SpeechDetected +=
              new EventHandler<SpeechDetectedEventArgs>(recognizer_SpeechDetected);
            recognizer.SpeechRecognized +=
              new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized);



            string path = Directory.GetCurrentDirectory();
            string grammarPath = path + @"\Grammer\";
            string xmlGrammar = grammarPath + "grammer.xml";
            string cfgGrammar = grammarPath + "grammer.cfg";

            ////  Console.WriteLine("xmlGrammar path: " + xmlGrammar);
            FileStream fs = new FileStream(cfgGrammar, FileMode.Create);
            SrgsGrammarCompiler.Compile(xmlGrammar, (Stream)fs);
            fs.Close();
            Grammar gr = new Grammar(cfgGrammar, "def");
            gr.Name = "ProgramEngine Voice Control";
            // Create and load a dictation grammar.
            recognizer.LoadGrammarAsync(gr);

            //recognizer.LoadGrammarAsync(g);



            // Configure input to the speech recognizer.
            recognizer.SetInputToDefaultAudioDevice();

            // Start asynchronous, continuous speech recognition.
            recognizer.RecognizeAsync(RecognizeMode.Multiple);
            // recognizer.RecognizeAsync(RecognizeMode.Single);
            // Start recognition.
            // recognizer.RecognizeAsync();

            Console.WriteLine("Recognized started");

            // Keep the console window open.
            //while (true)
            //{
            //Console.ReadLine();
            // }


        }




        // Handle the SpeechDetected event.
        static void recognizer_SpeechDetected(object sender, SpeechDetectedEventArgs e)
        {
            //   Console.WriteLine("  Speech detected at AudioPosition = {0}", e.AudioPosition);
        }

        // Handle the LoadGrammarCompleted event.
        static void recognizer_LoadGrammarCompleted(object sender, LoadGrammarCompletedEventArgs e)
        {
            Console.WriteLine("Grammar loaded: " + e.Grammar.Name);
        }

        // Handle the SpeechRecognized event.
        public void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {

            if (e.Result != null)
            {
                if (e.Result.Semantics != null)
                {
                    // a = (int)e.Result.Semantics["first_value"].Value;

                    Console.WriteLine("  Speech recognized: " + e.Result.Text);
                    GR_Data CurGRData = new GR_Data();

                    // SemanticValue semanticResults  = e.Result.Semantics;

                    CurGRData.Name = "";
                    CurGRData.Repeat = 1;
                    //Debug.Log( " action: " + args.semanticMeanings [2].values [0]+" type: "+args.semanticMeanings [1].values [0]+" name: "+args.semanticMeanings [0].values [0]+" repeat: "+args.semanticMeanings [3].values [0]);



                    //   CurGRData.Action=(string)e.Result.Semantics["Action"].Value;
                    //  CurGRData.Type = (string)e.Result.Semantics["Type"].Value;


                    foreach (KeyValuePair<string, SemanticValue> curmeaning in e.Result.Semantics)
                    {
                        SemanticValue tmpval = curmeaning.Value;

                        if (tmpval != null)
                        {

                            //Debug.Log(curmeaning.key);
                            if (curmeaning.Key == "Type")
                            {
                                CurGRData.Type = (string)e.Result.Semantics[curmeaning.Key].Value;
                            }
                            if (curmeaning.Key == "Action")
                            {
                                CurGRData.Action = (string)e.Result.Semantics[curmeaning.Key].Value;
                            }
                            if (curmeaning.Key == "Name")
                            {
                                CurGRData.Name = (string)e.Result.Semantics[curmeaning.Key].Value;
                            }
                            if (curmeaning.Key == "Repeat")
                            {
                                switch ((string)e.Result.Semantics[curmeaning.Key].Value)
                                {
                                    case "one":
                                        CurGRData.Repeat = 1;
                                        break;
                                    case "two":
                                        CurGRData.Repeat = 2;
                                        break;
                                    case "three":
                                        CurGRData.Repeat = 3;
                                        break;
                                    case "four":
                                        CurGRData.Repeat = 4;
                                        break;
                                    case "five":
                                        CurGRData.Repeat = 5;
                                        break;
                                    case "six":
                                        CurGRData.Repeat = 6;
                                        break;
                                    case "seven":
                                        CurGRData.Repeat = 7;
                                        break;
                                    case "eight":
                                        CurGRData.Repeat = 8;
                                        break;
                                    case "nine":
                                        CurGRData.Repeat = 9;
                                        break;
                                    case "ten":
                                        CurGRData.Repeat = 10;
                                        break;
                                }
                            }

                        }


                    }

                    //process data
                    ProcessGRData(CurGRData);

                }
            }
        }














        public void Code_native(GR_Data CurGRData)
        {
            Code_define__(CurGRData.Name);
        }


        public void Code_dictation(GR_Data CurGRData)
        {
            Code_define__(CurGRData.Name);
        }





        public void Code_call_PHP(GR_Data CurGRData)
        {
            if (CurGRData.Type == "function")
            {
                Code_call_PHP_function(CurGRData.Name.Replace(" ", ""));
            }

            if (CurGRData.Type == "variable")
            {
                Code_call_PHP_variable(CurGRData.Name.Replace(" ", ""));

            }

            if (CurGRData.Type == "camel function")
            {
                Code_call_PHP_function(ToCamelCase(CurGRData.Name));
            }

            if (CurGRData.Type == "camel variable")
            {
                Code_call_PHP_variable(ToCamelCase(CurGRData.Name));
            }


            if (CurGRData.Type == "score function")
            {
                Code_call_PHP_function(ToScoreCase(CurGRData.Name));
            }

            if (CurGRData.Type == "score variable")
            {
                Code_call_PHP_variable(ToScoreCase(CurGRData.Name));
            }

            if (CurGRData.Type == "dot function")
            {
                Code_call_PHP_function(ToDotCase(CurGRData.Name));
            }

            if (CurGRData.Type == "dot variable")
            {
                Code_call_PHP_variable(ToDotCase(CurGRData.Name));
            }
            if (CurGRData.Type == "dash function")
            {
                Code_call_PHP_function(ToDashCase(CurGRData.Name));
            }

            if (CurGRData.Type == "dash variable")
            {
                Code_call_PHP_variable(ToDashCase(CurGRData.Name));
            }

        }





        public void Code_call_CS(GR_Data CurGRData)
        {
            if (CurGRData.Type == "object")
            {
                Code_define__(CurGRData.Name);
            }

            if (CurGRData.Type == "function")
            {
                Code_call_CS_function(CurGRData.Name.Replace(" ", ""));
            }

            if (CurGRData.Type == "variable")
            {
                Code_call_CS_variable(CurGRData.Name.Replace(" ", ""));

            }

            if (CurGRData.Type == "camel function")
            {
                Code_call_CS_function(ToCamelCase(CurGRData.Name));
            }

            if (CurGRData.Type == "camel variable")
            {
                Code_call_CS_variable(ToCamelCase(CurGRData.Name));
            }


            if (CurGRData.Type == "score function")
            {
                Code_call_CS_function(ToScoreCase(CurGRData.Name));
            }

            if (CurGRData.Type == "score variable")
            {
                Code_call_CS_variable(ToScoreCase(CurGRData.Name));
            }

            if (CurGRData.Type == "dot function")
            {
                Code_call_CS_function(ToDotCase(CurGRData.Name));
            }

            if (CurGRData.Type == "dot variable")
            {
                Code_call_CS_variable(ToDotCase(CurGRData.Name));
            }
            if (CurGRData.Type == "dash function")
            {
                Code_call_CS_function(ToDashCase(CurGRData.Name));
            }

            if (CurGRData.Type == "dash variable")
            {
                Code_call_CS_variable(ToDashCase(CurGRData.Name));
            }
        }






        public void Code_call_CS_function(string name)
        {
            string template = "" + name + "();";
            Code_define__(template);
        }

        public void Code_call_CS_variable(string name)
        {
            string template = "" + name + "";
            Code_define__(template);
        }



        public void Code_call_PHP_function(string name)
        {
            string template = "" + name + "();";
            Code_define__(template);
        }

        public void Code_call_PHP_variable(string name)
        {
            string template = "$" + name + "";
            Code_define__(template);
        }




        public void Code_define__(string template)
        {

            cursorpos = richTextBox1.SelectionStart;
            richTextBox1.Text = richTextBox1.Text.Insert(richTextBox1.SelectionStart, template);
            richTextBox1.SelectionStart = cursorpos+template.Length;
            richTextBox1.SelectionLength = 0;
        }






        public string Code_elem_SC(string elemname)
        {
            return "<" + elemname + "/>";

        }
        public string Code_elem(string elemname)
        {
            return "<" + elemname + ">" + "</" + elemname + ">";

        }



        public void Code_Define_XML(GR_Data CurGRData)
        {

            if (CurGRData.Type == "element")
            {
                Code_define__(Code_elem(CurGRData.Name.Replace(" ", "")));
            }
            if (CurGRData.Type == "elementSC")
            {
                Code_define__(Code_elem_SC(CurGRData.Name.Replace(" ", "")));
            }

        }



        public void Code_Define_CS(GR_Data CurGRData)
        {

            if (CurGRData.Type == "variable")
            {
                Code_define__(CurGRData.Name.Replace(" ", ""));
            }
            if (CurGRData.Type == "camel variable")
            {
                Code_define__(ToCamelCase(CurGRData.Name));
            }
            if (CurGRData.Type == "score variable")
            {
                Code_define__(ToScoreCase(CurGRData.Name));
            }
            if (CurGRData.Type == "dot variable")
            {
                Code_define__(ToDotCase(CurGRData.Name));
            }
            if (CurGRData.Type == "dash variable")
            {
                Code_define__(ToDashCase(CurGRData.Name));
            }


            if (CurGRData.Type == "string")
            {
                Code_define__("string " + CurGRData.Name.Replace(" ", "") + "=\"\";");
            }
            if (CurGRData.Type == "camel string")
            {
                Code_define__("string " + ToCamelCase(CurGRData.Name) + "=\"\";");
            }
            if (CurGRData.Type == "score string")
            {
                Code_define__("string " + ToScoreCase(CurGRData.Name) + "=\"\";");
            }


            if (CurGRData.Type == "class")
            {
                Code_define__("class " + CurGRData.Name.Replace(" ", "") + "{\n\n}");
            }
            if (CurGRData.Type == "camel class")
            {
                Code_define__("class " + ToCamelCase(CurGRData.Name) + "{\n\n}");
            }
            if (CurGRData.Type == "score class")
            {
                Code_define__("class " + ToScoreCase(CurGRData.Name) + "{\n\n}");
            }



            if (CurGRData.Type == "new class")
            {
                Code_define__("new " + CurGRData.Name.Replace(" ", "") + "();");
            }
            if (CurGRData.Type == "new camel class")
            {
                Code_define__("new " + ToCamelCase(CurGRData.Name) + "();");
            }
            if (CurGRData.Type == "new score class")
            {
                Code_define__("new " + ToScoreCase(CurGRData.Name) + "();");
            }

            if (CurGRData.Type == "object")
            {
                Code_define__("new " + CurGRData.Name.Replace(" ", "") + "();");
            }
            if (CurGRData.Type == "camel object")
            {
                Code_define__("new " + ToCamelCase(CurGRData.Name) + "();");
            }
            if (CurGRData.Type == "score object")
            {
                Code_define__("new " + ToScoreCase(CurGRData.Name) + "();");
            }



            if (CurGRData.Type == "int")
            {
                Code_define__("int " + CurGRData.Name.Replace(" ", "") + "=0;");
            }
            if (CurGRData.Type == "camel int")
            {
                Code_define__("int " + ToCamelCase(CurGRData.Name) + "=0;");
            }
            if (CurGRData.Type == "score int")
            {
                Code_define__("int " + ToScoreCase(CurGRData.Name) + "=0;");
            }


            if (CurGRData.Type == "bool")
            {
                Code_define__("bool " + CurGRData.Name.Replace(" ", "") + "=false;");
            }
            if (CurGRData.Type == "camel bool")
            {
                Code_define__("bool " + ToCamelCase(CurGRData.Name) + "=false;");
            }
            if (CurGRData.Type == "score bool")
            {
                Code_define__("bool " + ToScoreCase(CurGRData.Name) + "=false;");
            }



            if (CurGRData.Type == "list")
            {
                Code_define__("List<T> " + CurGRData.Name.Replace(" ", "") + ";");
            }
            if (CurGRData.Type == "camel list")
            {
                Code_define__("List<T> " + ToCamelCase(CurGRData.Name) + ";");
            }
            if (CurGRData.Type == "score list")
            {
                Code_define__("List<T> " + ToScoreCase(CurGRData.Name) + ";");
            }


            if (CurGRData.Type == "new list")
            {
                Code_define__("List<T> " + CurGRData.Name.Replace(" ", "") + "= new List<T>();");
            }
            if (CurGRData.Type == "new camel list")
            {
                Code_define__("List<T> " + ToCamelCase(CurGRData.Name) + "= new List<T>();");
            }
            if (CurGRData.Type == "new score list")
            {
                Code_define__("List<T> " + ToScoreCase(CurGRData.Name) + "= new List<T>();");
            }


            if (CurGRData.Type == "enum")
            {
                Code_define__("enum " + CurGRData.Name.Replace(" ", "") + " {};");
            }
            if (CurGRData.Type == "camel enum")
            {
                Code_define__("enum " + ToCamelCase(CurGRData.Name) + " {};");
            }
            if (CurGRData.Type == "score enum")
            {
                Code_define__("enum " + ToScoreCase(CurGRData.Name) + " {};");
            }


            if (CurGRData.Type == "vector2")
            {
                Code_define__("Vector2 " + CurGRData.Name.Replace(" ", "") + ";");
            }
            if (CurGRData.Type == "camel vector2")
            {
                Code_define__("Vector2 " + ToCamelCase(CurGRData.Name) + ";");
            }
            if (CurGRData.Type == "score vector2")
            {
                Code_define__("Vector2 " + ToScoreCase(CurGRData.Name) + ";");
            }




            if (CurGRData.Type == "new vector2")
            {
                Code_define__("Vector2 " + CurGRData.Name.Replace(" ", "") + "= new Vector2(0,0);");
            }
            if (CurGRData.Type == "new camel vector2")
            {
                Code_define__("Vector2 " + ToCamelCase(CurGRData.Name) + "= new Vector2(0,0);");
            }
            if (CurGRData.Type == "new score vector2")
            {
                Code_define__("Vector2 " + ToScoreCase(CurGRData.Name) + "= new Vector2(0,0);");
            }


            if (CurGRData.Type == "vector3")
            {
                Code_define__("Vector3 " + CurGRData.Name.Replace(" ", "") + ";");
            }
            if (CurGRData.Type == "camel vector3")
            {
                Code_define__("Vector3 " + ToCamelCase(CurGRData.Name) + ";");
            }
            if (CurGRData.Type == "score vector3")
            {
                Code_define__("Vector3 " + ToScoreCase(CurGRData.Name) + ";");
            }




            if (CurGRData.Type == "new vector3")
            {
                Code_define__("Vector3 " + CurGRData.Name.Replace(" ", "") + "= new Vector3(0,0,0);");
            }
            if (CurGRData.Type == "new camel vector3")
            {
                Code_define__("Vector3 " + ToCamelCase(CurGRData.Name) + "= new Vector3(0,0,0);");
            }
            if (CurGRData.Type == "new score vector3")
            {
                Code_define__("Vector3 " + ToScoreCase(CurGRData.Name) + "= new Vector3(0,0,0);");
            }



            if (CurGRData.Type == "void")
            {
                Code_define__("void " + CurGRData.Name.Replace(" ", "") + "() {\n\n}");
            }
            if (CurGRData.Type == "camel void")
            {
                Code_define__("void " + ToCamelCase(CurGRData.Name) + "() {\n\n}");
            }
            if (CurGRData.Type == "score void")
            {
                Code_define__("void " + ToScoreCase(CurGRData.Name) + "() {\n\n}");
            }



            if (CurGRData.Type == "float")
            {
                Code_define__("float " + CurGRData.Name.Replace(" ", "") + "=0.0f;");
            }
            if (CurGRData.Type == "camel float")
            {
                Code_define__("float " + ToCamelCase(CurGRData.Name) + "=0.0f");
            }
            if (CurGRData.Type == "score float")
            {
                Code_define__("float " + ToScoreCase(CurGRData.Name) + "=0.0f;");
            }



            if (CurGRData.Type == "transform")
            {
                Code_define__("Transform " + CurGRData.Name.Replace(" ", "") + ";");
            }
            if (CurGRData.Type == "camel transform")
            {
                Code_define__("Transform " + ToCamelCase(CurGRData.Name) + ";");
            }
            if (CurGRData.Type == "score transform")
            {
                Code_define__("Transform " + ToScoreCase(CurGRData.Name) + ";");
            }



            if (CurGRData.Type == "game object")
            {
                Code_define__("GameObject " + CurGRData.Name.Replace(" ", "") + ";");
            }
            if (CurGRData.Type == "camel game object")
            {
                Code_define__("GameObject " + ToCamelCase(CurGRData.Name) + ";");
            }
            if (CurGRData.Type == "score game object")
            {
                Code_define__("GameObject " + ToScoreCase(CurGRData.Name) + ";");
            }



        }







        public void Code_Define_PHP(GR_Data CurGRData)
        {
            if (CurGRData.Type == "function")
            {
                Code_Define_PHP_function(CurGRData.Name.Replace(" ", ""));
            }

            if (CurGRData.Type == "variable")
            {
                Code_Define_PHP_variable(CurGRData.Name.Replace(" ", ""));

            }

            if (CurGRData.Type == "camel function")
            {
                Code_Define_PHP_function(ToCamelCase(CurGRData.Name));
            }

            if (CurGRData.Type == "camel variable")
            {
                Code_Define_PHP_variable(ToCamelCase(CurGRData.Name));
            }


            if (CurGRData.Type == "score function")
            {
                Code_Define_PHP_function(ToScoreCase(CurGRData.Name));
            }

            if (CurGRData.Type == "score variable")
            {
                Code_Define_PHP_variable(ToScoreCase(CurGRData.Name));
            }

            if (CurGRData.Type == "dot function")
            {
                Code_Define_PHP_function(ToDotCase(CurGRData.Name));
            }

            if (CurGRData.Type == "dot variable")
            {
                Code_Define_PHP_variable(ToDotCase(CurGRData.Name));
            }
            if (CurGRData.Type == "dash function")
            {
                Code_Define_PHP_function(ToDashCase(CurGRData.Name));
            }

            if (CurGRData.Type == "dash variable")
            {
                Code_Define_PHP_variable(ToDashCase(CurGRData.Name));
            }
        }







        public void Code_Define_PHP_function(string name)
        {
            string template = "function " + name + " (){\n\n\n}\n\n";
            Code_define__(template);
        }

        public void Code_Define_PHP_variable(string name)
        {
            string template = "$" + name + "=\"\";";
            Code_define__(template);
        }









        public void Code_navigation(GR_Data CurGRData)
        {

            for (int x = 0; x < CurGRData.Repeat; x++)
            {

                switch (CurGRData.Type)
                {


                    case "line number":
                        int navline = Int32.Parse(CurGRData.Name.Replace(" ", ""));
                        cursorpos =richTextBox1.GetFirstCharIndexFromLine(navline-1);
                        richTextBox1.Select(cursorpos, 0);
                        break;
                    case "new line":
                        cursorpos = richTextBox1.SelectionStart;
                        richTextBox1.Text = richTextBox1.Text.Insert(richTextBox1.SelectionStart, "\n");
                        richTextBox1.SelectionStart = cursorpos+1;
                        richTextBox1.SelectionLength = 0;
                        break;
                    case "home":
                        startofcurline = richTextBox1.GetFirstCharIndexOfCurrentLine();
                        richTextBox1.Select(startofcurline, 0);
                        break;
                    case "end":

                        alllines = richTextBox1.Lines;
                        cursorpos = richTextBox1.SelectionStart;
                        cursorlinenum = richTextBox1.GetLineFromCharIndex(cursorpos);
                        startofcurline = richTextBox1.GetFirstCharIndexOfCurrentLine();
                        curlinelength = alllines[cursorlinenum].Length;
                        richTextBox1.Select(startofcurline + curlinelength, 0);
                        break;

                    case "up":

                        navigateUp();

                        break;
                    case "down":
                        navigateDown();
                        break;

                    case "next comma":
                        findText(",");
                        break;

                    case "last comma":
                        findText(",", false);
                        break;

                    case "next single quote":

                        findText("'");
                        break;

                    case "last single quote":

                        findText("'", false);
                        break;

                    case "next double quote":

                        findText("\"");
                        break;

                    case "last double quote":

                        findText("\"", false);
                        break;

                    case "next perenthises":
                        findText("()", true, true);
                        break;

                    case "last perenthises":
                        findText("()", false, true);
                        break;


                    case "next brace":
                        findText("{}", true, true);
                        break;

                    case "last brace":
                        findText("{}", false, true);
                        break;


                    case "next bracket":
                        findText("[]", true, true);
                        break;

                    case "last bracket":
                        findText("[]", false, true);
                        break;


                    case "next function":
                        findText("function");
                        break;

                    case "last function":
                        findText("function", false);
                        break;

                    case "next paragraph":
                        //editor.MoveParagraphForward();
                        break;
                    case "last paragraph":
                        //editor.MoveParagraphBackward();
                        break;
                    case "next line":
                        navigateNextLine();
                        break;
                    case "last line":
                        navigateLastLine();
                        break;
                    case "left":
                        cursorpos = richTextBox1.SelectionStart;
                        if (cursorpos > 0) {
                            richTextBox1.Select(cursorpos - 1, 0);
                        }
                        break;
                    case "right":
                        cursorpos = richTextBox1.SelectionStart;
                        if (cursorpos < richTextBox1.TextLength)
                        {
                            richTextBox1.Select(cursorpos + 1, 0);
                        }
                        break;
                    case "next word":
                        richTextBox1.Focus();
                        SendKeys.SendWait("^{RIGHT}");
                        //findText(" ",true);
                        break;
                    case "last word":
                        richTextBox1.Focus();
                        SendKeys.SendWait("^{LEFT}");
                        //findText(" ", false, false, 0);
                        break;
                }
            }
        }





        public void findText(string ToFind)
        {
            findText(ToFind, true, false);

        }

        public void findText(string ToFind, bool findNext)
        {
            findText(ToFind, findNext, false);

        }

        public void findText(string ToFind, bool findNext, bool findAny)
        {
           findText(ToFind, findNext, findAny,0);
        }
        public void findText(string ToFind, bool findNext, bool findAny, int startOffset)
        {

            string aftercursor;
            int indexnum;
            string beforecursor;
            int beforeindexnum;

            if (findNext)
            {
                cursorpos = richTextBox1.SelectionStart;
                aftercursor = richTextBox1.Text.Substring(cursorpos);
                if (findAny) {
                    indexnum = aftercursor.IndexOfAny(ToFind.ToCharArray());
                } else
                {
                    indexnum = aftercursor.IndexOf(ToFind);
                }
                if (cursorpos + indexnum + 1 + startOffset>=0) {
                    richTextBox1.Select(cursorpos + indexnum + 1 + startOffset, 0);
                }
                //editor.cursorIndex = editor.cursorIndex + indexnum + 1;
                //editor.SelectNone();
            }
            else
            {

                cursorpos = richTextBox1.SelectionStart;
                beforecursor = richTextBox1.Text.Substring(0, cursorpos);
                if (findAny)
                {
                    beforeindexnum = beforecursor.LastIndexOfAny(ToFind.ToCharArray());
                }
                else
                {
                    beforeindexnum = beforecursor.LastIndexOf(ToFind);
                }
                if (beforeindexnum + startOffset>=0) {
                    richTextBox1.Select(beforeindexnum + startOffset, 0);
                }
            }
        }




        public void navigateDown()
        {

            cursorpos = richTextBox1.SelectionStart;
            cursorlinenum = richTextBox1.GetLineFromCharIndex(cursorpos);
            alllines = richTextBox1.Lines;
            if (cursorlinenum < alllines.Length)
            {
                curlinelength = alllines[cursorlinenum].Length;
                nextlinelength = alllines[cursorlinenum + 1].Length;
                startofcurline = richTextBox1.GetFirstCharIndexOfCurrentLine();
                startofnextline = richTextBox1.GetFirstCharIndexFromLine(cursorlinenum + 1);
                colpos = (cursorpos - startofcurline);

                richTextBox1.Select(startofnextline + ((nextlinelength < colpos) ? nextlinelength : colpos), 0);
            }
        }

        public void navigateNextLine()
        {

            cursorpos = richTextBox1.SelectionStart;
            cursorlinenum = richTextBox1.GetLineFromCharIndex(cursorpos);
            if (cursorlinenum < alllines.Length)
            {
                startofnextline = richTextBox1.GetFirstCharIndexFromLine(cursorlinenum + 1);
                richTextBox1.Select(startofnextline, 0);
            }
        }




        public void SelectDown()
        {

            cursorpos = richTextBox1.SelectionStart;
            cursorlinenum = richTextBox1.GetLineFromCharIndex(cursorpos);
            alllines = richTextBox1.Lines;
            if (cursorlinenum < alllines.Length)
            {
                curlinelength = alllines[cursorlinenum].Length;
                nextlinelength = alllines[cursorlinenum + 1].Length;
                startofcurline = richTextBox1.GetFirstCharIndexOfCurrentLine();
                startofnextline = richTextBox1.GetFirstCharIndexFromLine(cursorlinenum + 1);
                colpos = (cursorpos - startofcurline);

                int downPos = startofnextline + ((nextlinelength < colpos) ? nextlinelength : colpos);
                int selectionlen = downPos- cursorpos;

              //  if (richTextBox1.SelectionLength > 0)//if has selection alreadt then append selection
                //{
                    richTextBox1.Select(cursorpos, selectionlen);
                //}
            }
        }



        public void SelectUp()
        {

            cursorpos = richTextBox1.SelectionStart;
            cursorlinenum = richTextBox1.GetLineFromCharIndex(cursorpos);
            alllines = richTextBox1.Lines;
            if (cursorlinenum > 0)
            {
                curlinelength = alllines[cursorlinenum].Length;
                prevlinelength = alllines[cursorlinenum - 1].Length;
                startofcurline = richTextBox1.GetFirstCharIndexOfCurrentLine();
                startofprevline = richTextBox1.GetFirstCharIndexFromLine(cursorlinenum - 1);
                colpos = (cursorpos - startofcurline);

                int upPos = startofprevline + ((prevlinelength < colpos) ? prevlinelength : colpos);
                int selectionlen = cursorpos - upPos;

                richTextBox1.Select(upPos, selectionlen);
            }
        }

        public void navigateUp()
        {

            cursorpos = richTextBox1.SelectionStart;
            cursorlinenum = richTextBox1.GetLineFromCharIndex(cursorpos);
            alllines = richTextBox1.Lines;
            if (cursorlinenum > 0)
            {
                curlinelength = alllines[cursorlinenum].Length;
                prevlinelength = alllines[cursorlinenum - 1].Length;
                startofcurline = richTextBox1.GetFirstCharIndexOfCurrentLine();
                startofprevline = richTextBox1.GetFirstCharIndexFromLine(cursorlinenum - 1);
                colpos = (cursorpos - startofcurline);

                richTextBox1.Select(startofprevline + ((prevlinelength < colpos) ? prevlinelength : colpos), 0);
            }
        }



        public void navigateLastLine()
        {

            cursorpos = richTextBox1.SelectionStart;
            cursorlinenum = richTextBox1.GetLineFromCharIndex(cursorpos);
            if (cursorlinenum > 0)
            {
                startofprevline = richTextBox1.GetFirstCharIndexFromLine(cursorlinenum - 1);

                richTextBox1.Select(startofprevline, 0);
            }
        }



        public void Code_selection(GR_Data CurGRData)
        {

            //string aftercursor;
            //string aftercursor2;
            //int indexnum;
            //int indexnum2;
            //string beforecursor;
            //int beforeindexnum;
            int navline;

            for (int x = 0; x < CurGRData.Repeat; x++)
            {
                switch (CurGRData.Type)
                {


                    case "between next":
                        switch (CurGRData.Name)
                        {


                            case "perenthesis":
                                findText("(");
                                selectTo(")");
                                break;

                            case "double quote":
                                findText("\"");
                                selectTo("\"");
                                break;

                            case "single quote":
                                findText("'");
                                selectTo("'");
                                break;
                            case "brace":
                                findText("{");
                                selectTo("}");
                                break;

                            case "bracket":
                                findText("[");
                                selectTo("]");
                                break;
                        }

                        break;





                    case "between last":
                        switch (CurGRData.Name)
                        {


                            case "perenthesis":
                                findText(")",false);
                                selectTo("(", false,false,1, -1);
                                break;

                            case "double quote":
                                //findText("\"", false);
                                findText("\"", false);
                                selectTo("\"", false, false, 1, -1);
                                break;

                            case "single quote":
                                //findText("'", false);
                                findText("'", false);
                                selectTo("'", false, false, 1, -1);
                                break;
                            case "brace":
                                findText("}", false);
                                selectTo("{", false, false, 1, -1);
                                break;

                            case "bracket":
                                findText("]", false);
                                selectTo("[", false, false, 1,-1);
                                break;
                        }

                        break;


                    case "encase":
                        switch (CurGRData.Name)
                        {


                            case "perenthesis":
                                richTextBox1.SelectedText = "(" + richTextBox1.SelectedText + ")";
                                break;

                            case "double quote":
                                richTextBox1.SelectedText = "\"" + richTextBox1.SelectedText + "\"";
                                break;

                            case "single quote":
                                richTextBox1.SelectedText = "'" + richTextBox1.SelectedText + "'";
                                break;
                            case "brace":
                                richTextBox1.SelectedText = "{" + richTextBox1.SelectedText + "}";
                                break;

                            case "bracket":
                                richTextBox1.SelectedText = "[" + richTextBox1.SelectedText + "]";
                                break;
                        }

                        break;


                    case "line number":
                        navline = Int32.Parse(CurGRData.Name.Replace(" ", ""));
                        if (navline > 0) {
                            alllines = richTextBox1.Lines;
                            cursorpos = richTextBox1.GetFirstCharIndexFromLine(navline - 1);
                            richTextBox1.Select(cursorpos, alllines[navline - 1].Length);
                        }
                        break;

                    case "line numbers":

                        alllines = richTextBox1.Lines;
                        string[] startendlines = CurGRData.Name.Split("-".ToCharArray());
                        navline = Int32.Parse(startendlines[0].Replace(" ", ""));
                        if (navline>0) { 
                        cursorpos = richTextBox1.GetFirstCharIndexFromLine(navline - 1);

                        navline = Int32.Parse(startendlines[1].Replace(" ", ""));
                        if (navline < alllines.Length)
                        {
                            startofnextline = richTextBox1.GetFirstCharIndexFromLine(navline - 1);

                            richTextBox1.Select(cursorpos, (startofnextline - cursorpos) + alllines[navline - 1].Length);

                        }
                }
                        break;

                            case "editor":
                        richTextBox1.Focus();
                                break;

                    case "home":
                        cursorpos = richTextBox1.SelectionStart;
                        startofcurline = richTextBox1.GetFirstCharIndexOfCurrentLine();
                        richTextBox1.Select(startofcurline, cursorpos - startofcurline);
                        break;
                    case "end":

                        alllines = richTextBox1.Lines;
                        cursorpos = richTextBox1.SelectionStart;
                        cursorlinenum = richTextBox1.GetLineFromCharIndex(cursorpos);
                        startofcurline = richTextBox1.GetFirstCharIndexOfCurrentLine();
                        curlinelength = alllines[cursorlinenum].Length;
                        richTextBox1.Select(cursorpos, curlinelength - (cursorpos - startofcurline));
                        break;

                    case "up":
                        SelectUp();
                        break;
                    case "down":
                        SelectDown();
                        break;
                    case "next line":
                        cursorpos = richTextBox1.SelectionStart;
                        cursorlinenum = richTextBox1.GetLineFromCharIndex(cursorpos);
                        alllines = richTextBox1.Lines;
                        if (alllines.Length> cursorlinenum) {
                        selectLineNumber(cursorlinenum + 1);
                        }
                        break;
                    case "last line":
                        cursorpos = richTextBox1.SelectionStart;
                        cursorlinenum = richTextBox1.GetLineFromCharIndex(cursorpos);
                        if (cursorlinenum>0)
                        {
                            selectLineNumber(cursorlinenum - 1);
                        }
                        break;
                    case "left":
                        cursorpos = richTextBox1.SelectionStart;
                        if (cursorpos>0) {
                            richTextBox1.Select(cursorpos - 1, richTextBox1.SelectionLength + 1);
                        }
                        break;
                    case "right":
                        richTextBox1.SelectionLength += 1;
                        break;

                    case "next word":

                        richTextBox1.Focus();
                        SendKeys.SendWait("^{RIGHT}");
                        SendKeys.SendWait("!^{RIGHT}");
                       // findText(" ", true);
                        //selectTo(" ", true);
                        break;
                    case "last word":
                        richTextBox1.Focus();
                        SendKeys.SendWait("^{LEFT}");
                        SendKeys.SendWait("!^{LEFT}");
                        //findText(" ", false, false, 0);
                        //selectTo(" ", false, false, 0);
                        break;


                    case "word":
                        richTextBox1.Focus();
                        SendKeys.SendWait("^{LEFT}");
                        SendKeys.SendWait("!^{RIGHT}");
                        //findText(" ", false, false, 0);
                        //selectTo(" ", false, false, 0);
                        break;
                    case "line":
                        cursorpos = richTextBox1.SelectionStart;
                        cursorlinenum = richTextBox1.GetLineFromCharIndex(cursorpos);
                        selectLineNumber(cursorlinenum);
                        break;
                            case "all":
                        richTextBox1.SelectAll();
                                break;
                    //        case "paragraph":
                    //            editor.SelectCurrentParagraph();
                    //            break;
                    //        case "next paragraph":
                    //            editor.SelectParagraphForward();
                    //            break;
                    //        case "last paragraph":
                    //            editor.SelectParagraphBackward();
                    //            break;
                            case "none":
                        cursorpos = richTextBox1.SelectionStart;
                        richTextBox1.Select(cursorpos, 0);
                                break;




                    case "next comma":

                        selectTo(",");
                        break;

                                case "last comma":
                        selectTo(",",false);
                                break;



                            case "next bracket":
                         selectTo("[]", true,true);
                        break;

                    case "last bracket":
                        selectTo("[]", false, true);
                        break;



                    case "next brace":
                        selectTo("{}", true, true);
                        break;

                    case "last brace":
                        selectTo("{}", false, true);
                        break;



                    case "next single quote":
                        selectTo("'");
                        break;

                    case "last single quote":
                        selectTo("'",false);
                        break;

                    case "next double quote":
                        selectTo("\"");
                        break;

                    case "last double quote":
                        selectTo("\"", false);
                        break;

                    case "next perenthises":
                        selectTo("()", true, true);
                        break;

                    case "last perenthises":
                        selectTo("()", false, true);
                        break;

                    case "next function":
                        selectTo("function");
                        break;

                    case "last function":
                        selectTo("function", false);
                        break;


                    case "capitalize":

                richTextBox1.SelectedText = FirstCharToUpper(richTextBox1.SelectedText);

                                break;

                            case "to upper":
                richTextBox1.SelectedText = richTextBox1.SelectedText.ToUpper();
                                break;

                            case "to lower":
                richTextBox1.SelectedText = richTextBox1.SelectedText.ToLower();

                                break;


                            case "to camel":
                        richTextBox1.SelectedText = ToCamelCase(richTextBox1.SelectedText);
                        break;



                            case "to dash":
                richTextBox1.SelectedText = ToDashCase(richTextBox1.SelectedText);
                                break;



                            case "to dot":
                richTextBox1.SelectedText = ToDotCase(richTextBox1.SelectedText);
                       break;



                    case "to score":
                        richTextBox1.SelectedText= ToScoreCase(richTextBox1.SelectedText);
                        break;

            //        case "undo":
            //            editorUndo();
            //            break;

            //        case "redo":
            //            editorRedo();
            //            break;
                    case "copy":
                        richTextBox1.Copy();
                        break;
                    case "cut":
                        richTextBox1.Cut();
                        break;
                    case "paste":
                        richTextBox1.Paste();
                        break;

                    case "backspace":
                        cursorpos = richTextBox1.SelectionStart;
                        if (cursorpos>0)
                        {
                            richTextBox1.Text = richTextBox1.Text.Remove(cursorpos - 1, 1);
                            richTextBox1.Select(cursorpos-1,0);
                        }
                        break;
                    case "delete":
                        richTextBox1.SelectedText = "";
                          break;
            
                        case "delete word":// kinda buggy
                        findText(" ", false, false, 0);
                        selectTo(" ", false, false, 0);
                        richTextBox1.SelectedText = "";
                        break;

                    case "delete line":

                        cursorpos = richTextBox1.SelectionStart;
                        cursorlinenum = richTextBox1.GetLineFromCharIndex(cursorpos);
                        selectLineNumber(cursorlinenum);
                        richTextBox1.SelectedText= "";
                        break;
                }
            }

        }



        public void selectTo(string ToFind)
        {
            selectTo(ToFind, true, false);
        }

        public void selectTo(string ToFind, bool FindNext)
        {
            selectTo(ToFind, FindNext, false);
        }

        public void selectTo(string ToFind, bool FindNext, bool Any)
        {
            selectTo(ToFind, FindNext, false, 0, 0);
        }
        public void selectTo(string ToFind, bool FindNext, bool Any, int startOffset)
        {
            selectTo(ToFind, FindNext, false, startOffset, 0);
        }

        public void selectTo(string ToFind, bool FindNext, bool Any,int startOffset, int endOffset)
        {


            string aftercursor;
           // string aftercursor2;
            int indexnum;
           // int indexnum2;
            string beforecursor;
            int beforeindexnum;

            cursorpos = richTextBox1.SelectionStart;
            cursorlinenum = richTextBox1.GetLineFromCharIndex(cursorpos);
            if (FindNext)
            {
                aftercursor = richTextBox1.Text.Substring(cursorpos);
                if (Any)
                {
                    indexnum = aftercursor.IndexOfAny(ToFind.ToCharArray());
                }
                else
                {
                    indexnum = aftercursor.IndexOf(ToFind);
                }
                
                richTextBox1.Select(cursorpos, indexnum);
            }
            else
            {
                            beforecursor = richTextBox1.Text.Substring(0, cursorpos);
                if (Any)
                {
                    beforeindexnum = beforecursor.LastIndexOfAny(ToFind.ToCharArray());
                }else
                {
                    beforeindexnum = beforecursor.LastIndexOf(ToFind);
                }
                if (beforeindexnum + startOffset >= 0)
                {
                    richTextBox1.Select(beforeindexnum + startOffset, (cursorpos - beforeindexnum) + endOffset);
                }
            }
        }



        public void selectLineNumber(int LineNum)
        {

            alllines = richTextBox1.Lines;
            int linelength = alllines[LineNum].Length;
            int startofline = richTextBox1.GetFirstCharIndexFromLine(LineNum);
            richTextBox1.Select(startofline, linelength);

        }




        public void webviewControl(GR_Data CurGRData)
        {
            switch (CurGRData.Type)
            {
                case "activate":
                    WebViewTab.Show();
                    break;

                case "maximize":
                    WebViewTab.WindowState = FormWindowState.Maximized;
                    break;
                case "minimize":
                    WebViewTab.WindowState = FormWindowState.Minimized;
                    break;
                case "refresh":
                    WebViewTab.webRefresh();
                    break;
                case "close":
                    WebViewTab.Close();
                    break;

                case "search":
                    WebViewTab.NavigateWebView(CurGRData.Name);
                    break;

                case "search dictate":
                    WebViewTab.NavigateWebView(CurGRData.Name);
                    break;
            }

        }



        public string ToTitleCase(string str)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }

        public static string FirstCharToUpper(string input)
        {
            if (input != null)
                return input.First().ToString().ToUpper() + input.Substring(1);
            return input;
        }
        public static string FirstCharToLower(string input)
        {
            if (input != null)
                return input.First().ToString().ToLower() + input.Substring(1);
            return input;
        }

        public string ToCamelCase(string str)
        {
            return FirstCharToLower(ToTitleCase(str)).Replace(" ", "");
        }

        public string ToScoreCase(string str)
        {
            return ToTitleCase(str).Replace(" ", "_");
        }

        public string ToDashCase(string str)
        {
            return ToTitleCase(str).Replace(" ", "-");
        }

        public string ToDotCase(string str)
        {
            return ToTitleCase(str).Replace(" ", ".");
        }



        public void EngineControl(GR_Data CurGRData)
        {
            switch (CurGRData.Type)
            {
                case "compile":
                    CompileCode();
                    break;
                case "run":
                    CompileCode(true);
                    break;
                case "save":
                    saveToolStripMenuItem.PerformClick();
                    break;
                case "load":
                    openToolStripMenuItem.PerformClick();
                    break;

                    case "activate":
                    this.Show();
                    richTextBox1.Focus();
                    break;

                case "maximize":
                    this.WindowState = FormWindowState.Maximized;
                    break;
                case "minimize":
                    this.WindowState = FormWindowState.Minimized;
                    break;
                case "close":
                    this.Close();
                    break;
            }

        }


        public void ProcessGRData(GR_Data CurGRData)
        {


            Console.WriteLine("Action:" + CurGRData.Action);
            Console.WriteLine("Type:" + CurGRData.Type);
            Console.WriteLine("Name:" + CurGRData.Name);
            Console.WriteLine("Repeat:" + CurGRData.Repeat);


            if (CurGRData.Action != "select" && CurGRData.Type != "undo" && CurGRData.Type != "redo")
            {
               // Code_selection_setundoData();
            }
            switch (CurGRData.Action)
            {

                case "engine":
                    EngineControl(CurGRData);
                    break;
                case "webview":
                    webviewControl(CurGRData);
                    break;

                case "native":
                    Code_native(CurGRData);
                    break;

                case "dictate":
                    Code_dictation(CurGRData);
                    break;
                case "call":
                    switch (VoiceLang)
                    {
                        case CodeLang.PHP:
                            Code_call_PHP(CurGRData);
                            break;
                        case CodeLang.CS:
                            Code_call_CS(CurGRData);
                            break;
                    }
                    break;
                case "callCS":
                    Code_call_CS(CurGRData);
                    break;
                case "navigate":
                    Code_navigation(CurGRData);
                    break;
                case "select":
                    Code_selection(CurGRData);
                    break;

                case "defineShort":

                    Code_define__(CurGRData.Name);
                    break;

                case "define":
                    switch (VoiceLang)
                    {
                        case CodeLang.PHP:
                            Code_Define_PHP(CurGRData);
                            break;
                        case CodeLang.CS:
                            Code_Define_CS(CurGRData);
                            break;
                    }

                    break;

                case "defineCS":
                    Code_Define_CS(CurGRData);
                    break;

                case "defineXML":
                    Code_Define_XML(CurGRData);
                    break;

                case "change setting":

                    switch (CurGRData.Type)
                    {
                        case "language":

                            switch (CurGRData.Name)
                            {
                                case "PHP":
                                    VoiceLang = CodeLang.PHP;
                                    break;
                                case "CS":
                                    VoiceLang = CodeLang.CS;
                                    break;
                                case "JavaScript":
                                    VoiceLang = CodeLang.JavaScript;
                                    break;
                                case "HTML":
                                    VoiceLang = CodeLang.HTML;
                                    break;
                                case "Text":
                                    VoiceLang = CodeLang.Text;
                                    break;
                            }

                            break;
                    }

                    break;
            }




        }

        private void webViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WebViewTab.Show();
        }

        private void compileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CompileCode();
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CompileCode(true);
        }

        private void CompileCode()
        {
        CompileCode(false);
        }

        private void CompileCode(bool doRun)
        {


            //
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            // ICodeCompiler icc = codeProvider.CreateCompiler();
            string Output = "Out.exe";
            string CompileErrors = "";
           // System.Windows.Forms.ToolStripMenuItem ButtonObject = (System.Windows.Forms.ToolStripMenuItem)sender;
         

            // textBox2.Text = "";
            System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters();
            //Make sure we generate an EXE, not a DLL
            parameters.GenerateExecutable = true;
            parameters.OutputAssembly = Output;
            //To compile multiple files into a single exe jusr pass CompileAssemblyFromFile a string [] of the files to compile
            //CompilerResults results = codeProvider.CompileAssemblyFromFile(parameters, filesInProject);
            CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, richTextBox1.Text);

            if (results.Errors.Count > 0)
            {
                // textBox2.ForeColor = Color.Red;
                foreach (CompilerError CompErr in results.Errors)
                {
                    CompileErrors = CompileErrors +
                                "Line number " + CompErr.Line +
                                ", Error Number: " + CompErr.ErrorNumber +
                                ", '" + CompErr.ErrorText + ";" +
                                Environment.NewLine + Environment.NewLine;
                }
            }
            else
            {
                //Successful Compile
                //textBox2.ForeColor = Color.Blue;
                CompileErrors = "Success!";
                //If we clicked run then launch our EXE
                if (doRun) Process.Start(Output);
            }

        }
    }
}
