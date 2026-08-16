using System;
using System.Windows.Forms;

namespace MIS_ELITE
{
    public partial class VerificationForm : Form
    {
        public static VerificationForm Instance
        {
            get; private set;
        }

        public VerificationForm()
        {
            InitializeComponent();
        }

        private void VerificationForm_Load(object sender, EventArgs e)
        {

        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            Sales.UserName = tbUserName.Text; // Fixed: Accessing static property directly with the class name
            Sales.Password = tbPassword.Text; // Fixed: Accessing static property directly with the class name
            Sales.UserType = cbUserType.Text; // Fixed: Accessing static property directly with the class name
            Hide();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Sales.UserName = null; // Fixed: Accessing static property directly with the class name
            Sales.Password = null; // Fixed: Accessing static property directly with the class name
            Sales.UserType = null; // Fixed: Accessing static property directly with the class name
            Hide();
        }
    }
}
