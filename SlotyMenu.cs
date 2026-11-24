using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Windows.Forms;
using ProgressBar = System.Windows.Forms.ProgressBar;

namespace CryptoTrainer
{
    public partial class SlotsMenu : Form
    {
        const int ileSlotow = 3;
        private readonly ComboBox wyborPoziomu;

        public SlotsMenu()
        {
            InitializeComponent();

            Label opisPoziomu = new Label
            {
                Text = "Wybierz poziom, a następnie slot:",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Location = new Point(256, 70),
                AutoSize = true
            };
            wyborPoziomu = new ComboBox
            {
                Name = "wyborPoziomu",
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = SystemColors.ActiveCaption,
                ForeColor = Color.Black,
                Location = new Point(256, 105),
                Size = new Size(361, 30),
                TabIndex = 0
            };
            wyborPoziomu.Items.Add("POZIOM 1 – Szyfr Cezara");
            wyborPoziomu.Items.Add("POZIOM 2 – Szyfr Atbasz");
            wyborPoziomu.Items.Add("POZIOM 3 – Szyfr Vigenère’a");
            wyborPoziomu.SelectedIndex = 0;
            Controls.Add(opisPoziomu);
            Controls.Add(wyborPoziomu);
            Slot1Btn.TabIndex = 1;
            Slot2Btn.TabIndex = 2;
            Slot3Btn.TabIndex = 3;

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

            JObject dane;
            try
            {
                dane = File.Exists(sciezka)
                    ? JObject.Parse(File.ReadAllText(sciezka))
                    : new JObject();
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException ||
                                       ex is Newtonsoft.Json.JsonException)
            {
                MessageBox.Show(this, $"Nie udało się odczytać postępów slotu {barNr}.\n\n{ex.Message}",
                    "Błąd odczytu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int poprawne = 0;
            const int wszystkie = 15;

            for (int poziom = 1; poziom <= 5; poziom++)
            {
                for (int zadanie = 1; zadanie <= 3; zadanie++)
                {
                    // Niepełna tablica nie może wywołać błędu indeksowania.
                    if (dane[$"{poziom}.{zadanie}"] is JArray { Count: > 0 } wartosci &&
                        wartosci[0]?.ToString() == "correct")
                        poprawne++;
                }
            }

            ProgressBar? progressBar = barNr switch
            {
                1 => progressBar1,
                2 => progressBar2,
                3 => progressBar3,
                _ => null
            };

            Label? label = barNr switch
            {
                1 => label1,
                2 => label2,
                3 => label3,
                _ => null
            };

            if (progressBar == null || label == null)
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
            Form gra;
            try
            {
                gra = wyborPoziomu.SelectedIndex switch
                {
                    1 => new Atbasz(slot, this),
                    2 => new Vigenere(slot, this),
                    _ => new Cezar(slot, this)
                };
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException ||
                                       ex is Newtonsoft.Json.JsonException)
            {
                MessageBox.Show(this,
                    $"Nie udało się otworzyć slotu {slot}. Plik nie został zmieniony.\n\n{ex.Message}",
                    "Błąd odczytu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            this.Hide();

            gra.FormClosed += (s, e) =>
            {
                aktualizujProgressBary();
            };

            gra.Show();
        }

    }
}
