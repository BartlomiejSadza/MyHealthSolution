#!/bin/bash

# MyHealth Solution - Test Deploy na Azure Container Apps
set -e

echo "🧪 Testowy deployment MyHealth Solution na Azure..."

# Zmienne konfiguracyjne - testowe
RESOURCE_GROUP="rg-myhealth-test"
LOCATION="East US"  # Inny region
CONTAINER_APP_ENV="myhealth-test-env"
REGISTRY_NAME="myhealthtest$(date +%H%M)"
APP_NAME="myhealth-test"

echo "📍 Używam lokalizacji: $LOCATION"
echo "📦 Resource Group: $RESOURCE_GROUP"
echo "🏗️ Registry: $REGISTRY_NAME"

# Sprawdź czy Azure CLI jest zalogowane
if ! az account show > /dev/null 2>&1; then
    echo "❌ Musisz się zalogować do Azure CLI: az login"
    exit 1
fi

echo "✅ Azure CLI zalogowane"

# 1. Tworzenie Resource Group
echo "📦 Tworzę Resource Group..."
az group create \
    --name $RESOURCE_GROUP \
    --location "$LOCATION"

# 2. Tworzenie Azure Container Registry
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

# 3. Build i push images do ACR (tylko ML Model dla szybkości)
echo "🔨 Testuję build ML Model..."

# Login do ACR
az acr login --name $REGISTRY_NAME

echo "Building ML Model..."
docker build -t $ACR_SERVER/ml-model:latest ./ml-model
docker push $ACR_SERVER/ml-model:latest

# 4. Tworzenie Container Apps Environment
echo "🌍 Tworzę Container Apps Environment..."
az containerapp env create \
    --name $CONTAINER_APP_ENV \
    --resource-group $RESOURCE_GROUP \
    --location "$LOCATION"

# 5. Deploy tylko ML Model dla testu
echo "🤖 Testuję deployment ML Model..."
az containerapp create \
    --name myhealth-ml-model-test \
    --resource-group $RESOURCE_GROUP \
    --environment $CONTAINER_APP_ENV \
    --image $ACR_SERVER/ml-model:latest \
    --registry-server $ACR_SERVER \
    --registry-username $ACR_USERNAME \
    --registry-password $ACR_PASSWORD \
    --target-port 5000 \
    --ingress external \
    --memory 1.0Gi \
    --cpu 0.5

# 6. Pokaż wyniki
echo ""
echo "🎉 TEST DEPLOYMENT ZAKOŃCZONY POMYŚLNIE!"
echo ""
ML_MODEL_URL=$(az containerapp show --name myhealth-ml-model-test --resource-group $RESOURCE_GROUP --query "properties.configuration.ingress.fqdn" --output tsv)
echo "🤖 Test ML Model URL: https://$ML_MODEL_URL"
echo ""
echo "🧪 Test health endpoint:"
echo "curl https://$ML_MODEL_URL/health"
echo ""
echo "📋 Jeśli to działa, użyj pełnego deploymentu z tym regionem!"
echo ""
echo "🗑️ Aby usunąć test:"
echo "az group delete --name $RESOURCE_GROUP --yes --no-wait" 