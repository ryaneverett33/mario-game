# Deploying the server

## Frontend stuff

- `public/index.html` and `public/othercastle.html` are copied to the root directory
    - If the game is available at `http://site.com/projects/mario-game/`, then `public/index.html` should be at `/var/www/html/projects/mario-game/index.html`
- `public` is symlinked to the root directory
    - Made need to allow symlinks in nginx via this [SO post](https://unix.stackexchange.com/a/157098)

## Backend stuff

- The node.js server exists in the root directory
    - Similar the above example, `server` should exist at `/var/www/html/projects/mario-game/server`
- [PM2](https://pm2.keymetrics.io/) is used to daemonize the server
    - `pm2 start index.js --name 'mario-game' --log 'server.log' --time`
- The `server` directory has access restriced:
```
location /projects/mario-game/server {
        # deny access to server internals
        deny all;
        return 403;
}
```
- There exists a reverse proxy for the node.js server with a rewrite rule:
```
location /projects/mario-game/leaderboard/ {
        rewrite /projects/mario-game/leaderboard/(.*) /$1  break;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_pass http://localhost:3000;
}
```