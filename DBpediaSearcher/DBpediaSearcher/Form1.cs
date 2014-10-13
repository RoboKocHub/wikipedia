using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Infrastructure;

namespace DBpediaSearcher
{
    public partial class Form1 : Form
    {   

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string FilePath = @"C:\D\Skola\STUBA\2roc\VI\Project\\DBpediaSearcher\Files\";

            FileOperation fo = new Infrastructure.FileOperation();
          
             richTextBox1.Text = fo.ParseToXml(@"(?<http>\w+):\/\/(?<dbpedia>[\w@][\w.:@]+)\/?[\w\.?=%&=\-@/$,]*", FilePath + fileName.Text);
           
            
            
         
        }
    }
}
