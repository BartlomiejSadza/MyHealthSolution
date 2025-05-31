# MyHealth Solution 🏥

Aplikacja do oceny stanu zdrowia z wykorzystaniem Machine Learning - kompletne rozwiązanie z frontendem Next.js, API .NET Core i modelem ML.

## 🚀 Quick Start - Deploy na Azure

### 1-klik deployment na Azure

```bash
# Zaloguj się do Azure
az login

# Deploy aplikację na Azure Container Apps
./deploy-azure-fixed.sh
```

**Rezultat**: Działająca aplikacja w chmurze za ~15 minut! 🎉

## 🏗️ Architektura

```
┌─────────────────────────────────────────┐
│          MyHealth Solution              │
├─────────────────────────────────────────┤
│  🌐 Frontend     ⚙️ API     🤖 ML Model │
│  (Next.js)      (.NET)    (Python)     │
│  Port 3000      Port 80    Port 5000   │
└─────────────────────────────────────────┘
```

### Komponenty:

- **Frontend**: Next.js aplikacja z formularzem zdrowotnym
- **API**: .NET Core Web API z zaawansowaną analizą zdrowotną
- **ML Model**: Python Flask z inteligentnym modelem predykcji

## 📋 Wymagania

### Deployment na Azure:

- Azure CLI
- Aktywna subskrypcja Azure
- Docker

### Lokalne uruchomienie:

- .NET 8 SDK
- Node.js 18+
- Python 3.11+

## ☁️ Deployment na Azure

### Automatyczny deployment

```bash
./deploy-azure-fixed.sh
```

### Automatyczny CI/CD z GitHub Actions

Skonfiguruj zgodnie z `CICD-SETUP.md` - po tym każdy `git push` automatycznie deployuje zmiany!

## 🔄 Aktualizacje

### GitHub Actions (Automatyczne)

```bash
git add .
git commit -m "Update feature"
git push origin main
# GitHub Actions automatycznie deployuje! 🎉
```

### Manualne aktualizacje

```bash
# Zbuduj nową wersję API
cd MyHealth.Api
az acr build --registry myhealthfixed1637 --image api:$(date +%Y%m%d-%H%M%S) .

# Wdróż nową wersję
az containerapp update --name myhealth-api --resource-group rg-myhealth-fixed --image myhealthfixed1637.azurecr.io/api:NOWY_TAG
```

## 🤖 ML Model

### Zaawansowana analiza zdrowia:

- **Input**: 17 parametrów zdrowotnych (wiek, wzrost, waga, styl życia)
- **Output**: Kompletna analiza zdrowotna z oceną 0-100 punktów
- **Funkcje**: 
  - Analiza BMI i wagi idealnej
  - Ocena żywienia i aktywności fizycznej
  - Analiza stylu życia i czynników ryzyka
  - Spersonalizowane rekomendacje
  - Plan działania z celami

### Status modelu:

Model używa inteligentnego algorytmu który:

- ✅ Oblicza BMI i wiek metaboliczny
- ✅ Analizuje nawyki żywieniowe
- ✅ Ocenia poziom aktywności fizycznej
- ✅ Identyfikuje czynniki ryzyka
- ✅ Generuje spersonalizowane rekomendacje
- ✅ Działa stabilnie w każdym środowisku

## 💰 Koszty Azure

| Zasób                      | Koszt/miesiąc    |
| -------------------------- | ---------------- |
| Container Apps Environment | $10              |
| ML Model (1GB RAM)         | $15              |
| API (0.5GB RAM)            | $8               |
| Frontend (0.5GB RAM)       | $8               |
| Container Registry         | $5               |
| Networking                 | $2-5             |
| **TOTAL**                  | **~$48/miesiąc** |

## 📚 Dokumentacja

- [`AZURE-DEPLOYMENT.md`](AZURE-DEPLOYMENT.md) - Szczegółowa dokumentacja Azure
- [`CICD-SETUP.md`](CICD-SETUP.md) - Konfiguracja automatycznego CI/CD
- [`GITHUB-SECRETS-SETUP.md`](GITHUB-SECRETS-SETUP.md) - Konfiguracja sekretów GitHub

## 🛠️ Development

### Struktura projektu:

```
MyHealthSolution/
├── health-frontend/          # Next.js frontend
├── MyHealth.Api/            # .NET Core API
├── ml-model/               # Python ML model
├── deploy-azure-fixed.sh   # Azure deployment script
└── .github/workflows/      # GitHub Actions CI/CD
```

### Lokalne uruchomienie dla developmentu:

```bash
# Frontend (Next.js)
cd health-frontend
npm run dev  # http://localhost:3000

# API (.NET)
cd MyHealth.Api
dotnet run   # http://localhost:5001

# ML Model (Python)
cd ml-model
python app.py  # http://localhost:5000
```

## 🎯 Funkcje aplikacji

### Frontend:
- Responsywny formularz zdrowotny
- Wizualizacja wyników analizy
- Interaktywne wykresy i wskaźniki
- Spersonalizowane rekomendacje

### API:
- Zaawansowana analiza zdrowotna
- Ocena BMI i wagi idealnej
- Analiza żywienia (84-punktowa skala)
- Ocena aktywności fizycznej
- Analiza stylu życia (95-punktowa skala)
- Identyfikacja czynników ryzyka
- Generowanie planów działania

### ML Model:
- Predykcja kategorii wagi
- Dummy model z inteligentną logiką
- Stabilne działanie w produkcji
- Szybkie odpowiedzi (< 1s)

## 🤝 Contributing

1. Fork repo
2. Utwórz branch: `git checkout -b feature/amazing-feature`
3. Commit: `git commit -m 'Add amazing feature'`
4. Push: `git push origin feature/amazing-feature`
5. Utwórz Pull Request

GitHub Actions automatycznie przetestuje zmiany!

## 📞 Support

- **Issues**: GitHub Issues
- **Monitoring**: Azure Portal → rg-myhealth-fixed
- **Logi**: `az containerapp logs show --name myhealth-api --resource-group rg-myhealth-fixed`

## 🎉 Status projektu

✅ **Wszystko działa!**
- Frontend: https://myhealth-frontend.happysea-444138bb.eastus.azurecontainerapps.io
- API: https://myhealth-api.happysea-444138bb.eastus.azurecontainerapps.io
- ML Model: Wewnętrzny endpoint
- GitHub Actions: Skonfigurowane i działające

---