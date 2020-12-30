using System;
using System.Text;
using System.Windows.Forms;






namespace P3Ribbon.Scripts
{
    public partial class Form_InfoP3 : System.Windows.Forms.Form

    {
        public Form_InfoP3()
        {

            InitializeComponent();
            pictureBox1.Hide();

            try
            {
                webBrowser1.Navigate(indirizzo.ToString());
            }
            catch
            {
                webBrowser1.Hide();
                pictureBox1.Show();

            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        StringBuilder indirizzo = new StringBuilder("https://www.google.com/maps/search/p3/@45.4612712,11.7692459,15.34z");

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private void Form_InfoP3_Load(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
