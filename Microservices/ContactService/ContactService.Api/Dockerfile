FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
RUN apt-get update && apt-get install -y wget && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ContactService/ContactService.Api/ContactService.Api.csproj", "ContactService/ContactService.Api/"]
COPY ["ContactService/ContactService.Application/ContactService.Application.csproj", "ContactService/ContactService.Application/"]
COPY ["ContactService/ContactService.Domain/ContactService.Domain.csproj", "ContactService/ContactService.Domain/"]
COPY ["ContactService/ContactService.Infrastructure/ContactService.Infrastructure.csproj", "ContactService/ContactService.Infrastructure/"]
COPY ["Shared/Shared.Kernel/Shared.Kernel.csproj", "Shared/Shared.Kernel/"]
COPY ["Shared/Shared.Infrastructure/Shared.Infrastructure.csproj", "Shared/Shared.Infrastructure/"]
RUN dotnet restore "ContactService/ContactService.Api/ContactService.Api.csproj"
COPY . .
WORKDIR "/ContactService/ContactService.Api"
RUN dotnet build "ContactService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ContactService.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ContactService.Api.dll"] 