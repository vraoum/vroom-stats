FROM node:17.2-alpine as dev
EXPOSE 3000 
WORKDIR /src
COPY . .
WORKDIR "/src/src/frontend"
RUN yarn install
ENTRYPOINT ["yarn", "start"]