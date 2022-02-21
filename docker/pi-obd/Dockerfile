FROM mcr.microsoft.com/dotnet/sdk:6.0.200-bullseye-slim-arm32v7 as build
WORKDIR /src
COPY . .
WORKDIR "/src/src/VroomStats.Obd"
RUN dotnet publish "VroomStats.Obd.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:6.0.2-bullseye-slim-arm32v7 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "VroomStats.Obd.dll"]