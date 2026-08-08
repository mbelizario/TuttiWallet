FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY TuttiWallet.slnx .
COPY src/TuttiWallet.Contracts/TuttiWallet.Contracts.csproj src/TuttiWallet.Contracts/
COPY src/TuttiWallet.Web/TuttiWallet.Web.csproj src/TuttiWallet.Web/
RUN dotnet restore src/TuttiWallet.Web/TuttiWallet.Web.csproj

COPY src/TuttiWallet.Contracts/ src/TuttiWallet.Contracts/
COPY src/TuttiWallet.Web/ src/TuttiWallet.Web/

RUN dotnet publish src/TuttiWallet.Web/TuttiWallet.Web.csproj -c Release -o /app --no-restore

FROM nginx:alpine AS final
COPY --from=build /app/wwwroot /usr/share/nginx/html
COPY src/TuttiWallet.Web/nginx.conf /etc/nginx/conf.d/default.conf
COPY src/TuttiWallet.Web/appsettings.template.json /usr/share/nginx/html/appsettings.template.json
COPY src/TuttiWallet.Web/docker-entrypoint.sh /docker-entrypoint.d/40-tuttiwallet-config.sh

ENV API_BASE_URL=http://localhost:8080

RUN chmod +x /docker-entrypoint.d/40-tuttiwallet-config.sh

EXPOSE 80
