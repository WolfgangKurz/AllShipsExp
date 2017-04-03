using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Reactive.Linq;
using Livet;
using System.IO;
using System.Windows.Forms;

using Grabacr07.KanColleWrapper;

namespace AllShipsExp
{
	class ToolViewModel : ViewModel
	{
		public ToolViewModel()
		{
		}

		public void Export()
		{
			var dialog = new SaveFileDialog();
			dialog.Filter = "CSV (*.csv)|*.csv";
			if (dialog.ShowDialog() == DialogResult.Cancel) return;

			var list = KanColleClient.Current.Homeport.Organization.Ships
				.Values.OrderByDescending(x => x.Exp).ToArray();

			using (FileStream fs = new FileStream(dialog.FileName, FileMode.Create))
			{
				StreamWriter writer = new StreamWriter(fs);
				fs.Write(new byte[] { 0xEF, 0xBB, 0xBF }, 0, 3);

				try
				{
					CSV.Write(fs,
						"Total Exp:",
						list.Sum(x => x.Exp)
					);

					CSV.Write(fs,
						"ID",
						"Type",
						"Name",
						"Level",
						"EXP"
					);

					foreach(var item in list)
					{
						CSV.Write(fs,
							item.Id,
							item.Info.ShipType.Name,
							item.Info.Name,
							item.Level,
							item.Exp
						);
					}
					writer.Flush();
				}
				catch (Exception e)
				{
					MessageBox.Show("Error:\n" + e.ToString(), "An error has occurred", MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}
	}

	internal class CSV
	{
		public static string[] Read(Stream stream)
		{
			List<byte> bytebuffer = new List<byte>();
			while (stream.Position < stream.Length)
			{
				int data = stream.ReadByte();
				if (data == -1) break;

				bytebuffer.Add((byte)data);
				if (data == 10) break;
			}

			string line = Encoding.UTF8.GetString(bytebuffer.ToArray()).Trim();
			string buffer = "";
			List<string> output = new List<string>();
			int mode = 0, pass = 0;
			/*
			  0: Token finding
			  1: String token exploring
			  2: Not-string token exploring
			*/

			for (int i = 0; i < line.Length; i++)
			{
				char c = line[i];

				switch (mode)
				{
					case 0:
						if ("\"".Contains(c))
						{
							mode = 1;
						}
						else if (!" \t,".Contains(c))
						{
							buffer += c;
							mode = 2;
						}
						else if (c == ',')
						{
							output.Add(buffer);
							buffer = "";
						}
						break;
					case 1:
						if (c == '\\') pass++;
						else if (pass > 0)
						{
							pass--;
							buffer += c;
						}
						else if (c == '"') mode = 0;
						else buffer += c;
						break;
					case 2:
						if (c == ',')
						{
							output.Add(buffer);
							buffer = "";
							mode = 0;
						}
						else buffer += c;
						break;
					default:
						return null;
				}
			}
			if (buffer.Length > 0)
				output.Add(buffer);

			return output.ToArray();
		}
		public static void Write(Stream stream, params object[] columns)
		{
			List<string> list = new List<string>();
			string line;

			foreach (var item in columns)
			{
				string content = item.ToString();

				if (content.Contains(","))
				{
					// \ -> \\, " -> \"
					content = content.Replace("\\", "\\\\").Replace("\"", "\\\"");
					content = "\"" + content + "\"";
				}

				list.Add(content);
			}

			line = string.Join(",", list.ToArray());

			byte[] output = Encoding.UTF8.GetBytes(line + Environment.NewLine);
			stream.Write(output, 0, output.Length);
		}
	}
}
