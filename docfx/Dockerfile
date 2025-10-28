# Build stage - Generate API documentation and build DocFx site
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Install DocFx
RUN dotnet tool install -g docfx
ENV PATH="$PATH:/root/.dotnet/tools"

# Copy project files for metadata extraction
COPY ["CoinPay.Api/CoinPay.Api.csproj", "CoinPay.Api/"]
COPY ["CoinPay.Api/", "CoinPay.Api/"]

# Copy DocFx configuration and content
COPY ["docfx/", "docfx/"]
WORKDIR /src/docfx

# Generate metadata and build documentation
RUN docfx metadata docfx.json
RUN docfx build docfx.json

# Production stage - Serve documentation with nginx
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# Remove default nginx content
RUN rm -rf ./*

# Copy built documentation from build stage
COPY --from=build /src/docfx/_site .

# Create nginx configuration for DocFx
RUN echo 'server { \
    listen 80; \
    server_name localhost; \
    root /usr/share/nginx/html; \
    index index.html; \
    \
    location / { \
        try_files $uri $uri/ /index.html; \
    } \
    \
    location ~* \.(js|css|png|jpg|jpeg|gif|ico|svg|woff|woff2|ttf|eot)$ { \
        expires 1y; \
        add_header Cache-Control "public, immutable"; \
    } \
}' > /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
