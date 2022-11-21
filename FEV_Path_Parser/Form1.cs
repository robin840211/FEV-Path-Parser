using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FEV_Path_Parser
{
	public class Form1 : Form
	{
		private class FevInfo
		{
			public int offset;

			public string path;

			public int fsbID;

			public int wavID;

			public int duration;
		}

		private IContainer components;

		private Button button1;

		private TextBox textBox1;

		private Label label1;

		private TextBox textBox2;
        private Label label3;
        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private Label label2;

		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
            try
			{
				Process();
			}
            catch (Exception ex)
            {
				MessageBox.Show(string.Format("Error:\n{0}", ex.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
			
		}

		private void Process()
		{
			string text = textBox1.Text;
			_ = textBox2.Text;
			foreach (string item in from o in Directory.GetFiles(text)
				where o.EndsWith(".fev")
				select o)
			{
				List<FevInfo> list = new List<FevInfo>();
				string[] array4;
				using (FileStream fileStream = File.Open(item, FileMode.Open))
				{
					using (MemoryStream memoryStream = new MemoryStream())
					{
						byte[] array = new byte[fileStream.Length];
						memoryStream.SetLength(fileStream.Length);
						fileStream.Read(array, 0, (int)fileStream.Length);
						memoryStream.Write(array, 0, (int)fileStream.Length);
						using (BinaryReader binaryReader = new BinaryReader(memoryStream))
						{
							int i = 0;
							byte[] array2 = new byte[4];
							int num = (int)fileStream.Length - 5;
							List<string> list2 = new List<string>();
							for (; i < num; i++)
							{
								array2[0] = array[i];
								array2[1] = array[i + 1];
								array2[2] = array[i + 2];
								array2[3] = array[i + 3];
								if (Encoding.ASCII.GetString(array2) == "LGCY")
								{
									break;
								}
							}
							i += 4;
							binaryReader.BaseStream.Seek(i, SeekOrigin.Begin);
							int num2 = binaryReader.ReadInt32();
							num = i + num2;
							array2 = new byte[2];
							byte[] array3 = new byte[6];
							string a = "";
							for (; i < num; i++)
							{
								if (!(a != "master"))
								{
									break;
								}
								array2[0] = array[i];
								array2[1] = array[i + 1];
								array3[0] = array[i];
								array3[1] = array[i + 1];
								array3[2] = array[i + 2];
								array3[3] = array[i + 3];
								array3[4] = array[i + 4];
								array3[5] = array[i + 5];
								Encoding.ASCII.GetString(array2);
								a = Encoding.ASCII.GetString(array3);
								if (IsASCII(array2[0]) && IsASCII(array2[1]))
								{
									i -= 8;
									binaryReader.BaseStream.Seek(i, SeekOrigin.Begin);
									if (binaryReader.ReadInt32() == 0)
									{
										int count = binaryReader.ReadInt32();
										string text2 = new string(binaryReader.ReadChars(count));
										text2 = text2.TrimEnd(default(char));
										list2.Add(text2);
										i = (int)binaryReader.BaseStream.Position;
									}
									else
									{
										i += 8;
									}
								}
							}
							array2 = new byte[5];
							array3 = new byte[4];
							array4 = list2.ToArray();
							for (; i < num; i++)
							{
								if (!(a != "comp"))
								{
									break;
								}
								array2[0] = array[i];
								array2[1] = array[i + 1];
								array2[2] = array[i + 2];
								array2[3] = array[i + 3];
								array2[4] = array[i + 4];
								array3[0] = array[i];
								array3[1] = array[i + 1];
								array3[2] = array[i + 2];
								array3[3] = array[i + 3];
								string @string = Encoding.ASCII.GetString(array2);
								a = Encoding.ASCII.GetString(array3);
								if (@string == "audio")
								{
									int num3 = 0;
									while (array[i - 1] != 0)
									{
										i--;
										num3++;
									}
									i -= 4;
									binaryReader.BaseStream.Seek(i, SeekOrigin.Begin);
									int count2 = binaryReader.ReadInt32();
									string text3 = new string(binaryReader.ReadChars(count2));
									int fsbID = binaryReader.ReadInt32();
									int wavID = binaryReader.ReadInt32();
									int duration = binaryReader.ReadInt32();
									text3 = text3.TrimEnd(default(char));
									list.Add(new FevInfo
									{
										offset = i,
										fsbID = fsbID,
										wavID = wavID,
										path = text3,
										duration = duration
									});
									i = (int)binaryReader.BaseStream.Position;
								}
							}
						}
					}
				}
				IOrderedEnumerable<FevInfo> orderedEnumerable = from fevInfo in list
					orderby fevInfo.fsbID, fevInfo.wavID
					select fevInfo;
				using (StreamWriter streamWriter = File.CreateText(item.Replace(".fev", ".txt")))
				{
					foreach (FevInfo item2 in orderedEnumerable)
					{
						if (this.radioButton1.Checked)
							streamWriter.WriteLine($"offset:{item2.offset} fsb:{array4[item2.fsbID]} wavID:{item2.wavID} path:{item2.path} duration:{item2.duration}");
						else if (this.radioButton2.Checked)
							streamWriter.WriteLine($"{item2.offset};{array4[item2.fsbID]};{item2.wavID};{item2.path.Replace(@"/", @"\")};{item2.duration}");
					}
				}
			}
			MessageBox.Show("finish");
		}

		private bool IsASCII(byte b)
		{
			if (b > 0 && b <= 127)
			{
				return true;
			}
			return false;
		}

		private void Form1_Load(object sender, EventArgs e)
		{
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(364, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "解析";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(68, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(290, 22);
            this.textBox1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "FEV 目錄";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(68, 40);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(290, 22);
            this.textBox2.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "輸出目錄";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(33, 70);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "格式";
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(68, 68);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(61, 16);
            this.radioButton1.TabIndex = 4;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Original";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(135, 68);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(106, 16);
            this.radioButton2.TabIndex = 5;
            this.radioButton2.Text = "For Batch Sorting";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 96);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "FEV Path Parser v0.3_Mod";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
	}
}
