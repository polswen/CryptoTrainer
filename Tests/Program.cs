using CryptoTrainer;
using Newtonsoft.Json.Linq;
using System.Reflection;

internal static class Program
{
    private static int _bledy;
    private static int _testy;
    private static readonly string Folder = Path.Combine(AppContext.BaseDirectory, "Jsons");

    [STAThread]
    private static int Main()
    {
        ApplicationConfiguration.Initialize();
        Test("Szyfrowanie znanego wektora", () =>
            Rowny("LXFOPVEFRNHR", SzyfrVigenere.Szyfruj("ATTACKATDAWN", "LEMON")));
        Test("Odszyfrowanie znanego wektora", () =>
            Rowny("ATTACKATDAWN", SzyfrVigenere.Odszyfruj("LXFOPVEFRNHR", "LEMON")));
        Test("Powtarzanie klucza", () =>
            Rowny("BCDBCDB", SzyfrVigenere.Szyfruj("AAAAAAA", "BCD")));
        Test("Spacje nie przesuwają klucza w obu kierunkach", () =>
        {
            Rowny("LXFOPV EF RNHR", SzyfrVigenere.Szyfruj("ATTACK AT DAWN", "LEMON"));
            Rowny("ATTACK AT DAWN", SzyfrVigenere.Odszyfruj("LXFOPV EF RNHR", "LEMON"));
            Rowny("B C  D\tB\nC", SzyfrVigenere.Szyfruj("A A  A\tA\nA", "BCD"));
        });
        Test("Granice A/Z, zerowe i maksymalne przesunięcie", () =>
        {
            Rowny("BAAB", SzyfrVigenere.Szyfruj("AZZA", "B"));
            Rowny("ZYYZ", SzyfrVigenere.Odszyfruj("AZZA", "B"));
            Rowny("Y", SzyfrVigenere.Szyfruj("Z", "Z"));
            Rowny("B", SzyfrVigenere.Odszyfruj("A", "Z"));
            const string alfabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            Rowny(alfabet, SzyfrVigenere.Szyfruj(alfabet, "A"));
            for (int k = 0; k < 26; k++)
                for (int p = 0; p < 26; p++)
                {
                    string szyfr = ((char)('A' + (p + k) % 26)).ToString();
                    string tekst = ((char)('A' + p)).ToString();
                    string klucz = ((char)('A' + k)).ToString();
                    Rowny(szyfr, SzyfrVigenere.Szyfruj(tekst, klucz));
                    Rowny(tekst, SzyfrVigenere.Odszyfruj(szyfr, klucz));
                }
        });
        Test("Małe litery, pusty tekst i znaki spoza alfabetu", () =>
        {
            Rowny("LXFOPV EF RNHR", SzyfrVigenere.Szyfruj("attack at dawn", "lemon"));
            Rowny("B!C 1D", SzyfrVigenere.Szyfruj("A!A 1A", "BCD"));
            Rowny("", SzyfrVigenere.Szyfruj("", "A"));
            Rowny("   ", SzyfrVigenere.Odszyfruj("   ", "A"));
        });
        Test("Nieprawidłowy klucz jest odrzucany", () =>
        {
            foreach (string klucz in new[] { "", " ", "A B", "K3Y", "Ż", "K" })
            {
                Rzuca<ArgumentException>(() => SzyfrVigenere.Szyfruj("ABC", klucz));
                Rzuca<ArgumentException>(() => SzyfrVigenere.Odszyfruj("ABC", klucz));
            }
            Rzuca<ArgumentNullException>(() => SzyfrVigenere.Szyfruj(null!, "A"));
            Rzuca<ArgumentNullException>(() => SzyfrVigenere.Szyfruj("ABC", null!));
        });
        Test("Jednoznaczne odpowiedzi trzech zadań", () =>
        {
            Rowny("HELLO", SzyfrVigenere.Odszyfruj("RIJVS", "KEY"));
            Rowny("DOMXM SVOQ", SzyfrVigenere.Szyfruj("TAJNY PLAN", "KOD"));
            Rowny("SPOTKAJMY SIE JUTRO PO SZKOLE",
                SzyfrVigenere.Odszyfruj("EPHKSXVMR JQB VUMIW MA SSBWIQ", "MATRIX"));
        });

        // Wyłącznie pliki w katalogu wyjściowym testów; kopie odtwarzamy w finally.
        Directory.CreateDirectory(Folder);
        var kopie = Enumerable.Range(1, 3).ToDictionary(i => i,
            i => File.Exists(Sciezka(i)) ? File.ReadAllBytes(Sciezka(i)) : null);
        try
        {
            Test("Interfejs, autozapis, ocena i odtworzenie wszystkich slotów", SprawdzSloty);
            Test("Brakujące wpisy i nowy slot", SprawdzNiepelneDane);
            Test("Uszkodzony JSON nie jest nadpisywany", SprawdzUszkodzoneDane);
            Test("Wybór poziomu 3 i niezmieniony licznik postępu", SprawdzMenu);
        }
        finally
        {
            foreach (var (slot, kopia) in kopie)
                if (kopia is null)
                    File.Delete(Sciezka(slot));
                else
                    File.WriteAllBytes(Sciezka(slot), kopia);
        }

        Console.WriteLine($"Wynik: {_testy - _bledy}/{_testy} testów zaliczonych.");
        return _bledy == 0 ? 0 : 1;
    }

