FROM node:20-alpine AS build

WORKDIR /app
COPY package*.json /app/

# Use npm install with legacy-peer-deps for ABP/Angular compatibility
RUN npm install --legacy-peer-deps
COPY . /app

RUN npm run build:prod

FROM nginx:alpine
# Optional: copy custom nginx config if needed for standalone run
COPY nginx.conf /etc/nginx/conf.d/default.conf
# Copy minimal config or empty json
COPY dynamic-env.json /usr/share/nginx/html

# Copy build output (Verify path matches your angular.json output)
# Standard Angular 17+ output is dist/<project>/browser
COPY --from=build /app/dist/LinkVault/browser /usr/share/nginx/html
