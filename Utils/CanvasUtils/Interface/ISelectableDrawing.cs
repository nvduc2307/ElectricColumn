using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CadDev.Utils.CanvasUtils.Interface
{
	public interface ISelectableDrawing : IDrawing
	{
		bool IsSelected { get; set; }
		bool IsCreated { get; set; }

		event EventHandler SelectedChanged;

		void OnMouseEnter();
		void OnMouseLeave();
	}
}