    private static void SprawdzSloty()
    {
        for (int slot = 1; slot <= 3; slot++)
        {
            JObject poczatkowe = NowySlot();
            poczatkowe["3.1"] = new JArray("incorrect", "stara odpowiedź");
            poczatkowe["3.2"] = new JArray("correct", "poprzednia odpowiedź");
            Zapisz(slot, poczatkowe);
            byte[] przedOdczytem = File.ReadAllBytes(Sciezka(slot));
            using Form menu = new Form { ShowInTaskbar = false, Opacity = 0 };
            using (var gra = new Vigenere(slot, menu) { ShowInTaskbar = false, Opacity = 0 })
            {
                Rowny("POZIOM 3 – Szyfr Vigenère’a", gra.Text);
                var pola = Pole<TextBox[]>(gra, "_odpowiedzi");
                var wyniki = Pole<Label[]>(gra, "_wyniki");
                Rowny("stara odpowiedź", pola[0].Text);
                Rowny("Niepoprawna odpowiedź.", wyniki[0].Text);
                Rowny("Poprawna odpowiedź!", wyniki[1].Text);
                Prawda(przedOdczytem.SequenceEqual(File.ReadAllBytes(Sciezka(slot))),
                    "Sam odczyt nie zapisuje ani nie ocenia danych.");

                gra.Show();
                Application.DoEvents();
                Panel intro = Pole<Panel>(gra, "_introPanel");
                Panel zadania = Pole<Panel>(gra, "_zadaniaPanel");
                Prawda(intro.Visible && !zadania.Visible, "Start na stronie wprowadzenia.");
                if (slot == 1) Zrzut(gra, "vigenere-wprowadzenie.png");
                Przycisk(intro, "Dalej").PerformClick();
                Prawda(!intro.Visible && zadania.Visible, "Dalej otwiera zadania.");
                Przycisk(zadania, "Wstecz").PerformClick();
                Prawda(intro.Visible && !zadania.Visible, "Wstecz otwiera instrukcję.");
                Przycisk(intro, "Dalej").PerformClick();

                pola[0].Text = " he llo ";
                pola[1].Text = "domxm svoq";
                pola[2].Text = "spotkajmy sie jutro po szkole";
                JObject automatyczne = Odczytaj(slot);
                Rowny("incorrect", automatyczne["3.1"]![0]!.ToString());
                Rowny("correct", automatyczne["3.2"]![0]!.ToString());
                Rowny("", automatyczne["3.3"]![0]!.ToString());
                Rowny(" he llo ", automatyczne["3.1"]![1]!.ToString());
                ZachowaneInnePoziomy(poczatkowe, automatyczne);

                Sprawdz(gra);
                JObject ocenione = Odczytaj(slot);
                for (int i = 1; i <= 3; i++)
                {
                    Rowny("correct", ocenione[$"3.{i}"]![0]!.ToString());
                    Rowny("Poprawna odpowiedź!", wyniki[i - 1].Text);
                }
                if (slot == 1) Zrzut(gra, "vigenere-zadania.png");
                pola[1].Text = "ZLA ODPOWIEDZ";
                Rowny("correct", Odczytaj(slot)["3.2"]![0]!.ToString());
                Przycisk(zadania, "Powrót do menu").PerformClick();
                Prawda(gra.IsDisposed && menu.Visible, "Powrót zamyka poziom i pokazuje menu.");
            }

            using (var ponownie = new Vigenere(slot, menu) { ShowInTaskbar = false, Opacity = 0 })
            {
                var pola = Pole<TextBox[]>(ponownie, "_odpowiedzi");
                var wyniki = Pole<Label[]>(ponownie, "_wyniki");
                Rowny(" he llo ", pola[0].Text);
                Rowny("ZLA ODPOWIEDZ", pola[1].Text);
                Rowny("Poprawna odpowiedź!", wyniki[1].Text);
                ponownie.Show();
                Przycisk(Pole<Panel>(ponownie, "_introPanel"), "Dalej").PerformClick();
                Sprawdz(ponownie);
                Rowny("incorrect", Odczytaj(slot)["3.2"]![0]!.ToString());
                Rowny("Niepoprawna odpowiedź.", wyniki[1].Text);
                ZachowaneInnePoziomy(poczatkowe, Odczytaj(slot));
                ponownie.Close();
            }
            using var odtworzone = new Vigenere(slot, menu);
            Rowny("Niepoprawna odpowiedź.", Pole<Label[]>(odtworzone, "_wyniki")[1].Text);
            Prawda(!Directory.EnumerateFiles(Folder, "*.tmp").Any(), "Brak plików tymczasowych po zapisie.");
        }
    }

