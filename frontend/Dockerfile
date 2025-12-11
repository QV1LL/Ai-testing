# Stage 1: Build the React application
# Use a Node image to compile the front-end assets
FROM node:20-alpine AS build

# Set the working directory inside the container
WORKDIR /app

# Copy package.json and install dependencies
# This is done separately to leverage Docker cache if only source code changes
COPY ./Ai-testing/package-lock.json ./
COPY ./Ai-testing/package.json ./
RUN npm install

# Copy the rest of the application source code
COPY ./Ai-testing/. .

# Build the application (Vite usually outputs to 'dist')
RUN npm run build


# Stage 2: Serve the application using a lightweight Nginx image
FROM nginx:alpine AS final

COPY ./nginx.conf /etc/nginx/nginx.conf

# Remove the default Nginx index.html and put the built React app instead
# The built assets are copied from the 'build' stage
COPY --from=build /app/dist /usr/share/nginx/html

# Expose the standard HTTP port (Nginx runs on 80 by default)
EXPOSE 80

# The default CMD of the nginx:alpine image starts the web server.
# Frontend will be accessible on port 3000 on your host machine.