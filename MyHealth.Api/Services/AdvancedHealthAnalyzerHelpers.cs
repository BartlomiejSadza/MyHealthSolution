using MyHealth.Api.Models;
using System.Text;

namespace MyHealth.Api.Services
{
    public partial class AdvancedHealthAnalyzer
    {
        // BMI Helper Methods
        private double CalculateIdealWeight(double height, string gender)
        {
            // Formuła Devine'a
            var baseWeight = gender == "Male" ? 50.0 : 45.5;
            var heightCm = height * 100;
            var additionalWeight = (heightCm - 152.4) * (gender == "Male" ? 2.3 : 2.3) / 2.54;
            return baseWeight + additionalWeight;
        }

        private string GetBMIHealthRisk(double bmi, int age)
        {
            var baseRisk = bmi switch
            {
                < 16 => "Bardzo wysokie ryzyko powikłań zdrowotnych",
                < 18.5 => "Zwiększone ryzyko niedoborów żywieniowych",
                < 25 => "Minimalne ryzyko",
                < 30 => "Umiarkowanie zwiększone ryzyko",
                < 35 => "Wysokie ryzyko chorób sercowo-naczyniowych",
                < 40 => "Bardzo wysokie ryzyko cukrzycy i chorób serca",
                _ => "Ekstremalne ryzyko powikłań zdrowotnych"
            };

            if (age > 65 && bmi < 23)
                return "Zwiększone ryzyko sarkopenii u osób starszych";

            return baseRisk;
        }

        private string GenerateBMIExplanation(double bmi, string category, int age, string gender)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine($"**Szczegółowa analiza BMI:**");
            sb.AppendLine($"Twoje BMI wynosi {bmi:F1}, co klasyfikuje Cię w kategorii: {category}.");
            sb.AppendLine();

            switch (category)
            {
                case "Niedowaga":
                    sb.AppendLine("**Implikacje zdrowotne:**");
                    sb.AppendLine("• Zwiększone ryzyko niedoborów żywieniowych");
                    sb.AppendLine("• Osłabiona odporność organizmu");
                    sb.AppendLine("• Możliwe problemy z gęstością kości");
                    sb.AppendLine("• Ryzyko zaburzeń hormonalnych");
                    break;

                case "Waga prawidłowa":
                    sb.AppendLine("**Gratulacje!** Twoja waga mieści się w zdrowym zakresie.");
                    sb.AppendLine("**Korzyści zdrowotne:**");
                    sb.AppendLine("• Optymalne funkcjonowanie układu sercowo-naczyniowego");
                    sb.AppendLine("• Zmniejszone ryzyko cukrzycy typu 2");
                    sb.AppendLine("• Lepsze samopoczucie i energia");
                    sb.AppendLine("• Zdrowe funkcjonowanie hormonów");
                    break;

                case "Nadwaga":
                    sb.AppendLine("**Implikacje zdrowotne:**");
                    sb.AppendLine("• 2-3x większe ryzyko cukrzycy typu 2");
                    sb.AppendLine("• Zwiększone ciśnienie krwi");
                    sb.AppendLine("• Większe obciążenie stawów");
                    sb.AppendLine("• Ryzyko bezdechu sennego");
                    break;

                case "Otyłość I stopnia":
                    sb.AppendLine("**Poważne implikacje zdrowotne:**");
                    sb.AppendLine("• 5-10x większe ryzyko cukrzycy");
                    sb.AppendLine("• Znacznie zwiększone ryzyko chorób serca");
                    sb.AppendLine("• Problemy z oddychaniem");
                    sb.AppendLine("• Zwiększone ryzyko niektórych nowotworów");
                    break;

                default:
                    sb.AppendLine("**Krytyczne implikacje zdrowotne - wymagana natychmiastowa interwencja medyczna.**");
                    break;
            }

            if (age > 65)
            {
                sb.AppendLine();
                sb.AppendLine("**Uwagi dla osób starszych:**");
                sb.AppendLine("U osób po 65. roku życia lekka nadwaga (BMI 25-27) może być ochronna przed sarkopenia i osteoporozą.");
            }

            return sb.ToString();
        }

