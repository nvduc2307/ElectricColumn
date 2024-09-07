using Autodesk.AutoCAD.Geometry;
using CadDev.Utils.Lines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadDev.Tools.ElectricColumnGeneral.models
{
    public class LineInPlan
    {
        public int Id { get; set; }
        public string Mark { get; set; }
        public LineCad Line { get; set; }
        public Vector3d Direction { get; set; }
        public Vector3d Normar { get; set; }

        public LineInPlan(LineCad lineCad)
        {
            Line = lineCad;
            Id = lineCad.Id;
            Mark = $"{lineCad.Id}";
            Direction = lineCad.Dir;
        }
    }
}
