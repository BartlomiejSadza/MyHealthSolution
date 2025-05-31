using MyHealth.Api.Models;
using System.Text;

namespace MyHealth.Api.Services
{
    public partial class AdvancedHealthAnalyzer
    {
        // Lifestyle Analysis Methods
        private string AnalyzeSmokingStatus(string smoke)
        {
            return smoke.ToLower() switch
            {
                "yes" => "Palenie tytoniu - główny czynnik ryzyka chorób serca, płuc i nowotworów",
                "no" => "Niepalący - doskonały wybór dla zdrowia",
                _ => "Status palenia nieznany"
            };
        }

        private string AnalyzeHealthMonitoring(string scc)
        {
            return scc.ToLower() switch
            {
                "yes" => "Monitoruje kalorie - świadomy wybór żywieniowy",
                "no" => "Nie monitoruje kalorii - rozważ śledzenie nawyków żywieniowych",
                _ => "Status monitorowania nieznany"
            };
        }

        private string AnalyzeFamilyHistory(string familyHistory)
        {
            return familyHistory.ToLower() switch
            {
                "yes" => "Historia rodzinna nadwagi - zwiększone ryzyko genetyczne",
                "no" => "Brak rodzinnej historii nadwagi - korzystny profil genetyczny",
                _ => "Historia rodzinna nieznana"
            };
        }

        private int CalculateLifestyleScore(HealthRequest request)
        {
            try
            {
                var data = request.DataFrame_Split.Data[0];
                var smoke = data[13]?.ToString() ?? "no";
                var scc = data[14]?.ToString() ?? "no";
                var calc = data[15]?.ToString() ?? "no";
                var tue = Convert.ToDouble(data[8]?.ToString() ?? "2");
                var faf = Convert.ToDouble(data[7]?.ToString() ?? "1");

                var score = 50; // Bazowy wynik

                // Palenie (największy wpływ)
                if (smoke.ToLower() == "yes") score -= 25;
                else score += 10;

                // Monitorowanie zdrowia
                if (scc.ToLower() == "yes") score += 10;

                // Alkohol
                score += calc.ToLower() switch
                {
                    "no" => 10,
                    "sometimes" => 5,
                    "frequently" => -10,
                    "always" => -20,
                    _ => 0
                };

                // Aktywność fizyczna
                score += (int)(faf * 5);

                // Czas przy technologii
                if (tue > 6) score -= 10;
                else if (tue < 3) score += 5;

                return Math.Max(0, Math.Min(100, score));
            }
            catch (Exception ex)
            {
                return 50; // Wartość domyślna
            }
        }

        private string EstimateStressLevel(HealthRequest request)
        {
            try
            {
                var data = request.DataFrame_Split.Data[0];
                var tue = Convert.ToDouble(data[8]?.ToString() ?? "2");
                var faf = Convert.ToDouble(data[7]?.ToString() ?? "1");
                var smoke = data[13]?.ToString() ?? "no";
                var caec = data[12]?.ToString() ?? "Sometimes";

                var stressScore = 0;

                // Wysokie użycie technologii może wskazywać na stres
                if (tue > 6) stressScore += 2;
                
                // Niska aktywność fizyczna
                if (faf < 2) stressScore += 2;
                
                // Palenie często związane ze stresem
                if (smoke.ToLower() == "yes") stressScore += 3;
                
                // Jedzenie między posiłkami może wskazywać na stres
                if (caec.ToLower() == "frequently" || caec.ToLower() == "always") stressScore += 2;

                return stressScore switch
                {
                    >= 6 => "Wysoki - rozważ techniki zarządzania stresem",
                    >= 4 => "Umiarkowany - wprowadź relaksację do rutyny",
                    >= 2 => "Niski - dobra równowaga życiowa",
                    _ => "Bardzo niski - doskonałe zarządzanie stresem"
                };
            }
            catch (Exception ex)
            {
                return "Nieznany - brak danych do oceny";
            }
        }

