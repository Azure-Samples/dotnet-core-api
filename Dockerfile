# create the build instance
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

WORKDIR /source
COPY /. ./

#restore
RUN dotnet restore

# build with configuration
RUN dotnet build TodoApi.csproj -c Release

RUN dotnet publish TodoApi.csproj -c Release -o /src/output

# create the runtime instance
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS runtime

# add globalization support
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# installs required packages
RUN apk add libgdiplus --no-cache --repository http://dl-3.alpinelinux.org/alpine/edge/testing/ --allow-untrusted
RUN apk add libc-dev --no-cache

COPY --from=build /src/output .

ENTRYPOINT ["dotnet", "TodoApi.dll"]