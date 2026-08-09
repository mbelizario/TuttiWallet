FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY TuttiWallet.slnx .
COPY src/TuttiWallet.Migrator/TuttiWallet.Migrator.csproj src/TuttiWallet.Migrator/
RUN dotnet restore src/TuttiWallet.Migrator/TuttiWallet.Migrator.csproj

COPY src/TuttiWallet.Migrator/ src/TuttiWallet.Migrator/

RUN dotnet publish src/TuttiWallet.Migrator/TuttiWallet.Migrator.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=build /app .

ENTRYPOINT ["dotnet", "TuttiWallet.Migrator.dll"]
