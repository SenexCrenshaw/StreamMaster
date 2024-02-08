#!/bin/bash

user_name="nonRootUser"
group_name="nonRootGroup"

. /env.sh

wait_for_postgres() {
    local host="$1"
    local port="$2"
    local max_attempts="$3"
    local wait_interval="$4"
    
    local attempt=0
    
    while [ $attempt -lt $max_attempts ]; do
        if pg_isready -h "$host" -p "$port" >/dev/null 2>&1; then
            echo "PostgreSQL is ready on $host:$port"
            return 0
        else
            attempt=$((attempt + 1))
            echo "Attempt $attempt: PostgreSQL is not yet ready on $host:$port. Retrying in $wait_interval seconds..."
            sleep "$wait_interval"
        fi
    done
    
    echo "Error: PostgreSQL on $host:$port is not ready after $max_attempts attempts."
    return 1
}


# Check if PUID or PGID is set to a non-root value
if [ "$PUID" -ne 0 ]; then
    if getent passwd $PUID > /dev/null 2>&1; then
        user_name=$(getent passwd $PUID | cut -d: -f1)
    else
        adduser --uid $PUID --disabled-password --gecos "nonRootUser" --force-badname "nonRootUser"
    fi
fi

if [ "$PGID" -ne 0 ]; then
    if getent group $PGID > /dev/null 2>&1; then
        group_name=$(getent group $PGID | cut -d: -f1)
    else
        addgroup --gid $PGID --force-badname "nonRootGroup"
    fi
fi

mkdir -p /config/Cache
mkdir -p /config/DB
mkdir -p /config/Logs
mkdir -p /config/PlayLists/EPG
mkdir -p /config/PlayLists/M3U
mkdir -p $PGDATA

# Change ownership of the /app directory
if [ "$PUID" -ne 0 ] || [ "$PGID" -ne 0 ]; then
    echo "Changing ownership of /app to ${PUID:-0}:${PGID:-0}"
    chown -R ${PUID:-0}:${PGID:-0} /app
    echo "Changing ownership of /config to ${PUID:-0}:${PGID:-0}"
    find /config -mindepth 1 -maxdepth 1 -type d -not -path '/config/tv-logos' -not -path '/config/DB' -exec chown -R ${PUID:-0}:${PGID:-0} {} \;
fi

chown ${PUID:-0}:${PGID:-0} '/config/tv-logos' 2> /dev/null

# Pretty printing the configuration
echo "Configuration:"
echo "--------------"
echo "PGADMIN:"
echo "  Platform Type: $PGADMIN_PLATFORM_TYPE"
echo "  Setup Email: $PGADMIN_SETUP_EMAIL"
echo "  Setup Password: ********" #$PGADMIN_SETUP_PASSWORD"
echo "POSTGRES:"
echo "  User: $POSTGRES_USER"
echo "  Password: ********" #$POSTGRES_PASSWORD"
echo "  DB Name: $POSTGRES_DB"
echo "  Data Directory: $PGDATA"
echo "  Set Perms: $POSTGRES_SET_PERMS"
echo "OS:"
echo "  User: $POSTGRES_USER Group: $POSTGRES_USER"
echo "  UID: $(id -u $POSTGRES_USER) GID: $(id -g $POSTGRES_USER)"


if [ $POSTGRES_SET_PERMS -eq 1 ]; then
    chown -R postgres:postgres $PGDATA
fi


# Start the database
/usr/local/bin/docker-entrypoint.sh postgres &

wait_for_postgres "127.0.0.1" "5432" 20 5
if [ $? -eq 0 ]; then
    # PostgreSQL is ready, you can proceed with your tasks
    echo "Postgres is up"
else
    # PostgreSQL is not ready after max_attempts, handle the error
    echo "Error: PostgreSQL is not ready."
    exit 1
fi

#PGADMIN_PLATFORM_TYPE=$PGADMIN_PLATFORM_TYPE PGADMIN_SETUP_EMAIL=$PGADMIN_SETUP_EMAIL PGADMIN_SETUP_PASSWORD=$PGADMIN_SETUP_PASSWORD /usr/pgadmin4/bin/setup-web.sh --yes
#service postgresql start
#service apache2 start

# Execute the main application as the specified user
if [ "$PUID" -ne 0 ] && [ "$PGID" -ne 0 ]; then
    echo "Running as $user_name:$group_name"
    exec gosu $user_name:$group_name "$@"
elif [ "$PUID" -ne 0 ]; then
    echo "Running as $user_name"
    exec gosu $user_name "$@"
elif [ "$PGID" -ne 0 ]; then
    echo "Running as :$group_name"
    exec gosu :$group_name "$@"
else
    echo "Running as root"
    exec "$@"
fi