        private string EstimateSleepQuality(HealthRequest request)
        {
            try
            {
                var data = request.DataFrame_Split.Data[0];
                var tue = Convert.ToDouble(data[8]?.ToString() ?? "2");
                var faf = Convert.ToDouble(data[7]?.ToString() ?? "1");
                var smoke = data[13]?.ToString() ?? "no";
                var calc = data[15]?.ToString() ?? "no";

                var sleepScore = 5; // Bazowy wynik (0-10)

                // Wysokie użycie technologii może wpływać na sen
                if (tue > 6) sleepScore -= 2;
                
                // Aktywność fizyczna poprawia jakość snu
                if (faf >= 3) sleepScore += 2;
                else if (faf < 1) sleepScore -= 1;
                
                // Palenie wpływa negatywnie na sen
                if (smoke.ToLower() == "yes") sleepScore -= 2;
                
                // Alkohol wpływa na jakość snu
                if (calc.ToLower() == "frequently" || calc.ToLower() == "always") sleepScore -= 1;

                sleepScore = Math.Max(0, Math.Min(10, sleepScore));

                return sleepScore switch
                {
                    >= 8 => "Doskonała - regeneracyjny sen",
                    >= 6 => "Dobra - zadowalająca jakość snu",
                    >= 4 => "Przeciętna - możliwe ulepszenia",
                    >= 2 => "Słaba - wymagane zmiany nawyków",
                    _ => "Bardzo słaba - skonsultuj się z lekarzem"
                };
            }
            catch (Exception ex)
            {
                return "Nieznana - brak danych do oceny";
            }
        }

        private List<string> GenerateLifestyleRecommendations(HealthRequest request)
        {
            var recommendations = new List<string>();
            
            try
            {
                var data = request.DataFrame_Split.Data[0];
                var smoke = data[13]?.ToString() ?? "no";
                var tue = Convert.ToDouble(data[8]?.ToString() ?? "2");
                var calc = data[15]?.ToString() ?? "no";

                if (smoke.ToLower() == "yes")
                {
                    recommendations.Add("🚭 Rzucenie palenia - najważniejsza zmiana dla zdrowia");
                    recommendations.Add("📞 Skorzystaj z poradni antynikotynowej");
                    recommendations.Add("💊 Rozważ terapię zastępczą nikotyny");
                }

                if (tue > 6)
                {
                    recommendations.Add("📱 Ogranicz czas przed ekranami do maksymalnie 6h dziennie");
                    recommendations.Add("👀 Reguła 20-20-20: co 20 min patrz 20 sekund na obiekt 20 stóp daleko");
                    recommendations.Add("🌙 Unikaj ekranów 2h przed snem");
                }

                if (calc.ToLower() == "frequently" || calc.ToLower() == "always")
                {
                    recommendations.Add("🍷 Ogranicz alkohol do maksymalnie 1-2 drinków tygodniowo");
                    recommendations.Add("💧 Zastąp alkohol wodą z cytryną lub herbatą ziołową");
                }

                // Ogólne rekomendacje lifestyle
                recommendations.Add("😴 Utrzymuj regularny rytm snu (7-9h nocą)");
                recommendations.Add("🧘 Wprowadź 10 minut medytacji dziennie");
                recommendations.Add("🌿 Spędzaj czas na świeżym powietrzu");
                recommendations.Add("👥 Utrzymuj aktywne kontakty społeczne");

                return recommendations;
            }
            catch (Exception ex)
            {
                return new List<string> { "Skonsultuj się z lekarzem w sprawie zdrowego stylu życia" };
            }
        }

        // Risk Factor Analysis Methods
        private void AnalyzeWeightRisk(HealthRequest request, List<RiskFactor> riskFactors)
        {
            try
            {
                var data = request.DataFrame_Split.Data[0];
                var height = Convert.ToDouble(data[2]?.ToString() ?? "1.7");
                var weight = Convert.ToDouble(data[3]?.ToString() ?? "70");
                var bmi = weight / (height * height);

                if (bmi < 18.5)
                {
                    riskFactors.Add(new RiskFactor
                    {
                        Name = "Niedowaga",
                        Level = "Średnie",
                        Description = "BMI poniżej normy może prowadzić do niedoborów żywieniowych",
                        Impact = "Osłabiona odporność, problemy hormonalne, osteoporoza",
                        PreventionTips = new List<string> { "Zwiększ spożycie kalorii", "Dodaj białko do diety", "Trening siłowy" }
                    });
                }
                else if (bmi > 30)
                {
                    riskFactors.Add(new RiskFactor
                    {
                        Name = "Otyłość",
                        Level = "Wysokie",
                        Description = "BMI powyżej 30 znacznie zwiększa ryzyko chorób przewlekłych",
                        Impact = "Cukrzyca, choroby serca, nowotwory, bezdech senny",
                        PreventionTips = new List<string> { "Deficyt kaloryczny", "Zwiększ aktywność", "Konsultacja dietetyka" }
                    });
                }
                else if (bmi > 25)
                {
                    riskFactors.Add(new RiskFactor
                    {
                        Name = "Nadwaga",
                        Level = "Umiarkowane",
                        Description = "BMI 25-30 zwiększa ryzyko rozwoju chorób metabolicznych",
                        Impact = "Zwiększone ciśnienie, insulinooporność, problemy stawowe",
                        PreventionTips = new List<string> { "Kontrola porcji", "Regularna aktywność", "Monitorowanie wagi" }
                    });
                }
            }
            catch (Exception ex)
            {
                // Dodaj domyślny czynnik ryzyka w przypadku błędu
                riskFactors.Add(new RiskFactor
                {
                    Name = "Nieznane ryzyko wagowe",
                    Level = "Nieznane",
                    Description = "Nie można ocenić ryzyka związanego z wagą",
                    Impact = "Brak danych",
                    PreventionTips = new List<string> { "Skonsultuj się z lekarzem" }
                });
            }
        }

