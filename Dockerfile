FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["apiB2e/apiB2e.csproj", "apiB2e/"]
RUN dotnet restore "apiB2e/apiB2e.csproj"
COPY . .
WORKDIR "/src/apiB2e"
RUN dotnet build "apiB2e.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "apiB2e.csproj" -c Release -o /app/publish
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=publish /root/.dotnet/tools /root/.dotnet/tools
ENV PATH="${PATH}:/root/.dotnet/tools"

# Configurar variáveis de ambiente
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Expor a porta da API
EXPOSE 80

ENTRYPOINT ["dotnet", "apiB2e.dll"]