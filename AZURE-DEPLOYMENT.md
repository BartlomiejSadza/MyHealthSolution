# 🚀 Deployment MyHealth Solution na Azure

## Wymagania wstępne

### 1. Zainstaluj Azure CLI

```bash
# macOS
brew install azure-cli

# Windows
winget install Microsoft.AzureCLI

# Linux
curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash
```

### 2. Zaloguj się do Azure

```bash
az login
```

### 3. Sprawdź subskrypcję

```bash
az account list --output table
az account set --subscription "Twoja-Subskrypcja-ID"
```

## Opcje deploymentu

### 🎯 Rekomendowane: Azure Container Apps (Automatyczny deployment)

**Koszty**: ~$20-50/miesiąc  
**Czas deploymentu**: ~10-15 minut  
**Poziom trudności**: ⭐⭐☆☆☆

```bash
# Nadaj uprawnienia wykonywania
chmod +x deploy-azure.sh

# Uruchom deployment
./deploy-azure.sh
```

### 📋 Manual deployment krok po kroku

#### 1. Utwórz Resource Group

```bash
az group create --name rg-myhealth --location "West Europe"
```

#### 2. Utwórz Container Registry

```bash
az acr create \
    --resource-group rg-myhealth \
    --name myhealthregistry \
    --sku Basic \
    --admin-enabled true
```

#### 3. Build i push images

```bash
# Login do registry
az acr login --name myhealthregistry

# Build images lokalnie
docker build -t myhealthregistry.azurecr.io/ml-model:latest ./ml-model
docker build -t myhealthregistry.azurecr.io/api:latest ./MyHealth.Api
docker build -t myhealthregistry.azurecr.io/frontend:latest ./health-frontend

# Push do Azure
docker push myhealthregistry.azurecr.io/ml-model:latest
docker push myhealthregistry.azurecr.io/api:latest
docker push myhealthregistry.azurecr.io/frontend:latest
```

#### 4. Utwórz Container Apps Environment

```bash
az containerapp env create \
    --name myhealth-env \
    --resource-group rg-myhealth \
    --location "West Europe"
```

#### 5. Deploy aplikacje

**ML Model (internal)**:

```bash
az containerapp create \
    --name myhealth-ml-model \
    --resource-group rg-myhealth \
    --environment myhealth-env \
    --image myhealthregistry.azurecr.io/ml-model:latest \
    --registry-server myhealthregistry.azurecr.io \
    --target-port 5000 \
    --ingress internal \
    --memory 1.0Gi \
    --cpu 0.5
```

**API (external)**:

```bash
az containerapp create \
    --name myhealth-api \
    --resource-group rg-myhealth \
    --environment myhealth-env \
    --image myhealthregistry.azurecr.io/api:latest \
    --registry-server myhealthregistry.azurecr.io \
    --target-port 80 \
    --ingress external \
    --memory 0.5Gi \
    --cpu 0.25 \
    --env-vars ASPNETCORE_ENVIRONMENT=Production
```

**Frontend (external)**:

```bash
# Najpierw pobierz URL API
API_URL=$(az containerapp show --name myhealth-api --resource-group rg-myhealth --query "properties.configuration.ingress.fqdn" --output tsv)

az containerapp create \
    --name myhealth-frontend \
    --resource-group rg-myhealth \
    --environment myhealth-env \
    --image myhealthregistry.azurecr.io/frontend:latest \
    --registry-server myhealthregistry.azurecr.io \
    --target-port 3000 \
    --ingress external \
    --memory 0.5Gi \
    --cpu 0.25 \
    --env-vars NODE_ENV=production NEXT_PUBLIC_API_URL=https://$API_URL/api/health/assess-simple
```

## Po deploymencie

### Sprawdź status aplikacji

```bash
az containerapp list --resource-group rg-myhealth --output table
```

### Pobierz URLs

