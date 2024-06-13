using System.Windows.Media.Media3D;

namespace CadDev.Utils.CanvasUtils.Models
{
    public class AddClickEventAgrs : EventArgs
    {
        public System.Windows.Point PointCanvas { get; set; }
        public Point3D PointDescarte { get; set; }
        public AddClickEventAgrs(System.Windows.Point pcanvas, Point3D pDescarte)
        {
            PointCanvas = pcanvas;
            PointDescarte = pDescarte;
        }
    }
}
