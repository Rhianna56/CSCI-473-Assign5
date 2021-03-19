using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChappellEberleAstorga_Assign5
{
    public partial class Form1 : Form
    {
        public static int count;
        private TextBox[] textBoxArray;
        private char[][] currentPuzzle = new char[9][];
        private char[][] finishedPuzzle = new char[9][];
        private char[][] originalPuzzle = new char[9][];
        private int lastClickedBox = 0;
        private List<Tuple<int, int>> indexOfAllZeros = new List<Tuple<int, int>>();
        private TimeSpan t;
        private string randomizedPuzzleSelection = "";
        private string selectedDifficulty;
        private string originalFilePath;
        private bool cheated = false;

        public Form1()
        {
            InitializeComponent();

            Initialize();
        }

        private void Initialize()
        {
            textBoxArray = new TextBox[81] { textBox1, textBox2, textBox3, textBox4, textBox5, textBox6, textBox7, textBox8, textBox9, textBox10,
                                             textBox11, textBox12, textBox13, textBox14, textBox15, textBox16, textBox17, textBox18, textBox19, textBox20,
                                             textBox21, textBox22, textBox23, textBox24, textBox25, textBox26, textBox27, textBox28, textBox29, textBox30,
                                             textBox31, textBox32, textBox33, textBox34, textBox35, textBox36, textBox37, textBox38, textBox39, textBox40,
                                             textBox41, textBox42, textBox43, textBox44, textBox45, textBox46, textBox47, textBox48, textBox49, textBox50,
                                             textBox51, textBox52, textBox53, textBox54, textBox55, textBox56, textBox57, textBox58, textBox59, textBox60,
                                             textBox61, textBox62, textBox63, textBox64, textBox65, textBox66, textBox67, textBox68, textBox69, textBox70,
                                             textBox71, textBox72, textBox73, textBox74, textBox75, textBox76, textBox77, textBox78, textBox79, textBox80, textBox81 };
            for (int i = 0; i < 81; i++)
            {
                textBoxArray[i].Enter += new System.EventHandler(TextBox_Focus);
                textBoxArray[i].Enabled = false;
            }

        }

        private void Easy_Click(object sender, EventArgs e)
        {
            bool result = false;
            if (originalFilePath != null)
                result = (MessageBox.Show("Would you like to save your progress?", "save?", MessageBoxButtons.YesNo) == DialogResult.Yes);

            if (result)
                SaveProgress();

            //populate the sudoku grid with an easy puzzle
            selectedDifficulty = "easy";
            getPuzzleAtDifficulty(selectedDifficulty);
            PopulateGrid();

            //start the timer
            timer1.Start();
            EnableButtons();
            EnableButtons();
        }

        private void Medium_Click(object sender, EventArgs e)
        {
            bool result = false;
            if (originalFilePath != null)
                result = (MessageBox.Show("Would you like to save your progress?", "save?", MessageBoxButtons.YesNo) == DialogResult.Yes);

            if (result)
                SaveProgress();

            //populate the sudoku grid with a medium puzzle
            selectedDifficulty = "medium";
            getPuzzleAtDifficulty(selectedDifficulty);
            PopulateGrid();

            //start the timer
            timer1.Start();
            EnableButtons();
        }

        private void Hard_Click(object sender, EventArgs e)
        {
            bool result = false;
            if (originalFilePath != null)
                result = (MessageBox.Show("Would you like to save your progress?", "save?", MessageBoxButtons.YesNo) == DialogResult.Yes);

            if (result)
                SaveProgress();

            //populate the sudoku grid with a hard puzzle
            selectedDifficulty = "hard";
            getPuzzleAtDifficulty(selectedDifficulty);
            PopulateGrid();

            //start the timer
            timer1.Start();
            EnableButtons();
        }

        private void TextBox_Focus(object sender, EventArgs e)
        {
            textBoxArray[lastClickedBox].BackColor = Color.White;
            lastClickedBox = Array.IndexOf(textBoxArray, (TextBox)sender);
            textBoxArray[lastClickedBox].BackColor = Color.LightYellow;
            focusLabel.Focus();
        }

        private void Help_Click(object sender, EventArgs e)
        {
            if (!indexOfAllZeros.Any())
            {
                checkCompletion();
                return;
            }

            int randomIndexIndexer = Program.rand.Next(0, indexOfAllZeros.Count() - 1);

            //Set a random value in currentPuzzle ito the corresponding value in finishedPuzzle
            //then update the corresponding textbox.
            //(Leaving it like this as a joke for a friend)
            currentPuzzle[indexOfAllZeros[randomIndexIndexer].Item1][indexOfAllZeros[randomIndexIndexer].Item2] =
                finishedPuzzle[indexOfAllZeros[randomIndexIndexer].Item1][indexOfAllZeros[randomIndexIndexer].Item2];
            textBoxArray[(indexOfAllZeros[randomIndexIndexer].Item1 * 9) + indexOfAllZeros[randomIndexIndexer].Item2].Text =
                currentPuzzle[indexOfAllZeros[randomIndexIndexer].Item1][indexOfAllZeros[randomIndexIndexer].Item2].ToString();

            indexOfAllZeros.RemoveAt(randomIndexIndexer);
            cheated = true;
            if (!indexOfAllZeros.Any())
                checkCompletion();
        }

        private void CheckAnswers_Click(object sender, EventArgs e)
        {
            List<Tuple<int, int, char>> rowComp = new List<Tuple<int, int, char>>();
            List<Tuple<int, int, char>> colComp = new List<Tuple<int, int, char>>();
            List<Tuple<int, int, char>> boxComp = new List<Tuple<int, int, char>>();

            for (int i = 0; i < 81; i++)
                textBoxArray[i].BackColor = Color.White;

            textBoxArray[lastClickedBox].BackColor = Color.LightYellow;

            //check the textboxes that had zeros in them against the numbers in the solution
            int numWrong = 0;
            bool cellflag = false;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if ((currentPuzzle[i][j] != finishedPuzzle[i][j]) && (currentPuzzle[i][j] != '0'))
                    {
                        numWrong++;
                    }

                    if (rowComp.Where(x => x.Item3 == currentPuzzle[i][j]).Any())
                    {
                        Tuple<int, int, char> hold = rowComp.Where(x => x.Item3 == currentPuzzle[i][j]).First();
                        textBoxArray[(hold.Item1 * 9) + hold.Item2].BackColor = Color.Red;
                        textBoxArray[(i * 9) + j].BackColor = Color.Red;
                        cellflag = true;
                    }

                    if (colComp.Where(x => x.Item3 == currentPuzzle[j][i]).Any())
                    {
                        Tuple<int, int, char> hold = colComp.Where(x => x.Item3 == currentPuzzle[j][i]).First();
                        textBoxArray[(hold.Item1 * 9) + hold.Item2].BackColor = Color.Red;
                        textBoxArray[(j * 9) + i].BackColor = Color.Red;
                        cellflag = true;
                    }

                    if (currentPuzzle[i][j] != '0')
                        rowComp.Add(Tuple.Create(i, j, currentPuzzle[i][j]));

                    if (currentPuzzle[j][i] != '0')
                        colComp.Add(Tuple.Create(j, i, currentPuzzle[j][i]));
                }
                rowComp.Clear();
                colComp.Clear();
            }
            int iterCount = 0;
            for (int offset = 0; offset < 3; offset++)
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (boxComp.Where(x => x.Item3 == currentPuzzle[i][j + (offset*3)]).Any())
                        {
                            Tuple<int, int, char> hold = boxComp.Where(x => x.Item3 == currentPuzzle[i][j + (offset*3)]).First();
                            textBoxArray[(hold.Item1 * 9) + hold.Item2].BackColor = Color.Red;
                            textBoxArray[(i * 9) + (j + (offset*3))].BackColor = Color.Red;
                            cellflag = true;
                        }

                        if (currentPuzzle[i][j + (offset*3)] != '0')
                            boxComp.Add(Tuple.Create(i, j + (offset * 3), currentPuzzle[i][j + (offset*3)]));

                        iterCount++;
                    }

                    if (iterCount == 9)
                    {
                        boxComp.Clear();
                        iterCount = 0;
                    }
                }
            }



            if (numWrong != 0)
                richTextBox1.Text = "There are mistakes in " + numWrong + " boxes.";

            if (cellflag)
                richTextBox1.Text = "Errors are shown in red";

            if (numWrong == 0 && !(cellflag))
                richTextBox1.Text = "Everything looks good!";
        }

        private void Pause_Click(object sender, EventArgs e)
        {
            if (Pause.Text == "Pause")
            {
                Pause.Text = "Resume";
                GridCover.Visible = true;
                DisableButtons();
                Pause.Enabled = true;
                timer1.Stop();
            }

            else if (Pause.Text == "Resume")
            {
                Pause.Text = "Pause";
                GridCover.Visible = false;
                EnableButtons();
                timer1.Start();
            }
        }

        private void Reset_Click(object sender, EventArgs e)
        {
            indexOfAllZeros.Clear();

            //if the current puzzle has anything other than its original number, then set it back to the original number
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (currentPuzzle[i][j] != originalPuzzle[i][j])
                        currentPuzzle[i][j] = originalPuzzle[i][j];


                    if (currentPuzzle[i][j] == '0')
                        indexOfAllZeros.Add(Tuple.Create(i, j));
                }
            }
            cheated = false;
            PopulateGrid();
            t = TimeSpan.FromSeconds(0);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            t = t.Add(TimeSpan.FromSeconds(1));
            timerLabel.Text = "Time elapsed " + string.Format("{0:D2}h:{1:D2}m:{2:D2}s", t.Hours, t.Minutes, t.Seconds);
        }

        public void getPuzzleAtDifficulty(string difficulty)
        {
            bool result = false;
            double startTime = 0;

            if (!(currentPuzzle[0] == null))
            {
                for (int i = 0; i < 9; i++)
                {
                    Array.Clear(currentPuzzle[i], 0, 9);
                    Array.Clear(finishedPuzzle[i], 0, 9);
                }
            }

            if (indexOfAllZeros.Any())
                indexOfAllZeros.Clear();

            difficulty.ToLower();
            List<string> selectedDifficultyArray = Program.directory.FindAll(x => x.Contains(difficulty));
            string file = Directory.GetFiles("../../puzzles/" + selectedDifficulty + "/", "*.sav").FirstOrDefault();
   
            if (file != null)
                result = (MessageBox.Show("A save file at your selected difficulty was found! Do you want to Continue?", "Continue?", MessageBoxButtons.YesNo) == DialogResult.Yes);

            if (result)
                randomizedPuzzleSelection = file;
            else
                randomizedPuzzleSelection = selectedDifficultyArray[Program.rand.Next(0, selectedDifficultyArray.Count - 1)];

            originalFilePath = randomizedPuzzleSelection;
            
            try
            {
                using (StreamReader inFile = new StreamReader(randomizedPuzzleSelection))
                {
                    string holder;
                    for (int i = 0; i < 9; i++)
                    {
                        //saves the original state of the current puzzle
                        holder = inFile.ReadLine();
                        originalPuzzle[i] = holder.ToCharArray();
                        currentPuzzle[i] = holder.ToCharArray();
                    }

                    if (result)
                        startTime = double.Parse(inFile.ReadLine());
                    else
                        inFile.ReadLine();

                    for (int i = 0; i < 9; i++)
                        finishedPuzzle[i] = inFile.ReadLine().ToCharArray();

                    if (result)
                        for (int i = 0; i < 9; i++)
                            currentPuzzle[i] = inFile.ReadLine().ToCharArray();

                    if (!inFile.EndOfStream)
                        cheated = (inFile.ReadLine() == "True");

                }
            }
            catch (System.IO.FileNotFoundException)
            {
                MessageBox.Show("There dosn't seem to be a puzzle at the directory refrenced by \"directory.txt\"\nMissing file at\n" + randomizedPuzzleSelection,
                                "Puzzle File not found",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            t = TimeSpan.FromSeconds(startTime);
        }


        private void PopulateGrid()
        {
            for (int offset = 0; offset < 9; offset++)
            {
                for (int index = 0; index < 9; index++)
                {
                    if (currentPuzzle[offset][index] != '0')
                    {
                        textBoxArray[(offset * 9) + index].Text = currentPuzzle[offset][index].ToString();
                        textBoxArray[(offset * 9) + index].Enabled = (currentPuzzle[offset][index] != originalPuzzle[offset][index]);
                    }
                    else
                    {
                        indexOfAllZeros.Add(Tuple.Create(offset, index));
                        textBoxArray[(offset * 9) + index].Text = "";
                        textBoxArray[(offset * 9) + index].Enabled = true;
                        lastClickedBox = (offset * 9) + index;
                    }
                }
            }

            for (int i = 0; i < 81; i++)
                textBoxArray[i].BackColor = Color.White;

            textBoxArray[lastClickedBox].BackColor = Color.LightYellow;
        }


        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (originalFilePath == null)
                return;

            int offset = lastClickedBox % 9;
            int index = (int)Math.Floor(((double)lastClickedBox) / 9.0);
            string holder = "";

            if (e.KeyCode.ToString().StartsWith("D"))
                holder = e.KeyCode.ToString().Substring(1);
            else if (e.KeyCode.ToString().StartsWith("NumPad"))
                holder = e.KeyCode.ToString().Substring(6);


            if (int.TryParse(holder, out int numberPress))
            {
                if (textBoxArray[lastClickedBox].Text == "" && holder != "0")
                {
                    textBoxArray[lastClickedBox].Text = holder;
                    currentPuzzle[index][offset] = ((holder.Any()) ? holder[0] : '0');
                    indexOfAllZeros.Remove(Tuple.Create(index, offset));
                }
            }
            else if (e.KeyCode == Keys.Back)
            {
                textBoxArray[lastClickedBox].Text = "";
                currentPuzzle[index][offset] = '0';
                indexOfAllZeros.Add(Tuple.Create(index, offset));
            }

            if (!indexOfAllZeros.Any())
                checkCompletion();

            richTextBox1.Text = "";
        }

        private void checkCompletion()
        {
            bool iscomplete = true;
            for (int i = 0; (i < 9) && iscomplete; i++)
                for (int j = 0; (j < 9) && iscomplete; j++)
                    iscomplete = currentPuzzle[i][j] == finishedPuzzle[i][j];

            if (!iscomplete)
            {
                richTextBox1.Text = "You've yet to complete the puzzle correctly";
            }
            else
            {
                timer1.Stop();
                if (originalFilePath.Contains(".sav"))
                    File.Delete(originalFilePath);

                if (!cheated)
                    using (StreamWriter sw = File.AppendText("../../puzzles/" + selectedDifficulty + "/times.txt"))
                        sw.WriteLine(t.TotalSeconds);

                string yourTime = "Your time\n";
                yourTime += string.Format("{0:D2}h:{1:D2}m:{2:D2}s", t.Hours, t.Minutes, t.Seconds) + "\n";
                string fastestComp = "Fastest Completion\n";
                string avgComp = "Average Completion\n";
                List<double> times = new List<double>();
                if (File.Exists("../../puzzles/" + selectedDifficulty + "/times.txt"))
                {
                    using (StreamReader inFile = new StreamReader("../../puzzles/" + selectedDifficulty + "/times.txt"))
                    {
                        string holder = "";
                        while (!inFile.EndOfStream)
                        {
                            holder = inFile.ReadLine();
                            if (holder != "")
                            {
                                times.Add(double.Parse(holder));
                            }
                        }
                    }
                    t = TimeSpan.FromSeconds(times.Min());
                    fastestComp += string.Format("{0:D2}h:{1:D2}m:{2:D2}s", t.Hours, t.Minutes, t.Seconds) + "\n";
                    t = TimeSpan.FromSeconds(times.Average());
                    avgComp += string.Format("{0:D2}h:{1:D2}m:{2:D2}s", t.Hours, t.Minutes, t.Seconds) + "\n";
                }
                else
                {
                    fastestComp += "-\n";
                    avgComp += "-\n";
                }
                

                originalFilePath = null;
                DisableButtons();
                for (int i = 0; i < 81; i++)
                    textBoxArray[i].Enabled = false;

                MessageBox.Show(yourTime + fastestComp + avgComp, "Times for all " + selectedDifficulty + " Puzzles");
            }
        }

        

        private void SaveProgress()
        {
            string pathHolder = originalFilePath.Replace(".txt",".sav");
            string fullSaveString = "";
            for (int i = 0; i < 9; i++)
                fullSaveString += new string(originalPuzzle[i]) + "\n";

            fullSaveString += t.TotalSeconds.ToString() + "\n";

            for (int i = 0; i < 9; i++)
                fullSaveString += new string(finishedPuzzle[i]) + "\n";

            for (int i = 0; i < 9; i++)
                fullSaveString += new string(currentPuzzle[i]) + "\n";

            fullSaveString += cheated;
            File.WriteAllText(pathHolder, fullSaveString);
        }

        private void EnableButtons()
        {
            Help.Enabled = true;
            CheckAnswers.Enabled = true;
            Pause.Enabled = true;
            Reset.Enabled = true;
        }

        private void DisableButtons()
        {
            Help.Enabled = false;
            CheckAnswers.Enabled = false;
            Pause.Enabled = false;
            Reset.Enabled = false;
        }
    }
}
