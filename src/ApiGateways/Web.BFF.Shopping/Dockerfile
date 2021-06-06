#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

# We are copying only csproj file first, then restoring and finnaly copying all other files. This way, we are using docker caching. If csproj file hasn't changed, we don't have to restore the applciation.
# If we don't follow that way, we will have to restore the project everytime a file has changed in codebase.
COPY ["ApiGateways/Web.BFF.Shopping/Web.BFF.Shopping.csproj", "ApiGateways/Web.BFF.Shopping/"]
RUN dotnet restore "ApiGateways/Web.BFF.Shopping/Web.BFF.Shopping.csproj"
COPY . .
WORKDIR "/src/ApiGateways/Web.BFF.Shopping"
RUN dotnet build "Web.BFF.Shopping.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Web.BFF.Shopping.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Web.BFF.Shopping.dll"]