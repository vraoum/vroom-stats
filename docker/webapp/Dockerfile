FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine as build
WORKDIR /src
COPY ["src/VroomStats/VroomStats.csproj", "src/VroomStats/"]
RUN dotnet restore "src/VroomStats/VroomStats.csproj"
COPY . /app/vroomstats
COPY . .
WORKDIR "/src/src/VroomStats"
RUN dotnet publish "VroomStats.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS final
EXPOSE 8080
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
COPY --from=build /app/vroomstats /app/vroomstats
ENTRYPOINT ["dotnet", "VroomStats.dll"]