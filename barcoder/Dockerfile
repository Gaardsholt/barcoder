FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS builder
WORKDIR /source
# copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore
# copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o /app/ -r linux-x64

# build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=builder /app .
ENV ASPNETCORE_URLS http://*:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "barcoder.dll"]