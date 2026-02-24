# ---------- STAGE 1: build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos da solution
COPY ["Agenda.Api/Agenda.Api.csproj", "Agenda.Api/"]
COPY ["Agenda.Application/Agenda.Application.csproj", "Agenda.Application/"]
COPY ["Agenda.Domain/Agenda.Domain.csproj", "Agenda.Domain/"]
COPY ["Agenda.Infrastructure/Agenda.Infrastructure.csproj", "Agenda.Infrastructure/"]

# Restaurar dependências
RUN dotnet restore "Agenda.Api/Agenda.Api.csproj"

# Copiar todo o código
COPY . .

# Publicar aplicação
RUN dotnet publish "Agenda.Api/Agenda.Api.csproj" -c Release -o /app/publish


# ---------- STAGE 2: runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Agenda.Api.dll"]