"use client";
import { useState } from "react";
import Form, { type Features } from "../components/Form";
import Results from "../components/Results";
import type { AssessmentItem } from "../types";

export default function Home() {
  const [loading, setLoading] = useState(false);
  const [results, setResults] = useState<AssessmentItem[] | null>(null);
  const apiUrl = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5144/api/health/assess";

  const handleSubmit = async (f: Features) => {
    setLoading(true);
    setResults(null);
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
      
      const res = await fetch(apiUrl, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload),
      });
      if (!res.ok) throw new Error(await res.text());
      const json = await res.json();
      setResults(json.assessments);
    } catch (e) {
      alert(`Błąd: ${e}`);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="max-w-4xl mx-auto px-4">
        <div className="text-center mb-8">
          <h1 className="text-4xl font-bold text-gray-900 mb-2">MyHealth Assistant</h1>
          <p className="text-lg text-gray-600">Ocena stanu zdrowia na podstawie stylu życia</p>
        </div>
        
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
          <div>
            <Form onSubmit={handleSubmit} loading={loading} />
          </div>
          
          <div>
            {results && (
              <div className="bg-white rounded-lg shadow-lg p-6">
                <h2 className="text-2xl font-bold mb-4 text-gray-800">Wyniki analizy</h2>
                <Results items={results} />
              </div>
            )}
            
            {loading && (
              <div className="bg-white rounded-lg shadow-lg p-6">
                <div className="text-center">
                  <div className="animate-spin rounded-full h-16 w-16 border-b-2 border-blue-600 mx-auto mb-4" />
                  <p className="text-gray-600">Analizuję twoje dane zdrowotne...</p>
                </div>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
