FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
WORKDIR /src/src/LinkVault.DbMigrator
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
RUN apt-get update && apt-get install -y libkrb5-3 libgssapi-krb5-2
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LinkVault.DbMigrator.dll"]