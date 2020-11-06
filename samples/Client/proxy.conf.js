const PROXY_CONFIG = [
  {
    context: [
      "/api"
    ],
    target: "https://localhost:5000",
    secure: false
  }
]

module.exports = PROXY_CONFIG;
