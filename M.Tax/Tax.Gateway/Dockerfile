#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["Tax.Gateway/Tax.Gateway.csproj", "Tax.Gateway/"]
RUN dotnet restore "Tax.Gateway/Tax.Gateway.csproj"
COPY . .
WORKDIR "/src/Tax.Gateway"
RUN dotnet build "Tax.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tax.Gateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tax.Gateway.dll"]