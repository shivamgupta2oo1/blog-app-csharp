FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Bloggie.Web/Bloggie.Web.csproj", "Bloggie.Web/"]
RUN dotnet restore "Bloggie.Web/Bloggie.Web.csproj"
COPY . .
WORKDIR "/src/Bloggie.Web"
RUN dotnet build "Bloggie.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Bloggie.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bloggie.Web.dll"]
