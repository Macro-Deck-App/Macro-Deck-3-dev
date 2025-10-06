let backend = "http://localhost:8191";

const PROXY_CONFIG = [
  {
    context: [
      "/api"
    ],
    target: backend,
    secure: false,
    changeOrigin: true,
    ws: true,
  }
]

module.exports = PROXY_CONFIG;
