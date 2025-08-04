FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ContactService/ContactService.Api/ContactService.Api.csproj", "ContactService/ContactService.Api/"]
COPY ["ContactService/ContactService.Application/ContactService.Application.csproj", "ContactService/ContactService.Application/"]
COPY ["ContactService/ContactService.Domain/ContactService.Domain.csproj", "ContactService/ContactService.Domain/"]
COPY ["ContactService/ContactService.Infrastructure/ContactService.Infrastructure.csproj", "ContactService/ContactService.Infrastructure/"]
COPY ["Shared/Shared.Kernel/Shared.Kernel.csproj", "Shared/Shared.Kernel/"]
COPY ["Shared/Shared.Infrastructure/Shared.Infrastructure.csproj", "Shared/Shared.Infrastructure/"]
RUN dotnet restore "ContactService/ContactService.Api/ContactService.Api.csproj"
COPY . .
WORKDIR "/src/ContactService/ContactService.Api"
RUN dotnet build "ContactService.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ContactService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ContactService.Api.dll"] 