        private void AnalyzeNutritionalRisks(HealthRequest request, List<RiskFactor> riskFactors)
        {
            try
            {
                var data = request.DataFrame_Split.Data[0];
                var fcvc = Convert.ToDouble(data[4]?.ToString() ?? "2");
                var ch2o = Convert.ToDouble(data[6]?.ToString() ?? "2");
                var favc = data[11]?.ToString() ?? "no";

                if (fcvc < 2)
                {
                    riskFactors.Add(new RiskFactor
                    {
                        Name = "Niedobór warzyw w diecie",
                        Level = "Średnie",
                        Description = "Niedostateczne spożycie warzyw prowadzi do niedoborów witamin",
                        Impact = "Niedobory witamin, minerałów, błonnika, zwiększone ryzyko nowotworów",
                        PreventionTips = new List<string> { "5 porcji warzyw dziennie", "Różnorodność kolorów", "Warzywa w każdym posiłku" }
                    });
                }

                if (ch2o < 2)
                {
                    riskFactors.Add(new RiskFactor
                    {
                        Name = "Niewystarczające nawodnienie",
                        Level = "Niskie",
                        Description = "Zbyt małe spożycie wody wpływa na funkcjonowanie organizmu",
                        Impact = "Problemy z nerkami, zmęczenie, problemy skórne, zaparcia",
                        PreventionTips = new List<string> { "8 szklanek wody dziennie", "Woda przed posiłkami", "Monitoruj kolor moczu" }
                    });
                }

                if (favc.ToLower() == "yes")
                {
                    riskFactors.Add(new RiskFactor
                    {
                        Name = "Wysokokaloryczna dieta",
                        Level = "Średnie",
                        Description = "Częste spożycie wysokokalorycznej żywności",
                        Impact = "Przyrost masy ciała, cukrzyca, choroby serca",
                        PreventionTips = new List<string> { "Zamień na zdrowe alternatywy", "Kontroluj porcje", "Czytaj etykiety" }
                    });
                }
            }
            catch (Exception ex)
            {
                // Błąd w analizie żywieniowej - dodaj ogólny czynnik ryzyka
            }
        }

        private void AnalyzeActivityRisks(HealthRequest request, List<RiskFactor> riskFactors)
        {
            try
            {
                var data = request.DataFrame_Split.Data[0];
                var faf = Convert.ToDouble(data[7]?.ToString() ?? "1");
                var tue = Convert.ToDouble(data[8]?.ToString() ?? "2");

                if (faf < 2)
                {
                    riskFactors.Add(new RiskFactor
                    {
                        Name = "Siedzący tryb życia",
                        Level = "Wysokie",
                        Description = "Brak regularnej aktywności fizycznej",
                        Impact = "Choroby serca, cukrzyca, osteoporoza, depresja, skrócenie życia",
                        PreventionTips = new List<string> { "150 min aktywności tygodniowo", "Codzienne spacery", "Ćwiczenia siłowe 2x/tydzień" }
                    });
                }

                if (tue > 8)
                {
                    riskFactors.Add(new RiskFactor
                    {
                        Name = "Nadmierne użycie technologii",
                        Level = "Średnie",
                        Description = "Ponad 8 godzin dziennie przy ekranach",
                        Impact = "Problemy wzroku, ból pleców/szyi, zaburzenia snu, izolacja społeczna",
                        PreventionTips = new List<string> { "Przerwy co godzinę", "Ergonomiczne stanowisko", "Digital detox" }
                    });
                }
            }
            catch (Exception ex)
            {
                // Błąd w analizie aktywności
            }
        }

