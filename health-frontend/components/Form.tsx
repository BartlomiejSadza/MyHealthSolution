import { useState, type FormEvent } from "react";

export type Features = {
  id: number;
  Age: number;
  Height: number;
  Weight: number;
  FCVC: number;
  NCP: number;
  CH2O: number;
  FAF: number;
  TUE: number;
  Gender: "Male" | "Female";
  family_history_with_overweight: "yes" | "no";
  FAVC: "yes" | "no";
  CAEC: "Never" | "Sometimes" | "Frequently" | "Always";
  SMOKE: "yes" | "no";
  SCC: "yes" | "no";
  CALC: "Never" | "Sometimes" | "Frequently" | "Always";
  MTRANS:
    | "Automobile"
    | "Motorbike"
    | "Bike"
    | "Public_Transportation"
    | "Walking";
};

type Props = {
  onSubmit: (f: Features) => void;
  loading: boolean;
};

const initial: Features = {
  id: 1,
  Age: 30,
  Height: 1.7,
  Weight: 70,
  FCVC: 2,
  NCP: 2,
  CH2O: 2,
  FAF: 1,
  TUE: 1,
  Gender: "Male",
  family_history_with_overweight: "no",
  FAVC: "no",
  CAEC: "Sometimes",
  SMOKE: "no",
  SCC: "no",
  CALC: "Sometimes",
  MTRANS: "Public_Transportation",
};

