using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleHMI.Models
{
    public class AxisList
    {
        // f***ing ugly down there
        public Axis A0 { get; set; }
        public Axis A1 { get; set; }
        public Axis A2 { get; set; }
        public Axis A3 { get; set; }
        public Axis A4 { get; set; }
        public Axis A5 { get; set; }
        public Axis A6 { get; set; }
        public Axis A7 { get; set; }
        public Axis A8 { get; set; }
        public Axis A9 { get; set; }
        public Axis A10 { get; set; }

        public AxisList() {
            A0 = new Axis() { Name = "X", UnitOfMeasure = "mm" };
            A1 = new Axis() { Name = "Y", UnitOfMeasure = "mm" };
            A2 = new Axis() { Name = "Rot", UnitOfMeasure = "rad" };
        }
    }
}
