using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChappellEberleAstorga_Assign5
{
    static class Program
    {
        //remove this after testing contents read
        public static List<string> directory = new List<string>();
        public static Random rand = new Random();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                using (StreamReader inFile = new StreamReader("..\\..\\directory.txt")) //throws System.IO.FileNotFoundException
                {
                    while (!inFile.EndOfStream)
                    {
                        directory.Add("../../puzzles/" + inFile.ReadLine());
                        
                    }
                }
            }
            catch (System.IO.FileNotFoundException) //The file is not in the correct place or is missing
            {
                MessageBox.Show("Please ensure a Directory.txt file exsists in the 'puzzles' directory",
                                "Directory file not found!",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                Environment.Exit(1);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
