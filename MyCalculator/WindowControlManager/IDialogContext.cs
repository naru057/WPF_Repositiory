using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyCalculator.WindowControlManager
{
	public interface IDialogContext
	{
		IContext Context { get; set; }
	}
}