export default function Form({ onSubmit, loading }: Props) {
  const [f, setF] = useState<Features>(initial);

  const handleChange =
    <K extends keyof Features>(key: K) =>
    (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
      const val = e.target.value;
      setF((prev) => ({
        ...prev,
        [key]:
          typeof prev[key] === "number"
            ? Number.parseFloat(val)
            : (val as Features[K]),
      }));
    };

  const submit = (e: FormEvent) => {
    e.preventDefault();
    onSubmit(f);
  };

  return (
    <div className="max-w-2xl mx-auto p-6 bg-white rounded-lg shadow-lg">
      <h2 className="text-2xl font-bold mb-6 text-gray-800">Formularz Zdrowotny</h2>
      <form onSubmit={submit} className="space-y-6">
        
        {/* Sekcja: Dane podstawowe */}
        <div className="border-b pb-6">
          <h3 className="text-lg font-semibold mb-4 text-gray-700">Dane podstawowe</h3>
          
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label htmlFor="Age" className="block text-sm font-medium text-gray-700 mb-1">
                Wiek (lata):
              </label>
              <input
                id="Age"
                type="number"
                min="1"
                max="120"
                value={f.Age}
                onChange={handleChange("Age")}
                className="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                required
              />
            </div>

            <div>
              <label htmlFor="Gender" className="block text-sm font-medium text-gray-700 mb-1">
                Płeć:
              </label>
              <select
                id="Gender"
                value={f.Gender}
                onChange={handleChange("Gender")}
                className="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                required
              >
                <option value="Male">Mężczyzna</option>
                <option value="Female">Kobieta</option>
              </select>
            </div>

            <div>
              <label htmlFor="Height" className="block text-sm font-medium text-gray-700 mb-1">
                Wzrost (m):
              </label>
              <input
                id="Height"
                type="number"
                step="0.01"
                min="0.5"
                max="2.5"
                value={f.Height}
                onChange={handleChange("Height")}
                className="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                required
              />
            </div>

            <div>
              <label htmlFor="Weight" className="block text-sm font-medium text-gray-700 mb-1">
                Waga (kg):
              </label>
              <input
                id="Weight"
                type="number"
                step="0.1"
                min="20"
                max="300"
                value={f.Weight}
                onChange={handleChange("Weight")}
                className="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                required
              />
            </div>
          </div>
        </div>

        {/* Sekcja: Nawyki żywieniowe */}
        <div className="border-b pb-6">
          <h3 className="text-lg font-semibold mb-4 text-gray-700">Nawyki żywieniowe</h3>
          
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label htmlFor="FCVC" className="block text-sm font-medium text-gray-700 mb-1">
                Częstotliwość spożycia warzyw (1-3):
              </label>
              <input
                id="FCVC"
                type="number"
                step="0.1"
                min="1"
                max="3"
                value={f.FCVC}
                onChange={handleChange("FCVC")}
                className="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                required
              />
            </div>

            <div>
              <label htmlFor="NCP" className="block text-sm font-medium text-gray-700 mb-1">
                Liczba głównych posiłków dziennie:
              </label>
              <input
                id="NCP"
                type="number"
                step="0.1"
                min="1"
                max="5"
                value={f.NCP}
                onChange={handleChange("NCP")}
                className="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                required
              />
            </div>

            <div>
              <label htmlFor="CH2O" className="block text-sm font-medium text-gray-700 mb-1">
                Spożycie wody dziennie (litry):
              </label>
              <input
                id="CH2O"
                type="number"
                step="0.1"
                min="0.5"
                max="5"
                value={f.CH2O}
                onChange={handleChange("CH2O")}
                className="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                required
              />
            </div>

            <div>
              <label htmlFor="FAVC" className="block text-sm font-medium text-gray-700 mb-1">
                Czy spożywasz często wysokokaloryczne jedzenie?
              </label>
              <select
                id="FAVC"
                value={f.FAVC}
                onChange={handleChange("FAVC")}
                className="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                required
              >
                <option value="no">Nie</option>
                <option value="yes">Tak</option>
              </select>
            </div>

            <div>
              <label htmlFor="CAEC" className="block text-sm font-medium text-gray-700 mb-1">
                Jak często jesz między posiłkami?
              </label>
              <select
                id="CAEC"
                value={f.CAEC}
                onChange={handleChange("CAEC")}
                className="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                required
              >
                <option value="Never">Nigdy</option>
                <option value="Sometimes">Czasami</option>
                <option value="Frequently">Często</option>
                <option value="Always">Zawsze</option>
              </select>
            </div>

            <div>
              <label htmlFor="CALC" className="block text-sm font-medium text-gray-700 mb-1">
                Jak często spożywasz alkohol?
              </label>
              <select
                id="CALC"
                value={f.CALC}
                onChange={handleChange("CALC")}
                className="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                required
              >
                <option value="Never">Nigdy</option>
                <option value="Sometimes">Czasami</option>
                <option value="Frequently">Często</option>
                <option value="Always">Zawsze</option>
              </select>
            </div>
          </div>
        </div>

        {/* Sekcja: Aktywność fizyczna i styl życia */}
        <div className="border-b pb-6">
          <h3 className="text-lg font-semibold mb-4 text-gray-700">Aktywność fizyczna i styl życia</h3>
          
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label htmlFor="FAF" className="block text-sm font-medium text-gray-700 mb-1">
                Częstotliwość aktywności fizycznej (0-3):
              </label>
              <input
                id="FAF"
                type="number"
                step="0.1"
                min="0"
                max="3"
                value={f.FAF}
                onChange={handleChange("FAF")}
                className="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                required
              />
            </div>

            <div>
              <label htmlFor="TUE" className="block text-sm font-medium text-gray-700 mb-1">
                Czas korzystania z urządzeń technologicznych (godziny dziennie):
              </label>
              <input
                id="TUE"
                type="number"
                step="0.1"
                min="0"
                max="24"
                value={f.TUE}
                onChange={handleChange("TUE")}
                className="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                required
              />
            </div>

            <div>
              <label htmlFor="MTRANS" className="block text-sm font-medium text-gray-700 mb-1">
                Główny środek transportu:
              </label>
              <select
                id="MTRANS"
                value={f.MTRANS}
                onChange={handleChange("MTRANS")}
                className="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                required
              >
                <option value="Walking">Chodzenie pieszo</option>
                <option value="Bike">Rower</option>
                <option value="Public_Transportation">Transport publiczny</option>
                <option value="Automobile">Samochód</option>
                <option value="Motorbike">Motocykl</option>
              </select>
            </div>
          </div>
        </div>

        {/* Sekcja: Historia zdrowia i nawyki */}
        <div className="pb-6">
          <h3 className="text-lg font-semibold mb-4 text-gray-700">Historia zdrowia i nawyki</h3>
          
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <label htmlFor="family_history_with_overweight" className="block text-sm font-medium text-gray-700 mb-1">
                Historia nadwagi w rodzinie:
              </label>
              <select
                id="family_history_with_overweight"
                value={f.family_history_with_overweight}
                onChange={handleChange("family_history_with_overweight")}
                className="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                required
              >
                <option value="no">Nie</option>
                <option value="yes">Tak</option>
              </select>
            </div>

            <div>
              <label htmlFor="SMOKE" className="block text-sm font-medium text-gray-700 mb-1">
                Czy palisz papierosy?
              </label>
              <select
                id="SMOKE"
                value={f.SMOKE}
                onChange={handleChange("SMOKE")}
                className="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                required
              >
                <option value="no">Nie</option>
                <option value="yes">Tak</option>
              </select>
            </div>

            <div>
              <label htmlFor="SCC" className="block text-sm font-medium text-gray-700 mb-1">
                Czy monitorujesz spożycie kalorii?
              </label>
              <select
                id="SCC"
                value={f.SCC}
                onChange={handleChange("SCC")}
                className="w-full border border-gray-300 rounded-md px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
                required
              >
                <option value="no">Nie</option>
                <option value="yes">Tak</option>
              </select>
            </div>
          </div>
        </div>

        <button
          type="submit"
          disabled={loading}
          className="w-full bg-blue-600 hover:bg-blue-700 disabled:bg-blue-400 text-white font-medium py-3 px-6 rounded-md transition duration-200 ease-in-out transform hover:scale-105 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
        >
          {loading ? "Przetwarzanie..." : "Wyślij formularz"}
        </button>
      </form>
    </div>
  );
}