        private List<string> GenerateBMIRecommendations(double bmi, double weightDifference, int age, string gender)
        {
            var recommendations = new List<string>();

            if (bmi < 18.5)
            {
                recommendations.Add("Zwiększ spożycie kalorii o 300-500 kcal dziennie");
                recommendations.Add("Skup się na białku wysokiej jakości (1.2-1.6g/kg masy ciała)");
                recommendations.Add("Dodaj trening siłowy 3x w tygodniu");
                recommendations.Add("Rozważ konsultację z dietetykiem");
            }
            else if (bmi > 25)
            {
                var weeklyWeightLoss = Math.Min(1.0, Math.Abs(weightDifference) / 20);
                recommendations.Add($"Cel: redukcja {weeklyWeightLoss:F1} kg tygodniowo");
                recommendations.Add("Stwórz deficyt kaloryczny 500-750 kcal dziennie");
                recommendations.Add("Zwiększ aktywność fizyczną do 150-300 min tygodniowo");
                recommendations.Add("Ogranicz przetworzoną żywność i słodkie napoje");
            }
            else
            {
                recommendations.Add("Utrzymuj obecną wagę przez zrównoważoną dietę");
                recommendations.Add("Kontynuuj regularną aktywność fizyczną");
                recommendations.Add("Monitoruj wagę raz w tygodniu");
            }

            return recommendations;
        }

        // Nutrition Helper Methods
        private int CalculateNutritionScore(double fcvc, double ncp, double ch2o, string favc, string caec, string calc)
        {
            var score = 0;

            // Warzywa (0-25 punktów)
            score += fcvc switch
            {
                >= 3 => 25,
                >= 2 => 20,
                >= 1 => 10,
                _ => 0
            };

            // Posiłki (0-20 punktów)
            score += ncp switch
            {
                3 => 20,
                4 => 15,
                2 => 10,
                _ => 5
            };

            // Woda (0-20 punktów)
            score += ch2o switch
            {
                >= 3 => 20,
                >= 2 => 15,
                >= 1 => 10,
                _ => 0
            };

            // Wysokokaloryczna żywność (0-15 punktów)
            score += favc == "no" ? 15 : 0;

            // Jedzenie między posiłkami (0-10 punktów)
            score += caec switch
            {
                "no" => 10,
                "Sometimes" => 7,
                "Frequently" => 3,
                _ => 0
            };

            // Alkohol (0-10 punktów)
            score += calc switch
            {
                "no" => 10,
                "Sometimes" => 7,
                "Frequently" => 3,
                _ => 0
            };

            return score;
        }

        private string AnalyzeVegetableConsumption(double fcvc)
        {
            return fcvc switch
            {
                >= 3 => "Doskonałe - spożywasz wystarczającą ilość warzyw",
                >= 2 => "Dobre - możesz zwiększyć spożycie warzyw",
                >= 1 => "Niewystarczające - znacznie zwiększ spożycie warzyw",
                _ => "Bardzo niskie - natychmiast wprowadź warzywa do diety"
            };
        }

        private string AnalyzeMealFrequency(double ncp)
        {
            return ncp switch
            {
                3 => "Optymalne - 3 regularne posiłki dziennie",
                4 => "Dobre - 4 posiłki mogą pomóc w kontroli apetytu",
                2 => "Niewystarczające - zbyt mało posiłków może spowolnić metabolizm",
                1 => "Bardzo niskie - jeden posiłek dziennie jest niezdrowy",
                _ => "Zbyt częste jedzenie może prowadzić do przejadania się"
            };
        }

        private string AnalyzeHydration(double ch2o)
        {
            return ch2o switch
            {
                >= 3 => "Doskonałe nawodnienie",
                >= 2 => "Dobre nawodnienie - możesz pić więcej",
                >= 1 => "Niewystarczające - zwiększ spożycie wody",
                _ => "Odwodnienie - natychmiast zwiększ spożycie płynów"
            };
        }

        private string AnalyzeCalorieIntake(string favc, string caec)
        {
            if (favc == "yes" && caec == "Frequently")
                return "Bardzo wysokie - częste spożycie wysokokalorycznej żywności";
            if (favc == "yes")
                return "Wysokie - ograniczenie wysokokalorycznej żywności";
            if (caec == "Frequently")
                return "Umiarkowanie wysokie - częste przekąski";
            return "Kontrolowane - dobre nawyki żywieniowe";
        }

