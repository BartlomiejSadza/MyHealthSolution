# Konfiguracja GitHub Secrets dla automatycznego deploymentu

## 🔐 Wymagane sekrety

Aby GitHub Actions mogły automatycznie wdrażać aplikację na Azure, musisz skonfigurować następujący sekret:

### AZURE_CREDENTIALS

Idź do swojego repozytorium GitHub:

1. **Settings** → **Secrets and variables** → **Actions**
2. Kliknij **New repository secret**
3. Nazwa: `AZURE_CREDENTIALS`
4. Wartość: Skopiuj JSON z wyniku komendy `az ad sp create-for-rbac` (przykład poniżej):

```json
{
	"clientId": "TWÓJ_CLIENT_ID",
	"clientSecret": "TWÓJ_CLIENT_SECRET",
	"subscriptionId": "TWÓJ_SUBSCRIPTION_ID",
	"tenantId": "TWÓJ_TENANT_ID",
	"activeDirectoryEndpointUrl": "https://login.microsoftonline.com",
	"resourceManagerEndpointUrl": "https://management.azure.com/",
	"activeDirectoryGraphResourceId": "https://graph.windows.net/",
	"sqlManagementEndpointUrl": "https://management.core.windows.net:8443/",
	"galleryEndpointUrl": "https://gallery.azure.com/",
	"managementEndpointUrl": "https://management.core.windows.net/"
}
```

## 🔧 Tworzenie Service Principal

Uruchom w Azure CLI:

```bash
az ad sp create-for-rbac --name "MyHealthGitHubActions" --role contributor --scopes /subscriptions/$(az account show --query id --output tsv)/resourceGroups/rg-myhealth-fixed --sdk-auth
```

Skopiuj wynik i wklej jako sekret `AZURE_CREDENTIALS` w GitHub.

## 🚀 Jak to działa

Po skonfigurowaniu sekretów, każdy push do brancha `main` automatycznie:

1. **Wykrywa zmiany** w folderach:

   - `health-frontend/` → wdraża frontend
   - `MyHealth.Api/` → wdraża API
   - `ml-model/` → wdraża ML model

2. **Buduje i wdraża** tylko zmienione komponenty

3. **Aktualizuje** Container Apps na Azure

## 📋 Status deploymentu

Sprawdź status w zakładce **Actions** w GitHub:

- ✅ Zielony = deployment udany
- ❌ Czerwony = błąd (sprawdź logi)
- 🟡 Żółty = w trakcie

## 🌐 URL aplikacji

Po udanym deploymencie znajdziesz URL w logach GitHub Actions lub:

```bash
az containerapp show --name myhealth-frontend --resource-group rg-myhealth-fixed --query "properties.configuration.ingress.fqdn" --output tsv
```

## 🔧 Troubleshooting

Jeśli deployment się nie powiedzie:

1. Sprawdź logi w GitHub Actions
2. Upewnij się, że sekret `AZURE_CREDENTIALS` jest poprawnie skonfigurowany
3. Sprawdź czy Service Principal ma odpowiednie uprawnienia

## 🔄 Ręczny deployment (backup)

Jeśli GitHub Actions nie działają, możesz użyć:

```bash
./update-azure.sh all
```
