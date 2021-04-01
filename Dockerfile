FROM mcr.microsoft.com/dotnet/aspnet:5.0.3 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:5.0.103 AS build

WORKDIR /app
COPY [".editorconfig", "/"]
COPY ["Directory.Build.props", "/"]

WORKDIR /app
COPY ["src/Api/Api.csproj", "src/Api/"]
RUN dotnet restore "src/Api/Api.csproj"

COPY . .

WORKDIR /app/src/Api
RUN dotnet build "Api.csproj" --no-restore -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Api.dll"]
