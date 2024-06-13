using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace CadDev.Utils.CanvasUtils.Interface
{
	public interface IDrawing
	{
		List<Shape> ShapesOnCanvas { get; set; }
	}
}
