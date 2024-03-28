# Define default values
default_email="admin@sm.com"
default_password="sm123"
default_dbuser="postgres"
default_dbpassword="sm123"
default_db="StreamMaster"
default_host="127.0.0.1"
default_set_perms=1
default_backup_versions_to_keep=5

# Set and export environment variables with defaults if they are not set
export PGADMIN_SETUP_EMAIL="${PGADMIN_SETUP_EMAIL:-$default_email}" 
export PGADMIN_SETUP_PASSWORD="${PGADMIN_SETUP_PASSWORD:-$default_password}" 
export PGADMIN_PLATFORM_TYPE=debian
export POSTGRES_USER="${POSTGRES_USER:-$default_dbuser}" 
export POSTGRES_PASSWORD="${POSTGRES_PASSWORD:-$default_dbpassword}" 
export PGDATA=/config/DB
export POSTGRES_HOST="${POSTGRES_HOST:-$default_host}" 
export POSTGRES_DB="${POSTGRES_DB:-$default_db}" 
export POSTGRES_SET_PERMS="${POSTGRES_SET_PERMS:-$default_set_perms}" 

is_local() {
    [[ "$1" == "127.0.0.1" || "$1" == "localhost" ]] && echo "1" || echo "0"
}
export POSTGRES_ISLOCAL=$(is_local "$POSTGRES_HOST")

export BACKUP_VERSIONS_TO_KEEP="${BACKUP_VERSIONS_TO_KEEP:-$default_backup_versions_to_keep}" 
export PATH=$PATH:/usr/lib/postgresql/15/bin
export BACKUP_DIR=/config/Backups
export RESTORE_DIR=/config/Restore
