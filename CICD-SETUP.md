# 🤖 Automatyczny Deployment z GitHub Actions

## Przegląd opcji aktualizacji

### 1. 🔧 Ręczna aktualizacja (obecny stan)

```bash
# Po wprowadzeniu zmian
./update-azure.sh frontend  # Tylko frontend
./update-azure.sh api       # Tylko API
./update-azure.sh all       # Wszystko
```

### 2. 🚀 Automatyczny CI/CD z GitHub Actions

Po skonfigurowaniu: **każdy push do main = automatyczny deployment** 🎉

---

## 🛠️ Konfiguracja GitHub Actions (Automatyczny CI/CD)

### Krok 1: Utwórz Service Principal

```bash
# Utwórz Service Principal dla GitHub Actions
az ad sp create-for-rbac --name "myhealth-github-actions" \
    --role contributor \
    --scopes /subscriptions/YOUR_SUBSCRIPTION_ID \
    --sdk-auth
```

To zwróci JSON podobny do:

```json
{
	"clientId": "xxx",
	"clientSecret": "xxx",
	"subscriptionId": "xxx",
	"tenantId": "xxx",
	"activeDirectoryEndpointUrl": "https://login.microsoftonline.com",
	"resourceManagerEndpointUrl": "https://management.azure.com/",
	"activeDirectoryGraphResourceId": "https://graph.windows.net/",
	"sqlManagementEndpointUrl": "https://management.core.windows.net:8443/",
	"galleryEndpointUrl": "https://gallery.azure.com/",
	"managementEndpointUrl": "https://management.core.windows.net/"
}
```

### Krok 2: Dodaj Secrets w GitHub

1. Idź do **GitHub Repository** → **Settings** → **Secrets and variables** → **Actions**

2. Dodaj następujące secrets:

| Secret Name           | Wartość               |
| --------------------- | --------------------- |
| `AZURE_CREDENTIALS`   | Cały JSON z kroku 1   |
| `AZURE_CLIENT_ID`     | `clientId` z JSON     |
| `AZURE_CLIENT_SECRET` | `clientSecret` z JSON |
| `AZURE_TENANT_ID`     | `tenantId` z JSON     |

### Krok 3: Push kodu do GitHub

```bash
# Jeśli nie masz jeszcze repo na GitHub
gh repo create MyHealthSolution --public --push

# Lub jeśli masz już repo
git add .
git commit -m "Add CI/CD with GitHub Actions"
git push origin main
```

### Krok 4: Workflow automatycznie się uruchomi! 🎉

Sprawdź w **GitHub** → **Actions** tab

---

## 🎯 Jak działa automatyczny deployment

### Wykrywanie zmian

```yaml
# Workflow sprawdza które komponenty się zmieniły:
frontend: health-frontend/** # Zmiana we frontend
api: MyHealth.Api/** # Zmiana w API
ml-model: ml-model/** # Zmiana w ML Model
```

### Inteligentny deployment

- ✅ **Zmiana tylko frontend** → Deploy tylko frontend
- ✅ **Zmiana tylko API** → Deploy API + Frontend (nowy URL)
- ✅ **Zmiana ML Model** → Deploy wszystko w kolejności
- ✅ **Zero downtime** → Rolling updates

### Przykładowy workflow:

```
1. 📝 Zmieniasz kod w health-frontend/
2. 🔄 git push origin main
3. 🤖 GitHub Actions wykrywa zmianę
4. 🔨 Build nowego obrazu frontend
5. 📦 Push do Azure Container Registry
6. 🚀 Update Container App na Azure
7. ✅ Aplikacja zaktualizowana!
```

---

## 📋 Workflow po skonfigurowaniu

### Codzienne używanie:

```bash
# 1. Wprowadź zmiany w kodzie
vim health-frontend/src/components/SomeComponent.tsx

# 2. Commit i push
git add .
git commit -m "Update component styling"
git push origin main

# 3. Gotowe! 🎉
# GitHub Actions automatycznie deployuje zmiany
```

### Monitoring deployment:

- **GitHub Actions**: Zobacz postęp w GitHub → Actions
- **Azure Portal**: Monitoruj Container Apps
- **Logi**: `az containerapp logs show --name myhealth-frontend --resource-group rg-myhealth --follow`

---

## 🔄 Porównanie opcji

| Metoda              | Czas     | Trudność | Automatyzacja |
| ------------------- | -------- | -------- | ------------- |
| **Ręczna**          | ~5-10min | ⭐⭐☆☆☆  | ❌ Manual     |
| **update-azure.sh** | ~3-5min  | ⭐⭐☆☆☆  | 🟡 Semi-auto  |
| **GitHub Actions**  | ~5-8min  | ⭐⭐⭐☆☆ | ✅ Pełny auto |

---

## 🚨 Troubleshooting CI/CD

### GitHub Actions fails

```bash
# Sprawdź logi w GitHub → Actions → Failed workflow

# Najczęstsze problemy:
1. Błędne AZURE_CREDENTIALS
2. Brak uprawnień Service Principal
3. Nieprawidłowe nazwy resources
```

### Service Principal permission error

```bash
# Dodaj uprawnienia do Container Registry
az role assignment create \
    --assignee YOUR_CLIENT_ID \
    --role "AcrPush" \
    --scope /subscriptions/YOUR_SUBSCRIPTION_ID/resourceGroups/rg-myhealth/providers/Microsoft.ContainerRegistry/registries/myhealthregistry
```

### Verify deployment

```bash
# Po deployment sprawdź
az containerapp list --resource-group rg-myhealth --output table

# Sprawdź najnowsze image
az acr repository show-tags --name myhealthregistry --repository frontend --orderby time_desc
```

---

## 🎯 Podsumowanie

### Bez CI/CD (obecny stan):

```bash
# Po każdej zmianie w frontend:
./update-azure.sh frontend
```

### Z CI/CD (po konfiguracji):

```bash
# Po każdej zmianie w frontend:
git push origin main
# I to wszystko! 🎉
```

**Czas konfiguracji**: ~15 minut  
**Oszczędność czasu**: ~5 minut na każdy deployment  
**Korzyści**: Zero błędów, historia deploymentów, rollback możliwy
