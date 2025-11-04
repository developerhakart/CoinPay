# HashiCorp Vault Configuration
# Storage: File-based (persistent across restarts)

storage "file" {
  path = "/vault/data"
}

listener "tcp" {
  address     = "0.0.0.0:8200"
  tls_disable = 1
}

api_addr = "http://0.0.0.0:8200"
cluster_addr = "https://0.0.0.0:8201"
ui = true

# Disable mlock for development (allows running without IPC_LOCK capability)
disable_mlock = true