        private string AnalyzeAlcoholConsumption(string calc)
        {
            return calc switch
            {
                "no" => "Brak spożycia - doskonale dla zdrowia",
                "Sometimes" => "Umiarkowane - w granicach zdrowych norm",
                "Frequently" => "Częste - może wpływać na zdrowie",
                "Always" => "Nadmierne - poważne ryzyko zdrowotne",
                _ => "Nieznane"
            };
        }

        private string GenerateNutritionPlan(double fcvc, double ncp, double ch2o, string favc, string caec, string calc)
        {
            var sb = new StringBuilder();
            sb.AppendLine("**Spersonalizowany plan żywieniowy:**");
            sb.AppendLine();

            // Plan posiłków
            sb.AppendLine("**Struktura posiłków:**");
            if (ncp < 3)
            {
                sb.AppendLine("• Śniadanie (25% kalorii): Owsianka z owocami i orzechami");
                sb.AppendLine("• Obiad (40% kalorii): Białko + warzywa + węglowodany złożone");
                sb.AppendLine("• Kolacja (35% kalorii): Lekkie białko + sałatka");
            }
            else
            {
                sb.AppendLine("• Śniadanie (20% kalorii): Jajka z warzywami");
                sb.AppendLine("• Przekąska (10% kalorii): Owoce z orzechami");
                sb.AppendLine("• Obiad (35% kalorii): Pełnowartościowy posiłek");
                sb.AppendLine("• Kolacja (35% kalorii): Białko + warzywa");
            }

            sb.AppendLine();
            sb.AppendLine("**Zalecenia szczegółowe:**");

            if (fcvc < 2)
                sb.AppendLine("• Dodaj 2-3 porcje warzyw do każdego posiłku");

            if (ch2o < 2)
                sb.AppendLine("• Pij szklankę wody przed każdym posiłkiem");

            if (favc == "yes")
                sb.AppendLine("• Zastąp wysokokaloryczne przekąski owocami i orzechami");

            if (calc != "no")
                sb.AppendLine("• Ogranicz alkohol do 1-2 jednostek tygodniowo");

            return sb.ToString();
        }

        private List<string> GenerateSupplementRecommendations(HealthRequest request)
        {
            var supplements = new List<string>();
            var age = Convert.ToInt32(request.DataFrame_Split.Data[0][1]);
            var gender = request.DataFrame_Split.Data[0][9].ToString();
            var fcvc = Convert.ToDouble(request.DataFrame_Split.Data[0][4]);

            // Podstawowe suplementy
            supplements.Add("Witamina D3 (2000-4000 IU dziennie)");
            supplements.Add("Omega-3 (1000mg EPA/DHA dziennie)");

            if (fcvc < 2)
                supplements.Add("Multiwitamina wysokiej jakości");

            if (age > 50)
                supplements.Add("Witamina B12 (500-1000 mcg)");

            if (gender == "Female")
                supplements.Add("Żelazo (jeśli niedobór w badaniach)");

            if (age > 65)
                supplements.Add("Wapń + Witamina K2");

            return supplements;
        }

        // Physical Activity Helper Methods
        private string GetActivityLevel(double faf)
        {
            return faf switch
            {
                0 => "Brak aktywności - siedzący tryb życia",
                1 => "Bardzo niska - sporadyczna aktywność",
                2 => "Niska - lekka aktywność 1-2x w tygodniu",
                3 => "Umiarkowana - regularna aktywność 3x w tygodniu",
                4 => "Wysoka - intensywna aktywność 4-5x w tygodniu",
                _ => "Bardzo wysoka - codzienna intensywna aktywność"
            };
        }

        private string CalculateSedentaryRisk(double tue, string mtrans)
        {
            var riskScore = 0;

            // Czas przy technologii
            riskScore += tue switch
            {
                >= 8 => 3,
                >= 6 => 2,
                >= 4 => 1,
                _ => 0
            };

            // Typ transportu
            riskScore += mtrans switch
            {
                "Automobile" => 2,
                "Public_Transportation" => 1,
                "Bike" => -1,
                "Walking" => -2,
                _ => 0
            };

            return riskScore switch
            {
                >= 4 => "Bardzo wysokie - natychmiastowa interwencja",
                >= 3 => "Wysokie - wymagane zmiany",
                >= 2 => "Umiarkowane - wprowadź więcej ruchu",
                1 => "Niskie - kontynuuj obecne nawyki",
                _ => "Bardzo niskie - doskonały styl życia"
            };
        }

