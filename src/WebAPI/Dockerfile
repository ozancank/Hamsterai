# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
EXPOSE 5005

# Projeyi kopyala ve restore et
COPY *.csproj ./
RUN dotnet restore

# Geri kalan tüm dosyaları kopyala ve build et
COPY . .
RUN dotnet publish -c Release -o /out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "HamsteraiWebApi.dll"]