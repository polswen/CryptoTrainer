namespace CryptoTrainer
{
    public partial class Menu : Form
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void graj_Click(object sender, EventArgs e)
        {

        }

        private void wyjdz_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void graj_Click_1(object sender, EventArgs e)
        {
            SlotsMenu form = new SlotsMenu();
            form.Show();
            this.Hide();
        }
    }
}
