using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCalculator.WindowControlManager
{
	public interface IDialog
	{
		object Datacontext { get; set; }
		void Show();
		bool? ShowDialog();
		void Close();
	}
}
