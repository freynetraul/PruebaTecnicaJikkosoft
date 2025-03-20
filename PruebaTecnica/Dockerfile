FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PruebaTecnica.csproj", "./"]  # Cambia "PruebaTecnica.csproj" por el nombre real de tu archivo .csproj
RUN dotnet restore "./PruebaTecnica.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "PruebaTecnica.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PruebaTecnica.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PruebaTecnica.dll"] # Cambia "PruebaTecnica.dll" por el nombre real de tu .dll