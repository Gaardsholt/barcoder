FROM mcr.microsoft.com/dotnet/sdk:6.0.302 AS builder
WORKDIR /source
# copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore
# copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o /app/ -r linux-x64

# build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
RUN apt-get update \
    && apt-get install -y --no-install-recommends libgdiplus libc6-dev \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=builder /app .
ENV ASPNETCORE_URLS http://*:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "barcoder.dll"]