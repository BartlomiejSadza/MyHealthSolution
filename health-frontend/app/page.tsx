"use client";
import { useState } from "react";
import Form, { type Features } from "../components/Form";
import Results from "../components/Results";
import type { AssessmentItem } from "../types";

interface AdvancedHealthResult {
  prediction: string;
  personalProfile: {
    age: number;
    gender: string;
    height: number;
    weight: number;
    bmi: number;
    ageGroup: string;
    bmiCategory: string;
    metabolicAge: number;
  };
  healthScore: number;
  detailedDescription: string;
  recommendations: Array<{
    category: string;
    description: string;
    priority: number;
    timeframe: string;
    expectedBenefit: string;
  }>;
}

export default function Home() {
  const [loading, setLoading] = useState(false);
  const [results, setResults] = useState<AssessmentItem[] | null>(null);
  const [advancedResults, setAdvancedResults] = useState<AdvancedHealthResult | null>(null);
  
  // Używamy bezpośrednio Azure URL z nowym endpointem
  const apiUrl = "https://myhealth-api.happysea-444138bb.eastus.azurecontainerapps.io/api/health/assess-advanced";

  const handleSubmit = async (f: Features) => {
    setLoading(true);
    setResults(null);
    setAdvancedResults(null);
    
    try {
      // Ustawiamy kolumny w poprawnej kolejności zgodnej z API
      const columns = [
        "id",
        "Age",
        "Height", 
        "Weight",
        "FCVC",
        "NCP",
        "CH2O",
        "FAF",
        "TUE",
        "Gender",
        "family_history_with_overweight",
        "FAVC",
        "CAEC",
        "SMOKE",
        "SCC",
        "CALC",
        "MTRANS"
      ];
      
      // Tworzymy dane w tej samej kolejności
      const data = [
        f.id,
        f.Age,
        f.Height,
        f.Weight,
        f.FCVC,
        f.NCP,
        f.CH2O,
        f.FAF,
        f.TUE,
        f.Gender,
        f.family_history_with_overweight,
        f.FAVC,
        f.CAEC,
        f.SMOKE,
        f.SCC,
        f.CALC,
        f.MTRANS
      ];

      const payload = {
        dataframe_split: {
          columns: columns,
          data: [data],
        },
      };
      
      console.log("🚀 Wysyłam dane do zaawansowanej analizy:", payload);
      
      const res = await fetch(apiUrl, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      });
      
      if (!res.ok) {
        const errorText = await res.text();
        throw new Error(`HTTP ${res.status}: ${errorText}`);
      }
      
      const json = await res.json();
      console.log("✅ Otrzymano zaawansowane wyniki:", json);
      
      setAdvancedResults(json);
      
      // Dla kompatybilności z istniejącym komponentem Results
      setResults([{
        prediction: json.prediction || "Brak predykcji",
        description: json.personalProfile?.bmiCategory || "Analiza",
        observations: [`BMI: ${json.personalProfile?.bmi || 'N/A'}`, `Wiek metaboliczny: ${json.personalProfile?.metabolicAge || 'N/A'} lat`]
      }]);
      
    } catch (e) {
      console.error("❌ Fetch error:", e);
      alert(`Błąd podczas analizy: ${e}`);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="max-w-6xl mx-auto px-4">
        <div className="text-center mb-8">
          <h1 className="text-4xl font-bold text-gray-900 mb-2">MyHealth Assistant</h1>
          <p className="text-lg text-gray-600">Kompleksowa analiza zdrowia na podstawie stylu życia</p>
        </div>
        
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
          <div>
            <Form onSubmit={handleSubmit} loading={loading} />
          </div>
          
          <div className="space-y-6">
            {advancedResults && (
              <>
                {/* Profil osobisty */}
                <div className="bg-white rounded-lg shadow-lg p-6">
                  <h2 className="text-2xl font-bold mb-4 text-gray-800 flex items-center">
                    👤 Profil osobisty
                  </h2>
                  <div className="grid grid-cols-2 gap-4 text-sm">
                    <div><strong>Wiek:</strong> {advancedResults.personalProfile?.age} lat</div>
                    <div><strong>Płeć:</strong> {advancedResults.personalProfile?.gender === 'Male' ? 'Mężczyzna' : 'Kobieta'}</div>
                    <div><strong>Wzrost:</strong> {advancedResults.personalProfile?.height?.toFixed(1)} m</div>
                    <div><strong>Waga:</strong> {advancedResults.personalProfile?.weight?.toFixed(1)} kg</div>
                    <div><strong>BMI:</strong> {advancedResults.personalProfile?.bmi?.toFixed(1)}</div>
                    <div><strong>Kategoria:</strong> {advancedResults.personalProfile?.bmiCategory}</div>
                  </div>
                  <div className="mt-4 p-3 bg-blue-50 rounded-lg">
                    <div className="text-lg font-semibold text-blue-800">
                      Ocena zdrowia: {advancedResults.healthScore}/100 punktów
                    </div>
                    <div className="text-sm text-blue-600 mt-1">
                      Wiek metaboliczny: ~{advancedResults.personalProfile?.metabolicAge} lat
                    </div>
                  </div>
                </div>

                {/* Główne rekomendacje */}
                {advancedResults.recommendations && advancedResults.recommendations.length > 0 && (
                  <div className="bg-white rounded-lg shadow-lg p-6">
                    <h2 className="text-2xl font-bold mb-4 text-gray-800 flex items-center">
                      💡 Główne rekomendacje
                    </h2>
                    <div className="space-y-3">
                      {advancedResults.recommendations.slice(0, 5).map((rec, idx) => (
                        <div key={idx} className="border-l-4 border-blue-500 pl-4 py-2">
                          <div className="font-semibold text-gray-800">{rec.category}</div>
                          <div className="text-sm text-gray-600 mt-1">{rec.description}</div>
                          <div className="text-xs text-blue-600 mt-1">
                            Czas: {rec.timeframe} | Korzyść: {rec.expectedBenefit}
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>
                )}

                {/* Szczegółowy opis */}
                <div className="bg-white rounded-lg shadow-lg p-6">
                  <h2 className="text-2xl font-bold mb-4 text-gray-800 flex items-center">
                    📋 Szczegółowa analiza
                  </h2>
                  <div className="prose prose-sm max-w-none">
                    <pre className="whitespace-pre-wrap text-sm text-gray-700 font-sans leading-relaxed">
                      {advancedResults.detailedDescription}
                    </pre>
                  </div>
                </div>
              </>
            )}

            {/* Podstawowe wyniki (dla kompatybilności) */}
            {results && !advancedResults && (
              <div className="bg-white rounded-lg shadow-lg p-6">
                <h2 className="text-2xl font-bold mb-4 text-gray-800">Wyniki analizy</h2>
                <Results items={results} />
              </div>
            )}
            
            {loading && (
              <div className="bg-white rounded-lg shadow-lg p-6">
                <div className="text-center">
                  <div className="animate-spin rounded-full h-16 w-16 border-b-2 border-blue-600 mx-auto mb-4" />
                  <p className="text-gray-600">Przeprowadzam kompleksową analizę zdrowotną...</p>
                  <p className="text-sm text-gray-500 mt-2">To może potrwać kilka sekund</p>
                </div>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
