
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CryptoTrainer
{
    public partial class Cezar : Form
    {
        private readonly int _slotNr;
        private readonly Form _menuForm;
        private readonly string _sciezka;

        private JObject _dane = new JObject();
        private bool _wczytywanie;

        private Panel _introPanel;
        private Panel _zadaniaPanel;

        private TextBox[] _odpowiedzi = new TextBox[3];
        private Label[] _wyniki = new Label[3];

        private readonly string[] _pytania =
        {
            "Łatwe: odszyfruj NRW. Przesunięcie wynosi 3.",
            "Średnie: odszyfruj GRP. Przesunięcie wynosi 3.",
            "Trudne: odszyfruj SODQHWD. Przesunięcie wynosi 3."
        };

        private readonly string[] _poprawneOdpowiedzi =
        {
            "KOT",
            "DOM",
            "PLANETA"
        };

        public Cezar(int slotNr, Form menuForm)
        {
            InitializeComponent();

            _slotNr = slotNr;
            _menuForm = menuForm;

            Text = "CryptoTrainer - Szyfr Cezara";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(976, 697);
            BackColor = Color.Black;
            BackgroundImage = Properties.Resources._25336;

            _sciezka = ZnajdzPlikJson();

            WczytajDane();
            ZbudujInterfejs();

            FormClosed += (s, e) => _menuForm.Show();
        }

        private string ZnajdzPlikJson()
        {
            string folder = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Jsons"
            );

            Directory.CreateDirectory(folder);

            return Path.Combine(folder, $"Slot{_slotNr}.json");
        }

        private void WczytajDane()
        {
            if (File.Exists(_sciezka))
            {
                _dane = JObject.Parse(File.ReadAllText(_sciezka));
            }

            // Zapewniamy komplet 15 zadań, nie usuwając
            // istniejących odpowiedzi ani wyników.
            for (int poziom = 1; poziom <= 5; poziom++)
            {
                for (int zadanie = 1; zadanie <= 3; zadanie++)
                {
                    string klucz = $"{poziom}.{zadanie}";

                    if (_dane[klucz] is not JArray tablica)
                    {
                        _dane[klucz] = new JArray("", "");
                    }
                    else
                    {
                        while (tablica.Count < 2)
                            tablica.Add("");
                    }
                }
            }

            ZapiszDane();
        }

        private void ZapiszDane()
        {
            File.WriteAllText(
                _sciezka,
                _dane.ToString(Newtonsoft.Json.Formatting.Indented));
        }

        private void ZbudujInterfejs()
        {
            // ===== STRONA WPROWADZAJĄCA =====

            _introPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            Label tytul = new Label
            {
                Text = "POZIOM 1 - Szyfr Cezara",
                Font = new Font("Segoe UI", 25, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 90
            };

            Label instrukcja = new Label
            {
                Text =
                    "Szyfr Cezara to jeden z najprostszych szyfrów " +
                    "podstawieniowych.\n\n" +
                    "Zasada działania:\n" +
                    "Każdą literę zastępujemy literą przesuniętą " +
                    "o określoną liczbę miejsc w alfabecie.\n\n" +
                    "Przykład dla przesunięcia o 3:\n" +
                    "A → D, B → E, C → F\n\n" +
                    "Aby odszyfrować wiadomość, przesuwamy litery " +
                    "w przeciwnym kierunku.\n\n" +
                    "Przykład: NRW → KOT\n\n" +
                    "W tym poziomie wszystkie zadania wykorzystują " +
                    "przesunięcie o 3 miejsca.\n" +
                    "Wpisuj odpowiedzi bez spacji.",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Location = new Point(100, 120),
                Size = new Size(776, 400)
            };

            Button wstecz = new Button
            {
                Text = "Wstecz",
                Location = new Point(100, 570),
                Size = new Size(150, 50),
                BackColor = SystemColors.ActiveCaption,
                ForeColor = Color.Black,
                UseVisualStyleBackColor = false,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };

            Button dalej = new Button
            {
                Text = "Dalej",
                Location = new Point(726, 570),
                Size = new Size(150, 50),
                BackColor = SystemColors.ActiveCaption,
                ForeColor = Color.Black,
                UseVisualStyleBackColor = false,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };

            wstecz.Click += (s, e) => Close();

            dalej.Click += (s, e) =>
            {
                _introPanel.Visible = false;
                _zadaniaPanel.Visible = true;
            };

            _introPanel.Controls.Add(tytul);
            _introPanel.Controls.Add(instrukcja);
            _introPanel.Controls.Add(wstecz);
            _introPanel.Controls.Add(dalej);

            // ===== STRONA Z ZADANIAMI =====

            _zadaniaPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Visible = false
            };

            Label tytulZadan = new Label
            {
                Text = "POZIOM 1 - Szyfr Cezara",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 65
            };

            _zadaniaPanel.Controls.Add(tytulZadan);

            for (int i = 0; i < 3; i++)
            {
                int indeks = i;
                int numerZadania = i + 1;

                GroupBox grupa = new GroupBox
                {
                    Text = $"Zadanie {numerZadania}",
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    ForeColor = Color.White,
                    BackColor = Color.Transparent,
                    Location = new Point(100, 80 + i * 145),
                    Size = new Size(776, 125)
                };

                Label pytanie = new Label
                {
                    Text = _pytania[i],
                    Font = new Font("Segoe UI", 11, FontStyle.Bold),
                    ForeColor = Color.White,
                    BackColor = Color.Transparent,
                    Location = new Point(15, 25),
                    Size = new Size(740, 30)
                };

                _odpowiedzi[i] = new TextBox
                {
                    Font = new Font("Segoe UI", 12),
                    Location = new Point(15, 65),
                    Size = new Size(250, 30),
                    MaxLength = 100
                };

                _wyniki[i] = new Label
                {
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = Color.White,
                    BackColor = Color.Transparent,
                    Location = new Point(285, 65),
                    Size = new Size(450, 30)
                };

                _odpowiedzi[i].TextChanged += (s, e) =>
                {
                    if (!_wczytywanie)
                        ZapiszOdpowiedz(indeks);
                };

                grupa.Controls.Add(pytanie);
                grupa.Controls.Add(_odpowiedzi[i]);
                grupa.Controls.Add(_wyniki[i]);

                _zadaniaPanel.Controls.Add(grupa);
            }

            Button wrocDoInstrukcji = new Button
            {
                Text = "Wstecz",
                Location = new Point(100, 550),
                Size = new Size(150, 45),
                BackColor = SystemColors.ActiveCaption,
                ForeColor = Color.Black,
                UseVisualStyleBackColor = false,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            Button sprawdz = new Button
            {
                Text = "Sprawdź",
                Location = new Point(413, 550),
                Size = new Size(150, 45),
                BackColor = SystemColors.ActiveCaption,
                ForeColor = Color.Black,
                UseVisualStyleBackColor = false,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            Button zakoncz = new Button
            {
                Text = "Powrót do menu",
                Location = new Point(726, 550),
                Size = new Size(150, 45),
                BackColor = SystemColors.ActiveCaption,
                ForeColor = Color.Black,
                UseVisualStyleBackColor = false,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            wrocDoInstrukcji.Click += (s, e) =>
            {
                _zadaniaPanel.Visible = false;
                _introPanel.Visible = true;
            };

            sprawdz.Click += (s, e) => SprawdzOdpowiedzi();

            zakoncz.Click += (s, e) => Close();

            _zadaniaPanel.Controls.Add(wrocDoInstrukcji);
            _zadaniaPanel.Controls.Add(sprawdz);
            _zadaniaPanel.Controls.Add(zakoncz);

            Controls.Add(_zadaniaPanel);
            Controls.Add(_introPanel);

            WczytajOdpowiedziDoPol();
        }

        private JArray PobierzZadanie(int numerZadania)
        {
            string klucz = $"1.{numerZadania}";

            return (JArray)_dane[klucz];
        }

        private void WczytajOdpowiedziDoPol()
        {
            _wczytywanie = true;

            for (int i = 0; i < 3; i++)
            {
                JArray zadanie = PobierzZadanie(i + 1);

                _odpowiedzi[i].Text =
                    zadanie[1]?.ToString() ?? "";

                string status = zadanie[0]?.ToString();

                if (status == "correct")
                {
                    _wyniki[i].Text = "Poprawna odpowiedź";
                    _wyniki[i].ForeColor = Color.Lime;
                }
                else if (status == "incorrect")
                {
                    _wyniki[i].Text = "Niepoprawna odpowiedź";
                    _wyniki[i].ForeColor = Color.OrangeRed;
                }
            }

            _wczytywanie = false;
        }

        private void ZapiszOdpowiedz(int indeksPola)
        {
            JArray zadanie = PobierzZadanie(indeksPola + 1);

            zadanie[1] = _odpowiedzi[indeksPola].Text;

            ZapiszDane();
        }

        private void SprawdzOdpowiedzi()
        {
            for (int i = 0; i < 3; i++)
            {
                string odpowiedz = Normalizuj(
                    _odpowiedzi[i].Text);

                string poprawna = Normalizuj(
                    _poprawneOdpowiedzi[i]);

                bool czyPoprawna =
                    odpowiedz == poprawna;

                JArray zadanie = PobierzZadanie(i + 1);

                zadanie[0] = czyPoprawna
                    ? "correct"
                    : "incorrect";

                zadanie[1] = _odpowiedzi[i].Text;

                _wyniki[i].Text = czyPoprawna
                    ? "Poprawna odpowiedź!"
                    : "Niepoprawna odpowiedź.";

                _wyniki[i].ForeColor = czyPoprawna
                    ? Color.Lime
                    : Color.OrangeRed;
            }

            ZapiszDane();

            MessageBox.Show(
                "Wyniki zostały sprawdzone i zapisane.",
                "CryptoTrainer",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private string Normalizuj(string tekst)
        {
            return new string(
                tekst
                    .Where(c => !char.IsWhiteSpace(c))
                    .ToArray())
                .ToUpperInvariant();
        }
    }
}
