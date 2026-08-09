FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY TuttiWallet.slnx .
COPY src/TuttiWallet.Domain/TuttiWallet.Domain.csproj src/TuttiWallet.Domain/
COPY src/TuttiWallet.Application/TuttiWallet.Application.csproj src/TuttiWallet.Application/
COPY src/TuttiWallet.Infrastructure/TuttiWallet.Infrastructure.csproj src/TuttiWallet.Infrastructure/
COPY src/TuttiWallet.Contracts/TuttiWallet.Contracts.csproj src/TuttiWallet.Contracts/
COPY src/TuttiWallet.Api/TuttiWallet.Api.csproj src/TuttiWallet.Api/
RUN dotnet restore src/TuttiWallet.Api/TuttiWallet.Api.csproj

COPY src/TuttiWallet.Domain/ src/TuttiWallet.Domain/
COPY src/TuttiWallet.Application/ src/TuttiWallet.Application/
COPY src/TuttiWallet.Infrastructure/ src/TuttiWallet.Infrastructure/
COPY src/TuttiWallet.Contracts/ src/TuttiWallet.Contracts/
COPY src/TuttiWallet.Api/ src/TuttiWallet.Api/

RUN dotnet publish src/TuttiWallet.Api/TuttiWallet.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "TuttiWallet.Api.dll"]
