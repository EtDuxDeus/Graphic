using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace graphic
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}
		private void Form_Load(object sender, EventArgs e)
		{

			
		}

		private void button1_Click(object sender, EventArgs e)
		{
			chart1.Series[0].Points.Clear();
			string Expression = textBox1.Text;
			for(double i = -50;i < 50; i++)
			{
				chart1.Series[0].Points.AddXY(i, CalcTrans.Program.Calculate(Expression, i));
			}
		}
	}
}
