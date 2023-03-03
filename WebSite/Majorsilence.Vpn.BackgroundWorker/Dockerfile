FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Majorsilence.Vpn.BackgroundWorker/Majorsilence.Vpn.BackgroundWorker.csproj", "Majorsilence.Vpn.BackgroundWorker/"]
RUN dotnet restore "Majorsilence.Vpn.BackgroundWorker/Majorsilence.Vpn.BackgroundWorker.csproj"
COPY . .
WORKDIR "/src/Majorsilence.Vpn.BackgroundWorker"
RUN dotnet build "Majorsilence.Vpn.BackgroundWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Majorsilence.Vpn.BackgroundWorker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Majorsilence.Vpn.BackgroundWorker.dll"]
