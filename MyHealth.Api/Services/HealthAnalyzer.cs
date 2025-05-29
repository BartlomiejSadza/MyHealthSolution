using System;
using System.Collections.Generic;

namespace MyHealth.Api.Services
{
    public static class HealthAnalyzer
    {
        public static List<string> GenerateObservations(
            IDictionary<string, object> features,
            string prediction)
        {
            var obs = new List<string>();

            // 1) BMI
            if (features.TryGetValue("Height", out var hObj)
             && features.TryGetValue("Weight", out var wObj)
             && double.TryParse(hObj?.ToString(), out var h)
             && double.TryParse(wObj?.ToString(), out var w)
             && h > 0)
            {
                var bmi = w / (h * h);
                obs.Add($"Twoje BMI = {bmi:F1}, klasyfikacja: '{prediction}'.");
            }

            // 2) Palenie
            if (features.TryGetValue("SMOKE", out var smoke)
             && smoke.ToString().Equals("yes", StringComparison.OrdinalIgnoreCase))
                obs.Add("Palisz papierosy – ↑ ryzyko chorób serca i płuc.");

            // 3) Obciążenie rodzinne
            if (features.TryGetValue("family_history_with_overweight", out var fam)
             && fam.ToString().Equals("yes", StringComparison.OrdinalIgnoreCase))
                obs.Add("Rodzinne przypadki nadwagi – mogą istnieć predyspozycje.");

            // 4) Wysokokaloryczne potrawy
            if (features.TryGetValue("FAVC", out var favc)
             && favc.ToString().Equals("yes", StringComparison.OrdinalIgnoreCase))
                obs.Add("Częste wysokokaloryczne posiłki – rozważ zdrowszą dietę.");

            // 5) Słodzone napoje
            if (features.TryGetValue("SCC", out var scc)
             && scc.ToString().Equals("yes", StringComparison.OrdinalIgnoreCase))
                obs.Add("Spożywasz słodzone napoje – ogranicz je dla lepszego metabolizmu.");

            // 6) Woda
            if (features.TryGetValue("CH2O", out var ch2oObj)
             && double.TryParse(ch2oObj?.ToString(), out var ch2o)
             && ch2o < 2)
                obs.Add("Niska konsumpcja wody (<2 l/dzień) – pij więcej wody.");

            // 7) Aktywność fizyczna
            if (features.TryGetValue("FAF", out var fafObj)
             && double.TryParse(fafObj?.ToString(), out var faf)
             && faf < 1)
                obs.Add("Brak aktywności fizycznej – celuj w min. 150 min/tydzień.");

            // 8) Czas przed ekranem
            if (features.TryGetValue("TUE", out var tueObj)
             && double.TryParse(tueObj?.ToString(), out var tue)
             && tue > 2)
                obs.Add(">2 h/dzień przed ekranem – rób regularne przerwy.");

            // 9) Transport
            if (features.TryGetValue("MTRANS", out var mt))
            {
                var t = mt.ToString();
                if (t == "Walking" || t == "Bike" ||
                    t == "Public_Transportation")
                {
                    obs.Add("Aktywny transport – super.");
                }
                else
                {
                    obs.Add("Transport autem – rozważ spacer lub rower.");
                }
            }

            // 10) Alkohol (CALC)
            if (features.TryGetValue("CALC", out var calc)
             && calc.ToString().Equals("Always", StringComparison.OrdinalIgnoreCase))
                obs.Add("Częste spożycie alkoholu – ↑ ryzyko zdrowotne.");

            // 11) Wiek i płeć
            if (features.TryGetValue("Age", out var ageObj)
             && int.TryParse(ageObj?.ToString(), out var age))
            {
                obs.Add(age < 18
                    ? "Osoba niepełnoletnia – rekomendowana opieka specjalisty."
                    : age > 60
                        ? "Wiek >60 lat – zwiększone ryzyko chorób przewlekłych."
                        : "Wiek w normie – dbaj o zdrowie.");
            }

            return obs;
        }
    }
}
