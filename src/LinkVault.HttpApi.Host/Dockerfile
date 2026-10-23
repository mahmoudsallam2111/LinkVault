FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Node.js + ABP CLI are needed to restore the client-side libraries (wwwroot/libs).
# Those files are git-ignored, so they must be generated during the image build,
# otherwise server-rendered pages like /Account/Login fail with
# "Could not find file '/libs/abp/core/abp.css'".
RUN apt-get update \
    && apt-get install -y --no-install-recommends nodejs npm \
    && rm -rf /var/lib/apt/lists/*
RUN dotnet tool install -g Volo.Abp.Studio.Cli
ENV PATH="$PATH:/root/.dotnet/tools"

# Copy all project files
COPY . .
WORKDIR /src/src/LinkVault.HttpApi.Host

# Restore client-side libraries into wwwroot/libs
RUN abp install-libs

RUN dotnet restore

# Build and Publish
RUN dotnet publish -c Release -o /app/publish

# Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LinkVault.HttpApi.Host.dll"]