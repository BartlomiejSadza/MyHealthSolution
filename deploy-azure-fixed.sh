#!/bin/bash

# MyHealth Solution - Deploy na Azure Container Apps (FIXED)
set -e

echo "🚀 Deployment MyHealth Solution na Azure (z poprawką platformy)..."

# Zmienne konfiguracyjne
RESOURCE_GROUP="rg-myhealth-fixed"
LOCATION="East US"  # Działa!
CONTAINER_APP_ENV="myhealth-env"
REGISTRY_NAME="myhealthfixed$(date +%H%M)"
APP_NAME="myhealth"

echo "📍 Region: $LOCATION"
echo "📦 Resource Group: $RESOURCE_GROUP"
echo "🏗️ Registry: $REGISTRY_NAME"

# Sprawdź czy Azure CLI jest zalogowane
if ! az account show > /dev/null 2>&1; then
    echo "❌ Musisz się zalogować do Azure CLI: az login"
    exit 1
fi

echo "✅ Azure CLI zalogowane"

# 1. Usuwamy poprzedni test jeśli istnieje
echo "🧹 Czyszczę poprzedni test..."
az group delete --name rg-myhealth-test --yes --no-wait 2>/dev/null || true

# 2. Tworzenie Resource Group
echo "📦 Tworzę Resource Group..."
az group create \
    --name $RESOURCE_GROUP \
    --location "$LOCATION"

# 3. Tworzenie Azure Container Registry
echo "🏗️ Tworzę Azure Container Registry..."
az acr create \
    --resource-group $RESOURCE_GROUP \
    --name $REGISTRY_NAME \
    --sku Basic \
    --admin-enabled true

# Pobierz credentials do ACR
ACR_SERVER=$(az acr show --name $REGISTRY_NAME --resource-group $RESOURCE_GROUP --query "loginServer" --output tsv)
ACR_USERNAME=$(az acr credential show --name $REGISTRY_NAME --resource-group $RESOURCE_GROUP --query "username" --output tsv)
ACR_PASSWORD=$(az acr credential show --name $REGISTRY_NAME --resource-group $RESOURCE_GROUP --query "passwords[0].value" --output tsv)

echo "📋 ACR Details:"
echo "Server: $ACR_SERVER"
echo "Username: $ACR_USERNAME"

# 4. Build i push images do ACR z poprawną platformą
echo "🔨 Buduję obrazy dla linux/amd64..."

# Login do ACR
az acr login --name $REGISTRY_NAME

echo "Building ML Model dla linux/amd64..."
docker buildx build --platform linux/amd64 -t $ACR_SERVER/ml-model:latest ./ml-model --push

echo "Building API dla linux/amd64..."
docker buildx build --platform linux/amd64 -t $ACR_SERVER/api:latest ./MyHealth.Api --push

echo "Building Frontend dla linux/amd64..."
docker buildx build --platform linux/amd64 -t $ACR_SERVER/frontend:latest ./health-frontend --push

# 5. Tworzenie Container Apps Environment
echo "🌍 Tworzę Container Apps Environment..."
az containerapp env create \
    --name $CONTAINER_APP_ENV \
    --resource-group $RESOURCE_GROUP \
    --location "$LOCATION"

# 6. Deploy ML Model (FIRST!)
echo "🤖 Deployuję ML Model..."
az containerapp create \
    --name myhealth-ml-model \
    --resource-group $RESOURCE_GROUP \
    --environment $CONTAINER_APP_ENV \
    --image $ACR_SERVER/ml-model:latest \
    --registry-server $ACR_SERVER \
    --registry-username $ACR_USERNAME \
    --registry-password $ACR_PASSWORD \
    --target-port 5000 \
    --ingress internal \
    --memory 1.0Gi \
    --cpu 0.5

# Pobierz ML Model URL (internal FQDN)
echo "⏳ Czekam na URL ML modelu..."
sleep 30
ML_MODEL_URL=$(az containerapp show --name myhealth-ml-model --resource-group $RESOURCE_GROUP --query "properties.configuration.ingress.fqdn" --output tsv)
echo "🔗 ML Model URL: $ML_MODEL_URL"

# 7. Deploy API z poprawnym ML Model URL
echo "⚙️ Deployuję API..."
az containerapp create \
    --name myhealth-api \
    --resource-group $RESOURCE_GROUP \
    --environment $CONTAINER_APP_ENV \
    --image $ACR_SERVER/api:latest \
    --registry-server $ACR_SERVER \
    --registry-username $ACR_USERNAME \
    --registry-password $ACR_PASSWORD \
    --target-port 80 \
    --ingress external \
    --memory 0.5Gi \
    --cpu 0.25 \
    --env-vars ASPNETCORE_ENVIRONMENT=Production ASPNETCORE_URLS=http://+:80 ML_MODEL_URL=https://$ML_MODEL_URL

# Pobierz URL API
echo "⏳ Czekam na URL API..."
sleep 30
API_URL=$(az containerapp show --name myhealth-api --resource-group $RESOURCE_GROUP --query "properties.configuration.ingress.fqdn" --output tsv)
echo "🔗 API URL: $API_URL"

# 8. Deploy Frontend z prawidłowym API URL
echo "🌐 Deployuję Frontend..."
az containerapp create \
    --name myhealth-frontend \
    --resource-group $RESOURCE_GROUP \
    --environment $CONTAINER_APP_ENV \
    --image $ACR_SERVER/frontend:latest \
    --registry-server $ACR_SERVER \
    --registry-username $ACR_USERNAME \
    --registry-password $ACR_PASSWORD \
    --target-port 3000 \
    --ingress external \
    --memory 0.5Gi \
    --cpu 0.25 \
    --env-vars NODE_ENV=production NEXT_PUBLIC_API_URL=https://$API_URL/api/health/assess-simple

# 9. Pokaż wyniki
echo ""
echo "🎉 DEPLOYMENT ZAKOŃCZONY POMYŚLNIE!"
echo ""
echo "📋 URLs aplikacji:"
FRONTEND_URL=$(az containerapp show --name myhealth-frontend --resource-group $RESOURCE_GROUP --query "properties.configuration.ingress.fqdn" --output tsv)
echo "🌐 Frontend: https://$FRONTEND_URL"
echo "⚙️ API: https://$API_URL"
echo "🤖 ML Model: https://$ML_MODEL_URL (internal)"
echo ""
echo "🔗 GŁÓWNY URL APLIKACJI: https://$FRONTEND_URL"
echo ""
echo "💡 Sprawdź logi:"
echo "az containerapp logs show --name myhealth-frontend --resource-group $RESOURCE_GROUP"
echo "az containerapp logs show --name myhealth-api --resource-group $RESOURCE_GROUP"
echo "az containerapp logs show --name myhealth-ml-model --resource-group $RESOURCE_GROUP"
echo ""
echo "💰 Szacowane koszty: ~$20-50/miesiąc"
echo ""
echo "🛠️ Zarządzanie:"
echo "Portal Azure: https://portal.azure.com"
echo "Resource Group: $RESOURCE_GROUP"
echo ""
echo "🗑️ Aby usunąć wszystko:"
echo "az group delete --name $RESOURCE_GROUP --yes --no-wait" 