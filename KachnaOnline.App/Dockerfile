FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["KachnaOnline.App/KachnaOnline.App.csproj", "KachnaOnline.App/"]
RUN dotnet restore "KachnaOnline.App/KachnaOnline.App.csproj"
COPY . .
WORKDIR "/src/KachnaOnline.App"
RUN dotnet build "KachnaOnline.App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "KachnaOnline.App.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "KachnaOnline.App.dll"]
