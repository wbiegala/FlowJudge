FROM node:20-alpine AS build
ENV CI=true
WORKDIR /app
RUN corepack enable
COPY package.json pnpm-*.yaml ./
RUN --mount=type=cache,target=/root/.local/share/pnpm/store \
  pnpm fetch
COPY . .
RUN --mount=type=cache,target=/root/.local/share/pnpm/store \
  pnpm install --frozen-lockfile --offline
RUN NX_CLOUD=false NX_DAEMON=false pnpm nx build FlowJudge --configuration=docker --no-cloud


FROM nginx:1.27-alpine AS runtime
RUN mkdir -p /etc/nginx/ssl
COPY cert.pem /etc/nginx/ssl/cert.pem
COPY key.pem  /etc/nginx/ssl/key.pem
RUN chmod 600 /etc/nginx/ssl/key.pem /etc/nginx/ssl/cert.pem
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist/apps/FlowJudge/browser /usr/share/nginx/html

EXPOSE 80
EXPOSE 443
CMD ["nginx", "-g", "daemon off;"]
