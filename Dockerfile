# 1. AŞAMA: Base (Çalışma Ortamı)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# 2. AŞAMA: Build (Derleme)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# ÖNEMLİ: Katmanlı mimari olduğu için tüm projelerin .csproj dosyalarını kopyalıyoruz
COPY ["CureLogix.WebUI/CureLogix.WebUI.csproj", "CureLogix.WebUI/"]
COPY ["CureLogix.Business/CureLogix.Business.csproj", "CureLogix.Business/"]
COPY ["CureLogix.DataAccess/CureLogix.DataAccess.csproj", "CureLogix.DataAccess/"]
COPY ["CureLogix.Entity/CureLogix.Entity.csproj", "CureLogix.Entity/"]

# Bağımlılıkları yükle (Restore)
RUN dotnet restore "CureLogix.WebUI/CureLogix.WebUI.csproj"

# Kalan tüm dosyaları kopyala
COPY . .
WORKDIR "/src/CureLogix.WebUI"

# Projeyi derle
RUN dotnet build "CureLogix.WebUI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 3. AŞAMA: Publish (Yayınlama)
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "CureLogix.WebUI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 4. AŞAMA: Final (Ayağa Kaldırma)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CureLogix.WebUI.dll"]