    private static void SprawdzNiepelneDane()
    {
        using Form menu = new Form();
        JObject niepelne = new JObject { ["1.1"] = new JArray("correct"), ["3.1"] = new JArray("incorrect") };
        Zapisz(1, niepelne);
        using (var gra = new Vigenere(1, menu))
        {
            Pole<TextBox[]>(gra, "_odpowiedzi")[2].Text = "robocza";
            JObject zapis = Odczytaj(1);
            ZachowaneInnePoziomy(niepelne, zapis);
            Rowny(2, ((JArray)zapis["3.1"]!).Count);
            Rowny("incorrect", zapis["3.1"]![0]!.ToString());
            Rowny("", zapis["3.2"]![0]!.ToString());
            Rowny("robocza", zapis["3.3"]![1]!.ToString());
        }
        File.Delete(Sciezka(1));
        using (var nowy = new Vigenere(1, menu))
            Pole<TextBox[]>(nowy, "_odpowiedzi")[0].Text = "nowy slot";
        Rowny(15, Odczytaj(1).Count);
        Rzuca<ArgumentOutOfRangeException>(() => new Vigenere(0, menu));
        Rzuca<ArgumentOutOfRangeException>(() => new Vigenere(4, menu));
    }

    private static void SprawdzUszkodzoneDane()
    {
        using Form menu = new Form();
        foreach (string dane in new[] { "{zepsuty json", "{\"3.1\":123}" })
        {
            File.WriteAllText(Sciezka(1), dane);
            Rzuca<Newtonsoft.Json.JsonException>(() => new Vigenere(1, menu));
            Rowny(dane, File.ReadAllText(Sciezka(1)));
        }
    }

