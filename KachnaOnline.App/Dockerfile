# Set context to the root of the repo

FROM mcr.microsoft.com/dotnet/sdk:8.0-jammy AS build
ENV DOTNET_PRINT_TELEMETRY_MESSAGE="false"
ENV DOTNET_SKIP_FIRST_TIME_EXPERIENCE="1"
ENV DOTNET_NOLOGO="true"

WORKDIR /src

# Run restore separately
COPY ./KachnaOnline.App/*.csproj ./KachnaOnline.App/
COPY ./KachnaOnline.Business/*.csproj ./KachnaOnline.Business/
COPY ./KachnaOnline.Business.Data/*.csproj ./KachnaOnline.Business.Data/
COPY ./KachnaOnline.Data/*.csproj ./KachnaOnline.Data/
COPY ./KachnaOnline.Dto/*.csproj ./KachnaOnline.Dto/
RUN dotnet restore ./KachnaOnline.App/KachnaOnline.App.csproj -r linux-x64

# Copy entire source code
COPY . .

# Build and publish
WORKDIR /src/KachnaOnline.App
RUN ls -la

RUN dotnet publish --no-restore -o /publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-chiseled-composite as runtime
EXPOSE 8080
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["./KachnaOnline.App"]
