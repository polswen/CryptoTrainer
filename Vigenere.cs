using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CryptoTrainer
{
    public partial class Vigenere : Form
    {
        private readonly int _slotNr;
        private readonly Form _menuForm;
        private readonly string _sciezka;

        private JObject _dane = new JObject();
        private bool _wczytywanie;

        private Panel _introPanel = null!;
        private Panel _zadaniaPanel = null!;
        private bool _niezapisaneZmiany;

        private readonly TextBox[] _odpowiedzi = new TextBox[3];
        private readonly Label[] _wyniki = new Label[3];

        private readonly string[] _pytania =
        {
            "Łatwe: odszyfruj wiadomość. Klucz: KEY.\nWiadomość: RIJVS",
            "Średnie: zaszyfruj wiadomość. Klucz: KOD.\nWiadomość: TAJNY PLAN",
            "Trudne: odszyfruj wiadomość. Klucz: MATRIX.\nWiadomość: EPHKSXVMR JQB VUMIW MA SSBWIQ"
        };

        private readonly string[] _poprawneOdpowiedzi =
        {
            SzyfrVigenere.Odszyfruj("RIJVS", "KEY"),
            SzyfrVigenere.Szyfruj("TAJNY PLAN", "KOD"),
            SzyfrVigenere.Odszyfruj("EPHKSXVMR JQB VUMIW MA SSBWIQ", "MATRIX")
        };

        public Vigenere(int slotNr, Form menuForm)
        {
            if (slotNr < 1 || slotNr > 3)
                throw new ArgumentOutOfRangeException(nameof(slotNr));

            InitializeComponent();

            _slotNr = slotNr;
            _menuForm = menuForm;

            Text = "POZIOM 3 – Szyfr Vigenère’a";
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(976, 697);
            BackColor = Color.Black;
            BackgroundImage = Properties.Resources._25336;

            _sciezka = ZnajdzPlikJson();

            WczytajDane();
            ZbudujInterfejs();

            FormClosing += (s, e) =>
            {
                if (_niezapisaneZmiany && !ZapiszDane())
                    e.Cancel = true;
            };
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

            else
            {
                // Nowy slot ma ten sam układ 15 zadań co pozostałe sloty.
                for (int poziom = 1; poziom <= 5; poziom++)
                    for (int zadanie = 1; zadanie <= 3; zadanie++)
                        _dane[$"{poziom}.{zadanie}"] = new JArray("", "");
            }

            // Uzupełniamy tylko zadania Vigenère’a. Pozostałe poziomy zostają bez zmian.
            for (int zadanie = 1; zadanie <= 3; zadanie++)
            {
                string klucz = $"3.{zadanie}";
                if (_dane[klucz] == null)
                {
                    _dane[klucz] = new JArray("", "");
                }
                else if (_dane[klucz] is JArray tablica)
                {
                    while (tablica.Count < 2)
                        tablica.Add("");
                }
                else
                {
                    throw new Newtonsoft.Json.JsonException(
                        $"Nieprawidłowe dane zadania {klucz}. Oczekiwano tablicy [wynik, odpowiedź].");
                }
            }
        }

        private bool ZapiszDane()
        {
            _niezapisaneZmiany = true;
            string plikTymczasowy = _sciezka + "." + Guid.NewGuid().ToString("N") + ".tmp";
            try
            {
                // Najpierw zapisujemy cały dokument obok pliku docelowego.
                // Błąd zapisu nie może wyzerować dotychczasowych postępów.
                File.WriteAllText(plikTymczasowy,
                    _dane.ToString(Newtonsoft.Json.Formatting.Indented));
                // Przeniesienie w tym samym folderze zastępuje dopiero gotowym plikiem.
                // File.Replace potrafi odrzucić zastąpienie w folderze OneDrive.
                File.Move(plikTymczasowy, _sciezka, overwrite: true);

                _niezapisaneZmiany = false;
                return true;
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                MessageBox.Show(this,
                    $"Nie udało się zapisać odpowiedzi w slocie {_slotNr}.\n" +
                    "Odpowiedzi pozostają w formularzu. Spróbuj ponownie.\n\n" + ex.Message,
                    "Błąd zapisu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                try { File.Delete(plikTymczasowy); }
                catch (IOException) { }
                catch (UnauthorizedAccessException) { }
            }
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
                Text = "POZIOM 3 – Szyfr Vigenère’a",
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
                    "Słowo-klucz wyznacza przesunięcie każdej kolejnej litery tekstu.\n" +
                    "Używamy alfabetu A–Z, bez polskich znaków: A = 0, B = 1, ... Z = 25.\n\n" +
                    "Powtarzamy klucz do długości wiadomości, pomijając spacje.\n" +
                    "Spacje pozostają na miejscu i nie przesuwają klucza.\n\n" +
                    "P – litera tekstu jawnego, K – litera klucza, C – litera szyfrogramu.\n" +
                    "Szyfrowanie: C = (P + K) mod 26.\n" +
                    "Odszyfrowywanie: P = (C - K + 26) mod 26.\n" +
                    "mod 26 to reszta z dzielenia; po Z wracamy do A.\n" +
                    "Przykład: A (0) + L (11) → L; L (11) - L (11) → A.\n\n" +
                    "Tekst: ATTACKATDAWN    Klucz: LEMON → LEMONLEMONLE\n" +
                    "Wynik szyfrowania: LXFOPVEFRNHR\n\n" +
                    "Wielkość liter i spacje nie wpływają na ocenę.\n" +
                    "Odpowiedzi zapisują się automatycznie. Ocenia je Sprawdź.",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Location = new Point(100, 105),
                Size = new Size(776, 440)
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
                Text = "POZIOM 3 – Szyfr Vigenère’a",
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
                    Size = new Size(740, 40)
                };

                _odpowiedzi[i] = new TextBox
                {
                    Font = new Font("Segoe UI", 12),
                    Location = new Point(15, 65),
                    Size = new Size(440, 30),
                    MaxLength = 100
                };

                _wyniki[i] = new Label
                {
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = Color.White,
                    BackColor = Color.Transparent,
                    Location = new Point(470, 65),
                    Size = new Size(285, 30)
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

            sprawdz.Click += (s, e) =>
            {
                if (SprawdzOdpowiedzi())
                    MessageBox.Show(this, "Wyniki zostały sprawdzone i zapisane.",
                        "CryptoTrainer", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

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
            string klucz = $"3.{numerZadania}";

            return (JArray)_dane[klucz]!;
        }

        private void WczytajOdpowiedziDoPol()
        {
            _wczytywanie = true;
            try
            {
                for (int i = 0; i < _odpowiedzi.Length; i++)
                {
                    JArray zadanie = PobierzZadanie(i + 1);
                    _odpowiedzi[i].Text = zadanie[1]?.ToString() ?? "";
                    UstawWynik(i, zadanie[0]?.ToString() ?? "");
                }
            }
            finally
            {
                _wczytywanie = false;
            }
        }

        private void ZapiszOdpowiedz(int indeksPola)
        {
            JArray zadanie = PobierzZadanie(indeksPola + 1);

            zadanie[1] = _odpowiedzi[indeksPola].Text;

            ZapiszDane();
        }

        private bool SprawdzOdpowiedzi()
        {
            for (int i = 0; i < _odpowiedzi.Length; i++)
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

                UstawWynik(i, zadanie[0]!.ToString());
            }

            return ZapiszDane();
        }

        private void UstawWynik(int indeks, string status)
        {
            _wyniki[indeks].Text = status switch
            {
                "correct" => "Poprawna odpowiedź!",
                "incorrect" => "Niepoprawna odpowiedź.",
                _ => ""
            };
            _wyniki[indeks].ForeColor = status switch
            {
                "correct" => Color.Lime,
                "incorrect" => Color.OrangeRed,
                _ => Color.White
            };
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
