using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CryptoTrainer
{
    public partial class Atbasz : Form
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
            "Łatwe: odszyfruj OZH za pomocą szyfru Atbasz.",
            "Średnie: zaszyfruj TAJNY PLAN za pomocą szyfru Atbasz.",
            "Trudne: odszyfruj XABGZMRV ILADRQZ DBLYIZAMRV za pomocą szyfru Atbasz."
        };

        private readonly string[] _poprawneOdpowiedzi =
        {
            Szyfruj("OZH"),
            Szyfruj("TAJNY PLAN"),
            Szyfruj("XABGZMRV ILADRQZ DBLYIZAMRV")
        };

        public Atbasz(int slotNr, Form menuForm)
        {
            if (slotNr < 1 || slotNr > 3)
                throw new ArgumentOutOfRangeException(nameof(slotNr));

            InitializeComponent();

            _slotNr = slotNr;
            _menuForm = menuForm;

            Text = "CryptoTrainer - Szyfr Atbasz";
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

            // Uzupełniamy tylko zadania Atbasza. Pozostałe poziomy zostają bez zmian.
            for (int zadanie = 1; zadanie <= 3; zadanie++)
            {
                string klucz = $"2.{zadanie}";
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
                if (File.Exists(_sciezka))
                    File.Replace(plikTymczasowy, _sciezka, null);
                else
                    File.Move(plikTymczasowy, _sciezka);

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
                Text = "POZIOM 2 – Szyfr Atbasz",
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
                    "Atbasz odwraca alfabet A–Z (bez polskich znaków).\n" +
                    "Każdą literę zastępujemy literą z drugiego końca alfabetu.\n\n" +
                    "Mapowanie liter:\n" +
                    "A ↔ Z, B ↔ Y, C ↔ X, D ↔ W, E ↔ V\n" +
                    "F ↔ U, G ↔ T, H ↔ S, I ↔ R, J ↔ Q\n" +
                    "K ↔ P, L ↔ O, M ↔ N\n\n" +
                    "Przykład: HELLO → SVOOL.\n" +
                    "Szyfrujemy i odszyfrowujemy według tego samego mapowania.\n\n" +
                    "Spacje pozostają na swoich miejscach.\n" +
                    "Wielkość liter i spacje nie wpływają na ocenę.\n" +
                    "Odpowiedzi zapisują się automatycznie. Ocenia je Sprawdź.",
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
                Text = "POZIOM 2 – Szyfr Atbasz",
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
            string klucz = $"2.{numerZadania}";

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

        private void SprawdzOdpowiedzi()
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

            if (!ZapiszDane())
                return;

            MessageBox.Show(
                this,
                "Wyniki zostały sprawdzone i zapisane.",
                "CryptoTrainer",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
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

        // Ten sam sposób działa w obie strony, a spacji nie zmieniamy.
        private static string Szyfruj(string tekst)
        {
            char[] litery = tekst.ToUpperInvariant().ToCharArray();
            for (int i = 0; i < litery.Length; i++)
            {
                if (litery[i] >= 'A' && litery[i] <= 'Z')
                {
                    int X = litery[i] - 'A';
                    litery[i] = (char)('Z' - X);
                }
            }
            return new string(litery);
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
