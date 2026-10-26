using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using ProgressBar = System.Windows.Forms.ProgressBar;

namespace CryptoTrainer
{
    public partial class SlotsMenu : Form
    {
        const int ileSlotow = 3;
        public SlotsMenu()
        {
            InitializeComponent();
            aktualizujProgressBary();

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void aktualizujProgressBary()
        {
            for (int i = 1; i <= ileSlotow; i++)
            {
                aktualizujProgressBar(i);
            }
        }

        private void aktualizujProgressBar(int barNr)
        {
            string sciezka = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Jsons",
                $"Slot{barNr}.json"
            );

            if (!File.Exists(sciezka))
            {
                MessageBox.Show($"Nie znaleziono pliku:\n{sciezka}");
                return;
            }

            string json = File.ReadAllText(sciezka);
            JObject dane = JObject.Parse(json);

            int poprawne = 0;
            int wszystkie = 0;

            foreach (var zadanie in dane.Properties())
            {
                JArray wartosci = (JArray)zadanie.Value;
                string wynik = wartosci[0]?.ToString();

                wszystkie++;

                if (wynik == "correct")
                {
                    poprawne++;
                }
            }

            ProgressBar progressBar = barNr switch
            {
                1 => progressBar1,
                2 => progressBar2,
                3 => progressBar3,
                _ => null
            };

            Label label = barNr switch
            {
                1 => label1,
                2 => label2,
                3 => label3,
                _ => null
            };

            if (progressBar == null || label == null || wszystkie == 0)
                return;

            progressBar.Minimum = 0;
            progressBar.Maximum = wszystkie;
            progressBar.Value = poprawne;

            int procent = poprawne * 100 / wszystkie;
            label.Text = $"{poprawne}/{wszystkie} ({procent}%)";
        }

        private void Slot1Btn_Click(object sender, EventArgs e)
        {
            ZacznijGre(1);
        }
        private void Slot2Btn_Click(object sender, EventArgs e)
        {
            ZacznijGre(2);
        }
        private void Slot3Btn_Click(object sender, EventArgs e)
        {
            ZacznijGre(3);
        }
        private void ZacznijGre(int slot)
        {
            this.Hide();

            Cezar gra = new Cezar(slot, this);

            gra.FormClosed += (s, e) =>
            {
                this.Show();
                aktualizujProgressBary();
            };

            gra.Show();
        }

    }
}