        private void AnalyzeLifestyleRisks(HealthRequest request, List<RiskFactor> riskFactors)
        {
            try
            {
                var data = request.DataFrame_Split.Data[0];
                var smoke = data[13]?.ToString() ?? "no";
                var calc = data[15]?.ToString() ?? "no";

                if (smoke.ToLower() == "yes")
                {
                    riskFactors.Add(new RiskFactor
                    {
                        Name = "Palenie tytoniu",
                        Level = "Bardzo wysokie",
                        Description = "Palenie jest główną przyczyną chorób przewlekłych",
                        Impact = "Nowotwory, choroby serca, udar, POCHP, przedwczesna śmierć",
                        PreventionTips = new List<string> { "Natychmiastowe rzucenie", "Terapia nikotynowa", "Wsparcie psychologiczne" }
                    });
                }

                if (calc.ToLower() == "frequently" || calc.ToLower() == "always")
                {
                    riskFactors.Add(new RiskFactor
                    {
                        Name = "Nadmierne spożycie alkoholu",
                        Level = "Wysokie",
                        Description = "Częste spożycie alkoholu zwiększa ryzyko chorób",
                        Impact = "Choroby wątroby, nowotwory, uzależnienie, problemy społeczne",
                        PreventionTips = new List<string> { "Ogranicz do 1-2 drinków tygodniowo", "Dni bez alkoholu", "Szukaj wsparcia" }
                    });
                }
            }
            catch (Exception ex)
            {
                // Błąd w analizie stylu życia
            }
        }

        private void AnalyzeGeneticRisks(HealthRequest request, List<RiskFactor> riskFactors)
        {
            try
            {
                var data = request.DataFrame_Split.Data[0];
                var familyHistory = data[10]?.ToString() ?? "no";
                var age = Convert.ToInt32(data[1]?.ToString() ?? "25");

                if (familyHistory.ToLower() == "yes")
                {
                    riskFactors.Add(new RiskFactor
                    {
                        Name = "Predyspozycje genetyczne",
                        Level = "Średnie",
                        Description = "Rodzinna historia nadwagi zwiększa ryzyko",
                        Impact = "Zwiększone ryzyko otyłości, cukrzycy, chorób metabolicznych",
                        PreventionTips = new List<string> { "Regularne badania", "Profilaktyka żywieniowa", "Aktywny styl życia" }
                    });
                }

                if (age > 50)
                {
                    riskFactors.Add(new RiskFactor
                    {
                        Name = "Wiek powyżej 50 lat",
                        Level = "Niskie",
                        Description = "Naturalny proces starzenia zwiększa niektóre ryzyka",
                        Impact = "Spowolnienie metabolizmu, utrata masy mięśniowej, problemy hormonalne",
                        PreventionTips = new List<string> { "Regularne badania", "Trening siłowy", "Suplementacja" }
                    });
                }
            }
            catch (Exception ex)
            {
                // Błąd w analizie genetycznej
            }
        }

        private void AnalyzeProtectiveFactors(HealthRequest request, List<string> protectiveFactors)
        {
            try
            {
                var data = request.DataFrame_Split.Data[0];
                var faf = Convert.ToDouble(data[7]?.ToString() ?? "1");
                var smoke = data[13]?.ToString() ?? "no";
                var fcvc = Convert.ToDouble(data[4]?.ToString() ?? "2");
                var scc = data[14]?.ToString() ?? "no";
                var calc = data[15]?.ToString() ?? "no";

                if (faf >= 3)
                    protectiveFactors.Add("Regularna aktywność fizyczna - silna ochrona przed chorobami przewlekłymi");

                if (smoke.ToLower() == "no")
                    protectiveFactors.Add("Niepalenie - znacząco zmniejsza ryzyko nowotworów i chorób serca");

                if (fcvc >= 3)
                    protectiveFactors.Add("Wysokie spożycie warzyw - bogate źródło antyoksydantów i błonnika");

                if (scc.ToLower() == "yes")
                    protectiveFactors.Add("Świadome monitorowanie zdrowia - wczesne wykrywanie problemów");

                if (calc.ToLower() == "no")
                    protectiveFactors.Add("Abstynencja alkoholowa - ochrona wątroby i układu nerwowego");
            }
            catch (Exception ex)
            {
                protectiveFactors.Add("Skonsultuj się z lekarzem w sprawie czynników ochronnych");
            }
        }

