FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
RUN apt-get update && apt-get install -y wget && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ReportService/ReportService.Api/ReportService.Api.csproj", "ReportService/ReportService.Api/"]
COPY ["ReportService/ReportService.Application/ReportService.Application.csproj", "ReportService/ReportService.Application/"]
COPY ["ReportService/ReportService.Domain/ReportService.Domain.csproj", "ReportService/ReportService.Domain/"]
COPY ["ReportService/ReportService.Infrastructure/ReportService.Infrastructure.csproj", "ReportService/ReportService.Infrastructure/"]
COPY ["Shared/Shared.Kernel/Shared.Kernel.csproj", "Shared/Shared.Kernel/"]
COPY ["Shared/Shared.Infrastructure/Shared.Infrastructure.csproj", "Shared/Shared.Infrastructure/"]
RUN dotnet restore "ReportService/ReportService.Api/ReportService.Api.csproj"
COPY . .
WORKDIR "/ReportService/ReportService.Api"
RUN dotnet build "ReportService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ReportService.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReportService.Api.dll"] 