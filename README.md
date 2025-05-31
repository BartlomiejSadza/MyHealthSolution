# MyHealth Solution 🏥

Aplikacja do oceny stanu zdrowia z wykorzystaniem Machine Learning - kompletne rozwiązanie z frontendem Next.js, API .NET Core i lokalnym modelem ML.

## 🚀 Quick Start - Deploy na Azure

### 1-klik deployment na Azure

```bash
# Zaloguj się do Azure
az login

# Deploy aplikację na Azure Container Apps
./deploy-azure.sh
```

**Rezultat**: Działająca aplikacja w chmurze za ~15 minut! 🎉

### Aktualizacja po zmianach

```bash
# Aktualizuj tylko frontend po zmianach
./update-azure.sh frontend

# Aktualizuj tylko API
./update-azure.sh api

# Aktualizuj wszystko
./update-azure.sh all
```

## 🏗️ Architektura

```
┌─────────────────────────────────────────┐
│          MyHealth Solution              │
├─────────────────────────────────────────┤
│  🌐 Frontend     ⚙️ API     🤖 ML Model │
│  (Next.js)      (.NET)    (Python)     │
│  Port 3001      Port 5144  Port 5002   │
└─────────────────────────────────────────┘
```

### Komponenty:

- **Frontend**: Next.js aplikacja z formularzem zdrowotnym
- **API**: .NET Core Web API z endpointami zdrowotnymi
- **ML Model**: Python Flask z inteligentnym modelem predykcji

## 📋 Wymagania

### Lokalne uruchomienie:

- Docker & Docker Compose
- .NET 8 SDK (opcjonalnie)
- Node.js 18+ (opcjonalnie)

### Deployment na Azure:

- Azure CLI
- Aktywna subskrypcja Azure
- Docker

## 🔧 Lokalne uruchomienie

```bash
# Sklonuj repo
git clone <repo-url>
cd MyHealthSolution

# Uruchom wszystko w Docker
docker compose up -d

# Aplikacja dostępna na:
# Frontend: http://localhost:3001
# API: http://localhost:5144/swagger
# ML Model: http://localhost:5002/health
```

## ☁️ Deployment na Azure

### Opcja 1: Automatyczny deployment

```bash
./deploy-azure.sh
```

### Opcja 2: Automatyczny CI/CD z GitHub Actions

Skonfiguruj zgodnie z `CICD-SETUP.md` - po tym każdy `git push` automatycznie deployuje zmiany!

### Opcja 3: Manual deployment

Zobacz szczegóły w `AZURE-DEPLOYMENT.md`

## 🔄 Aktualizacje

### Po wprowadzeniu zmian:

#### Opcja 1: Semi-automatyczna

```bash
./update-azure.sh frontend  # Tylko frontend (najczęściej)
./update-azure.sh api       # Tylko API
./update-azure.sh all       # Wszystkie komponenty
```

#### Opcja 2: Pełna automatyzacja (CI/CD)

```bash
git add .
git commit -m "Update styling"
git push origin main
# GitHub Actions automatycznie deployuje! 🎉
```

## 🤖 ML Model

### Predykcja zdrowia:

- **Input**: 17 parametrów zdrowotnych (wiek, wzrost, waga, styl życia)
- **Output**: Klasyfikacja BMI (Normal_Weight, Overweight, Obesity, itp.)
- **Model**: Inteligentny algorytm BMI z uwzględnieniem wieku i aktywności

### Status oryginalnego modelu Databricks:

Model został wymigrowany z Databricks Cloud na lokalny deployment. Ze względu na problemy kompatybilności z environment Databricks, używamy inteligentnego dummy modelu który:

- ✅ Oblicza BMI prawidłowo
- ✅ Uwzględnia wiek i poziom aktywności
- ✅ Działa stabilnie w każdym środowisku
- ✅ Daje sensowne predykcje zdrowotne

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

- [`QUICK-DEPLOY.md`](QUICK-DEPLOY.md) - Szybki start deployment
- [`AZURE-DEPLOYMENT.md`](AZURE-DEPLOYMENT.md) - Szczegółowa dokumentacja Azure
- [`CICD-SETUP.md`](CICD-SETUP.md) - Konfiguracja automatycznego CI/CD
- [`deployment-guide.md`](deployment-guide.md) - Oryginalny przewodnik

## 🛠️ Development

### Struktura projektu:

```
MyHealthSolution/
├── health-frontend/          # Next.js frontend
├── MyHealth.Api/            # .NET Core API
├── ml-model/               # Python ML model
├── deploy-azure.sh         # 1-klik Azure deployment
├── update-azure.sh         # Szybkie aktualizacje
└── .github/workflows/      # GitHub Actions CI/CD
```

### Lokalne uruchomienie dla developmentu:

```bash
# Frontend (Next.js)
cd health-frontend
npm run dev  # http://localhost:3000

# API (.NET)
cd MyHealth.Api
dotnet run   # http://localhost:5144

# ML Model (Python)
cd ml-model
python app.py  # http://localhost:5000
```

## 🤝 Contributing

1. Fork repo
2. Utwórz branch: `git checkout -b feature/amazing-feature`
3. Commit: `git commit -m 'Add amazing feature'`
4. Push: `git push origin feature/amazing-feature`
5. Utwórz Pull Request

GitHub Actions automatycznie przetestuje zmiany!

## 📞 Support

- **Issues**: GitHub Issues
- **Monitoring**: Azure Portal → rg-myhealth
- **Logi**: `az containerapp logs show --name myhealth-frontend --resource-group rg-myhealth`

---

## 🎯 TL;DR - Quick Commands

```bash
# 🚀 Deploy na Azure
./deploy-azure.sh

# 🔄 Aktualizuj frontend
./update-azure.sh frontend

# 🧪 Test lokalnie
./test-before-deploy.sh

# 🔍 Sprawdź logi
az containerapp logs show --name myhealth-frontend --resource-group rg-myhealth --follow

# 🗑️ Usuń z Azure
az group delete --name rg-myhealth --yes --no-wait
```

**Aplikacja gotowa do produkcji w 15 minut!** ⚡
# Test