        private string CalculateOverallRisk(List<RiskFactor> riskFactors)
        {
            if (!riskFactors.Any()) return "Bardzo niskie";

            var highRiskCount = riskFactors.Count(r => r.Level == "Bardzo wysokie" || r.Level == "Wysokie");
            var mediumRiskCount = riskFactors.Count(r => r.Level == "Średnie");

            if (highRiskCount >= 2) return "Bardzo wysokie";
            if (highRiskCount >= 1) return "Wysokie";
            if (mediumRiskCount >= 3) return "Wysokie";
            if (mediumRiskCount >= 1) return "Umiarkowane";
            return "Niskie";
        }

        private int CalculateRiskScore(List<RiskFactor> riskFactors)
        {
            var score = 0;
            foreach (var risk in riskFactors)
            {
                score += risk.Level switch
                {
                    "Bardzo wysokie" => 25,
                    "Wysokie" => 20,
                    "Średnie" => 15,
                    "Niskie" => 10,
                    _ => 5
                };
            }
            return Math.Min(100, score);
        }

        private List<string> GeneratePreventionStrategies(List<RiskFactor> riskFactors)
        {
            var strategies = new List<string>();

            if (riskFactors.Any(r => r.Name.Contains("Otyłość") || r.Name.Contains("Nadwaga")))
            {
                strategies.Add("Program redukcji masy ciała z deficytem 500-750 kcal dziennie");
                strategies.Add("Zwiększenie aktywności fizycznej do 300 minut tygodniowo");
            }

            if (riskFactors.Any(r => r.Name.Contains("Palenie")))
            {
                strategies.Add("Natychmiastowy program rzucania palenia z wsparciem medycznym");
            }

            if (riskFactors.Any(r => r.Name.Contains("Siedzący")))
            {
                strategies.Add("Wprowadzenie codziennej aktywności - minimum 30 minut spaceru");
            }

            if (riskFactors.Any(r => r.Name.Contains("dieta") || r.Name.Contains("żywienie")))
            {
                strategies.Add("Konsultacja z dietetykiem i opracowanie spersonalizowanego planu");
            }

            strategies.Add("Regularne badania kontrolne co 6-12 miesięcy");
            strategies.Add("Edukacja zdrowotna i budowanie świadomości");

            return strategies;
        }

        private List<string> GenerateMonitoringRecommendations(List<RiskFactor> riskFactors)
        {
            var monitoring = new List<string>();

            monitoring.Add("Cotygodniowe ważenie się (w tym samym dniu i czasie)");
            monitoring.Add("Miesięczne pomiary obwodów (talia, biodra, ramiona)");

            if (riskFactors.Any(r => r.Level == "Wysokie" || r.Level == "Bardzo wysokie"))
            {
                monitoring.Add("Badania krwi co 3 miesiące (glukoza, lipidogram, CRP)");
                monitoring.Add("Pomiar ciśnienia krwi 2x w tygodniu");
            }
            else
            {
                monitoring.Add("Badania krwi co 6-12 miesięcy");
                monitoring.Add("Pomiar ciśnienia krwi raz w miesiącu");
            }

            monitoring.Add("Dziennik żywieniowy przez pierwsze 4 tygodnie");
            monitoring.Add("Aplikacja do śledzenia aktywności fizycznej");

            return monitoring;
        }