```bash
# Frontend URL
az containerapp show --name myhealth-frontend --resource-group rg-myhealth --query "properties.configuration.ingress.fqdn" --output tsv

# API URL
az containerapp show --name myhealth-api --resource-group rg-myhealth --query "properties.configuration.ingress.fqdn" --output tsv
```

### Sprawdź logi

```bash
# Frontend logi
az containerapp logs show --name myhealth-frontend --resource-group rg-myhealth

# API logi
az containerapp logs show --name myhealth-api --resource-group rg-myhealth

# ML Model logi
az containerapp logs show --name myhealth-ml-model --resource-group rg-myhealth
```

## Skalowanie i monitoring

### Automatyczne skalowanie

```bash
az containerapp update \
    --name myhealth-api \
    --resource-group rg-myhealth \
    --min-replicas 1 \
    --max-replicas 10
```

### Monitoring w Azure Portal

1. Otwórz [Azure Portal](https://portal.azure.com)
2. Przejdź do Resource Group `rg-myhealth`
3. Sprawdź Container Apps
4. Monitoruj metryki CPU/Memory/Requests

## Aktualizacja aplikacji

### Rebuild i redeploy

```bash
# Build nową wersję
docker build -t myhealthregistry.azurecr.io/frontend:v2 ./health-frontend
docker push myhealthregistry.azurecr.io/frontend:v2

# Update container app
az containerapp update \
    --name myhealth-frontend \
    --resource-group rg-myhealth \
    --image myhealthregistry.azurecr.io/frontend:v2
```

## Alternatywne opcje deploymentu

### 🐳 Azure Container Instances (ACI)

- Prostszy, ale mniej funkcjonalny
- Bezpośredni docker-compose deployment
- Koszty: ~$10-30/miesiąc

### 🌐 Azure App Service (Multi-container)

- Dedykowane dla web apps
- Integracja z CI/CD
- Koszty: ~$50-100/miesiąc

### ⚙️ Azure Kubernetes Service (AKS)

- Największa kontrola
- Wymaga knowledge Kubernetes
- Koszty: ~$100+/miesiąc

## Szacowane koszty (West Europe)

| Zasób                          | Rozmiar           | Koszt/miesiąc    |
| ------------------------------ | ----------------- | ---------------- |
| Container Apps Environment     | Standard          | $10              |
| ML Model (1GB RAM, 0.5 CPU)    | 24/7              | $15              |
| API (0.5GB RAM, 0.25 CPU)      | 24/7              | $8               |
| Frontend (0.5GB RAM, 0.25 CPU) | 24/7              | $8               |
| Container Registry             | Basic             | $5               |
| Networking                     | Outbound transfer | $2-5             |
| **TOTAL**                      |                   | **~$48/miesiąc** |

## Troubleshooting

### Container nie startuje

```bash
# Sprawdź logi szczegółowe
az containerapp logs show --name [APP_NAME] --resource-group rg-myhealth --follow

# Sprawdź health probes
az containerapp show --name [APP_NAME] --resource-group rg-myhealth
```

### Problemy z obrazami

```bash
# Sprawdź images w registry
az acr repository list --name myhealthregistry

# Re-build i push
docker build --no-cache -t [IMAGE_NAME] .
```

### Networking issues

- ML Model musi być `internal` (dostępny tylko wewnętrznie)
- API i Frontend muszą być `external` (dostępne publicznie)
- Sprawdź environment variables dla URL connections

## Usuwanie zasobów

```bash
# Usuń całą grupę zasobów (UWAGA: usuwa wszystko!)
az group delete --name rg-myhealth --yes --no-wait
```

---

## 🎯 Quick Start Summary

1. **Przygotowanie**: `az login`
2. **Deployment**: `chmod +x deploy-azure.sh && ./deploy-azure.sh`
3. **Sprawdzenie**: Otwórz URL z outputu
4. **Monitoring**: Azure Portal → rg-myhealth

**Czas total**: ~15 minut  
**Rezultat**: Działająca aplikacja w chmurze Azure 🎉
