using System.Windows.Controls;

namespace SimpleHMI.Controls
{
    /// <summary>
    /// Interaction logic for AxisDef
    /// </summary>
    public partial class AxisDef : UserControl
    {
        public AxisDef()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Cambio valore
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void A0_RestPose_Lostfocus(object sender, System.Windows.RoutedEventArgs e) {
            int i;

            i = 21;
        }

        private void NumericUpDown_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

        }
    }
}
