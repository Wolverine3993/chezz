# syntax=docker/dockerfile:1

FROM node:22-bookworm-slim AS web-build
WORKDIR /src/web
ENV COREPACK_ENABLE_DOWNLOAD_PROMPT=0
RUN corepack enable && corepack prepare pnpm@9 --activate
COPY web/package.json web/pnpm-lock.yaml ./
RUN pnpm install --frozen-lockfile
COPY web/ ./
RUN pnpm build

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS api-build
WORKDIR /src
COPY Chezz.csproj ./
RUN dotnet restore Chezz.csproj
COPY . ./
RUN dotnet publish Chezz.csproj -c Release -o /app/api -p:OpenApiGenerateDocumentsOnBuild=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        nginx \
        supervisor \
        nodejs \
        ca-certificates \
    && rm -rf /var/lib/apt/lists/* \
    && rm -f /etc/nginx/sites-enabled/default

WORKDIR /app
COPY --from=api-build /app/api ./api
COPY --from=web-build /src/web/.output ./web

COPY docker/nginx.conf /etc/nginx/conf.d/default.conf
COPY docker/supervisord.conf /etc/supervisor/conf.d/chezz.conf

ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://127.0.0.1:5000 \
    NITRO_HOST=127.0.0.1 \
    NITRO_PORT=3000 \
    NUXT_PUBLIC_WS_ENDPOINT=/

EXPOSE 80

CMD ["supervisord", "-c", "/etc/supervisor/conf.d/chezz.conf"]
