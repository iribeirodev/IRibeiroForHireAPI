# Estágio de Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copia o csproj e restaura
COPY ["IRibeiroForHireAPI.csproj", "./"]
RUN dotnet restore "IRibeiroForHireAPI.csproj"

# Copia tudo e publica
COPY . .
RUN dotnet publish "IRibeiroForHireAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio de Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

# Variável de porta obrigatória para o Koyeb
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "IRibeiroForHireAPI.dll"]