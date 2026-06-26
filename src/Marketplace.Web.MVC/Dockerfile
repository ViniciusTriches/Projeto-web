FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
# Porta real é definida dinamicamente via variável de ambiente PORT (ver Program.cs)
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Marketplace.Web.MVC.dll"]