        private int EstimateCaloriesBurned(double faf, HealthRequest request)
        {
            var weight = Convert.ToDouble(request.DataFrame_Split.Data[0][3]);
            var age = Convert.ToInt32(request.DataFrame_Split.Data[0][1]);

            // Bazowy współczynnik spalania kalorii na podstawie wieku i wagi
            var baseCaloriesPerHour = weight * (age > 40 ? 6 : 7);

            // Szacowane godziny aktywności tygodniowo
            var hoursPerWeek = faf * 1.5; // Każdy punkt FAF = ~1.5h aktywności

            return (int)(baseCaloriesPerHour * hoursPerWeek);
        }

        private List<string> GenerateFitnessRecommendations(double faf, double tue, string mtrans, HealthRequest request)
        {
            var recommendations = new List<string>();
            var age = Convert.ToInt32(request.DataFrame_Split.Data[0][1]);

            if (faf < 2)
            {
                recommendations.Add("Zacznij od 10-15 minut spaceru dziennie");
                recommendations.Add("Wprowadź ćwiczenia siłowe 2x w tygodniu");
                recommendations.Add("Użyj schodów zamiast windy");
            }
            else if (faf < 4)
            {
                recommendations.Add("Zwiększ intensywność ćwiczeń");
                recommendations.Add("Dodaj trening interwałowy HIIT 1-2x w tygodniu");
                recommendations.Add("Wprowadź sport zespołowy lub hobby aktywne");
            }

            if (tue > 6)
            {
                recommendations.Add("Co godzinę rób 5-minutową przerwę na ruch");
                recommendations.Add("Rozważ biurko do pracy na stojąco");
                recommendations.Add("Wprowadź ćwiczenia rozciągające w pracy");
            }

            if (age > 50)
            {
                recommendations.Add("Skup się na ćwiczeniach równoważnych");
                recommendations.Add("Wprowadź jogę lub tai chi");
                recommendations.Add("Priorytet: utrzymanie masy mięśniowej");
            }

            return recommendations;
        }

        private string GenerateExercisePlan(double faf, HealthRequest request)
        {
            var sb = new StringBuilder();
            var age = Convert.ToInt32(request.DataFrame_Split.Data[0][1]);
            var height = Convert.ToDouble(request.DataFrame_Split.Data[0][2]);
            var weight = Convert.ToDouble(request.DataFrame_Split.Data[0][3]);

            sb.AppendLine("**Spersonalizowany plan ćwiczeń:**");
            sb.AppendLine();

            if (faf < 2)
            {
                sb.AppendLine("**Faza początkowa (4-6 tygodni):**");
                sb.AppendLine("• Poniedziałek: 20 min spacer + rozciąganie");
                sb.AppendLine("• Środa: Podstawowe ćwiczenia siłowe (15 min)");
                sb.AppendLine("• Piątek: 25 min spacer lub jazda rowerem");
                sb.AppendLine("• Niedziela: Aktywność rekreacyjna (taniec, pływanie)");
            }
            else
            {
                sb.AppendLine("**Plan zaawansowany:**");
                sb.AppendLine("• Poniedziałek: Trening siłowy górna część ciała (45 min)");
                sb.AppendLine("• Wtorek: Cardio HIIT (30 min)");
                sb.AppendLine("• Środa: Trening siłowy dolna część ciała (45 min)");
                sb.AppendLine("• Czwartek: Aktywne regeneracja - joga (30 min)");
                sb.AppendLine("• Piątek: Trening całego ciała (40 min)");
                sb.AppendLine("• Sobota: Długie cardio (60 min)");
                sb.AppendLine("• Niedziela: Odpoczynek lub lekka aktywność");
            }

            sb.AppendLine();
            sb.AppendLine("**Cele tygodniowe:**");
            var targetCalories = weight * 30; // Cel spalania kalorii
            sb.AppendLine($"• Spalenie {targetCalories:F0} kalorii tygodniowo");
            sb.AppendLine($"• Minimum {(faf < 2 ? 150 : 300)} minut aktywności");

            return sb.ToString();
        }

        // Będę kontynuować z pozostałymi metodami...
    }
} 