        // Recommendation Generation Methods
        private List<Recommendation> GenerateDietaryRecommendations(HealthRequest request, string prediction)
        {
            var recommendations = new List<Recommendation>();
            
            try
            {
                var data = request.DataFrame_Split.Data[0];
                var height = Convert.ToDouble(data[2]?.ToString() ?? "1.7");
                var weight = Convert.ToDouble(data[3]?.ToString() ?? "70");
                var bmi = weight / (height * height);

                if (bmi > 25)
                {
                    recommendations.Add(new Recommendation
                    {
                        Category = "Dieta",
                        Description = "Wprowadź deficyt kaloryczny 500-750 kcal dziennie dla bezpiecznej redukcji wagi",
                        Priority = 10,
                        Timeframe = "Natychmiast - długoterminowo",
                        ExpectedBenefit = "Redukcja 0.5-1 kg tygodniowo, poprawa parametrów metabolicznych",
                        ActionSteps = new List<string>
                        {
                            "Oblicz dzienne zapotrzebowanie kaloryczne",
                            "Zaplanuj 5-6 mniejszych posiłków dziennie",
                            "Zwiększ spożycie białka do 1.2-1.6g/kg masy ciała",
                            "Ogranicz węglowodany proste i tłuszcze nasycone"
                        }
                    });
                }

                recommendations.Add(new Recommendation
                {
                    Category = "Nawyki żywieniowe",
                    Description = "Wprowadź zasadę talerza: 1/2 warzywa, 1/4 białko, 1/4 węglowodany złożone",
                    Priority = 8,
                    Timeframe = "1-2 tygodnie na wdrożenie",
                    ExpectedBenefit = "Lepsze nasycenie, kontrola glikemii, większa różnorodność składników",
                    ActionSteps = new List<string>
                    {
                        "Kup większe talerze dla warzyw",
                        "Przygotuj warzywa z wyprzedzeniem",
                        "Wybieraj białko chude (ryby, drób, rośliny strączkowe)",
                        "Zastąp białe pieczywo pełnoziarnistym"
                    }
                });
            }
            catch (Exception ex)
            {
                recommendations.Add(new Recommendation
                {
                    Category = "Dieta",
                    Description = "Skonsultuj się z dietetykiem w sprawie zdrowej diety",
                    Priority = 5,
                    Timeframe = "W ciągu miesiąca",
                    ExpectedBenefit = "Spersonalizowane zalecenia żywieniowe",
                    ActionSteps = new List<string> { "Umów wizytę u dietetyka" }
                });
            }

            return recommendations;
        }

