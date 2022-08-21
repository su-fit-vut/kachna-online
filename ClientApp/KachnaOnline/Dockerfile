FROM docker.io/nginx:alpine as certs
WORKDIR /certs
RUN apk update && apk add --no-cache openssl && \
    openssl req -newkey rsa:2048 -new -nodes -x509 -days 3650 -keyout key.pem -out cert.pem -batch

FROM node:lts
EXPOSE 4200
WORKDIR /src
COPY package.json package-lock.json ./
RUN mkdir /certs
COPY --from=certs /certs/key.pem /certs/cert.pem /certs/
RUN npm install
RUN npm install -g @angular/cli@12
ENTRYPOINT ["ng", "serve", "--host=0.0.0.0", "--ssl=true", "--ssl-cert=/certs/cert.pem", "--ssl-key=/certs/key.pem", "--serve-path=/kachna/"]
