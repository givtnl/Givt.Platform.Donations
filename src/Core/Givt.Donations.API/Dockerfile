#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Core/Givt.API/Givt.API.csproj", "src/Core/Givt.API/"]
RUN dotnet restore "src/Core/Givt.API/Givt.API.csproj"
COPY . .
WORKDIR "/src/src/Core/Givt.API"
RUN dotnet build "Givt.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Givt.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Givt.API.dll"]