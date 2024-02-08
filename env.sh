# Define default values
default_email="admin@sm.com"
default_password="sm123"
default_dbuser="sm"
default_dbpassword="sm123"
default_db="StreamMaster"
default_set_perms=1

# Set and export environment variables with defaults if they are not set
export PGADMIN_SETUP_EMAIL="${PGADMIN_SETUP_EMAIL:-$default_email}" 
export PGADMIN_SETUP_PASSWORD="${PGADMIN_SETUP_PASSWORD:-$default_password}" 
export PGADMIN_PLATFORM_TYPE=debian
export POSTGRES_USER="${POSTGRES_USER:-$default_dbuser}" 
export POSTGRES_PASSWORD="${POSTGRES_PASSWORD:-$default_dbpassword}" 
export PGDATA=/config/DB
export POSTGRES_DB="${POSTGRES_DB:-$default_db}" 
export POSTGRES_SET_PERMS="${POSTGRES_SET_PERMS:-$default_set_perms}" 
export PATH=$PATH:/usr/lib/postgresql/15/bin