    private static void SprawdzMenu()
    {
        for (int slot = 1; slot <= 3; slot++) Zapisz(slot, NowySlot());
        JObject dane = Odczytaj(2);
        for (int i = 1; i <= 3; i++) dane[$"3.{i}"] = new JArray("correct", "odpowiedź");
        Zapisz(2, dane);
        using var menu = new SlotsMenu { ShowInTaskbar = false, Opacity = 0 };
        ComboBox wybor = menu.Controls.OfType<ComboBox>().Single();
        Rowny(3, wybor.Items.Count);
        Rowny("POZIOM 3 – Szyfr Vigenère’a", wybor.Items[2]!.ToString()!);
        var pasek = menu.Controls.OfType<ProgressBar>().Single(p => p.Name == "progressBar2");
        Rowny(15, pasek.Maximum);
        Rowny(3, pasek.Value);
        wybor.SelectedIndex = 2;
        menu.Show();
        // Wywołanie tej samej ścieżki, co przyciski slotów.
        typeof(SlotsMenu).GetMethod("ZacznijGre", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(menu, new object[] { 2 });
        var gra = Application.OpenForms.OfType<Vigenere>().Single();
        using (gra)
        {
            gra.Opacity = 0;
            Pole<TextBox[]>(gra, "_odpowiedzi")[0].Text = "zmiana tylko w slocie 2";
            Rowny("zapis 3.1", Odczytaj(1)["3.1"]![1]!.ToString());
            Rowny("zapis 3.1", Odczytaj(3)["3.1"]![1]!.ToString());
            Rowny("zmiana tylko w slocie 2", Odczytaj(2)["3.1"]![1]!.ToString());
            gra.Close();
        }
        Prawda(menu.Visible, "Menu wraca po zamknięciu poziomu.");
        Rowny(3, pasek.Value);
        menu.Close();
    }

    private static JObject NowySlot()
    {
        var dane = new JObject { ["dodatkowe"] = new JObject { ["opis"] = "Zachowaj polskie znaki: ąęł" } };
        for (int poziom = 1; poziom <= 5; poziom++)
            for (int zadanie = 1; zadanie <= 3; zadanie++)
                dane[$"{poziom}.{zadanie}"] = new JArray("", $"zapis {poziom}.{zadanie}");
        return dane;
    }

    private static void ZachowaneInnePoziomy(JObject przed, JObject po)
    {
        foreach (var wpis in przed.Properties().Where(p => !p.Name.StartsWith("3.", StringComparison.Ordinal)))
            Prawda(JToken.DeepEquals(wpis.Value, po[wpis.Name]), $"Zachowano {wpis.Name}.");
        Rowny(przed.Properties().Count(p => !p.Name.StartsWith("3.", StringComparison.Ordinal)),
            po.Properties().Count(p => !p.Name.StartsWith("3.", StringComparison.Ordinal)));
    }

    private static void Zrzut(Form gra, string nazwa)
    {
        using var obraz = new System.Drawing.Bitmap(gra.Width, gra.Height);
        gra.DrawToBitmap(obraz, new System.Drawing.Rectangle(0, 0, gra.Width, gra.Height));
        obraz.Save(Path.Combine(AppContext.BaseDirectory, nazwa));
    }

    private static T Pole<T>(object obiekt, string nazwa) =>
        (T)obiekt.GetType().GetField(nazwa, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(obiekt)!;
    private static Button Przycisk(Control panel, string tekst) => panel.Controls.OfType<Button>().Single(b => b.Text == tekst);
    private static string Sciezka(int slot) => Path.Combine(Folder, $"Slot{slot}.json");
    private static JObject Odczytaj(int slot) => JObject.Parse(File.ReadAllText(Sciezka(slot)));
    private static void Zapisz(int slot, JObject dane) => File.WriteAllText(Sciezka(slot), dane.ToString());

    // Ta sama metoda oceny i zapisu co w obsłudze Sprawdź, bez modalnego potwierdzenia.
    private static void Sprawdz(Vigenere gra)
    {
        var metoda = typeof(Vigenere).GetMethod("SprawdzOdpowiedzi", BindingFlags.Instance | BindingFlags.NonPublic)!;
        Prawda((bool)metoda.Invoke(gra, null)!, "Ocena i zapis zakończone powodzeniem.");
    }

    private static void Test(string nazwa, Action test)
    {
        _testy++;
        try { test(); Console.WriteLine($"OK: {nazwa}"); }
        catch (Exception ex) { _bledy++; Console.Error.WriteLine($"BŁĄD: {nazwa}\n{ex}"); }
    }
    private static void Prawda(bool warunek, string opis)
    {
        if (!warunek) throw new InvalidOperationException(opis);
    }
    private static void Rowny<T>(T oczekiwany, T wynik)
    {
        if (!EqualityComparer<T>.Default.Equals(oczekiwany, wynik))
            throw new InvalidOperationException($"Oczekiwano: {oczekiwany}; otrzymano: {wynik}");
    }
    private static void Rzuca<T>(Action akcja) where T : Exception
    {
        try { akcja(); }
        catch (T) { return; }
        throw new InvalidOperationException($"Oczekiwano wyjątku {typeof(T).Name}.");
    }
}
