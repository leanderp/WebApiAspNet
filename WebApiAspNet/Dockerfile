FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use Alpine Base Image
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY WebApiAspNet5.csproj ./
RUN dotnet restore "./WebApiAspNet5.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "WebApiAspNet5.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApiAspNet5.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApiAspNet5.dll"]