        private List<Recommendation> GenerateActivityRecommendations(HealthRequest request, string prediction)
        {
            var recommendations = new List<Recommendation>();
            
            try
            {
                var data = request.DataFrame_Split.Data[0];
                var faf = Convert.ToDouble(data[7]?.ToString() ?? "1");

                if (faf < 2)
                {
                    recommendations.Add(new Recommendation
                    {
                        Category = "Aktywność fizyczna",
                        Description = "Rozpocznij program aktywności od 150 minut umiarkowanej aktywności tygodniowo",
                        Priority = 9,
                        Timeframe = "Stopniowe wprowadzanie przez 4-6 tygodni",
                        ExpectedBenefit = "Poprawa kondycji, redukcja ryzyka chorób, lepsze samopoczucie",
                        ActionSteps = new List<string>
                        {
                            "Tydzień 1-2: 15 minut spaceru dziennie",
                            "Tydzień 3-4: 25 minut aktywności dziennie",
                            "Tydzień 5-6: 30 minut aktywności + 2x trening siłowy",
                            "Znajdź aktywność, która sprawia Ci przyjemność"
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                recommendations.Add(new Recommendation
                {
                    Category = "Aktywność fizyczna",
                    Description = "Wprowadź regularną aktywność fizyczną",
                    Priority = 7,
                    Timeframe = "Stopniowo",
                    ExpectedBenefit = "Poprawa kondycji i zdrowia",
                    ActionSteps = new List<string> { "Zacznij od codziennych spacerów" }
                });
            }

            return recommendations;
        }

        private List<Recommendation> GenerateMedicalRecommendations(HealthRequest request, string prediction)
        {
            var recommendations = new List<Recommendation>();
            
            try
            {
                var data = request.DataFrame_Split.Data[0];
                var age = Convert.ToInt32(data[1]?.ToString() ?? "25");
                var height = Convert.ToDouble(data[2]?.ToString() ?? "1.7");
                var weight = Convert.ToDouble(data[3]?.ToString() ?? "70");
                var bmi = weight / (height * height);

                if (bmi > 30 || age > 50)
                {
                    recommendations.Add(new Recommendation
                    {
                        Category = "Badania medyczne",
                        Description = "Wykonaj kompleksowe badania krwi i konsultację lekarską",
                        Priority = 7,
                        Timeframe = "W ciągu 2-4 tygodni",
                        ExpectedBenefit = "Wczesne wykrycie problemów, spersonalizowane zalecenia",
                        ActionSteps = new List<string>
                        {
                            "Umów wizytę u lekarza rodzinnego",
                            "Wykonaj morfologię, biochemię, lipidogram",
                            "Sprawdź poziom witaminy D i B12",
                            "Rozważ badanie tarczycy (TSH, fT3, fT4)"
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                recommendations.Add(new Recommendation
                {
                    Category = "Badania medyczne",
                    Description = "Regularne badania kontrolne",
                    Priority = 5,
                    Timeframe = "Raz w roku",
                    ExpectedBenefit = "Monitorowanie stanu zdrowia",
                    ActionSteps = new List<string> { "Umów wizytę kontrolną u lekarza" }
                });
            }

            return recommendations;
        }

        private List<Recommendation> GeneratePsychologicalRecommendations(HealthRequest request)
        {
            var recommendations = new List<Recommendation>();

            recommendations.Add(new Recommendation
            {
                Category = "Zdrowie psychiczne",
                Description = "Wprowadź codzienne praktyki mindfulness i zarządzania stresem",
                Priority = 6,
                Timeframe = "Codziennie przez 21 dni na wyrobienie nawyku",
                ExpectedBenefit = "Redukcja stresu, lepsza kontrola emocji, poprawa jakości snu",
                ActionSteps = new List<string>
                {
                    "5-10 minut medytacji rano",
                    "Prowadź dziennik wdzięczności",
                    "Praktykuj głębokie oddychanie w stresie",
                    "Zaplanuj czas na hobby i relaks"
                }
            });

            return recommendations;
        }

        // Action Plan Generation Methods
        private List<string> GenerateImmediateActions(HealthRequest request, string prediction)
        {
            var actions = new List<string>();
            
            try
            {
                var data = request.DataFrame_Split.Data[0];
                var height = Convert.ToDouble(data[2]?.ToString() ?? "1.7");
                var weight = Convert.ToDouble(data[3]?.ToString() ?? "70");
                var smoke = data[13]?.ToString() ?? "no";
                var bmi = weight / (height * height);

                actions.Add("Zainstaluj aplikację do śledzenia kalorii i aktywności");
                actions.Add("Kup wagę i rozpocznij codzienne ważenie się");
                actions.Add("Zaplanuj posiłki na najbliższe 3 dni");

                if (smoke.ToLower() == "yes")
                    actions.Add("PILNE: Skontaktuj się z lekarzem w sprawie rzucenia palenia");

                if (bmi > 30)
                    actions.Add("Umów wizytę u lekarza i dietetyka w ciągu 2 tygodni");

                actions.Add("Rozpocznij od 15-minutowego spaceru dzisiaj");
            }
            catch (Exception ex)
            {
                actions.Add("Skonsultuj się z lekarzem w sprawie natychmiastowych działań");
            }

            return actions;
        }

        private List<string> GenerateShortTermGoals(HealthRequest request, string prediction)
        {
            var goals = new List<string>();
            
            try
            {
                var data = request.DataFrame_Split.Data[0];
                var height = Convert.ToDouble(data[2]?.ToString() ?? "1.7");
                var weight = Convert.ToDouble(data[3]?.ToString() ?? "70");
                var bmi = weight / (height * height);

                if (bmi > 25)
                    goals.Add("Redukcja wagi o 2-4 kg w ciągu pierwszych 2 miesięcy");

                goals.Add("Osiągnięcie 150 minut aktywności fizycznej tygodniowo");
                goals.Add("Wprowadzenie 5 porcji warzyw i owoców dziennie");
                goals.Add("Redukcja czasu ekranowego o 25%");
                goals.Add("Wykonanie kompleksowych badań krwi");
            }
            catch (Exception ex)
            {
                goals.Add("Ustaw realistyczne cele zdrowotne z pomocą specjalisty");
            }

            return goals;
        }

        private List<string> GenerateLongTermGoals(HealthRequest request, string prediction)
        {
            var goals = new List<string>();
            
            try
            {
                var data = request.DataFrame_Split.Data[0];
                var height = Convert.ToDouble(data[2]?.ToString() ?? "1.7");
                var weight = Convert.ToDouble(data[3]?.ToString() ?? "70");
                var bmi = weight / (height * height);

                if (bmi > 25)
                    goals.Add("Osiągnięcie i utrzymanie zdrowej wagi (BMI 18.5-24.9)");

                goals.Add("Budowa nawyku regularnej aktywności fizycznej (300+ min/tydzień)");
                goals.Add("Osiągnięcie optymalnych parametrów krwi (glukoza, cholesterol)");
                goals.Add("Redukcja ryzyka chorób przewlekłych o 50%");
                goals.Add("Poprawa jakości życia i energii o 40%");
            }
            catch (Exception ex)
            {
                goals.Add("Opracuj długoterminowy plan zdrowotny z lekarzem");
            }

            return goals;
        }

        private Dictionary<string, List<string>> GenerateWeeklySchedule(HealthRequest request)
        {
            return new Dictionary<string, List<string>>
            {
                ["Poniedziałek"] = new List<string> { "30 min spacer", "Planowanie posiłków", "Zakupy zdrowej żywności" },
                ["Wtorek"] = new List<string> { "Trening siłowy 20 min", "Przygotowanie zdrowych przekąsek" },
                ["Środa"] = new List<string> { "45 min aktywność cardio", "Medytacja 10 min" },
                ["Czwartek"] = new List<string> { "Joga/stretching 30 min", "Przygotowanie posiłków na piątek" },
                ["Piątek"] = new List<string> { "Trening całego ciała 40 min", "Relaks i hobby" },
                ["Sobota"] = new List<string> { "Długa aktywność na świeżym powietrzu", "Przygotowanie posiłków na tydzień" },
                ["Niedziela"] = new List<string> { "Aktywny odpoczynek", "Planowanie następnego tygodnia", "Pomiary i ocena postępów" }
            };
        }

        private List<string> GenerateMonthlyMilestones(HealthRequest request, string prediction)
        {
            return new List<string>
            {
                "Miesiąc 1: Wdrożenie podstawowych nawyków żywieniowych i aktywności",
                "Miesiąc 2: Osiągnięcie regularności w ćwiczeniach i diecie",
                "Miesiąc 3: Pierwsze widoczne rezultaty i kontrolne badania",
                "Miesiąc 6: Znacząca poprawa parametrów zdrowotnych",
                "Miesiąc 12: Osiągnięcie docelowej wagi i optymalnego zdrowia"
            };
        }

        private List<string> GenerateProgressTracking(HealthRequest request)
        {
            return new List<string>
            {
                "Codzienny pomiar wagi (rano, na czczo)",
                "Tygodniowe zdjęcia postępów",
                "Miesięczne pomiary obwodów ciała",
                "Kwartalne badania krwi",
                "Cotygodniowa ocena samopoczucia (1-10)",
                "Miesięczna analiza dziennika żywieniowego",
                "Półroczna konsultacja z lekarzem/dietetykiem"
            };
        }

        // Final Helper Methods
        private string GetHealthCategory(int healthScore)
        {
            return healthScore switch
            {
                >= 90 => "Doskonałe zdrowie",
                >= 80 => "Bardzo dobre zdrowie",
                >= 70 => "Dobre zdrowie",
                >= 60 => "Przeciętne zdrowie",
                >= 50 => "Poniżej przeciętnej",
                _ => "Wymaga pilnej interwencji"
            };
        }

        private string GetHealthPrognosis(HealthRequest request, string prediction)
        {
            try
            {
                var data = request.DataFrame_Split.Data[0];
                var age = Convert.ToInt32(data[1]?.ToString() ?? "25");
                var height = Convert.ToDouble(data[2]?.ToString() ?? "1.7");
                var weight = Convert.ToDouble(data[3]?.ToString() ?? "70");
                var faf = Convert.ToDouble(data[7]?.ToString() ?? "1");
                var smoke = data[13]?.ToString() ?? "no";
                var bmi = weight / (height * height);

                var prognosisFactors = 0;
                if (bmi >= 18.5 && bmi <= 24.9) prognosisFactors++;
                if (faf >= 3) prognosisFactors++;
                if (smoke.ToLower() == "no") prognosisFactors++;
                if (age < 50) prognosisFactors++;

                return prognosisFactors switch
                {
                    4 => "Doskonała - przy utrzymaniu obecnych nawyków możesz cieszyć się długim i zdrowym życiem",
                    3 => "Bardzo dobra - niewielkie zmiany mogą znacząco poprawić Twoje zdrowie",
                    2 => "Dobra - wprowadzenie zdrowych nawyków przyniesie wymierne korzyści",
                    1 => "Umiarkowana - wymagane znaczące zmiany stylu życia",
                    _ => "Wymagana natychmiastowa interwencja - wysokie ryzyko powikłań zdrowotnych"
                };
            }
            catch (Exception ex)
            {
                return "Nieznana - skonsultuj się z lekarzem w sprawie prognozy zdrowotnej";
            }
        }
    }
} 