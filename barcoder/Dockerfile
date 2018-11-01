FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["barcoder/barcoder.csproj", "barcoder/"]
RUN dotnet restore "barcoder/barcoder.csproj"
COPY . .
WORKDIR "/src/barcoder"
RUN dotnet build "barcoder.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "barcoder.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "barcoder.dll"]